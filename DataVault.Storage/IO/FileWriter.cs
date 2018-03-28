using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace DataVault.Storage.IO
{
    internal class FileWriter : IFileWriter
    {
        private string ConvertToString(IEnumerable<string> data)
        {
            var builder = new StringBuilder();
            
            foreach(var str in data)
            {
                builder.Append(str);
            }

            return builder.ToString();
        }

        public async Task WriteAsync(string path, IEnumerable<string> data)
        {
            using (var writer = File.Open(path, FileMode.Open, FileAccess.Write, FileShare.Read))
            {
                var bytes = Encoding.UTF8.GetBytes(ConvertToString(data));
                await writer.WriteAsync(bytes, 0, bytes.Length).ConfigureAwait(false);
            }
        }
    }
}
