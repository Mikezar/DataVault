using System;
using System.Collections.Generic;

namespace DataVault.Storage.Core.QueryBuilder
{
    public interface IQueryResult<TResult>
    {
        IEnumerable<TResult> Result { get; set; }
        bool Success { get; set; }
        Exception Exception { get; set; }
    }
}
