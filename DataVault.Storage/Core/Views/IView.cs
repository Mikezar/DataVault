using System.Collections.Generic;

namespace DataVault.Storage.Core.Views
{
    internal interface IView
    {
        void Bind<TEntity>(IEnumerable<TEntity> list);
        void Bind(IDictionary<string, IEnumerable<object>> source);
        IDictionary<string, IEnumerable<object>> Restore();
        bool Exists { get; }
        bool UpToDate { get; }

    }
}
