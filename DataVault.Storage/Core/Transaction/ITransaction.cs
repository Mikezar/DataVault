using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataVault.Storage.Core.Transaction
{
    public interface ITransaction
    {
        void Commit();
        void Rollback();
    }
}
