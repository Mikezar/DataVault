using System.Collections.ObjectModel;
using System.Collections.Specialized;
using DataVault.Storage.Core.Storage;
using DataVault.Storage.Core.QueryBuilder;
using System.Collections.Generic;

namespace DataVault.Storage.Core.Observables
{
    internal class ObservationCollection<TEntity> : IObservationCollection<TEntity> where TEntity : class
    {
        private IStorage _storage;

        private void Init()
        {
            Collection.CollectionChanged += OnChangeObserver;
            _storage = StorageFactory.GetStorage();
        }

        public ObservationCollection()
        {
            Collection = new ObservableCollection<TEntity>(_storage.ReadData<TEntity>());
            Init();
        }

        public ObservationCollection(IEnumerable<TEntity> entities)
        {
            Collection = new ObservableCollection<TEntity>(entities);
            Init();
        }

        public ObservableCollection<TEntity> Collection { get; private set; }

        public void OnChangeObserver(object sender, NotifyCollectionChangedEventArgs e)
        {
           switch(e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    _storage.ManipulateData(QueryType.Add, (IEnumerable<TEntity>)e.NewItems);
                    return;
                case NotifyCollectionChangedAction.Remove:
                    _storage.ManipulateData(QueryType.Delete, (IEnumerable<TEntity>)e.NewItems);
                    return;
                case NotifyCollectionChangedAction.Replace:
                    _storage.ManipulateData(QueryType.Update, (IEnumerable<TEntity>)e.NewItems);
                    return;
            }
        }
    }
}
