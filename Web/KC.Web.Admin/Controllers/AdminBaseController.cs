using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KC.Service.Admin;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using KC.Framework.SecurityHelper;

namespace KC.Web.Admin.Controllers
{
    //[Microsoft.AspNetCore.Authorization.Authorize]
    public abstract class AdminBaseController : KC.Web.Controllers.WebBaseController
    {
        protected ITenantUserService TenantService => ServiceProvider.GetService<ITenantUserService>();
        protected Service.WebApiService.Business.IAccountApiService AccountApiService
        {
            get
            {
                return ServiceProvider.GetService<Service.WebApiService.Business.IAccountApiService>();
            }
        }
        public AdminBaseController(
            IServiceProvider serviceProvider,
            ILogger logger)
            : base(serviceProvider, logger)
        {
        }

        #region 选小图标控件: Shared/_SelectUserPartial.cshtml
        [HttpGet]
        public PartialViewResult _SelectIconPartial()
        {
            return PartialView();
        }

        #endregion 选小图标控件: Shared/_SelectUserPartial.cshtml

        #region 选人控件: Shared/_SelectUserPartial.cshtml
        /// <summary>
        /// 获取所有的部门信息及下属员工
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetRootOrganizationsWithUsers()
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

        public IActionResult GeneratePasswordHash(string password)
        {
            var result = EncryptPasswordUtil.EncryptPassword(password);
            return base.ThrowErrorJsonMessage(true, result);
        }
    }
}