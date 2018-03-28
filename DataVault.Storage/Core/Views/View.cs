using System.Collections.Generic;

namespace DataVault.Storage.Core.Views
{
    internal class View : IView
    {
        private IDictionary<string, IEnumerable<object>> _views;

        public bool Exists => _views != null;

        public bool UpToDate { get; private set; }

        public void Bind<TEntity>(IEnumerable<TEntity> list)
        {
            if(!Exists)
            {
                _views = new Dictionary<string, IEnumerable<object>>();
            }

            if (!_views.ContainsKey(typeof(TEntity).Name))
            {
                _views.Add(typeof(TEntity).Name, (IEnumerable<object>)list);
                UpToDate = true;
            }
        }

        public void Bind(IDictionary<string, IEnumerable<object>> dictionary)
        {
            _views = new Dictionary<string, IEnumerable<object>>(dictionary);
            UpToDate = true;
        }

        public IDictionary<string, IEnumerable<object>> Restore() => new Dictionary<string, IEnumerable<object>>(_views);
    }
}
