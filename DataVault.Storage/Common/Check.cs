using System;
using System.Collections.Generic;

namespace DataVault.Storage.Common
{
    internal class Check
    {
        public static void IsNull<T>(T obj)
        {
            if (obj == null)
                throw new NullReferenceException(obj.GetType().Name);
        }

        public static void IsDefault<T>(T obj)
        {
            if (EqualityComparer<T>.Default.Equals(obj, default(T)))
                throw new ArgumentException("Default value is not accepted");
        }
    }
}
