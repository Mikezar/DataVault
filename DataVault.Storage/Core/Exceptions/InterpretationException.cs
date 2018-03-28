using System;

namespace DataVault.Storage.Core.Exceptions
{
    public class InterpretationException : Exception
    {
        public InterpretationException() : base("There wes an error occured on interpreting data") { }
        public InterpretationException(Exception e) : base("There wes an error occured on interpreting data", e) { }
    }
}
