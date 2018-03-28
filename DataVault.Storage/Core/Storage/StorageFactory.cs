using System;

namespace DataVault.Storage.Core.Storage
{
    internal class StorageFactory
    {
        private static readonly Lazy<IStorage> storage = new Lazy<IStorage>(() => new Storage());

        private StorageFactory() { }

        public static IStorage GetStorage()
        {
            return storage.Value;
        }
    }
}
