using System.Collections.Generic;
using System.Runtime.Caching;
using System.Linq;
using DataVault.Storage.Core.Views;

namespace DataVault.Storage.Core.Cache
{
   internal class Cache : ICache
    {
        private readonly ObjectCache VaultCache;
        private readonly CacheItemPolicy policy;
        private IDictionary<string, bool> Actuality;

        public Cache()
        {
            VaultCache = MemoryCache.Default;
            policy = new CacheItemPolicy()
            {
                Priority = CacheItemPriority.Default
            };
            Actuality = new Dictionary<string, bool>();
        }

        public IEnumerable<TEntity> TryGet<TEntity>(string region)
        {
           var type = typeof(TEntity).Name;

           if(VaultCache.Contains(type))
                return (IEnumerable<TEntity>)VaultCache.Get(type, region);

            return null;
        }

        public IDictionary<string, IEnumerable<object>> GetAll() => 
            VaultCache.ToDictionary(x => x.Key, x => x.Value as IEnumerable<object>);

        public void Set<TEntity>(IEnumerable<TEntity> objs, string region)
        {
            VaultCache.Set(typeof(TEntity).Name, objs, policy, region);
            SetActuality<TEntity>(true);
        }

        public bool GetActuality<TEntity>()
        {
            var type = typeof(TEntity).Name;

            if (Actuality.ContainsKey(type))
                return Actuality[type];
            else
            {
                Actuality.Add(type, false);
                return false;
            }
        }

        public void SetActuality<TEntity>(bool actuality)
        {
            var type = typeof(TEntity).Name;

            if (Actuality.ContainsKey(type))
                Actuality[type] = actuality;
            else
                Actuality.Add(type, actuality);
        }

        public void Reset(string region)
        {
            var keys = VaultCache.Select(x => x.Key).ToList();

            keys.ForEach(a => VaultCache.Remove(a, region));
            Actuality = new Dictionary<string, bool>();
        }

        public void Remove(string key, string region)
        {
            VaultCache.Remove(key, region);
            Actuality.Remove(key);
        }

        public void BindView(IView view,  string region)
        {
            Reset(region);
             
            foreach(var entry in view.Restore())
            {
                VaultCache.Set(entry.Key, entry.Value, policy, region);
                Actuality.Add(entry.Key, true);
            }
        }
    }
}
