using System.Collections.Generic;
using System;
using System.Threading.Tasks;

namespace DataVault.Storage.Core.Sets
{
    /// <summary>
    /// Set to manipulate with entity data
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    public interface IDataSet<TEntity> where TEntity : class
    {
        /// <summary>
        /// Returns all entities
        /// </summary>
        IEnumerable<TEntity> Entities { get;}

        TEntity Find(TEntity entity);

        Task<TEntity> FindAsync(TEntity entity);

        TEntity Add(TEntity entity);

        Task<TEntity> AddAsync(TEntity entity);

        IEnumerable<TEntity> AddRange(IEnumerable<TEntity> entities);

        Task<IEnumerable<TEntity>> AddRangeAsync(IEnumerable<TEntity> entities);

        TEntity Update(TEntity entity);

        Task<TEntity> UpdateAsync(TEntity entity);

        void Delete(TEntity entity);

        Task DeleteAsync(TEntity entity);

        void DeleteRange(IEnumerable<TEntity> entities);

        Task DeleteRangeAsync(IEnumerable<TEntity> entities);
    }
}
