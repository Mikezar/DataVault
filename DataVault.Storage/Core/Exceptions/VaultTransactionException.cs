using System;


namespace DataVault.Storage.Core.Exceptions
{
    public class VaultTransactionException : Exception
    {
        public VaultTransactionException(Exception e) 
            : base("Error ocurred during transaction. Rollback was successfully performed.", e) { }

    }
}
