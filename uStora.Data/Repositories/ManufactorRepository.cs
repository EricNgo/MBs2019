using uStora.Data.Infrastructure;
using uStora.Model.Models;

namespace uStora.Data.Repositories
{
    public class ManufactorRepository : RepositoryBase<Manufactor>, IManufactorRepository
    {
        public ManufactorRepository(IDbFactory dbFactory)
           : base(dbFactory)
        {
        }
    }

    public interface IManufactorRepository : IRepository<Manufactor>
    {
    }
}
