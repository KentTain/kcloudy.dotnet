using KC.Common;
using KC.Enums.Portal;
using KC.Framework.Base;
using KC.Framework.Tenant;
using KC.Service.DTO.Portal;
using KC.Service.Portal;
using KC.Service.Portal.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace KC.Web.Portal.Admin.Controllers
{
    [Area("Admin")]
    public class PortalDecoratorController : Portal.Controllers.PortalBaseController
    {
        private IWebSiteService webSiteService => ServiceProvider.GetService<IWebSiteService>();
        private ICompanyInfoService _companyInfoService => ServiceProvider.GetService<ICompanyInfoService>();
        public PortalDecoratorController(
            Tenant tenant,
            IServiceProvider serviceProvider,
            ILogger<HomeController> logger)
            : base(tenant, serviceProvider, logger)
        {
        }

        #region 三级菜单：门户管理/门户装饰管理/门户皮肤

        /// <summary>
        /// 三级菜单：门户管理/门户装饰管理/门户皮肤
        /// </summary>
        [Web.Extension.MenuFilter("门户装饰管理", "门户皮肤", "/Admin/PortalDecorator/PortalSkin",
            Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType,
            SmallIcon = "fa fa-file-code-o", AuthorityId = "06C62C74-70C7-434A-99DC-5EF35384612F",
            DefaultRoleId = RoleConstants.OperatorManagerRoleId, Order = 1, IsExtPage = false, Level = 3)]
        [Web.Extension.PermissionFilter("门户皮肤", "门户皮肤", "/Admin/PortalDecorator/PortalSkin",
            "06C62C74-70C7-434A-99DC-5EF35384612F", DefaultRoleId = RoleConstants.OperatorManagerRoleId,
            Order = 31, IsPage = true, ResultType = ResultType.ActionResult)]
        public async Task<IActionResult> PortalSkin()
        {
            var model = await webSiteService.GetWebSiteInfoAsync();
            if (model == null)
            {
                var data = await _companyInfoService.GetCompanyInfoAsync();
                model = new WebSiteInfoDTO()
                {
                    Name = data != null ? data.CompanyName : Tenant.TenantDisplayName,
                    LogoImage = data != null ? data.CompanyLogo : Tenant.TenantLogo,
                    ServiceDate = "周一至周五",
                    ServiceTime = "上午9:00至下午6:00",
                    SkinCode= "skn2021010100001",
                    SkinName = "企业门户网设计",
                };
            }
            else
            {
                model.IsEditMode = true;
            }

            ViewBag.AllSkins = GetAllSkins(model.SkinCode);

            return View(model);
        }

        private List<WebSiteSkinDTO> GetAllSkins(string selectedCode)
        {
            var result = new List<WebSiteSkinDTO>() 
            { 
                new WebSiteSkinDTO()
                {
                    SkinCode = "skn2021010100001",
                    SkinName= "企业门户网设计",
                    SkinImageUrl = "/images/skin/skn2021010100001.png",
                    SkinDescription = "用于于展示企业风采及其相关信息，适用于企业产品较少的企业",
                    IsSelected = selectedCode =="skn2021010100001",
                },
                new WebSiteSkinDTO()
                {
                    SkinCode = "skn2021010100002",
                    SkinName= "企业电商设计",
                    SkinImageUrl = "/images/skin/skn2021010100002.png",
                    SkinDescription = "用于展示企业产品及其相关信息，适用于企业产品类目较少的企业",
                    IsSelected = selectedCode =="skn2021010100001",
                },
                new WebSiteSkinDTO()
                {
                    SkinCode = "skn2021010100003",
                    SkinName= "集团企业平台设计",
                    SkinImageUrl = "/images/skin/skn2021010100003.png",
                    SkinDescription = "用于展示集团企业上下游产品品类，适用于集团企业",
                    IsSelected = selectedCode =="skn2021010100001",
                },
            };

            return result;
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Web.Extension.PermissionFilter("门户皮肤", "保存门户皮肤", "/Admin/PortalDecorator/SavePortalSkin",
            "11C4A7A3-1205-47C1-959F-247D9EA99FFE", DefaultRoleId = RoleConstants.OperatorManagerRoleId,
            Order = 2, IsPage = false, ResultType = ResultType.JsonResult)]
        public async Task<IActionResult> SavePortalSkin(WebSiteInfoDTO model)
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

        #region 三级菜单：门户管理/门户装饰管理/门户页面
        [Web.Extension.MenuFilter("门户装饰管理", "门户页面", "/Admin/PortalDecorator/PortalPageList",
        Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType, SmallIcon = "fa fa-database", AuthorityId = "DEFD31B6-397B-4828-899F-6F10B1643E23",
        DefaultRoleId = RoleConstants.OperatorManagerRoleId, Order = 1, IsExtPage = false, Level = 3)]
        [Web.Extension.PermissionFilter("门户页面", "门户页面", "/Admin/PortalDecorator/PortalPageList", "DEFD31B6-397B-4828-899F-6F10B1643E23",
            DefaultRoleId = RoleConstants.OperatorManagerRoleId, Order = 32, IsPage = true, ResultType = ResultType.ActionResult)]
        public IActionResult PortalPageList()
        {
            return View();
        }

        public async Task<IActionResult> LoadWebSitePageList(int page, int rows, string name, string skinName)
        {
            var result = await webSiteService.FindPaginatedWebSitePages(page, rows, name, skinName);
            return Json(result);
        }
        public async Task<IActionResult> LoadWebSiteColumnList(Guid pageId)
        {
            var result = await webSiteService.FindWebSiteColumnsByPageId(pageId);
            return Json(result);
        }
        public async Task<IActionResult> LoadWebSiteItemList(Guid columnId)
        {
            var result = await webSiteService.FindWebSiteItemsByColumnId(columnId);
            return Json(result);
        }

        [Web.Extension.PermissionFilter("门户页面", "启用/禁止门户页面", "/Admin/PortalDecorator/ChangeWebSitePageStatus", "8DB139EF-2CAB-4737-843F-108111728A1B",
            DefaultRoleId = RoleConstants.OperatorManagerRoleId, Order = 1, IsPage = false, ResultType = ResultType.JsonResult)]
        public async Task<IActionResult> ChangeWebSitePageStatus(Guid id)
        {
            return await GetServiceJsonResultAsync(async () =>
            {
                return await webSiteService.ChangeWebSitePageStatusAsync(id);
            });
        }

        [Web.Extension.PermissionFilter("门户页面", "删除门户页面", "/Admin/PortalDecorator/RemoveWebSitePage", "EBCB8106-709A-416F-A95C-0E955C9F1156",
            DefaultRoleId = RoleConstants.OperatorManagerRoleId, Order = 2, IsPage = false, ResultType = ResultType.JsonResult)]
        public async Task<IActionResult> RemoveWebSitePage(Guid id)
        {
            return await GetServiceJsonResultAsync(async () =>
            {
                return await webSiteService.RemoveWebSitePageAsync(id);
            });
        }

        [Web.Extension.PermissionFilter("门户页面", "启用/禁止门户页面栏目", "/Admin/PortalDecorator/ChangeWebSiteColumnStatus", "FF8CD68F-238B-42B8-B67E-99C6894E8466",
            DefaultRoleId = RoleConstants.OperatorManagerRoleId, Order = 3, IsPage = false, ResultType = ResultType.JsonResult)]
        public async Task<IActionResult> ChangeWebSiteColumnStatus(Guid id)
        {
            return await GetServiceJsonResultAsync(async () =>
            {
                return await webSiteService.ChangeWebSiteColumnStatusAsync(id);
            });
        }
        [Web.Extension.PermissionFilter("门户页面", "删除门户页面栏目", "/Admin/PortalDecorator/RemoveWebSiteColumn", "D6FCEAEE-9954-4B40-B395-A480BCC7A9F0",
            DefaultRoleId = RoleConstants.OperatorManagerRoleId, Order = 4, IsPage = false, ResultType = ResultType.JsonResult)]
        public async Task<IActionResult> RemoveWebSiteColumn(Guid id)
        {
            return await GetServiceJsonResultAsync(async () =>
            {
                return await webSiteService.RemoveWebSiteColumnAsync(id);
            });
        }
        #endregion

        #region 三级菜单：门户管理/门户装饰管理/新增/编辑门户页面
        /// <summary>
        /// 三级菜单：门户管理/门户装饰管理/新增/编辑门户页面
        /// </summary>
        [Web.Extension.MenuFilter("门户装饰管理", "新增/编辑门户页面", "/Admin/PortalDecorator/GetWebSitePageForm",
        Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType, SmallIcon = "fa fa-list-alt", AuthorityId = "BDE216C1-5714-41A4-8F05-81C917A28681",
        DefaultRoleId = RoleConstants.OperatorManagerRoleId, Order = 2, IsExtPage = true, Level = 3)]
        [Web.Extension.PermissionFilter("新增/编辑门户页面", "新增/编辑门户页面", "/Admin/PortalDecorator/GetWebSitePageForm", "BDE216C1-5714-41A4-8F05-81C917A28681",
            DefaultRoleId = RoleConstants.OperatorManagerRoleId, Order = 22, IsPage = true, ResultType = ResultType.ActionResult)]
        public async Task<PartialViewResult> GetWebSitePageForm(Guid id)
        {
            var model = new WebSitePageDTO();
            if (id != Guid.Empty)
            {
                model = await webSiteService.GetWebSitePageById(id);
                model.IsEditMode = true;
            }
            else
            {
                // 获取网站首页轮播图，并设置新的BlobId，以便区分网站首页轮播图
                var websiteInfo = await webSiteService.GetWebSiteInfoAsync();
                model.Id = Guid.NewGuid();
                model.Name = "自定义页面";
                model.SkinCode = websiteInfo.SkinCode;
                model.SkinName = websiteInfo.SkinName;
                model.Type = WebSitePageType.Customize;
                model.Status = WorkflowBusStatus.Draft;
                model.IsEnable = true;
                model.MainColor = "#2277DD";
                model.SecondaryColor = "#33DDFF";
                model.UseMainSlide = !string.IsNullOrEmpty(websiteInfo?.HomePageSlide);
                model.MainSlide = websiteInfo?.HomePageSlide;
                model.CanEdit = true;
            }
            ViewBag.ColumnTypes = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum<WebSiteColumnType>((int)WebSiteColumnType.Card);
            ViewBag.StatusList = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnumWithAll<RecommendStatus>();
            return PartialView("PortalPageForm", model);
        }

        [HttpPost]
        [Web.Extension.PermissionFilter("新增/编辑门户页面", "保存门户页面", "/Admin/PortalDecorator/SaveWebSitePage", "6B26AFCA-EB62-4AC7-97BB-71CBB0B10A03",
            DefaultRoleId = RoleConstants.OperatorManagerRoleId, Order = 2, IsPage = false, ResultType = ResultType.JsonResult)]
        public async Task<IActionResult> SaveWebSitePage(WebSitePageDTO model)
        {
            return await GetServiceJsonResultAsync(async () =>
            {
                //model.BusinessModel = Request.Form["BusinessModel"];

                if (!model.IsEditMode)
                {
                    model.CreatedBy = CurrentUserId;
                    model.CreatedName = CurrentUserDisplayName;
                    model.CreatedDate = DateTime.UtcNow;
                }
                model.ModifiedBy = CurrentUserId;
                model.ModifiedName = CurrentUserDisplayName;
                model.ModifiedDate = DateTime.UtcNow;

                return await webSiteService.SaveWebSitePage(model);
            });

        }

        [Web.Extension.PermissionFilter("新增/编辑门户页面", "删除门户页面栏目下的项目", "/Admin/PortalDecorator/RemoveWebSiteItem", "973DC918-E00E-4157-AC4B-74480CD25C6A",
            DefaultRoleId = RoleConstants.OperatorManagerRoleId, Order = 2, IsPage = false, ResultType = ResultType.JsonResult)]
        public async Task<IActionResult> RemoveWebSiteItem(Guid id)
        {
            return await GetServiceJsonResultAsync(async () =>
            {
                return await webSiteService.RemoveWebSiteItemAsync(id);
            });

        }
        
        #endregion


    }
}
