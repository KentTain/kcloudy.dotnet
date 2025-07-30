using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using KC.Framework.Tenant;
using KC.Web.Portal.Models;
using Microsoft.AspNetCore.Authorization;
using KC.Service.Portal;
using Microsoft.Extensions.DependencyInjection;
using KC.Service.ViewModel.Portal;
using KC.Enums.Portal;
using KC.Service.DTO.Portal;
using KC.Service.Portal.WebApiService.Business;
using KC.Service.Enums.Message;

namespace KC.Web.Portal.Controllers
{
    public class HomeController : PortalBaseController
    {
        protected IFrontWebInfoService _webInfoService => ServiceProvider.GetService<IFrontWebInfoService>();

        protected INewsBulletinApiService _newsApiService => ServiceProvider.GetService<INewsBulletinApiService>();

        public HomeController(
            Tenant tenant,
            IServiceProvider serviceProvider,
            ILogger<HomeController> logger)
            : base(tenant, serviceProvider, logger)
        {
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index(string skinCode)
        {
            // 设置ViewBag.WebSiteInfo值，提供给HomeLayout使用
            var skin = await SetWebSiteInfoViewBag();
            if (!string.IsNullOrEmpty(skinCode))
                skin = skinCode;

            var model = await _webInfoService.GetWebSitePageDetailBySkinCode(skin, "/Home/Index");
            return View(skin + "/Index", model);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Template(string skinCode, Guid id)
        {
            // 设置ViewBag.WebSiteInfo值，提供给HomeLayout使用
            var skin = await SetWebSiteInfoViewBag();
            if (!string.IsNullOrEmpty(skinCode))
                skin = skinCode;

            var model = await _webInfoService.GetWebSitePageDetailById(id);
            return View(skin + "/Template", model);
        }

        [AllowAnonymous]
        public async Task<IActionResult> CompanyInfo(string skinCode, int selectType = 0)
        {
            // 设置ViewBag.WebSiteInfo值，提供给HomeLayout使用
            var skin = await SetWebSiteInfoViewBag();
            if (!string.IsNullOrEmpty(skinCode))
                skin = skinCode;

            ViewBag.Type = selectType;
            var model = await _webInfoService.GetCompanyDetailInfoAsync();
            return View(skin + "/CompanyInfo", model);
        }

        #region 推荐商品

        [AllowAnonymous]
        public async Task<IActionResult> ProductList(string skinCode, int? catogroyId = null, string name = "")
        {
            var skin = await SetWebSiteInfoViewBag();
            if (!string.IsNullOrEmpty(skinCode))
                skin = skinCode;

            ViewBag.total = await _webInfoService.GetOfferingTotalCountAsync(catogroyId, name);
            ViewBag.name = name;
            return View(skin + "/Offering/ProductList");

        }

        [AllowAnonymous]
        public async Task<IActionResult> LoadProductData (int pageIndex, int pageSize, int? catogroyId = null, string name = "")
        {
            var model = await _webInfoService.LoadPaginatedOfferingsByNameAsync(pageIndex, pageSize, catogroyId, name);
            return Json(model);
        }

        [AllowAnonymous]
        public async Task<IActionResult> ProductDetail(string skinCode, int id)
        {
            var skin = await SetWebSiteInfoViewBag();
            if (!string.IsNullOrEmpty(skinCode))
                skin = skinCode;

            var detail = await _webInfoService.GetOfferingByIdAsync(id);
            var model = new FrontSiteDetailViewModel<RecommendOfferingDTO>();
            model.DetailInfo = detail;
            model.RecommendInfos = await _webInfoService.FindTop10RequirementsAsync();
            return View(skin + "/Offering/ProductDetail", model);
        }

        #endregion

        #region 推荐采购需求
        [AllowAnonymous]
        public async Task<IActionResult> RequirementList(string skinCode, int? catogroyId = null, string name = "", RequirementType? recommend = null)
        {
            var skin = await SetWebSiteInfoViewBag();
            if (!string.IsNullOrEmpty(skinCode))
                skin = skinCode;

            ViewBag.total = await _webInfoService.GetRequirementTotalCountAsync(catogroyId, name);
            ViewBag.name = name;

            return View(skin + "/Require/RequirementList");
        }

        [AllowAnonymous]
        public async Task<IActionResult> LoadRequirementData(int pageIndex, int pageSize, int? catogroyId = null, string name = "", RequirementType? nameType = null)
        {
            var model = await _webInfoService.LoadPaginatedRequirementsByNameAsync(pageIndex, pageSize, catogroyId, name, nameType);
            return Json(model);
        }


        [AllowAnonymous]
        public async Task<IActionResult> RequirementDetail(string skinCode, int id)
        {
            var skin = await SetWebSiteInfoViewBag();
            if (!string.IsNullOrEmpty(skinCode))
                skin = skinCode;

            var model = await _webInfoService.GetRequirementByIdAsync(id);
            return View(skin + "/Require/RequirementDetail", model);
        }

        #endregion

        #region 新闻
        [AllowAnonymous]
        public async Task<IActionResult> NewsList(string skinCode, string name = null)
        {
            var skin = await SetWebSiteInfoViewBag();
            if (!string.IsNullOrEmpty(skinCode))
                skin = skinCode;

            var model = await _newsApiService.LoadPaginatedNewsBulletins(1, 10, name, NewsBulletinType.External);
            ViewBag.total = model.total;
            return View(skin + "/News/NewsList", model);
        }

        [AllowAnonymous]
        public async Task<IActionResult> LoadNewsData(int pageIndex, int pageSize, string name = null)
        {
            var model = await _newsApiService.LoadPaginatedNewsBulletins(pageIndex, pageSize,  name, NewsBulletinType.External);
            return Json(model);
        }

        [AllowAnonymous]
        public async Task<IActionResult> NewsDetail(string skinCode, int id)
        {
            var skin = await SetWebSiteInfoViewBag();
            if (!string.IsNullOrEmpty(skinCode))
                skin = skinCode;

            var model = await _newsApiService.GetNewsBulletinById(id);
            return View(skin + "/News/NewsDetail", model);
        }
        #endregion

        #region 辅助方法

        /// <summary>
        /// 设置ViewBag.WebSiteInfo值，提供给HomeLayout使用
        /// </summary>
        /// <returns></returns>
        private async Task<string> SetWebSiteInfoViewBag()
        {
            var model = await _webInfoService.GetWebSiteInfoAsync();
            ViewBag.CanAddPortalInfo = User.Identity.IsAuthenticated && CurrentUserRoleIds.Contains(RoleConstants.OperatorManagerRoleId);
            ViewBag.CanAddNews = User.Identity.IsAuthenticated && CurrentUserRoleIds.Contains(RoleConstants.ExecutorManagerRoleId);

            var result = model != null && !string.IsNullOrEmpty(model.SkinCode)
                ? model.SkinCode.ToLower()
                : "skn2021010100001";

            ViewBag.TenantDisplayName = TenantDisplayName;
            ViewBag.WebSiteInfo = model;
            ViewBag.WebSiteHeaders = await _webInfoService.LoadWebSitePagesBySkinCode(result);

            return result;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [ResponseCache(Duration = 3600, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult AccessDenied(string returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult DownLoadLogFile(string name)
        {
            var url = ServerPath + "/logs/" + name + ".log";
            return File(url, "text/plain");
        }
        #endregion
    }
}
