using System;
using System.Collections.Generic;


namespace DataVault.Storage.Core.QueryBuilder
{
    internal sealed class QueryResult<TResult> : IQueryResult<TResult>
    {
        public Exception Exception { get; set; }

        public IEnumerable<TResult> Result { get; set; }

        public bool Success { get; set; }
    }
}
