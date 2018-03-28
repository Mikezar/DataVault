using System.Collections.Generic;
using DataVault.Storage.Core.QueryBuilder;

namespace DataVault.Storage.Core.Storage
{
    internal interface IStorage
    {
        bool TryGetFromCache<TEntity>(out IEnumerable<TEntity> entities);
        IEnumerable<TEntity> ReadData<TEntity>();
        void ManipulateData<TEntity>(QueryType type, IEnumerable<TEntity> entities) where TEntity : class;
        void Commit();
        void Clear();
    }
}
