using System;
using DataVault.Storage.Core.QueryBuilder;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using DataVault.Storage.IO;
using DataVault.Storage.Core.Exceptions;
using DataVault.Storage.Core.Intepreter;
using DataVault.Storage.Core.Cache;

namespace DataVault.Storage.Core.Providers
{
    internal sealed class VaultProvider : ITextFileProvider
    {
        private readonly Lazy<IFileReader> _fileReader;
        private readonly Lazy<IFileWriter> _fileWriter;
        private readonly Lazy<IIntepreter> _intepreter;
        private ConcurrentQueue<IntepretedData> _queries;

        private async Task<IQueryResult<TEntity>> Select<TEntity>(string source)
        {
            try
            {
                string filePath = IOHelper.CreateIfNotExists(source, false);

                var rawData = await _fileReader.Value.ReadAsync(filePath);

                if(rawData != null)
                {
                    var intepretedData = await _intepreter.Value.Interprete<TEntity>(rawData);

                    return new QueryResult<TEntity>()
                    {
                        Success = true,
                        Result = intepretedData
                    };
                }
                throw new StorageException("Exception occured during SELECT operation.");
            }
            catch (Exception e)
            {
                return new QueryResult<TEntity>()
                {
                    Success = false,
                    Result = null,
                    Exception = e
                };
            }
        }

        public VaultProvider()
        {
            _fileReader = new Lazy<IFileReader>(() => new FileReader());
            _fileWriter = new Lazy<IFileWriter>(() => new FileWriter());
            _intepreter = new Lazy<IIntepreter>(() => new VaultInterpreter());
            _queries = new ConcurrentQueue<IntepretedData>();
        }

        public VaultProvider(IFileReader reader, IFileWriter writer)
        {
            _fileReader = new Lazy<IFileReader>(() => reader);
            _fileWriter = new Lazy<IFileWriter>(() => writer);
            _intepreter = new Lazy<IIntepreter>(() => new VaultInterpreter());
            _queries = new ConcurrentQueue<IntepretedData>();
        }

        public async Task<IQueryResult<TEntity>> HandleQuery<TEntity>(IQuery query)
        {
            try
            {
                var processedData = await _intepreter.Value.Interprete(query, _fileReader.Value);

                _queries.Enqueue(new IntepretedData()
                {
                    TypeName = typeof(TEntity).Name,
                    Data = processedData
                });

                return new QueryResult<TEntity>()
                {
                    Success = true,
                    Result = null,
                    Exception = null
                };
            }
            catch(Exception e)
            {
                return new QueryResult<TEntity>()
                {
                    Success = false,
                    Result = null,
                    Exception = e
                };
            }
        }

        public async Task<IQueryResult<TEntity>> Execute<TEntity>(IQuery query)
        {
            if (query.Type == QueryType.Select)
            {
                return await Task.Run(async() => await Select<TEntity>(query.Source));
            }
            else
            {
                return await Task.Run(async() => await HandleQuery<TEntity>(query));
            }
        }

        public async Task ApplyChanges()
        {
            var queries = _queries.ToArray();
            _queries = null;
            var files = new List<string>();

            try
            {
                await Task.Run(async () => 
                {
                    for (int i = 0; i < queries.Length; i++)
                    {
                        string filePath = IOHelper.CreateIfNotExists(queries[i].TypeName, true);
                        files.Add(filePath);
                        await _fileWriter.Value.WriteAsync(filePath, queries[i].Data);
                    }
                });
            }
            catch(Exception e)
            {
                RollBack(files);

                throw new VaultTransactionException(e);
            }
            finally
            {
                CleanUp(files);
            }
        }

        public void RollBack(IList<string> filePathes)
        {
            IOHelper.RestoreFromBackUp(filePathes.ToArray());
        }

        public void CleanUp(IList<string> filePathes)
        {
            IOHelper.DeleteBackUps(filePathes.ToArray());
        }

        public Task Clear()
        {
            _queries = null;

            return Task.CompletedTask;
        }
    }
}
