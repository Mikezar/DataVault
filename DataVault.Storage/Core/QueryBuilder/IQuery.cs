using System.Collections.Generic;
using System;

namespace DataVault.Storage.Core.QueryBuilder
{
    public interface IQuery
    {
        QueryType Type { get; set; }

        Type EntityType { get; set; }
        
        string Source { get; set; }

        IDictionary<string, object> SystemData { get; set; }
    }
}
