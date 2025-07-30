using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KC.Framework.Tenant;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace KC.Web.Message.Controllers
{
    public abstract class MessageBaseController : Web.Controllers.TenantWebBaseController
    {
        protected Service.WebApiService.Business.IAccountApiService AccountApiService
        {
            get
            {
                return ServiceProvider.GetService<Service.WebApiService.Business.IAccountApiService>();
            }
        }

        public MessageBaseController(
            Tenant tenant,
            IServiceProvider serviceProvider,
            ILogger logger)
            : base(tenant, serviceProvider, logger)
        {
        }

        /// <summary>
        /// 将临时文件从临时空间转移至客户文件夹，并删除临时文件
        /// </summary>
        /// <param name="blobIds"></param>
        /// <param name="containThumbnail"></param>
        protected bool CopyTempsToClientAzureBlob(List<string> blobIds, bool containThumbnail = false)
        {
            return Storage.Util.BlobUtil.CopyTempsToClientBlob(Tenant, blobIds, CurrentUserId, containThumbnail);
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
    }
}