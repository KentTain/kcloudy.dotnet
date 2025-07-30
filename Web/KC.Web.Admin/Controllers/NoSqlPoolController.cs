using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KC.Framework.SecurityHelper;
using KC.Framework.Tenant;
using KC.Service.Admin;
using KC.Service.DTO.Admin;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using KC.Framework.Base;

namespace KC.Web.Admin.Controllers
{
    [Web.Extension.MenuFilter("资源管理", "NoSql池管理", "/NoSqlPool/Index",
        Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType, SmallIcon = "fa fa-archive", AuthorityId = "A1D301D9-93B5-49AA-A574-526AF7E3249D",
        DefaultRoleId = RoleConstants.AdminRoleId, Order = 7, IsExtPage = false, Level = 3)]
    public class NoSqlPoolController : AdminBaseController
    {
        protected INoSqlPoolService NoSqlPoolService => ServiceProvider.GetService<INoSqlPoolService>();
        public NoSqlPoolController(
            IServiceProvider serviceProvider,
            ILogger<NoSqlPoolController> logger)
            : base(serviceProvider, logger)
        {
        }

        [Web.Extension.PermissionFilter("NoSql池管理", "NoSql池管理", "/NoSqlPool/Index", "A1D301D9-93B5-49AA-A574-526AF7E3249D",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 1, IsPage = true, ResultType = ResultType.ActionResult, AuthorityId = "A1D301D9-93B5-49AA-A574-526AF7E3249D")]
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 加载表单
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="searchKey"></param>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        //[Web.Extension.PermissionFilter("NoSql池管理", "加载NoSql池列表数据", "/NoSqlPool/LoadNoSqlPoolList", "B16CCDED-D817-4607-BC97-23CAE298C9EF",
        //    DefaultRoleId = RoleConstants.AdminRoleId, Order = 1, IsPage = true, ResultType = ResultType.JsonResult, AuthorityId = "B16CCDED-D817-4607-BC97-23CAE298C9EF")]
        public JsonResult LoadNoSqlPoolList(int page, int rows, string selectname, string order = "desc")
        {
            string accessName = string.Empty;
            if (!string.IsNullOrWhiteSpace(selectname))
            {

                accessName = selectname;

            }

            var model = NoSqlPoolService.FindNoSqlPoolsByFilter(page, rows, accessName, order);
            return Json(model);
        }

        /// <summary>
        ///  弹窗
        /// </summary>
        /// <returns></returns>
        public PartialViewResult GetNoSqlPoolForm(int id = 0)
        {
            var model = new NoSqlPoolDTO()
            {
                //AccessKeyPasswordHash = EncryptPasswordUtil.EncryptPassword(EncryptPasswordUtil.GetRandomString()),
                IsEditMode = false
            };
            if (id != 0)
            {
                model = NoSqlPoolService.GetNoSqlPoolById(id);
                model.IsEditMode = true;
                ViewBag.CloudTypeList = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum<CloudType>((int)model.CloudType);
                ViewBag.NoSqlTypeList = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum<NoSqlType>((int)model.NoSqlType);
            }
            else
            {
                ViewBag.CloudTypeList = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum<CloudType>();
                ViewBag.NoSqlTypeList = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum<NoSqlType>();
            }
            return PartialView("_noSqlPoolForm", model);
        }

        /// <summary>
        /// 添加/新增队列链接
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Web.Extension.PermissionFilter("NoSql池管理", "保存NoSql连接", "/NoSqlPool/SaveNoSqlPoolForm", "927EEC2D-B61C-48BA-8D79-D6FC4CC1059D",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 2, IsPage = false, ResultType = ResultType.JsonResult, AuthorityId = "927EEC2D-B61C-48BA-8D79-D6FC4CC1059D")]
        public JsonResult SaveNoSqlPoolForm(NoSqlPoolDTO model)
        {
            return GetServiceJsonResult(() =>
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
                return NoSqlPoolService.SaveNoSqlPool(model);
            });
        }

        /// <summary>
        /// 删除队列链接
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Web.Extension.PermissionFilter("NoSql池管理", "删除NoSql连接", "/NoSqlPool/RemoveNoSqlPool", "5052BB38-F1A8-4940-8E4B-C62E9F61186F",
           DefaultRoleId = RoleConstants.AdminRoleId, Order = 3, IsPage = false, ResultType = ResultType.JsonResult, AuthorityId = "5052BB38-F1A8-4940-8E4B-C62E9F61186F")]
        public JsonResult RemoveNoSqlPool(int id)
        {
            return GetServiceJsonResult(() =>
            {
                return NoSqlPoolService.RemoveNoSqlPool(id);
            });
        }

        [Web.Extension.PermissionFilter("NoSql池管理", "测试NoSql连接", "/NoSqlPool/TestNoSqlConnection", "D1DF0FB3-FA24-496C-B59A-6D50C6B6D5A4",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 4, IsPage = false, ResultType = ResultType.JsonResult, AuthorityId = "D1DF0FB3-FA24-496C-B59A-6D50C6B6D5A4")]
        public JsonResult TestNoSqlConnection(NoSqlPoolDTO model)
        {
            return GetServiceJsonResult(() =>
            {
                return NoSqlPoolService.TestNoSqlConnection(model);
            });
        }
    }
}