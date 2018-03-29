using System;

namespace DataVault.Storage.Core.Exceptions
{
    public class StorageException : Exception
    {
        public StorageException(string message)  : base(message) { }
        public StorageException(string message, Exception e) : base(message, e) { }
    }
}
