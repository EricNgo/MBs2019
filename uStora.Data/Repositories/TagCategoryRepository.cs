using uStora.Data.Infrastructure;
using uStora.Model.Models;

namespace uStora.Data.Repositories
{
    public interface ITagCategoryRepository : IRepository<TagCategory> { }

    public class TagCategoryRepository : RepositoryBase<TagCategory>, ITagCategoryRepository
    {
        public TagCategoryRepository(IDbFactory dbFactory)
            : base(dbFactory)
        {
        }
    }
}