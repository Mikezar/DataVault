using System.Collections.Generic;
using System.Linq;
using System.Collections.Concurrent;
using Newtonsoft.Json;
using DataVault.Storage.IO;
using System;

namespace DataVault.Storage.Core.MetaData
{
    internal class MetadaHandler : IMetaHandler
    {
        private readonly IFileReader _reader;
        private readonly IFileWriter _writer;
        private ConcurrentDictionary<string, MetaList> _metaDictionary;

        public MetadaHandler(IFileReader reader, IFileWriter writer)
        {
            _reader = reader;
            _writer = writer;
            _metaDictionary = new ConcurrentDictionary<string, MetaList>();
        }

        public MetaList GenerateMetadataFor<TEntity>(IEnumerable<MetaList> data = null)
        {
            return GenerateMetadataFor(typeof(TEntity));
        }

        public MetaList GenerateMetadataFor(string type, IEnumerable<MetaList> data = null)
        {
            return GenerateMetadataFor(Type.GetType(type));
        }


        public MetaList GenerateMetadataFor(Type type, int version = 1, IEnumerable<MetaList> data = null)
        {
            int newVersion = version;

            if(data == null)
            {
                data = new List<MetaList>();
            }

            var meta = new MetaList()
            {
                Version = newVersion,
                Created = DateTimeOffset.Now
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

            var dataList = data.ToList();

            dataList.Add(meta);

            string filepath = IOHelper.CreateFileIfNotExists($"{type.Namespace}_meta", false);

            _writer.WriteAsync(filepath, new List<string>() { JsonConvert.SerializeObject(dataList) });

            return meta;
        }

        public bool CheckVersion<TEntity>(MetaList metadata)
        {
            var properties = typeof(TEntity).GetType().GetProperties();

            if (properties.Count() == metadata.Metas.Count)
            {
                for(int i = 0; i < properties.Count(); i++)
                {
                    if (properties[i].Name == metadata.Metas[i].FieldName 
                        && properties[i].PropertyType.Name == metadata.Metas[i].Type) continue;

                    return false;
                }

                return true;
            }

            return false;
        }

        public MetaList GetOptimalVerson<TEntity>(IEnumerable<MetaList> data)
        {
            if (data.Count() == 1) return data.First();

            var last = data.Last();

            if (CheckVersion<TEntity>(last)) return last;

            foreach(var entry in data)
            {
                if(CheckVersion<TEntity>(entry)) return entry;
            }

            var newMetaData = GenerateMetadataFor(typeof(TEntity));

            return newMetaData;
        }


        public MetaList GetMetadataFor<TEntity>()
        {
            var type = typeof(TEntity).Name;
            return _metaDictionary.GetOrAdd(type, x =>
            {
                string filepath = IOHelper.CreateFileIfNotExists($"{type}_meta.txt", false);

                var rawData = _reader.ReadAsync(filepath).Result;

                if (!string.IsNullOrEmpty(rawData))
                {
                    var data =  JsonConvert.DeserializeObject<IEnumerable<MetaList>>(rawData);

                    return GetOptimalVerson<TEntity>(data);
                }

                return  GenerateMetadataFor<TEntity>();
            });
        }
    }
}
