using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataVault.Storage.Core.Intepreter
{
    public class IntepretedData
    {
        public string TypeName { get; set; }
        public IEnumerable<string> Data { get; set; }
    }
}
