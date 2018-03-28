using System.Collections.Generic;
using System.Linq;
using System.Collections.Concurrent;
using Newtonsoft.Json;
using DataVault.Storage.IO;
using System;

namespace DataVault.Storage.Core.MetaData
{
    internal class MetadaHandler
    {
        private readonly IFileReader _reader;
        private readonly IFileWriter _writer;
        private ConcurrentDictionary<string, MetaList> _metaDictionary = new ConcurrentDictionary<string, MetaList>();

        public MetadaHandler(IFileReader reader, IFileWriter writer)
        {
            _reader = reader;
            _writer = writer;           
        }

        public MetaList GenerateMetadataFor<TEntity>()
        {
            return GenerateMetadataFor(typeof(TEntity));
        }

        public MetaList GenerateMetadataFor(string type)
        {
            return GenerateMetadataFor(Type.GetType(type));
        }


        public MetaList GenerateMetadataFor(Type type, int version = 1)
        {
            var meta = new MetaList()
            {
                Version = version
            };

            var properties = type.GetProperties().ToArray();

            for (int i = 0; i < properties.Length; i++)
            {
                meta.Metas.Add(new Metadata()
                {
                    FieldName = properties[i].Name,
                    Type = properties[i].PropertyType.Name,
                    Order = i
                });
            }

            string filepath = IOHelper.CreateIfNotExists($"{type}_meta.txt", false);

            _writer.WriteAsync(filepath, new List<string>() { JsonConvert.SerializeObject(meta) });

            return meta;
        }

        public bool CheckVersion<TEntity>(MetaList metadata)
        {
            var properties = typeof(TEntity).GetType().GetProperties();
            if (properties.Count() == metadata.Metas.Count)
            {
                for(int i = 0; i < properties.Count(); i++)
                {
                    if (properties[i].Name == metadata.Metas[i].FieldName && properties[i].PropertyType.Name == metadata.Metas[i].Type) continue;

                    return false;
                }

                return true;
            }

            return false;
        }

        public MetaList GetMetadataFor<TEntity>()
        {
            var type = typeof(TEntity).Name;
            return _metaDictionary.GetOrAdd(type, x =>
            {
                string filepath = IOHelper.CreateIfNotExists($"{type}_meta.txt", false);

                var rawData = _reader.ReadAsync(filepath).Result;

                if (!string.IsNullOrEmpty(rawData))
                {
                    var data =  JsonConvert.DeserializeObject<MetaList>(rawData);
                    
                    if(CheckVersion<TEntity>(data))
                    {
                        return data;
                    }
                    else
                    {
                        return GenerateMetadataFor(typeof(TEntity), data.Version + 1);
                    }
                }

                return  GenerateMetadataFor<TEntity>();
            });
        }
    }
}
