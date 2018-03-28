using DataVault.Storage.Core.Views;
using System.Collections.Generic;

namespace DataVault.Storage.Core.Cache
{
   internal interface ICache
    {
        IEnumerable<TEntity> TryGet<TEntity>(string region);
        IDictionary<string, IEnumerable<object>> GetAll();
        void Set<TEntity>(IEnumerable<TEntity> objs, string region);
        bool GetActuality<TEntity>();
        void SetActuality<TEntity>(bool actuality);
        void Reset(string region);
        void Remove(string key, string region);
        void BindView(IView view, string region);
    }
}
