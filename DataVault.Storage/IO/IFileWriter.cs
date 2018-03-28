using System.Threading.Tasks;
using System.Collections.Generic;

namespace DataVault.Storage.IO
{
    public interface IFileWriter
    {
        Task WriteAsync(string path, IEnumerable<string> data);
    }
}
