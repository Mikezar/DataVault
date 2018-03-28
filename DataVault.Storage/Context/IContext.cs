using DataVault.Storage.Core.Sets;

namespace DataVault.Storage.Context
{
    /// <summary>
    /// Context interface to get sets to manipulate related entity data
    /// </summary>
    public interface IContext
    {
        /// <summary>
        /// Returns set to manipulate with entity data
        /// </summary>
        /// <typeparam name="TEntity">Type of entity</typeparam>
        /// <returns>Entity set</returns>
        IDataSet<TEntity> GetDataSet<TEntity>() where TEntity : class;
    }
}
