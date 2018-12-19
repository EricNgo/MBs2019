using System.Collections.Generic;
using uStora.Common.ViewModels;
using uStora.Data.Infrastructure;
using uStora.Model.Models;

namespace uStora.Data.Repositories
{
    public interface ITagRepository : IRepository<Tag> {
        IEnumerable<TagbyTagCategoryViewModel> GetTagsByTagCategories();
    }

    public class TagRepository : RepositoryBase<Tag>, ITagRepository
    {
        public TagRepository(IDbFactory dbFactory)
            : base(dbFactory)
        {
        }
        public IEnumerable<TagbyTagCategoryViewModel> GetTagsByTagCategories()
        {
            return DbContext.Database.SqlQuery<TagbyTagCategoryViewModel>("GetTagsByTagCategories");
        }
    }
}