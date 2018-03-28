using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace DataVault.Storage.IO
{
    internal class FileReader : IFileReader
    {
        public async Task<string> ReadAsync(string path)
        {
            using (var reader = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var buffer = new byte[reader.Length];
                await reader.ReadAsync(buffer, 0, buffer.Length);
         
                return await Task.FromResult(Encoding.UTF8.GetString(buffer));
            }
        }

        public IDictionary<string, string> ReadAll(string[] pathes)
        {
            IDictionary<string, string> dictionary = new Dictionary<string, string>();

            for (int i = 0; i < pathes.Length; i++)
            {
                using (var reader = File.Open(pathes[i], FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    var buffer = new byte[reader.Length];
                    reader.ReadAsync(buffer, 0, buffer.Length);

                    if(dictionary.ContainsKey(pathes[i]))
                    {
                        dictionary.Add(pathes[i], Encoding.UTF8.GetString(buffer));
                    }
                }
            }

            return dictionary;
        }
    }
}
