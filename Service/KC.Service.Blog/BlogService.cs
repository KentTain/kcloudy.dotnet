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
    public interface IBlogService : IEFService
    {
        #region Category

        List<CategoryDTO> GetAllCategories();

        CategoryDTO GetCategoryById(int id);

        bool SaveCategory(CategoryDTO model);

        bool RemoveCategoryById(int id);

        #endregion

        #region Blog
        List<BlogDTO> GetBlogsByCategoryId(int configId);
        PaginatedBaseDTO<BlogDTO> FindPagenatedBlogs(int pageIndex, int pageSize, string title);

        BlogDTO GetBlogById(int propertyId);
        bool SaveBlog(BlogDTO data);
        bool SoftRemoveBlogById(int id);

        #endregion

        #region Setting

        PrivateSettingDTO GetPrivateSetting(string userId, string userName);
        bool SaveSetting(PrivateSettingDTO model);

        #endregion
    }

    public class BlogService : EFServiceBase, IBlogService
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

        public BlogService(
            Tenant tenant,
            IMapper mapper,
            EFUnitOfWorkContextBase unitOfContext,
            
            IBlogRepository blogRepository,
            IDbRepository<Category> categoryRepository,
            IDbRepository<Comment> commentRepository,
            IDbRepository<Setting> settingRepository,

            System.Net.Http.IHttpClientFactory clientFactory,
            Microsoft.Extensions.Logging.ILogger<BlogService> logger)
            : base(tenant, clientFactory, logger)
        {
            _mapper = mapper;
            _unitOfContext = unitOfContext;

            _blogRepository = blogRepository;
            _categoryRepository = categoryRepository;
            _commentRepository = commentRepository;
            _settingRepository = settingRepository;
        }

        #region Category

        public List<CategoryDTO> GetAllCategories()
        {
            var data = _categoryRepository.FindAll();
            return _mapper.Map<List<CategoryDTO>>(data);
        }

        public CategoryDTO GetCategoryById(int id)
        {
            var data = _categoryRepository.GetById(id);
            return _mapper.Map<CategoryDTO>(data);
        }

        public bool SaveCategory(CategoryDTO model)
        {
            var data = _mapper.Map<Category>(model);
            if (data.Id == 0)
            {
                return _categoryRepository.Add(data);
            }
            else
            {
                return _categoryRepository.Modify(data, true);
            }
        }

        public bool RemoveCategoryById(int configId)
        {
            return _categoryRepository.RemoveById(configId, true);
        }

        #endregion

        #region Blog

        public List<BlogDTO> GetBlogsByCategoryId(int configId)
        {
            Expression<Func<Model.Blog.Blog, bool>> predicate = m => m.CategoryId.Equals(configId) && !m.IsDeleted;

            var data = _blogRepository.FindAll(predicate, k => k.CreatedDate, false);
            return _mapper.Map<List<BlogDTO>>(data);
        }

        public PaginatedBaseDTO<BlogDTO> FindPagenatedBlogs(int pageIndex, int pageSize, string title)
        {
            Expression<Func<Model.Blog.Blog, bool>> predicate = m => !m.IsDeleted;
            if (!string.IsNullOrWhiteSpace(title))
            {
                predicate = predicate.And(m => m.Title.Contains(title));
            }

            var data = _blogRepository.FindPagenatedListWithCount(pageIndex, pageSize, predicate);
            var total = data.Item1;
            var rows = _mapper.Map<List<BlogDTO>>(data.Item2);
            return new PaginatedBaseDTO<BlogDTO>(pageIndex, pageSize, total, rows);
        }

        
        public BlogDTO GetBlogById(int propertyId)
        {
            var data = _blogRepository.GetById(propertyId);
            return _mapper.Map<BlogDTO>(data);
        }
        public bool SaveBlog(BlogDTO data)
        {
            var model = _mapper.Map<Model.Blog.Blog>(data);
            if (model.Id != 0)
            {
                return _blogRepository.Modify(model, true);
            }
            else
            {
                return _blogRepository.Add(model);
            }
        }
        public bool SoftRemoveBlogById(int id)
        {
            return _blogRepository.SoftRemoveById(id);
        }

        public List<BlogDTO> GetNewBlogs()
        {
            var data = _blogRepository.GetNewBlogs();
            return _mapper.Map<List<BlogDTO>>(data);
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

            return result;
        }
        public List<EFKeyValuePair<int, string>> GetTopBlogTitles()
        {
            return _blogRepository.GetTopBlogs();
        }
        #endregion

        #region Setting
        public PrivateSettingDTO GetPrivateSetting(string userId, string userName)
        {
            var result = new PrivateSettingDTO();
            result.UserId = userId;
            result.UserName = userName;
            var dbSettings = _settingRepository.FindAll(m => m.UserId != null && m.UserId.Equals(userId));
            if(dbSettings.Any())
            {
                var titleSetting = dbSettings.FirstOrDefault(m => m.Name != null && m.Name.Equals(SettingDTO.Setting_Title));
                if (titleSetting != null)
                    result.Title = titleSetting.Value;
                var aboutmeSetting = dbSettings.FirstOrDefault(m => m.Name != null && m.Name.Equals(SettingDTO.Setting_AboutMe));
                if (aboutmeSetting != null)
                    result.AboutMe = aboutmeSetting.Value;
            }

            return result;
        }

        public bool SaveSetting(PrivateSettingDTO model)
        {
            var dbSettings = _settingRepository.FindAll(m => m.UserId != null && m.UserId.Equals(model.UserId));
            if (dbSettings.Any())
            {
                var titleSetting = dbSettings.FirstOrDefault(m => m.Name != null && m.Name.Equals(SettingDTO.Setting_Title));
                if (titleSetting != null)
                {
                    titleSetting.DataType = AttributeDataType.String;
                    titleSetting.Value = model.Title;
                    titleSetting.UserId = model.UserId;
                    titleSetting.UserName = model.UserName;
                    _settingRepository.Modify(titleSetting, false);
                }
                else
                {
                    titleSetting = new Setting()
                    {
                        DataType = AttributeDataType.String,
                        Name = SettingDTO.Setting_Title,
                        UserId = model.UserId,
                        UserName = model.UserName,
                        Value = model.Title
                    };
                    _settingRepository.Add(titleSetting, false);
                }

                var aboutmeSetting = dbSettings.FirstOrDefault(m => m.Name != null && m.Name.Equals(SettingDTO.Setting_AboutMe));
                if (aboutmeSetting != null)
                {
                    aboutmeSetting.DataType = AttributeDataType.String;
                    aboutmeSetting.Value = model.AboutMe;
                    aboutmeSetting.UserId = model.UserId;
                    aboutmeSetting.UserName = model.UserName;
                    _settingRepository.Modify(aboutmeSetting, false);
                }
                else
                {
                    aboutmeSetting = new Setting()
                    {
                        DataType = AttributeDataType.String,
                        Name = SettingDTO.Setting_AboutMe,
                        UserId = model.UserId,
                        UserName = model.UserName,
                        Value = model.AboutMe
                    };
                    _settingRepository.Add(aboutmeSetting, false);
                }
            }
            else
            {
                List<Setting> settings = new List<Setting>() {
                    new Setting()
                    {
                        DataType = AttributeDataType.String,
                        Name = SettingDTO.Setting_Title,
                        UserId = model.UserId,
                        UserName = model.UserName,
                        Value = model.Title
                    },
                    new Setting()
                    {
                        DataType = AttributeDataType.String,
                        Name = SettingDTO.Setting_AboutMe,
                        UserId = model.UserId,
                        UserName = model.UserName,
                        Value = model.AboutMe
                    }
                };
                _settingRepository.Add(settings, false);
            }

            return _unitOfContext.Commit() > 0;
        }

        #endregion
    }
}
