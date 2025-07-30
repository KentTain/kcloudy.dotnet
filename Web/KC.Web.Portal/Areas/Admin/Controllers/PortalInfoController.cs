using KC.Common;
using KC.Enums.Portal;
using KC.Framework.Base;
using KC.Framework.Extension;
using KC.Framework.Tenant;
using KC.Service.DTO;
using KC.Service.DTO.Portal;
using KC.Service.Portal;
using KC.Service.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KC.Web.Portal.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PortalInfoController : Portal.Controllers.PortalBaseController
    {
        private IWebSiteService webSiteService => ServiceProvider.GetService<IWebSiteService>();
        private IRecommendService _recommendService => ServiceProvider.GetService<IRecommendService>();
        private ICompanyInfoService _companyInfoService => ServiceProvider.GetService<ICompanyInfoService>();
        public PortalInfoController(
            Tenant tenant,
            IServiceProvider serviceProvider,
            ILogger<PortalInfoController> logger)
            : base(tenant, serviceProvider, logger)
        {
        }

        #region 三级菜单：门户管理/门户信息管理/门户基本信息
        /// <summary>
        /// 三级菜单：门户管理/门户信息管理/门户信息
        /// </summary>
        [Web.Extension.MenuFilter("门户信息管理", "门户基本信息", "/Admin/PortalInfo/PortalBasicInfo",
            Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType, SmallIcon = "fa fa-list-alt", AuthorityId = "9F33AEA3-76E1-49B5-A214-5AC0DAB8BECF",
            DefaultRoleId = RoleConstants.OperatorManagerRoleId, Order = 1, IsExtPage = false, Level = 3)]
        [Web.Extension.PermissionFilter("门户基本信息", "门户基本信息", "/Admin/PortalInfo/PortalBasicInfo", "9F33AEA3-76E1-49B5-A214-5AC0DAB8BECF",
            DefaultRoleId = RoleConstants.OperatorManagerRoleId, Order = 11, IsPage = true, ResultType = ResultType.ActionResult)]
        public async Task<IActionResult>  PortalBasicInfo()
        {
            var model = await webSiteService.GetWebSiteInfoAsync();
            if (model == null)
            {
                var data = await _companyInfoService.GetCompanyInfoAsync();
                // 获取企业Logo，并设置新的BlobId，以便区分企业Logo和网站Logo
                var blob = data != null && data.CompanyLogoBlob != null
                    ? data.CompanyLogo 
                    : Tenant.TenantLogo;
                model = new WebSiteInfoDTO()
                {
                    Name = data != null ? data.CompanyName : Tenant.TenantDisplayName,
                    LogoImage = blob,
                    ServiceDate = "周一至周五",
                    ServiceTime = "上午9:00至下午6:00",
                };
            }
            else
            {
                model.IsEditMode = true;
            }

            return View(model);
        }

        //EasyUi validatebox Remote validate use setTimeout: http://www.cnblogs.com/qiancheng509/archive/2012/11/23/2783700.html
        //https://uule.iteye.com/blog/1849690
        [AllowAnonymous]
        public async Task<JsonResult> ExistPortalName(string name, string orginalName, bool isEditMode) //邮箱
        {
            if (isEditMode && name.Equals(orginalName, StringComparison.OrdinalIgnoreCase))
            {
                return Json(true);
            }

            var result = await webSiteService.ExistWebSiteName(name);
            return Json(!result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Web.Extension.PermissionFilter("门户基本信息", "保存门户基本信息", "/Admin/PortalInfo/SaveWebSiteInfo", "11006F6E-6372-4C94-B34B-4EADC7B58AF0",
            DefaultRoleId = RoleConstants.OperatorManagerRoleId, Order = 1, IsPage = false, ResultType = ResultType.JsonResult)]
        public async Task<IActionResult> SaveWebSiteInfo(WebSiteInfoDTO model)
        {
            return await GetServiceJsonResultAsync(async () =>
            {
                if (!model.IsEditMode)
                {
                    model.CreatedBy = CurrentUserId;
                    model.CreatedName = CurrentUserDisplayName;
                    model.CreatedDate = DateTime.UtcNow;
                }
                model.ModifiedBy = CurrentUserId;
                model.ModifiedName = CurrentUserDisplayName;
                model.ModifiedDate = DateTime.UtcNow;
                return await webSiteService.SaveWebSiteInfo(model);
            });
        }
        #endregion

        #region 三级菜单：门户管理/门户信息管理/门户分类信息
        /// <summary>
        /// 三级菜单：门户管理/门户信息管理/门户分类信息
        /// </summary>
        [Web.Extension.MenuFilter("门户信息管理", "门户分类信息", "/Admin/PortalInfo/CategoryList",
            Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType,
            SmallIcon = "fa fa-list-alt", AuthorityId = "E563917E-4956-422D-93DC-5F5FD9503B55",
            DefaultRoleId = RoleConstants.OperatorManagerRoleId, Order = 2, IsExtPage = false, Level = 3)]
        [Web.Extension.PermissionFilter("门户分类信息", "门户分类信息", "/Admin/PortalInfo/CategoryList",
            "E563917E-4956-422D-93DC-5F5FD9503B55", DefaultRoleId = RoleConstants.OperatorManagerRoleId,
            Order = 12, IsPage = true, ResultType = ResultType.ActionResult)]
        public IActionResult CategoryList()
        {
            ViewBag.StatusList = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnumWithAll<RecommendStatus>(new List<RecommendStatus>() { RecommendStatus.Disagree });

            return View();
        }

        public JsonResult LoadRecommendCategoryTree(string name, int excludeId, int selectedId, bool hasAll = false, bool hasRoot = true, int maxLevel = 3)
        {
            var result = new List<RecommendCategoryDTO>();
            if (hasAll)
            {
                result.Add(new RecommendCategoryDTO()
                {
                    Id = 0,
                    Text = "所有推荐",
                    Children = null,
                    Level = 1
                });
                result.Add(new RecommendCategoryDTO()
                {
                    Id = -1,
                    Text = "未分类推荐",
                    Children = null,
                    Level = 1
                });
            }

            var data = webSiteService.LoadRecommendCategoryTree(CurrentUserId, name);
            if (data != null && data.Count() > 0)
                result.AddRange(data);

            if (hasRoot)
            {
                var rootMenu = new RecommendCategoryDTO() { Text = "顶级分类", Children = result, Level = 0 };
                var org = TreeNodeUtil.GetNeedLevelTreeNodeDTO(rootMenu, maxLevel, new List<int>() { excludeId }, new List<int>() { selectedId });
                return Json(new List<RecommendCategoryDTO> { org });
            }

            var orgList = TreeNodeUtil.LoadNeedLevelTreeNodeDTO(result, maxLevel, new List<int>() { excludeId }, new List<int>() { selectedId });
            return Json(orgList);
        }

        public PartialViewResult GetRecommendCategory(int id, int? pid)
        {
            RecommendCategoryDTO model;
            if (id > 0)
            {
                model = webSiteService.GetRecommendCategoryById(id);
                model.IsEditMode = true;
            }
            else
            {
                model = new RecommendCategoryDTO() { ParentId = pid.HasValue && pid.Value > 0 ? pid : 0 };
            }
            var dropItems = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum(null, new List<RecommendStatus>());
            ViewBag.LableTypeList = dropItems;

            return PartialView("_categoryForm", model);
        }

        [Web.Extension.PermissionFilter("门户分类信息", "保存门户分类", "/Admin/PortalInfo/SaveRecommendCategory",
            "6868DCC7-3911-4F7D-9DE9-3659A16A2378", DefaultRoleId = RoleConstants.OperatorManagerRoleId,
            Order = 2, IsPage = false, ResultType = ResultType.JsonResult)]
        public async Task<JsonResult> SaveRecommendCategory(RecommendCategoryDTO model)
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

                return await webSiteService.SaveRecommendCategoryAsync(model); ;
            });
        }

        [Web.Extension.PermissionFilter("门户分类信息", "删除门户分类", "/Admin/PortalInfo/RemoveRecommendCategory",
            "50364705-C3D4-45D4-A80B-B382626B03F2", DefaultRoleId = RoleConstants.OperatorManagerRoleId,
            Order = 3, IsPage = false, ResultType = ResultType.JsonResult)]
        public JsonResult RemoveRecommendCategory(int id)
        {
            return GetServiceJsonResult(() =>
            {
                var message = webSiteService.SoftRemoveRecommendCategoryById(id);
                if (string.IsNullOrEmpty(message))
                    throw new Exception(message);

                return true;
            });
        }

        [AllowAnonymous]
        public async Task<JsonResult> ExistCategoryName(int id, string name, string orginalName, bool isEditMode)
        {
            if (isEditMode && name.Equals(orginalName, StringComparison.OrdinalIgnoreCase))
                return Json(false);

            return Json(await webSiteService.ExistCategoryNameAsync(id, name));
        }

        public JsonResult FindRecommendInfos(int page, int rows, string name, int? categoryId = 0)
        {
            var result = _recommendService.LoadPaginatedRecommendInfosByFilter(page, rows, categoryId, name);
            return Json(result);
        }

        #endregion

        #region 三级菜单：门户管理/门户信息管理/门户友情链接
        /// <summary>
        /// 三级菜单：门户管理/门户信息管理/门户友情链接
        /// </summary>
        [Web.Extension.MenuFilter("门户信息管理", "门户友情链接", "/Admin/PortalInfo/WebSiteLink",
            Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType, SmallIcon = "fa fa-list-alt", AuthorityId = "ED87A64B-66B7-4CD1-B3C5-9DEB7A630FC3",
            DefaultRoleId = RoleConstants.OperatorManagerRoleId, Order = 3, IsExtPage = false, Level = 3)]
        [Web.Extension.PermissionFilter("门户友情链接", "门户友情链接", "/Admin/PortalInfo/WebSiteLink", "ED87A64B-66B7-4CD1-B3C5-9DEB7A630FC3",
            DefaultRoleId = RoleConstants.OperatorManagerRoleId, Order = 13, IsPage = true, ResultType = ResultType.ActionResult)]
        public IActionResult WebSiteLink()
        {
            return View();
        }

        /// <summary>
        /// 获取文档详情列表
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public JsonResult LoadWebSiteLinks(int page, int rows, string name)
        {
            var result = webSiteService.LoadPaginatedLinksByTitle(page, rows, name);
            return Json(result);
        }

        /// <summary>
        /// 根据id返回界面
        /// </summary>
        /// <param name="id">相对应的数据id</param>
        /// <returns></returns>
        public PartialViewResult GetWebSiteLinkForm(int id)
        {
            var model = id != 0
                ? webSiteService.GetLinkById(id, null)
                : new WebSiteLinkDTO()
                {
                    LinkType = LinkType.Links,
                };
            ViewBag.LinkTypes = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnumWithAll<LinkType>();
            return PartialView("_webSiteLinkForm", model);
        }

        /// <summary>
        /// 新增/编辑文档
        /// </summary>
        /// <param name="cateId"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [Web.Extension.PermissionFilter("门户友情链接", "保存门户友情链接", "/Admin/PortalInfo/SaveWebSiteLink", "39CA9039-0CCB-42B2-A33B-DEE85A4B0B9A",
            DefaultRoleId = RoleConstants.OperatorManagerRoleId, Order = 1, IsPage = false, ResultType = ResultType.JsonResult)]
        public JsonResult SaveWebSiteLink(WebSiteLinkDTO model)
        {
            return GetServiceJsonResult(() =>
            {
                if (model.Id != 0)
                {
                    model.CreatedBy = CurrentUserId;
                    model.CreatedName = CurrentUserDisplayName;
                    model.CreatedDate = DateTime.UtcNow;
                }
                model.ModifiedBy = CurrentUserId;
                model.ModifiedName = CurrentUserDisplayName;
                model.ModifiedDate = DateTime.UtcNow;

                return webSiteService.SaveWebSiteLink(model);
            });
        }

        /// <summary>
        /// 根据id删除相应文档
        /// </summary>
        /// <param name="id">文档对应的id</param>
        /// <returns></returns>
        [Web.Extension.PermissionFilter("门户友情链接", "删除门户友情链接", "/Admin/PortalInfo/DeleteWebSiteLink", "B31AFD53-5251-4BEF-8CD3-BC02E65FBF98",
            DefaultRoleId = RoleConstants.OperatorManagerRoleId, Order = 2, IsPage = false, ResultType = ResultType.JsonResult)]
        public JsonResult DeleteWebSiteLink(int id)
        {
            return GetServiceJsonResult(() => webSiteService.RemoveWebSiteLink(id));
        }
        #endregion

        
    }
}
