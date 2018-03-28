using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DataVault.Storage.IO;
using DataVault.Storage.Core.QueryBuilder;
using System.Threading.Tasks;
using DataVault.Storage.Core.MetaData;
using DataVault.Storage.Core.Exceptions;
using System.Globalization;

namespace DataVault.Storage.Core.Intepreter
{
    internal class VaultInterpreter : IIntepreter
    {
        private const char Line = (char)30;
        private const char Field = (char)31;
        private readonly MetadaHandler _metaHandler = new MetadaHandler(new FileReader(), new FileWriter());

        private string[] SplitIntoRows(string rawData)
        {
            return rawData.Split(Line).Where(x => x != "\r\n" && !string.IsNullOrEmpty(x)).ToArray();
        }

        private TEntity GenerateType<TEntity>(string[] fields)
        {
            try
            {
                var apex = fields.Count();
                var entity = (TEntity)Activator.CreateInstance(typeof(TEntity));

                var metadata = _metaHandler.GetMetadataFor<TEntity>().Metas.OrderBy(x => x.Order).ToArray();
                var properties = entity.GetType().GetProperties();

                if (metadata.Count() != properties.Length) throw new InterpretationException();

                for (int i = 0; i < apex; i++)
                {
                    Metadata meta = metadata[i];

                    var property = properties.FirstOrDefault(x => x.Name == meta.FieldName);

                    if (property == null) continue;

                    if (property.PropertyType.Name == meta.Type)
                    {
                        Type type = property.PropertyType;
                        if (type == typeof(DateTime))
                        {
                            var value = DateTime.Parse(fields[i], CultureInfo.InvariantCulture);
                            property.SetValue(entity, value);
                        }
                        else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                        {
                            var actualType = Nullable.GetUnderlyingType(type) ?? type;
                            var safeValue = (string.IsNullOrEmpty(fields[i])) ? null : Convert.ChangeType(fields[i], type);
                            property.SetValue(entity, safeValue);

                            property.SetValue(entity, null);
                        }
                        else
                            property.SetValue(entity, Convert.ChangeType(fields[i], type), null);
                    }
                }

                return entity;
            }
            catch(Exception e)
            {
                throw new InterpretationException(e);
            }
        }

        public Task<IEnumerable<TEntity>> Interprete<TEntity>(string rawData)
        {
            string[] lines = SplitIntoRows(rawData);
            List<TEntity> list = new List<TEntity>();

            foreach (var line in lines)
            {
                var fields = line.Split(Field);
                list.Add(GenerateType<TEntity>(fields));
            }

            return Task.FromResult(list.AsEnumerable());
        }


        public async Task<IEnumerable<string>> Interprete(IQuery query, IFileReader reader)
        {
            if (query.Type == QueryType.Select) throw new InvalidOperationException();

            var rawData = await reader.ReadAsync(query.Source);
            string[] rows = SplitIntoRows(rawData);

            if (query.Type == QueryType.Add)
            {
                if (query.SystemData == null) throw new ArgumentNullException("Entities");

                await Add(query, rows);
            }
            else if(query.Type == QueryType.Delete)
            {
                if (query.SystemData == null) throw new ArgumentNullException("Entities");

                await Delete(query, rows);
            }
            else if (query.Type == QueryType.Update)
            {
                if (query.SystemData == null) throw new ArgumentNullException("Entities");

                await Update(query, rows);
            }

            throw new NotImplementedException();
        }

        public async Task<IEnumerable<string>> Add(IQuery query, string[] rows)
        {
            int maxId = 0;
            if (rows.Length > 0)
            {
                var lastLine = rows.Last();
                maxId = Convert.ToInt32(lastLine.Substring(0, lastLine.IndexOf(Field)));
            }

            var modified = rows.Concat(InterpreteToString(query.SystemData, maxId)).ToArray();

            return await Task.FromResult(modified.Select(x => x + $"{Line}").AsEnumerable());
        }

        public async Task<IEnumerable<string>> Delete(IQuery query, string[] rows)
        {
            var keys = (IList<Int32>)query.SystemData["CorrelationId"];

            var updated =
                rows.Where(
                        x =>
                            keys.Contains(Convert.ToInt32(x.Substring(0, x.IndexOf(Field)))) ==
                            false)
                    .ToArray();

            return await Task.FromResult(updated.Select(x => x + $"{Line}"));
        }

        public async Task<IEnumerable<string>> Update(IQuery query, string[] rows)
        {
            var data = InterpreteToString(query.SystemData, (string)query.SystemData["SCIName"]);

            var updated = rows.ToArray();

            for (int i = 0; i < updated.Length; i++)
            {
                int id = Convert.ToInt32(updated[i].Substring(0, updated[i].IndexOf(Field)));

                if (data.ContainsKey(id))
                {
                    updated[i] = data[id];
                }
            }
            return await Task.FromResult(updated.Select(x => x + $"{Line}"));
        }

        public IEnumerable<string> InterpreteToString<TEntity>(IEnumerable<TEntity> entities, int maxId)
        {
            int counter = ++maxId;
            var result = new List<string>();
            var metadata = _metaHandler.GetMetadataFor<TEntity>().Metas.OrderBy(x => x.Order).ToArray();

            foreach (var entity in entities)
            {
                var identity = Convert.ToString(counter);
                counter++;

                var properties = entity.GetType().GetProperties(BindingFlags.Public);
                var values = new List<object>();

                for(int i = 0; i < metadata.Length; i++)
                {
                    for(int j = 0;  j < properties.Length; j++)
                    {
                        if(properties[i].Name == metadata[j].FieldName)
                        {
                            values.Add(properties[i].GetValue(entity));
                        }
                    }
                }
                           
                var entityString = String.Join($"{Field}", values.Select(c => c).ToArray());
                result.Add(String.Concat(identity, entityString));
            }

            return result;
        }

        public Dictionary<int, string> InterpreteToString<TEntity>(IEnumerable<TEntity> entities, string sciField)
        {
            var result = new Dictionary<int, string>();
            _metaHandler.GenerateMetadataFor<TEntity>();

            foreach (var entity in entities)
            {
                var properties = entity.GetType().GetProperties();
                var values = properties.Select(x => x.GetValue(entity)).ToArray();
                var entityString = String.Join($"{Field}", values.Select(c => c).ToArray());
                result.Add((int)entity.GetType().GetProperty(sciField).GetValue(entity), entityString);
            }

            return result;
        }
    }
}
