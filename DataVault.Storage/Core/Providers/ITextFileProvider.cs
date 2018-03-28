using DataVault.Storage.Core.QueryBuilder;
using System.Threading.Tasks;

namespace DataVault.Storage.Core.Providers
{
    public interface ITextFileProvider
    {
        Task<IQueryResult<TEntity>> Execute<TEntity>(IQuery query);
        Task ApplyChanges();
        Task Clear();
    }
}
