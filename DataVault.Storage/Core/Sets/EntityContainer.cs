using System;
using System.Collections.Generic;

namespace DataVault.Storage.Core.Sets
{
    /// <summary>
    /// Container to hold entities
    /// </summary>
    internal static class EntityContainer
    {
        internal static IDictionary<string, Type> EntityDictionary = new Dictionary<string, Type>();

        /// <summary>
        /// Register the specific type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void RegisterEntity<TEntity>()
        {
            if (EntityDictionary.ContainsKey(typeof(TEntity).FullName))
            {
                throw new InvalidOperationException("Try to register the entity that has already been added.");
            }

            EntityDictionary.Add(typeof(TEntity).FullName, typeof(TEntity));
        }
    }
}
