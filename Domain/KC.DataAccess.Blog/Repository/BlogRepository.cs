using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using KC.Database.EFRepository;
using Microsoft.EntityFrameworkCore;
using KC.Framework.Base;

namespace KC.DataAccess.Account.Repository
{
    public class BlogRepository : EFRepositoryBase<Model.Blog.Blog>, IBlogRepository
    {
        public BlogRepository(EFUnitOfWorkContextBase unitOfWork)
            : base(unitOfWork)
        {
        }

        public List<Model.Blog.Blog> GetBlogsWithCategoryByFilter(Expression<Func<Model.Blog.Blog, bool>> predicate)
        {
            var result = Entities
                .Include(o => o.Category)
                .AsNoTracking()
                .Where(predicate)
                .ToList();
            return result;
        }

        public Tuple<int, IList<Model.Blog.Blog>> FindPagenatedListWithCount(int pageIndex, int pageSize, Expression<Func<Model.Blog.Blog, bool>> predicate)
        {
            var databaseItems = Entities.AsNoTracking().Where(predicate);
            var data = databaseItems.Include(m => m.Category).OrderByDescending(m => m.CreateTime);
            int recordCount = databaseItems.Count();

            return new Tuple<int, IList<Model.Blog.Blog>>(recordCount,
                data.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList());
        }

        public Model.Blog.Blog GetBlogDetailById(int id)
        {
            return Entities.AsNoTracking()
                .Include(m => m.Category)
                .Include(m => m.Comments)
                .FirstOrDefault(c => c.Id == id && !c.IsDeleted);
        }

        public List<string> GetAllTags()
        {
            return Entities.AsNoTracking()
                .Where(m => !m.IsDeleted)
                .Select(m => m.Tags)
                .ToList();

        }

        public List<EFKeyValuePair<int, string>> GetTopBlogs()
        {
            return Entities.AsNoTracking()
                .Where(m => m.IsTop && !m.IsDeleted)
                .OrderByDescending(m => m.CreateTime)
                .Skip(0)
                .Take(10)
                .Select(m => new EFKeyValuePair<int, string>(m.Id, m.Title))
                .ToList();
        }

        public List<EFKeyValuePair<int, string>> GetNewBlogs()
        {
            return Entities.AsNoTracking()
                .Where(m => !m.IsDeleted)
                .OrderByDescending(m => m.CreateTime)
                .Skip(0)
                .Take(10)
                .Select(m => new EFKeyValuePair<int, string>(m.Id, m.Title))
                .ToList();
        }
    }
}
