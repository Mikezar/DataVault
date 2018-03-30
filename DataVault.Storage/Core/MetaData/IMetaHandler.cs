using System;
using System.Collections.Generic;

namespace DataVault.Storage.Core.MetaData
{
    internal interface IMetaHandler
    {
        MetaList GenerateMetadataFor<TEntity>(IEnumerable<MetaList> data = null);
        MetaList GenerateMetadataFor(string type, IEnumerable<MetaList> data = null);
        MetaList GenerateMetadataFor(Type type, int version, IEnumerable<MetaList> data);
        bool CheckVersion<TEntity>(MetaList metadata);
        MetaList GetMetadataFor<TEntity>();
    }
}
