using KC.Framework.Extension;
using KC.Framework.Tenant;
using KC.Service.DTO.Dict;
using KC.Service.WebApiService.Business;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KC.Web.Portal.Controllers
{
    public abstract class PortalBaseController : Web.Controllers.TenantWebBaseController
    {
        protected IAccountApiService AccountApiService => ServiceProvider.GetService<IAccountApiService>();
        protected IDictionaryApiService DictionaryApiService => ServiceProvider.GetService<IDictionaryApiService>();
        public PortalBaseController(
            Tenant tenant,
            IServiceProvider serviceProvider,
            ILogger logger)
            : base(tenant, serviceProvider, logger)
        {
        }

        #region 选小图标控件: Shared/_SelectUserPartial.cshtml
        [Microsoft.AspNetCore.Mvc.HttpGet]
        public Microsoft.AspNetCore.Mvc.PartialViewResult _SelectIconPartial()
        {
            return PartialView();
        }

        #endregion 选小图标控件: Shared/_SelectUserPartial.cshtml

        #region 选人控件: Shared/_SelectUserPartial.cshtml

        /// <summary>
        /// 获取所有的部门信息及下属员工
        /// </summary>
        /// <returns></returns>
        [Microsoft.AspNetCore.Mvc.HttpGet]
        public Microsoft.AspNetCore.Mvc.JsonResult GetRootOrganizationsWithUsers()
        {
            var res = AccountApiService.LoadOrgTreesWithUsers();
            return Json(res);
        }

        /// <summary>
        /// 获取相关部门以及角色信息及下属员工
        /// </summary>
        /// <param name="searchModel">筛选条件：部门Ids列表、角色Ids列表</param>
        /// <returns></returns>
        [Microsoft.AspNetCore.Mvc.HttpPost]
        public async Task<Microsoft.AspNetCore.Mvc.JsonResult> GetOrgsWithUsersByRoleIdsAndOrgids(Service.DTO.Search.OrgTreesAndRolesWithUsersSearchDTO searchModel)
        {
            var orgs = await AccountApiService.LoadOrgTreesWithUsersByOrgIds(searchModel);
            var roles = await AccountApiService.LoadRolesWithUsersByRoleIds(searchModel);
            return this.Json(new { orgInfos = orgs, roleInfos = roles });
        }

        #endregion 选人控件: Shared/_SelectUserPartial.cshtml

        #region 选择省市控件

        /// <summary>
        /// 获取省份信息
        /// </summary>
        /// <returns></returns>
        public IActionResult LoadAllDictValuesByCode(string code)
        {
            return base.GetServiceJsonResult(() =>
            {
                return DictionaryApiService.LoadAllDictValuesByTypeCode(code);
            });
        }

        /// <summary>
        /// 获取省份信息
        /// </summary>
        /// <returns></returns>
        public IActionResult LoadProvinceList()
        {
            return base.GetServiceJsonResult(() =>
            {
                return DictionaryApiService.LoadAllProvinces();
            });
        }

        /// <summary>
        /// 获取城市信息
        /// </summary>
        /// <returns></returns>
        public IActionResult LoadCityList()
        {
            return base.GetServiceJsonResult(() =>
            {
                return DictionaryApiService.LoadAllCities();
            });
        }

        /// <summary>
        /// 根据省份Id，获取城市信息
        /// </summary>
        /// <param name="provinceID">省份Id</param>
        /// <returns></returns>
        public IActionResult LoadCitiesByProvinceId(int provinceID)
        {
            return base.GetServiceJsonResult(() =>
            {
                return DictionaryApiService.LoadCitiesByProvinceId(provinceID);
            });
        }

        public IActionResult GetRootIndustryClassfications(string key)
        {
            return base.GetServiceJsonResult(() =>
            {
                var result = new List<IndustryClassficationDTO>();
                var root = DictionaryApiService.LoadRootIndustryClassfications();

                if (string.IsNullOrEmpty(key))
                    return root;

                Func<IndustryClassficationDTO, bool> predicate = m => m.Text.Contains(key);
                foreach (var industry in root)
                {
                    KC.Service.Util.TreeNodeUtil.NestTreeNodeDTO(industry, result, null, predicate);
                }

                return result;
            });
        }
        #endregion 选择省市控件

        protected string GetModelErrors()
        {
            var errorMsgs = ModelState
                        .Where(m => m.Value.Errors.Any())
                        .SelectMany(x => x.Value.Errors);
            var errs = errorMsgs
                .Select(e => e.ErrorMessage)
                .ToCommaSeparatedString();
            return errs;
        }
    }
}