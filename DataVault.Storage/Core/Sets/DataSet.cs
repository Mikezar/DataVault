using System.Collections.Generic;
using System;
using DataVault.Storage.Common;
using DataVault.Storage.Core.Observables;
using System.Threading.Tasks;

namespace DataVault.Storage.Core.Sets
{
    public class DataSet<TEntity> : IDataSet<TEntity> where TEntity : class
    {
        private Lazy<IObservationCollection<TEntity>> Observable;

        internal DataSet(IObservationCollection<TEntity> collection)
        {
            Observable = new Lazy<IObservationCollection<TEntity>>(() => collection);
        }

        public DataSet()
        {
            Observable = new Lazy<IObservationCollection<TEntity>>(() => new ObservationCollection<TEntity>());
        }

        public IEnumerable<TEntity> Entities => Observable.Value.Collection;

        public TEntity Find(TEntity entity)
        {
            Check.IsDefault(entity);

            var index = Observable.Value.Collection.IndexOf(entity);

            if (index == -1) return null;

            return Observable.Value.Collection[index];
        }

        public async Task<TEntity> FindAsync(TEntity entity)
        {
            return await Task.Run(() => Find(entity));
        }

        public TEntity Add(TEntity entity)
        {
            Check.IsNull(entity);

            Observable.Value.Collection.Add(entity);
            return entity;
        }

        public async Task<TEntity> AddAsync(TEntity entity)
        {
            return await Task.Run(() => Add(entity));
        }

        public IEnumerable<TEntity> AddRange(IEnumerable<TEntity> entities)
        {
            Check.IsNull(entities);

            foreach(var entity in entities)
            {
                Add(entity);
            }

            return entities;
        }

        public async Task<IEnumerable<TEntity>> AddRangeAsync(IEnumerable<TEntity> entities)
        {
            Check.IsNull(entities);

            await Task.Run(() => Parallel.ForEach(entities, (x) => Add(x)));
     
            return await Task.FromResult(entities);
        }

        public TEntity Update(TEntity entity)
        {
            Check.IsNull(entity);

            int index = Observable.Value.Collection.IndexOf(entity);

            if (index == -1) return null;

            Observable.Value.Collection[index] = entity;

            return entity;
        }

        public async Task<TEntity> UpdateAsync(TEntity entity)
        {
            return await Task.Run(() => Update(entity));
        }

        public void Delete(TEntity entity)
        {
            Check.IsNull(entity);

            Observable.Value.Collection.Remove(Find(entity));
        }

        public async Task DeleteAsync(TEntity entity)
        {
            Check.IsNull(entity);

            await Task.Run(async () => Observable.Value.Collection.Remove(await FindAsync(entity)));
        }

        public void DeleteRange(IEnumerable<TEntity> entities)
        {
            Check.IsNull(entities);

            foreach (var entity in entities)
            {
                Delete(entity);
            }
        }

        public async Task DeleteRangeAsync(IEnumerable<TEntity> entities)
        {
            Check.IsNull(entities);

            await Task.Run(() => Parallel.ForEach(entities, (x) => Delete(x)));
        }
    }
}
