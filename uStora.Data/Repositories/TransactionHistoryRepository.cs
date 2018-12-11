using uStora.Data.Infrastructure;
using uStora.Model.Models;

namespace uStora.Data.Repositories
{
    public interface ITransactionHistoryRepository : IRepository<TransactionHistory>
    {
       
    }

    public class TransactionHistoryRepository : RepositoryBase<TransactionHistory>, ITransactionHistoryRepository
    {
        public TransactionHistoryRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }
    }
}
