using System;

namespace DataVault.Storage.Core.Exceptions
{
   public class DataSetOperationException : Exception
    {
        public DataSetOperationException() 
            : base("Exception was caught during DataSet operation.") {}

        public DataSetOperationException(string message)
           : base(message) { }

        public DataSetOperationException(string message, Exception e)
           : base(message, e) { }
    }
}
