using System.Collections.Generic;

namespace DataVault.Storage.Core.QueryBuilder
{
   public interface IQueryBuilder
    {
        IQuery CreateQuery<TEntity>(QueryType type, IEnumerable<TEntity> entities);
    }
}
