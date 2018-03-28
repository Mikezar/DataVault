using DataVault.Storage.Common;
using DataVault.Storage.Core.Sets;
using System.Collections.Generic;
using System.Linq;

namespace DataVault.Storage.Core.QueryBuilder
{
    internal sealed class QueryBuilder : IQueryBuilder
    {
        public IQuery CreateQuery<TEntity>(QueryType type, IEnumerable<TEntity> entities)
        {
            var query = new Query()
            {
                Type = type,
                Source = $"{typeof(TEntity).Name}",
                EntityType = typeof(TEntity)
            };

            if(type == QueryType.Delete | type == QueryType.Add | type == QueryType.Update)
            {
                var systemIds = new List<int>();

               var sciProperty =  typeof(TEntity).GetProperties()
                    .FirstOrDefault(x => x.GetCustomAttributes(true).FirstOrDefault(f => f as SCIAttribute != null) != null);

                foreach(var entity in entities)
                {
                   int value = (int)entity.GetType().GetProperty(sciProperty.Name).GetValue(entity);
                   systemIds.Add(value);
                }
                query.SystemData.Add("CorrelationId", systemIds);
                query.SystemData.Add("SCIName", sciProperty.Name);
            }

            if(type == QueryType.Update | type == QueryType.Add)
            {
                query.SystemData.Add("Data", entities);
            }

            return query;
        }
    }
}
