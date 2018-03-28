using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace DataVault.Storage.Core.Observables
{
    internal interface IObservationCollection<TEntity> where TEntity : class
    {
        ObservableCollection<TEntity> Collection { get; }

        void OnChangeObserver(object sender, NotifyCollectionChangedEventArgs e);
    }
}
