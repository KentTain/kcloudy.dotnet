using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using KC.Framework.Base;
using KC.Model.Blog;

namespace KC.DataAccess.Account.Repository
{
    public interface IBlogRepository : Database.IRepository.IDbRepository<Model.Blog.Blog>
    {
        List<Model.Blog.Blog> GetBlogsWithCategoryByFilter(Expression<Func<Model.Blog.Blog, bool>> predicate);

        Tuple<int, IList<Model.Blog.Blog>> FindPagenatedListWithCount(int pageIndex, int pageSize, Expression<Func<Model.Blog.Blog, bool>> predicate);

        Model.Blog.Blog GetBlogDetailById(int id);

        List<string> GetAllTags();
        List<EFKeyValuePair<int, string>> GetNewBlogs();
        List<EFKeyValuePair<int, string>> GetTopBlogs();
    }
}