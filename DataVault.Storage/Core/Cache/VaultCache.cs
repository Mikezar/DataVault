using System;
using System.Runtime.Caching;

namespace DataVault.Storage.Core.Cache
{
    internal sealed class VaultCache : MemoryCache
    {
        private const string DefaultRegion = "default_vault";

        public VaultCache() : base("VaultCache") { }

        public override void Set(CacheItem item, CacheItemPolicy policy)
        {
            Set(item.Key, item.Value, policy, item.RegionName);
        }

        public override void Set(string key, object value, DateTimeOffset absoluteExpiration, string regionName = null)
        {
            Set(key, value, new CacheItemPolicy { AbsoluteExpiration = absoluteExpiration }, regionName);
        }

        public override void Set(string key, object value, CacheItemPolicy policy, string regionName = null)
        {
            base.Set(CombineRegion(key, regionName), value, policy);
        }

        public override CacheItem GetCacheItem(string key, string regionName = null)
        {
            CacheItem temporary = base.GetCacheItem(CombineRegion(key, regionName));
            return new CacheItem(key, temporary.Value, regionName);
        }

        public override object Get(string key, string regionName = null)
        {
            return base.Get(CombineRegion(key, regionName));
        }

        public override DefaultCacheCapabilities DefaultCacheCapabilities
        {
            get
            {
                return (base.DefaultCacheCapabilities | DefaultCacheCapabilities.CacheRegions);
            }
        }

        internal static string CombineRegion(string key, string region)
        {
            return $"{(string.IsNullOrEmpty(region) ? DefaultRegion : region)}_{key}"; 
        }
    }
}
