using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    [Web.Extension.MenuFilter("资源管理", "数据库池管理", "/DatabasePool/Index",
        Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType, SmallIcon = "fa fa-database", AuthorityId = "A4062638-05C0-4045-BE45-4E3486FF6CA3",
        DefaultRoleId = RoleConstants.AdminRoleId, Order = 1, IsExtPage = false, Level = 3)]
    public class DatabasePoolController : AdminBaseController
    {
        protected IDatabasePoolService DatabasePoolService => ServiceProvider.GetService<IDatabasePoolService>();

        public DatabasePoolController(
            IServiceProvider serviceProvider,
            ILogger<DatabasePoolController> logger)
            : base(serviceProvider, logger)
        {
        }

        [Web.Extension.PermissionFilter("数据库池管理", "数据库池管理", "/DatabasePool/Index", "A4062638-05C0-4045-BE45-4E3486FF6CA3",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 1, IsPage = true, ResultType = ResultType.ActionResult, AuthorityId = "A4062638-05C0-4045-BE45-4E3486FF6CA3")]
        public IActionResult Index()
        {
            ViewBag.CloudTypes = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum<CloudType>();
            return View();
        }

        //[Web.Extension.PermissionFilter("数据库池管理", "加载数据库池列表数据", "/DatabasePool/LoadDatabasePoolList", "57023818-AD33-4729-945A-4834A8FDE465",
        //    DefaultRoleId = RoleConstants.AdminRoleId, Order = 1, IsPage = false, ResultType = ResultType.JsonResult, AuthorityId = "57023818-AD33-4729-945A-4834A8FDE465")]
        public IActionResult LoadDatabasePoolList(int page, int rows, string searchKey = "", string searchValue = "")
        {
            string server = string.Empty;
            string database = string.Empty;
            string userName = string.Empty;
            if (!string.IsNullOrWhiteSpace(searchKey)
                && !string.IsNullOrWhiteSpace(searchValue))
            {
                switch (searchKey.ToLower())
                {
                    case "server":
                        server = searchValue;
                        break;
                    case "database":
                        database = searchValue;
                        break;
                    case "username":
                        userName = searchValue;
                        break;
                }
            }
            var result = DatabasePoolService.FindDatabasePoolsByFilter(page, rows, server, database, userName);

            return Json(result);
        }

        //[Web.Extension.PermissionFilter("数据库池管理", "加载数据库池下租户数据", "/DatabasePool/LoadTenantUserListByDatabasePool", "D59E68C3-1A98-4187-BA59-537C10455AAB",
        //    DefaultRoleId = RoleConstants.AdminRoleId, Order = 2, IsPage = false, ResultType = ResultType.JsonResult, AuthorityId = "D59E68C3-1A98-4187-BA59-537C10455AAB")]
        public IActionResult LoadTenantUserListByDatabasePool(int DatabasePoolId)
        {
            var result = TenantService.FindTenantUserByDatabasePoolId(DatabasePoolId);
            return Json(result);
        }

        
        public PartialViewResult GetDatabasePoolForm(int id = 0)
        {
            var model = new DatabasePoolDTO() { IsEditMode = false };
            if (id != 0)
            {
                model = DatabasePoolService.GetDatabasePoolById(id);
                model.IsEditMode = true;
                ViewBag.CloudTypeList = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum<CloudType>((int)model.CloudType);
                ViewBag.DatabaseTypeList = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum<DatabaseType>((int)model.DatabaseType);
            }
            else
            {
                ViewBag.CloudTypeList = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum<CloudType>();
                ViewBag.DatabaseTypeList = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum<DatabaseType>();
            }
            return PartialView("_databasePoolForm", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Web.Extension.PermissionFilter("数据库池管理", "保存数据库连接", "/DatabasePool/SaveDatabasePoolForm", "13914A9C-49AB-4001-BA3B-C1C1D66969CB",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 2, IsPage = false, ResultType = ResultType.JsonResult, AuthorityId = "13914A9C-49AB-4001-BA3B-C1C1D66969CB")]
        public IActionResult SaveDatabasePoolForm(DatabasePoolDTO model)
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

                return DatabasePoolService.SaveDatabasePool(model);
            });
        }

        [Web.Extension.PermissionFilter("数据库池管理", "删除数据库连接", "/DatabasePool/RemoveDatabasePool", "F6169808-FC59-49FD-99C5-9B98FF99F7E4",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 3, IsPage = false, ResultType = ResultType.JsonResult, AuthorityId = "F6169808-FC59-49FD-99C5-9B98FF99F7E4")]
        public IActionResult RemoveDatabasePool(int id)
        {
            return GetServiceJsonResult(() =>
            {
                return DatabasePoolService.RemoveDatabasePool(id);
            });
        }

        [Web.Extension.PermissionFilter("数据库池管理", "测试数据库连接", "/DatabasePool/TestDatabaseConnection", "26F67515-370C-4E65-A71E-0E78B31B8E4E",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 4, IsPage = false, ResultType = ResultType.JsonResult, AuthorityId = "26F67515-370C-4E65-A71E-0E78B31B8E4E")]
        public IActionResult TestDatabaseConnection(DatabasePoolDTO model)
        {
            return GetServiceJsonResult(() =>
            {
                return DatabasePoolService.TestDatabaseConnection(model);
            });
        }
        
    }
}
