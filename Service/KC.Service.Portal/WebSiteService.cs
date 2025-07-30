using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using KC.Framework.Extension;
using KC.Service.EFService;
using KC.Service.DTO.Portal;
using KC.DataAccess.Portal.Repository;
using KC.Model.Portal;
using KC.Database.IRepository;
using KC.Service.DTO;
using KC.Enums.Portal;
using KC.Database.EFRepository;
using KC.Framework.Tenant;
using KC.Framework.Base;
using KC.Storage.Util;
using System.IO;
using System.Drawing;
using KC.ThirdParty;
using KC.Common.FileHelper;
using KC.Framework.Exceptions;
using KC.Framework.LINQHelper;

namespace KC.Service.Portal
{
    public interface IWebSiteService : IEFService
    {
        #region 门户信息
        Task<WebSiteInfoDTO> GetWebSiteInfoAsync();

        Task<bool> ExistWebSiteName(string name);

        Task<bool> SaveWebSiteInfo(WebSiteInfoDTO model);

        #endregion

        #region 门户分类
        List<RecommendCategoryDTO> LoadRecommendCategoryTree(string userId, string name);
        List<RecommendCategoryDTO> LoadFatherCategories(int? id);

        RecommendCategoryDTO GetRecommendCategoryById(int id);

        Task<bool> SaveRecommendCategoryAsync(RecommendCategoryDTO model);

        Task<bool> RemoveRecommendCategoryByIdAsync(int id);
        string SoftRemoveRecommendCategoryById(int id);
        Task<bool> ExistCategoryNameAsync(int pid, string name);

        #endregion

        #region 门户装饰
        Task<PaginatedBaseDTO<WebSitePageDTO>> FindPaginatedWebSitePages(int pageIndex, int pageSize, string name, string skinName);
        Task<WebSitePageDTO> GetWebSitePageById(Guid id);
        Task<bool> ChangeWebSitePageStatusAsync(Guid id);
        Task<bool> RemoveWebSitePageAsync(Guid id);
        Task<bool> SaveWebSitePage(WebSitePageDTO model);

        Task<List<WebSiteColumnDTO>> FindWebSiteColumnsByPageId(Guid pageId);
        Task<bool> ChangeWebSiteColumnStatusAsync(Guid id);
        Task<bool> RemoveWebSiteColumnAsync(Guid id);

        Task<List<WebSiteItemDTO>> FindWebSiteItemsByColumnId(Guid columnId);
        Task<bool> ChangeWebSiteItemStatusAsync(Guid id);
        Task<bool> RemoveWebSiteItemAsync(Guid id);
        #endregion

        #region 底部导航

        List<WebSiteLinkDTO> GetTopLinks();

        PaginatedBaseDTO<WebSiteLinkDTO> LoadPaginatedLinksByTitle(int pageIndex, int pageSize, string title);

        WebSiteLinkDTO GetLinkById(int? id, LinkType? type);

        bool SaveWebSiteLink(WebSiteLinkDTO model);
        bool RemoveWebSiteLink(int id);

        #endregion
    }

    public class WebSiteService : EFServiceBase, IWebSiteService
    {
        private readonly IMapper _mapper;

        #region Construction & Repository
        /// <summary>
        /// ComAccountUnitOfWorkContext </br>
        ///     use AccountUnitOfWorkContext.Commit() to save the context change(transaction).
        /// </summary>
        protected EFUnitOfWorkContextBase _unitOfWorkContext { get; private set; }

        private readonly IWebSiteInfoRepository _webSiteInfoRepository;
        private readonly IDbRepository<WebSiteLink> _webLinkRepository;
        private readonly IRecommendCategoryRepository _redCategoryRepository;

        private readonly IWebSitePageRepository _webSitePageRepository;
        private readonly IWebSiteColumnRepository _webSiteColumnRepository;
        private readonly IDbRepository<WebSiteItem> _webSiteItemRepository;
        private readonly IDbRepository<WebSitePageLog> _webSitePageLogRepository;

        public WebSiteService(
            Tenant tenant,
            IMapper mapper,
            EFUnitOfWorkContextBase unitOfWorkContext,

            IWebSiteInfoRepository webSiteInfoRepository,
            IDbRepository<WebSiteLink> webLinkRepository,
            IRecommendCategoryRepository redCategoryRepository,

            IWebSitePageRepository webSitePageRepository,
            IWebSiteColumnRepository webSiteColumnRepository,
            IDbRepository<WebSiteItem> webSiteItemRepository,
            IDbRepository<WebSitePageLog> webSitePageLogRepository,

            System.Net.Http.IHttpClientFactory clientFactory,
            Microsoft.Extensions.Logging.ILogger<WebSiteService> logger)
            : base(tenant, clientFactory, logger)
        {
            _mapper = mapper;
            _unitOfWorkContext = unitOfWorkContext;


            _webSiteInfoRepository = webSiteInfoRepository;
            _webLinkRepository = webLinkRepository;
            _redCategoryRepository = redCategoryRepository;

            _webSitePageRepository = webSitePageRepository;
            _webSiteColumnRepository = webSiteColumnRepository;
            _webSiteItemRepository = webSiteItemRepository;
            _webSitePageLogRepository = webSitePageLogRepository;
        }
        #endregion

        #region 门户信息

        public async Task<WebSiteInfoDTO> GetWebSiteInfoAsync()
        {
            var result = await _webSiteInfoRepository.GetByFilterAsync(m => !m.IsDeleted);
            if (result == null)
                return null;

            var shopDTO = _mapper.Map<WebSiteInfoDTO>(result);
            return shopDTO;
        }
        public async Task<WebSiteInfoDTO> GetWebSiteInfoByIdAsync(int id)
        {
            var model = await _webSiteInfoRepository.GetByIdAsync(id);
            return _mapper.Map<WebSiteInfoDTO>(model);
        }

        public async Task<bool> ExistWebSiteName(string name)
        {
            var isValid = name.Length >= 2 && name.Length <= 500;
            if (!isValid || string.IsNullOrEmpty(name)) return false;

            Expression<Func<WebSiteInfo, bool>> predicate = m => m.Name.Equals(name);

            return await _webSiteInfoRepository.ExistByFilterAsync(predicate);
        }

        public async Task<bool> SaveWebSiteInfo(WebSiteInfoDTO model)
        {
            var md = _mapper.Map<WebSiteInfo>(model);
            if (!model.IsEditMode)
            {
                await _webSiteInfoRepository.AddAsync(md, false);
                return await _unitOfWorkContext.CommitAsync() > 0;
            }

            await _webSiteInfoRepository.ModifyAsync(md, null, false);
            var success = await _unitOfWorkContext.CommitAsync() > 0;
            if (success)
            {
                // 删除前端缓存
                var tenantName = Tenant.TenantName;
                var apiMethod1 = typeof(IFrontWebInfoService).GetMethod("GetWebSiteInfoAsync");
                var cacheKey1 = Util.CacheKeyUtil.CacheKeyGenerator(tenantName, apiMethod1, null);
                if (!string.IsNullOrEmpty(cacheKey1))
                    Service.CacheUtil.RemoveCache(cacheKey1);

                // 删除前端缓存
                var apiMethod2 = typeof(IFrontWebInfoService).GetMethod("GetHomeViewModelAsync");
                var cacheKey2 = Util.CacheKeyUtil.CacheKeyGenerator(tenantName, apiMethod2, null);
                if (!string.IsNullOrEmpty(cacheKey2))
                    Service.CacheUtil.RemoveCache(cacheKey2);
            }
            return success;
        }

        #endregion

        #region 门户装饰
        #region 门户页面
        public async Task<PaginatedBaseDTO<WebSitePageDTO>> FindPaginatedWebSitePages(int pageIndex, int pageSize, string name, string skinName)
        {
            Expression<Func<WebSitePage, bool>> predicate = m => true;
            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate = predicate.And(m => m.Name.Contains(name));
            }
            if (!string.IsNullOrWhiteSpace(skinName))
            {
                predicate = predicate.And(m => m.SkinName.Contains(skinName));
            }
            var data = await _webSitePageRepository.FindPagenatedListWithCountAsync(
                pageIndex, pageSize, predicate, m => m.Index, true);

            var total = data.Item1;
            var rows = _mapper.Map<List<WebSitePageDTO>>(data.Item2);
            return new PaginatedBaseDTO<WebSitePageDTO>(pageIndex, pageSize, total, rows);
        }
        public async Task<WebSitePageDTO> GetWebSitePageById(Guid id)
        {
            var data = await _webSitePageRepository.GetWebSiteColumnDetailInfoAsync(id);

            return _mapper.Map<WebSitePageDTO>(data);
        }
        public async Task<bool> ChangeWebSitePageStatusAsync(Guid id)
        {
            var model = await _webSitePageRepository.GetByIdAsync(id);
            model.IsEnable = !model.IsEnable;
            var success = await _webSitePageRepository.ModifyAsync(model, new string[] { "IsEnable" });
            if (success)
            {
                // 删除前端缓存
                var skinCode = model.SkinCode;
                var tenantName = Tenant.TenantName;
                var apiMethod1 = typeof(IFrontWebInfoService).GetMethod("LoadWebSitePagesBySkinCode");
                var cacheKey1 = Util.CacheKeyUtil.CacheKeyGenerator(tenantName, apiMethod1, new object[] { skinCode });
                if (!string.IsNullOrEmpty(cacheKey1))
                    Service.CacheUtil.RemoveCache(cacheKey1);
            }

            return success;
        }
        public async Task<bool> RemoveWebSitePageAsync(Guid id)
        {
            var model = await _webSitePageRepository.GetByIdAsync(id);
            var success = await _webSitePageRepository.SoftRemoveAsync(model);
            if (success)
            {
                _webSitePageLogRepository.Add(new WebSitePageLog()
                {
                    WebSiteColumnId = model.Id,
                    WebSiteColumnName = model.Name,
                    OperatorId = model.ModifiedBy,
                    Operator = model.ModifiedName,
                    OperateDate = DateTime.UtcNow,
                    Remark = "删除页面数据",
                });

                // 删除前端缓存
                var skinCode = model.SkinCode;
                var tenantName = Tenant.TenantName;
                var apiMethod1 = typeof(IFrontWebInfoService).GetMethod("LoadWebSitePagesBySkinCode");
                var cacheKey1 = Util.CacheKeyUtil.CacheKeyGenerator(tenantName, apiMethod1, new object[] { skinCode });
                if (!string.IsNullOrEmpty(cacheKey1))
                    Service.CacheUtil.RemoveCache(cacheKey1);
            }
            return success;
        }
        public async Task<bool> SaveWebSitePage(WebSitePageDTO model)
        {
            var md = _mapper.Map<WebSitePage>(model);
            if (model.IsEditMode)
            {
                var data = await _webSitePageRepository.GetWebSiteColumnDetailInfoAsync(md.Id);
                var dbColumns = data.WebSiteColumns.ToList();
                var dbItems = data.WebSiteColumns.SelectMany(m => m.WebSiteItems);
                var newColumns = md.WebSiteColumns.ToList();
                var newItems = md.WebSiteColumns.SelectMany(m => m.WebSiteItems);

                #region 更新页面栏目下的项目
                var itemEquality = new CommonEqualityComparer<WebSiteItem, Guid>(x => x.Id);
                //需要新增的项目
                var addItems = newItems.Except(dbItems, itemEquality);
                if (addItems.Any())
                    await _webSiteItemRepository.AddAsync(addItems, false);
                //需要删除的项目
                var deletedItems = dbItems.Except(newItems, itemEquality);
                if (deletedItems.Any())
                    await _webSiteItemRepository.RemoveAsync(deletedItems, false);
                //需要编辑的项目
                var updatedItems = newItems.Intersect(dbItems, itemEquality);
                if (updatedItems.Any())
                    await _webSiteItemRepository.ModifyAsync(updatedItems, new string[] { "Title", "SubTitle", "IsImage", "ImageOrIConCls", "Link", "LinkIsOpenNewPage", "IsShow", "Index", "Description", "CustomizeContent" }, false);
                #endregion

                #region 更新页面栏目
                var columnEquality = new CommonEqualityComparer<WebSiteColumn, Guid>(x => x.Id);
                //需要新增的项目
                var addColumns = newColumns.Except(dbColumns, columnEquality);
                if (addColumns.Any())
                    await _webSiteColumnRepository.AddAsync(addColumns, false);
                //需要删除的项目
                var deletedColumns = dbColumns.Except(newColumns, columnEquality);
                if (deletedColumns.Any())
                    await _webSiteColumnRepository.RemoveAsync(deletedColumns, false);
                //需要编辑的项目
                var updatedColumns = newColumns.Intersect(dbColumns, columnEquality);
                if (updatedColumns.Any())
                    await _webSiteColumnRepository.ModifyAsync(updatedColumns, new string[] { "Title", "SubTitle", "Type", "ItemType", "RowCount", "ColumnCount", "IsShow", "Index", "Description", "CustomizeContent" }, false);
                #endregion

                #region 更新页面
                var modifiedFields = new List<string>() { "Name", "Type", "IsEnable", "Description", "Index", "ModifiedBy", "ModifiedName", "ModifiedDate" };
                if (model.Type == WebSitePageType.Link)
                {
                    modifiedFields.Add("Link");
                    modifiedFields.Add("LinkIsOpenNewPage");
                }
                else if (model.Type == WebSitePageType.System)
                {
                    modifiedFields.Add("MainColor");
                    modifiedFields.Add("SecondaryColor");
                    modifiedFields.Add("UseMainSlide");
                    modifiedFields.Add("MainSlide");
                }
                else if (model.Type == WebSitePageType.Customize)
                {
                    model.Link = "/Home/Template/" + model.Id;
                    model.LinkIsOpenNewPage = false;
                    modifiedFields.Add("Link");
                    modifiedFields.Add("LinkIsOpenNewPage");
                    modifiedFields.Add("CustomizeContent");
                }
                await _webSitePageRepository.ModifyAsync(md, modifiedFields.ToArray(), false);
                #endregion
            }
            else
            {
                if (model.Type == WebSitePageType.Customize)
                {
                    model.Link = "/Home/Template/" + model.Id;
                    model.LinkIsOpenNewPage = false;
                }
                await _webSitePageRepository.AddAsync(md, false);
            }

            var success = await _unitOfWorkContext.CommitAsync() > 0;
            if (success)
            {
                var msg = !model.IsEditMode
                       ? "创建了新的页面数据"
                       : "修改了页面数据";
                _webSitePageLogRepository.Add(new WebSitePageLog()
                {
                    WebSiteColumnId = md.Id,
                    WebSiteColumnName = md.Name,
                    OperatorId = model.ModifiedBy,
                    Operator = model.ModifiedName,
                    OperateDate = model.ModifiedDate,
                    Remark = msg,
                });

                // 删除前端缓存
                var skinCode = model.SkinCode;
                var tenantName = Tenant.TenantName; 
                var apiMethod1 = typeof(IFrontWebInfoService).GetMethod("LoadWebSitePagesBySkinCode");
                var cacheKey1 = Util.CacheKeyUtil.CacheKeyGenerator(tenantName, apiMethod1, new object[] { skinCode });
                if (!string.IsNullOrEmpty(cacheKey1))
                    Service.CacheUtil.RemoveCache(cacheKey1);

                // 删除页面数据
                var url = model.Link;
                var apiMethod2 = typeof(IFrontWebInfoService).GetMethod("GetWebSitePageDetailBySkinCode");
                var cacheKey2 = Util.CacheKeyUtil.CacheKeyGenerator(tenantName, apiMethod2, new object[] { skinCode, url });
                if (!string.IsNullOrEmpty(cacheKey2))
                    Service.CacheUtil.RemoveCache(cacheKey2);
            }

            return success;
        }
        #endregion

        #region 门户栏目
        public async Task<List<WebSiteColumnDTO>> FindWebSiteColumnsByPageId(Guid columnId)
        {
            Expression<Func<WebSiteColumn, bool>> predicate = m => m.WebSitePageId == columnId;
            var data = await _webSiteColumnRepository.FindAllAsync(predicate, m => m.Index, true);

            return _mapper.Map<List<WebSiteColumnDTO>>(data);
        }
        public async Task<bool> ChangeWebSiteColumnStatusAsync(Guid id)
        {
            var model = await _webSiteColumnRepository.GetByIdAsync(id);
            model.IsShow = !model.IsShow;
            return await _webSiteColumnRepository.ModifyAsync(model, new string[] { "IsShow" });
        }
        public async Task<bool> RemoveWebSiteColumnAsync(Guid id)
        {
            return await _webSiteColumnRepository.SoftRemoveAsync(m => m.Id == id) > 0;
        }
        #endregion

        #region 门户栏目项目
        public async Task<List<WebSiteItemDTO>> FindWebSiteItemsByColumnId(Guid groupId)
        {
            Expression<Func<WebSiteItem, bool>> predicate = m => m.WebSiteColumnId == groupId;
            var data = await _webSiteItemRepository.FindAllAsync(predicate, m => m.Index, true);

            return _mapper.Map<List<WebSiteItemDTO>>(data);
        }
        public async Task<bool> ChangeWebSiteItemStatusAsync(Guid id)
        {
            var model = await _webSiteItemRepository.GetByIdAsync(id);
            model.IsShow = !model.IsShow;
            return await _webSiteItemRepository.ModifyAsync(model, new string[] { "IsShow" });
        }
        public async Task<bool> RemoveWebSiteItemAsync(Guid id)
        {
            return await _webSiteItemRepository.SoftRemoveAsync(m => m.Id == id) > 0;
        }
        #endregion
        #endregion

        #region 推荐分类
        public List<RecommendCategoryDTO> LoadRecommendCategoryTree(string userId, string name)
        {
            Expression<Func<RecommendCategory, bool>> predicate = m => !m.IsDeleted;
            if (!userId.IsNullOrEmpty())
            {
                predicate = predicate.And(m => m.CreatedBy.Equals(userId));
            }
            if (!name.IsNullOrEmpty())
            {
                predicate = predicate.And(m => m.Name.Contains(name));
            }

            var data = _redCategoryRepository.FindTreeNodesWithNestParentAndChildByFilter(predicate);
            return _mapper.Map<List<RecommendCategoryDTO>>(data);
        }

        public List<RecommendCategoryDTO> LoadFatherCategories(int? id)
        {
            var result = _redCategoryRepository.FindFatherCategory(id);
            return _mapper.Map<List<RecommendCategoryDTO>>(result);
        }

        public RecommendCategoryDTO GetRecommendCategoryById(int id)
        {
            //var data = _docCategoryRepository.GetRecommendCategoryWithChildrenById(id);
            var data = _redCategoryRepository.GetTreeNodeWithNestChildById(id);
            return _mapper.Map<RecommendCategoryDTO>(data);
        }


        public async Task<bool> SaveRecommendCategoryAsync(RecommendCategoryDTO model)
        {
            if (model == null)
                throw new ArgumentException(string.Format("传入对象为空"));

            var exist = await _redCategoryRepository.ExistByTreeNameAsync(model.Id, model.ParentId, model.Text);
            if (exist)
                throw new ArgumentException("已存在名为【" + model.Text + "】的分类，请勿重复添加！");
            if (model.ParentId <= 0)
                model.ParentId = null;

            var data = _mapper.Map<RecommendCategory>(model);
            var success = await _redCategoryRepository.SaveRecommendCategoryAsync(data);
            if (success)
                model.Id = data.Id;
            return success;
        }

        public async Task<bool> RemoveRecommendCategoryByIdAsync(int id)
        {
            if (id <= 0)
                return true;

            return await _redCategoryRepository.RemoveByIdAsync(id);
        }

        public string SoftRemoveRecommendCategoryById(int id)
        {
            if (id <= 0)
                return "未找到Id【" + id + "】的文件分类记录！";

            var result = _redCategoryRepository.GetRecommendCategoryWithChildrenById(id);
            if (result == null)
                return "未找到Id【" + id + "】的文件分类记录！";

            if (result.ChildNodes.Any(x => x.IsDeleted == false))
                return "文件分类【" + result.Name + "】下有子级文件分类，请先将其子级文件分类移到到其他文件分类下再进行删除！";

            result.IsDeleted = true;
            var res = _redCategoryRepository.Modify(result, new[] { "IsDeleted" });
            if (!res)
                return "删除文件分类失败，请重试！";

            return string.Empty;
        }

        public async Task<bool> ExistCategoryNameAsync(int pid, string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name", "分类名称为空.");

            Expression<Func<RecommendCategory, bool>> predicate = m => !m.IsDeleted && m.Name.Equals(name);
            if (pid == 0)
                predicate = predicate.And(c => c.ParentId == null);
            else
                predicate = predicate.And(c => c.ParentId == pid);

            return await _redCategoryRepository.ExistByFilterAsync(predicate);
        }

        
        #endregion

        #region 底部导航

        public List<WebSiteLinkDTO> GetTopLinks()
        {
            var result = _webLinkRepository.FindAll(c => c.IsNav && !c.IsDeleted).Select(b => new WebSiteLinkDTO()
            {
                Title = b.LinkType.ToDescription(),
                Id = b.Id,
                LinkType = b.LinkType
            }).ToList();
            return result;
        }

        public PaginatedBaseDTO<WebSiteLinkDTO> LoadPaginatedLinksByTitle(int pageIndex, int pageSize, string title)
        {
            Expression<Func<WebSiteLink, bool>> predicate = m => !m.IsDeleted && m.LinkType == LinkType.Links;
            if (!string.IsNullOrWhiteSpace(title))
            {
                predicate = predicate.And(m => m.Title.Contains(title));
            }

            var data = _webLinkRepository.FindPagenatedListWithCount(pageIndex, pageSize, predicate, m => m.ModifiedDate, false);
            var total = data.Item1;
            var rows = _mapper.Map<List<WebSiteLinkDTO>>(data.Item2);
            return new PaginatedBaseDTO<WebSiteLinkDTO>(pageIndex, pageSize, total, rows);
        }

        public WebSiteLinkDTO GetLinkById(int? id, LinkType? type)
        {
            if (id.HasValue && type.HasValue)
            {
                return _mapper.Map<WebSiteLinkDTO>(_webLinkRepository.GetByFilter(c => c.Id == id.Value && c.LinkType == type.Value));
            }
            if (id.HasValue)
            {
                return _mapper.Map<WebSiteLinkDTO>(_webLinkRepository.GetByFilter(c => c.Id == id));
            }
            return _mapper.Map<WebSiteLinkDTO>(_webLinkRepository.FindAll(c => c.LinkType == type.Value).FirstOrDefault());
        }

        public bool SaveWebSiteLink(WebSiteLinkDTO model)
        {
            var result = _mapper.Map<WebSiteLink>(model);
            //同一分类同一标题只能有一个
            Expression<Func<WebSiteLink, bool>> predicate = m => !m.IsDeleted && m.LinkType == result.LinkType && m.Title == result.Title;
            if (model.Id > 0)
            {
                predicate = predicate.And(m => m.Id != model.Id);
            }
            var data = _webLinkRepository.GetByFilter(predicate);
            if (data != null)
            {
                throw new BusinessPromptException("同一分类的标题只能添加一个.");
            }

            if (model.Id > 0)
            {
                return _webLinkRepository.Modify(result, new[] { "Title", "Content", "Links", "LinkType" });
            }
            return _webLinkRepository.Add(result);
        }

        public bool RemoveWebSiteLink(int id)
        {
            return _webLinkRepository.RemoveById(id);
        }

        #endregion

    }
}
