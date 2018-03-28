using System.Collections.Generic;

namespace DataVault.Storage.Core.MetaData
{
   internal class MetaList
    {
        public MetaList()
        {
            Metas = new List<Metadata>();
        }

        public int Version { get; set; }

        public List<Metadata> Metas { get; set; }
    }
}
