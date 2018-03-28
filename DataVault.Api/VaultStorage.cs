using DataVault.Storage.Core.Sets;
using DataVault.Storage.Context;
using DataVault.Storage.Core.Storage;

namespace DataVault.Api
{
    public static class VaultStorage
    {
        public static void RegisterEntity<TEntity>()
        {
            EntityContainer.RegisterEntity<TEntity>();
        }

        public static IDataSet<TEntity> GetSetFor<TEntity>() where TEntity : class
        {
            return VaultContext.GetContext().GetDataSet<TEntity>();
        }

        public static void Save()
        {
            StorageFactory.GetStorage().Commit();
        }


        public static void Rollback()
        {
            StorageFactory.GetStorage().Clear();
        }
    }
}
