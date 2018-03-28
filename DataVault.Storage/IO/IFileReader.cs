using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataVault.Storage.IO
{
    public interface IFileReader
    {
        Task<string> ReadAsync(string path);
        IDictionary<string, string> ReadAll(string[] pathes);
    }
}
