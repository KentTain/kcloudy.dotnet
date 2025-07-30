
using KC.Framework.Base;
using KC.Framework.Extension;
using KC.Framework.Tenant;
using KC.Service.DTO.Message;
using KC.Service.Message;
using KC.Service.Enums.Message;
using KC.Service.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KC.Web.Message.Controllers
{
    public class NewsBulletinController : MessageBaseController
    {
        private INewsBulletinService _newsBulletinService => ServiceProvider.GetService<INewsBulletinService>();
        public NewsBulletinController(
            Tenant tenant,
            IServiceProvider serviceProvider,
            ILogger<NewsBulletinController> logger)
            : base(tenant, serviceProvider, logger)
        {
        }

        /// <summary>
        /// 三级菜单：消息管理/新闻公告管理/新闻公告管理
        /// </summary>
        [Web.Extension.MenuFilter("新闻公告管理", "新闻公告管理", "/NewsBulletin/Index",
            Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType,
            SmallIcon = "fa fa-folder", AuthorityId = "D3F01C37-D3F1-4C5A-8207-6DB83A40AA64",
            DefaultRoleId = RoleConstants.ExecutorManagerRoleId, Order = 4, IsExtPage = false, Level = 3)]
        [Web.Extension.PermissionFilter("新闻公告管理", "新闻公告管理", "/NewsBulletin/Index",
            "D3F01C37-D3F1-4C5A-8207-6DB83A40AA64", DefaultRoleId = RoleConstants.ExecutorManagerRoleId,
            Order = 4, IsPage = true, ResultType = ResultType.ActionResult)]
        public IActionResult Index()
        {
            ViewBag.CurrentUserName = CurrentUserName;
            ViewBag.TypeList = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum<NewsBulletinType>();
            return View();
        }

        #region 新闻公告分类
        public JsonResult LoadCategoryTree(string name, NewsBulletinType? pType, int excludeId, int selectedId, bool hasAll = false, bool hasRoot = true, int maxLevel = 3)
        {
            var result = new List<NewsBulletinCategoryDTO>();
            if (hasAll)
            {
                result.Add(new NewsBulletinCategoryDTO()
                {
                    Id = 0,
                    TypeString = "",
                    Text = "所有新闻公告",
                    Description = "所有新闻公告",
                    Children = null,
                    Level = 1
                });
                result.Add(new NewsBulletinCategoryDTO()
                {
                    Id = -1,
                    TypeString = "",
                    Text = "未分类新闻公告",
                    Description = "未分配类型的新闻公告",
                    Children = null,
                    Level = 1
                });
            }
            var data = _newsBulletinService.FindCategoryTreesByName(name, pType);
            if (data != null && data.Any())
                result.AddRange(data);

            if (hasRoot)
            {
                var rootMenu = new NewsBulletinCategoryDTO() { Text = "顶级分类", Children = result, Level = 0 };
                var org = TreeNodeUtil.GetNeedLevelTreeNodeDTO(rootMenu, maxLevel, new List<int>() { excludeId }, new List<int>() { selectedId });
                return Json(new List<NewsBulletinCategoryDTO> { org });
            }

            var orgList = TreeNodeUtil.LoadNeedLevelTreeNodeDTO(result, maxLevel, new List<int>() { excludeId }, new List<int>() { selectedId });
            return Json(orgList);
        }

        public PartialViewResult GetCategoryForm(int id, int? pId, NewsBulletinType pType = NewsBulletinType.Internal)
        {
            NewsBulletinCategoryDTO model;
            if (id > 0)
            {
                model = _newsBulletinService.GetNewsBulletinCategoryById(id);
                model.IsEditMode = true;
                ViewBag.TypeList = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum<NewsBulletinType>((int)model.Type);
            }
            else
            {
                model = new NewsBulletinCategoryDTO() { ParentId = pId.HasValue && pId.Value > 0 ? pId : 0, Type = pType };
                ViewBag.TypeList = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum<NewsBulletinType>((int)pType);
            }

            return PartialView("_newsBulletinCategoryForm", model);
        }

        [Web.Extension.PermissionFilter("新闻公告管理", "保存新闻公告分类", "/NewsBulletin/SaveCategory",
            "4EAA0A79-8210-4EE0-936C-AEC1D8B3B06D", DefaultRoleId = RoleConstants.ExecutorManagerRoleId,
            Order = 2, IsPage = false, ResultType = ResultType.JsonResult)]
        public async Task<JsonResult> SaveCategory(NewsBulletinCategoryDTO model)
        {
            return await GetServiceJsonResultAsync(async () =>
            {
                if (model == null)
                    throw new ArgumentException(string.Format("传入对象为空"));

                //对象属性验证
                var errMsg = GetAllModelErrorMessages();
                if (!errMsg.IsNullOrEmpty())
                    throw new ArgumentException(errMsg);

                if (!model.IsEditMode)
                {
                    model.CreatedBy = CurrentUserId;
                    model.CreatedName = CurrentUserDisplayName;
                    model.CreatedDate = DateTime.UtcNow;
                }
                model.ModifiedBy = CurrentUserId;
                model.ModifiedName = CurrentUserDisplayName;
                model.ModifiedDate = DateTime.UtcNow;

                return await _newsBulletinService.SaveNewsBulletinCategoryAsync(model);
            });
        }

        [Web.Extension.PermissionFilter("新闻公告管理", "删除新闻公告分类", "/NewsBulletin/RemoveCategory",
            "D5C672F5-19C5-4438-A453-BD5DD16DC216", DefaultRoleId = RoleConstants.ExecutorManagerRoleId,
            Order = 3, IsPage = false, ResultType = ResultType.JsonResult)]
        public JsonResult RemoveCategory(int id)
        {
            return GetServiceJsonResult(() =>
            {
                return _newsBulletinService.SoftRemoveNewsBulletinCategoryById(id);
            });
        }

        [AllowAnonymous]
        public async Task<JsonResult> ExistCategoryName(int id, string name, string orginalName, bool isEditMode)
        {
            if (isEditMode && name.Equals(orginalName, StringComparison.OrdinalIgnoreCase))
                return Json(false);

            return Json(await _newsBulletinService.ExistCategoryNameAsync(id, name));
        }
        #endregion

        #region 新闻公告

        public JsonResult LoadNewsBulletins(int page, int rows, string name, NewsBulletinType? type, int? categoryId = 0)
        {
            var result = _newsBulletinService.LoadPaginatedNewsBulletinsByFilter(page, rows, categoryId, name, type);
            return Json(result);
        }

        /// <summary>
        /// 三级菜单：消息管理/新闻公告管理/新增/编辑新闻公告
        /// </summary>
        /// <returns></returns>
        [Web.Extension.MenuFilter("新闻公告管理", "新增/编辑新闻公告", "/NewsBulletin/GetNewsBulletinForm",
            Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType,
            SmallIcon = "fa fa-list-alt", AuthorityId = "7A430A8D-000B-4375-90DF-F4092A08E67C",
            DefaultRoleId = RoleConstants.ExecutorManagerRoleId, Order = 5, IsExtPage = true, Level = 3)]
        [Web.Extension.PermissionFilter("新增/编辑新闻公告", "新增/编辑新闻公告", "/NewsBulletin/GetNewsBulletinForm",
            "7A430A8D-000B-4375-90DF-F4092A08E67C", DefaultRoleId = RoleConstants.ExecutorManagerRoleId,
            Order = 5, IsPage = true, ResultType = ResultType.ActionResult)]
        public IActionResult GetNewsBulletinForm(int id, int selectCategoryId, NewsBulletinType? selectType)
        {
            var model = new NewsBulletinDTO() { IsShow = true };
            if (selectCategoryId > 0)
                ViewBag.selectCategoryId = selectCategoryId;
            if (selectType.HasValue)
            {
                model.Type = selectType.Value;
                ViewBag.selectType = selectType.Value;
            }
            if (id > 0)
            {
                model = _newsBulletinService.GetNewsBulletinById(id);
                model.IsEditMode = true;
                model.CategoryId = selectCategoryId > 0 ? selectCategoryId : 0;
            }

            ViewBag.TypeList = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum<NewsBulletinType>((int)model.Type);

            return View("NewsBulletinForm", model);
        }

        [Web.Extension.PermissionFilter("新增/编辑新闻公告", "保存新闻公告", "/NewsBulletin/SaveNewsBulletin",
            "2B05A70D-8542-4F36-905A-B96035E8AED7", DefaultRoleId = RoleConstants.ExecutorManagerRoleId,
            Order = 2, IsPage = false, ResultType = ResultType.JsonResult)]
        public JsonResult SaveNewsBulletin(NewsBulletinDTO model)
        {
            return GetServiceJsonResult(() =>
            {
                if (model == null)
                    throw new ArgumentException(string.Format("传入对象为空"));

                //对象属性验证
                var errMsg = GetAllModelErrorMessages();
                if (!errMsg.IsNullOrEmpty())
                    throw new ArgumentException(errMsg);

                if (!model.IsEditMode)
                {
                    model.CreatedBy = CurrentUserId;
                    model.CreatedName = CurrentUserDisplayName;
                    model.CreatedDate = DateTime.UtcNow;
                    model.Author = CurrentUserDisplayName;
                    model.AuthorEmail = CurrentUserEmail;
                }
                model.ModifiedBy = CurrentUserId;
                model.ModifiedName = CurrentUserDisplayName;
                model.ModifiedDate = DateTime.UtcNow;

                var result = _newsBulletinService.SaveNewsBulletin(model);
                if (result)
                {
                    var blobIds = new List<string>();
                    if (model.Image != null)
                        blobIds.Add(model.Image.BlobId);
                    if (model.File != null)
                        blobIds.Add(model.File.BlobId);
                    if(blobIds.Any())
                        CopyTempsToClientAzureBlob(blobIds);
                }
                return result;
            });
        }

        [Web.Extension.PermissionFilter("新增/编辑新闻公告", "删除新闻公告", "/NewsBulletin/DeleteNewsBulletin",
            "D252EA8C-4BBE-4B16-8EF9-BBD3371752F6", DefaultRoleId = RoleConstants.ExecutorManagerRoleId,
            Order = 3, IsPage = false, ResultType = ResultType.JsonResult)]
        public JsonResult DeleteNewsBulletin(int id)
        {
            return GetServiceJsonResult(() => _newsBulletinService.RemoveNewsBulletinById(id));
        }
        #endregion

        #region 二级菜单（公共页面）：新闻公告列表、新闻公告详情
        /// <summary>
        /// 二级菜单：消息管理/新闻公告列表
        /// </summary>
        /// <returns></returns>
        [Web.Extension.MenuFilter("消息管理", "新闻公告列表", "/NewsBulletin/NewsBulletinList",
            Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType,
            SmallIcon = "fa fa-list-alt", AuthorityId = "1252D9A6-AF78-415F-B835-6148EE09354A",
            DefaultRoleId = RoleConstants.DefaultRoleId, Order = 3, IsExtPage = true, Level = 2)]
        [Web.Extension.PermissionFilter("新闻公告列表", "新闻公告列表", "/NewsBulletin/NewsBulletinList",
            "1252D9A6-AF78-415F-B835-6148EE09354A", DefaultRoleId = RoleConstants.DefaultRoleId,
            Order = 3, IsPage = true, ResultType = ResultType.ActionResult)]
        public IActionResult NewsBulletinList(NewsBulletinType? type)
        {
            ViewBag.selectType = type.HasValue ? type.Value.ToString() : string.Empty;
            ViewBag.TypeList = type.HasValue 
                ? KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum<NewsBulletinType>(new List<int>() { (int)type.Value })
                : KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum<NewsBulletinType>();

            return View();
        }

        /// <summary>
        /// 二级菜单：消息管理/新闻公告详情
        /// </summary>
        /// <returns></returns>
        [Web.Extension.MenuFilter("消息管理", "新闻公告详情", "/NewsBulletin/NewsBulletinDetail",
            Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType,
            SmallIcon = "fa fa-list-alt", AuthorityId = "8D6CAF28-C3BC-4361-A7AB-EA54C4DA8E26",
            DefaultRoleId = RoleConstants.DefaultRoleId, Order = 4, IsExtPage = true, Level = 2)]
        [Web.Extension.PermissionFilter("新闻公告详情", "新闻公告详情", "/NewsBulletin/NewsBulletinDetail",
            "8D6CAF28-C3BC-4361-A7AB-EA54C4DA8E26", DefaultRoleId = RoleConstants.DefaultRoleId,
            Order = 4, IsPage = true, ResultType = ResultType.ActionResult)]
        public IActionResult NewsBulletinDetail(int id)
        {
            var model = _newsBulletinService.GetNewsBulletinById(id);
            ViewBag.TypeList = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum<NewsBulletinType>((int)model.Type);

            return View("NewsBulletinDetail", model);
        }

        #endregion

        #region 新闻公告日志
        /// <summary>
        /// 三级菜单：消息管理/新闻公告管理/新闻公告日志
        /// </summary>
        /// <returns></returns>
        [Web.Extension.MenuFilter("新闻公告管理", "新闻公告日志", "/NewsBulletin/NewsBulletinLog",
            Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType,
            SmallIcon = "fa fa-list-alt", AuthorityId = "11DC9DEC-042B-4DD6-BE59-5D324DCEE163",
            DefaultRoleId = RoleConstants.ExecutorManagerRoleId, Order = 6, IsExtPage = false, Level = 3)]
        [Web.Extension.PermissionFilter("新闻公告管理", "新闻公告日志", "/NewsBulletin/NewsBulletinLog",
            "11DC9DEC-042B-4DD6-BE59-5D324DCEE163", DefaultRoleId = RoleConstants.ExecutorManagerRoleId,
            Order = 6, IsPage = true, ResultType = ResultType.ActionResult)]
        public IActionResult NewsBulletinLog()
        {
            return View();
        }

        public JsonResult LoadNewsBulletinList(int page = 1, int rows = 10, string name = null)
        {
            var result = _newsBulletinService.FindPaginatedNewsBulletinLog(page, rows, name);

            return Json(result);
        }
        #endregion
    }
}