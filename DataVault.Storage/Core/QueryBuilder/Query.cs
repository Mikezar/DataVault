using System.Collections.Generic;
using System;

namespace DataVault.Storage.Core.QueryBuilder
{
    public class Query : IQuery
    {
        public Query()
        {
            SystemData = new Dictionary<string, object>();
        }

        public Type EntityType { get; set; }
        public string Source { get; set; }
        public QueryType Type { get; set; }
        public IDictionary<string, object> SystemData { get; set; }
    }
}
