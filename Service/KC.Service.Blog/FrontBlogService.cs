using AutoMapper;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using KC.DataAccess.Account.Repository;
using KC.Database.EFRepository;
using KC.Database.IRepository;
using KC.Framework.Extension;
using KC.Framework.Tenant;
using KC.Service.EFService;
using KC.Service.DTO.Blog;
using KC.Service.DTO;
using KC.Service.WebApiService.Business;
using KC.Model.Blog;
using KC.Framework.Base;

namespace KC.Service.Blog
{
    public interface IFrontBlogService : IEFService
    {
        string GetTitle();
        string GetAboutMe();

        List<string> GetAllTags();
        List<EFKeyValuePair<int, string>> GetNewBlogTitles();
        List<EFKeyValuePair<int, string>> GetTopBlogTitles();

        List<EFKeyValuePair<int, string>> GetCategoryNames();

        PaginatedBaseDTO<BlogSimpleDTO> FindPagenatedBlogs(int pageIndex, int pageSize, int categoryId, string tag, string title);
        BlogDTO GetBlogDetailById(int id);

        bool SaveComment(CommentDTO comment);
    }

    public class FrontBlogService : EFServiceBase, IFrontBlogService
    {
        private readonly IMapper _mapper;

        /// <summary>
        /// ComAccountUnitOfWorkContext
        ///     use AccountUnitOfWorkContext.Commit() to save the context change(transaction).
        /// </summary>
        private EFUnitOfWorkContextBase _unitOfContext;

        private IDbRepository<Category> _categoryRepository;
        private IDbRepository<Comment> _commentRepository;
        private IBlogRepository _blogRepository;
        private IDbRepository<Setting> _settingRepository;

        public FrontBlogService(
            Tenant tenant,
            IMapper mapper,
            EFUnitOfWorkContextBase unitOfContext,
            
            IBlogRepository blogRepository,
            IDbRepository<Category> categoryRepository,
            IDbRepository<Comment> commentRepository,
            IDbRepository<Setting> settingRepository,

            System.Net.Http.IHttpClientFactory clientFactory,
            Microsoft.Extensions.Logging.ILogger<FrontBlogService> logger)
            : base(tenant, clientFactory, logger)
        {
            _mapper = mapper;
            _unitOfContext = unitOfContext;

            _blogRepository = blogRepository;
            _categoryRepository = categoryRepository;
            _commentRepository = commentRepository;
            _settingRepository = settingRepository;
        }

        public string GetTitle()
        {
            var data = _settingRepository.GetByFilter(m => m.Name.Equals(SettingDTO.Setting_Title));
            if (data == null)
                return null;
            return data.Value;
        }

        public string GetAboutMe()
        {
            var data = _settingRepository.GetByFilter(m => m.Name.Equals(SettingDTO.Setting_AboutMe));
            if (data == null)
                return null;
            return data.Value;
        }

        public List<EFKeyValuePair<int, string>> GetNewBlogTitles()
        {
            return _blogRepository.GetNewBlogs();
        }
        public List<string> GetAllTags()
        {
            var data = _blogRepository.GetAllTags();
            var result = new List<string>();
            foreach (var tags in data)
            {
                var taglist = tags.ArrayFromCommaDelimitedStringsBySplitChar(',');
                result.AddRange(taglist);
            }

            return result.Distinct().ToList();
        }
        public List<EFKeyValuePair<int, string>> GetTopBlogTitles()
        {
            return _blogRepository.GetTopBlogs();
        }

        public List<EFKeyValuePair<int, string>> GetCategoryNames()
        {
            var data = _categoryRepository.FindAll();

            return data.Select(m => new EFKeyValuePair<int, string>(m.Id, m.Name)).ToList();
        }

        public PaginatedBaseDTO<BlogSimpleDTO> FindPagenatedBlogs(int pageIndex, int pageSize, int categoryId, string tag, string title)
        {
            Expression<Func<Model.Blog.Blog, bool>> predicate = m => !m.IsDeleted;
            if (categoryId != 0)
            {
                predicate = predicate.And(m => m.CategoryId == categoryId);
            }
            if (!string.IsNullOrWhiteSpace(tag))
            {
                predicate = predicate.And(m => m.Tags.Contains(tag));
            }
            if (!string.IsNullOrWhiteSpace(title))
            {
                predicate = predicate.And(m => m.Title.Contains(title));
            }

            var data = _blogRepository.FindPagenatedListWithCount(pageIndex, pageSize, predicate);
            var total = data.Item1;
            var rows = _mapper.Map<List<BlogSimpleDTO>>(data.Item2);
            return new PaginatedBaseDTO<BlogSimpleDTO>(pageIndex, pageSize, total, rows);
        }

        public BlogDTO GetBlogDetailById(int id)
        {
            var data = _blogRepository.GetBlogDetailById(id);
            return _mapper.Map<BlogDTO>(data);
        }

        public bool SaveComment(CommentDTO comment)
        {
            var data = _mapper.Map<Comment>(comment);
            return _commentRepository.Add(data);
        }
    }
}
