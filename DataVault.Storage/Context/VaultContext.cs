using System;
using System.Collections.Concurrent;
using System.Linq;
using DataVault.Storage.Core.Sets;
using DataVault.Storage.Core.Exceptions;
using System.Threading.Tasks;

namespace DataVault.Storage.Context
{
    internal sealed class VaultContext : IContext
    {
        /// <summary>
        /// Store all sets to manipulate with registered entities
        /// </summary>
        private Lazy<ConcurrentDictionary<string, object>> DataSets;
        private static IContext _context;

        private VaultContext()
        {
            DataSets = new Lazy<ConcurrentDictionary<string, object>>(
                () => new ConcurrentDictionary<string, object>());

            if (EntityContainer.EntityDictionary == null)
            {
                throw new NullReferenceException("No registered entity was found");
            }

            InitDataSet();
        }

        /// <summary>
        /// Init parallel set creating for all registered entities
        /// </summary>
        private void InitDataSet()
        {
            Parallel.ForEach(EntityContainer.EntityDictionary.Select(x => x.Value), type =>
            {
                CreateDataSet(type);
            });
        }

        /// <summary>
        /// Create set for the type and add it to the cash
        /// </summary>
        /// <param name="type"></param>
        private void CreateDataSet(Type type)
        {
            try
            {
                var result = DataSets.Value.TryAdd(type.FullName,Activator.CreateInstance(typeof(DataSet<>).MakeGenericType(type)));

                if (result == false) throw new InvalidOperationException();
            }
            catch (Exception e)
            {
                throw new DataSetOperationException("Failed to register entity", e);
            }
        }

        public IDataSet<TEntity> GetDataSet<TEntity>() where TEntity : class
        {
            if (DataSets.Value.ContainsKey(typeof(TEntity).FullName))
            {
                return (IDataSet<TEntity>)DataSets.Value[typeof(TEntity).FullName];
            }

            return null;
        }

        public static IContext GetContext()
        {
            if (_context == null)
            {
                _context = new VaultContext();
            }

            return _context;
        }
    }
}
