using System;
using DataVault.Storage.Core.Cache;
using DataVault.Storage.Core.QueryBuilder;
using DataVault.Storage.Core.Providers;
using DataVault.Storage.Core.Exceptions;
using System.Collections.Generic;
using DataVault.Storage.Core.Views;

namespace DataVault.Storage.Core.Storage
{
    internal class Storage : IStorage
    {
        private readonly Lazy<IQueryBuilder> _queryBuilder;
        private Lazy<ICache> Cache;
        private readonly Lazy<ITextFileProvider> _provider;
        private readonly Lazy<IView> View;
        private const string region = "VaultDataKey";

        public Storage()
        {
            _queryBuilder = new Lazy<IQueryBuilder>(() => new QueryBuilder.QueryBuilder());
            Cache = new Lazy<ICache>(() => new Cache.Cache());
            _provider = new Lazy<ITextFileProvider>(() => new VaultProvider());
            View = new Lazy<IView>(() => new View());
        }

        public Storage(ITextFileProvider provider, ICache cache) : this()
        {
            _provider = new Lazy<ITextFileProvider>(() => provider);
            Cache  = new Lazy<ICache>(() => cache);
        }

        public bool TryGetFromCache<TEntity>(out IEnumerable<TEntity> entities)
        {
            if(Cache.Value.GetActuality<TEntity>())
            {
                entities = Cache.Value.TryGet<TEntity>(region);

                if (entities != null)
                    return true;
                else
                    return false;
            }

            entities = null;
            return false;
        }

        public IEnumerable<TEntity> ReadData<TEntity>()
        {
            IEnumerable<TEntity> entities;
            if (TryGetFromCache(out entities))
            {
                return entities;
            }

            try
            {
                var result =  _provider.Value.Execute<TEntity>(
                    _queryBuilder.Value.CreateQuery<TEntity>(QueryType.Select, null)).Result;

                if (result.Success == false) throw new StorageException(result.Exception.Message);

                Cache.Value.Set(result.Result, region);
                View.Value.Bind(result.Result);

                return result.Result;
            }
            catch(StorageException e)
            {
                throw new VaultTransactionException(e);
            }    
        }

        public void ManipulateData<TEntity>(QueryType type, IEnumerable<TEntity> entities) where TEntity : class
        {
            try
            {
                var result = _provider.Value.Execute<TEntity>(_queryBuilder.Value.CreateQuery(type, entities)).Result;

                if (result.Success == false)
                {
                    throw new StorageException(result.Exception.Message);
                }
            }
            catch(StorageException e)
            {
                throw new StorageException("An error occured on executing write operation in vault storage", e);
            }
        }

        public void Commit()
        {
            _provider.Value.ApplyChanges();
            Cache.Value.Reset(region);
        }

        public void Clear()
        {
            _provider.Value.Clear();
            Cache.Value.BindView(View.Value, region);
        }
    }
}
