using System.Collections.Generic;
using System.Threading.Tasks;
using DataVault.Storage.Core.QueryBuilder;
using DataVault.Storage.IO;

namespace DataVault.Storage.Core.Intepreter
{
    public interface IIntepreter
    {
        Task<IEnumerable<TEntity>> Interprete<TEntity>(string rawData);
        Task<IEnumerable<string>> Interprete(IQuery query, IFileReader reader);
    }
}
