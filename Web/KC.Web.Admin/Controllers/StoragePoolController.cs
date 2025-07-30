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
    [Web.Extension.MenuFilter("资源管理", "存储池管理", "/StoragePool/Index",
        Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType, SmallIcon = "fa fa-archive", AuthorityId = "55DFECCB-D814-444A-89E6-14119D140BF1",
        DefaultRoleId = RoleConstants.AdminRoleId, Order = 2, IsExtPage = false, Level = 3)]
    public class StoragePoolController : AdminBaseController
    {
        protected IStoragePoolService StoragePoolService => ServiceProvider.GetService<IStoragePoolService>();

        public StoragePoolController(
            IServiceProvider serviceProvider,
            ILogger<StoragePoolController> logger)
            : base(serviceProvider, logger)
        {
        }

        [Web.Extension.PermissionFilter("存储池管理", "存储池管理", "/StoragePool/Index", "55DFECCB-D814-444A-89E6-14119D140BF1",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 1, IsPage = true, ResultType = ResultType.ActionResult, AuthorityId = "55DFECCB-D814-444A-89E6-14119D140BF1")]
        public IActionResult Index()
        {
            return View();
        }

        //[Web.Extension.PermissionFilter("存储池管理", "加载存储池列表数据", "/StoragePool/LoadStoragePoolList", "9ADDA2D4-7702-48E1-AEF6-EBED52E066CF",
        //    DefaultRoleId = RoleConstants.AdminRoleId, Order = 1, IsPage = false, ResultType = ResultType.JsonResult, AuthorityId = "9ADDA2D4-7702-48E1-AEF6-EBED52E066CF")]
        public IActionResult LoadStoragePoolList(int page, int rows, string selectname, string order = "desc")
        {
            string accessName = string.Empty;
            if (!string.IsNullOrWhiteSpace(selectname))
            {

                accessName = selectname;
            }
            var result = StoragePoolService.FindStoragePoolsByFilter(page, rows, accessName, order);
            return Json(result);
        }

        //[Web.Extension.PermissionFilter("存储池管理", "加载存储池下租户列表数据", "/StoragePool/LoadTenantUserListByStoragePool", "02456BE2-0595-4294-AF81-B5CB86F25EE5",
        //    DefaultRoleId = RoleConstants.AdminRoleId, Order = 1, IsPage = false, ResultType = ResultType.JsonResult, AuthorityId = "02456BE2-0595-4294-AF81-B5CB86F25EE5")]
        public IActionResult LoadTenantUserListByStoragePool(int StoragePoolId, int CloudType)
        {
            var result = TenantService.FindTenantUserByStoragePoolId(StoragePoolId);
            return Json(result);
        }

        public PartialViewResult GetStoragePoolForm(int id = 0)
        {
            var model = new StoragePoolDTO()
            {
                //AccessKeyPasswordHash = EncryptPasswordUtil.EncryptPassword(EncryptPasswordUtil.GetRandomString()),
                IsEditMode = false
            };
            if (id != 0)
            {
                model = StoragePoolService.GetStoragePoolById(id);
                model.IsEditMode = true;
                ViewBag.CloudTypeList = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum<CloudType>((int)model.CloudType);
                ViewBag.StorageTypeList = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum<StorageType>((int)model.StorageType);
            }
            else
            {
                ViewBag.CloudTypeList = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum<CloudType>();
                ViewBag.StorageTypeList = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum<StorageType>();
            }
            return PartialView("_storagePoolForm", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Web.Extension.PermissionFilter("存储池管理", "保存存储连接", "/StoragePool/SaveStoragePoolForm", "F258F8A9-B2A1-4779-AA39-D0079541E5DA",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 2, IsPage = false, ResultType = ResultType.JsonResult, AuthorityId = "F258F8A9-B2A1-4779-AA39-D0079541E5DA")]
        public IActionResult SaveStoragePoolForm(StoragePoolDTO model)
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
                return StoragePoolService.SaveStoragePool(model);
            });
        }

        [Web.Extension.PermissionFilter("存储池管理", "删除存储连接", "/StoragePool/RemoveStoragePool", "C10FB77B-DEEC-4609-91C2-A8B8079A1135",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 3, IsPage = false, ResultType = ResultType.JsonResult, AuthorityId = "C10FB77B-DEEC-4609-91C2-A8B8079A1135")]
        public IActionResult RemoveStoragePool(int id)
        {
            return GetServiceJsonResult(() =>
            {
                return StoragePoolService.RemoveStoragePool(id);
            });
        }

        [Web.Extension.PermissionFilter("存储池管理", "测试存储连接", "/StoragePool/TestStorageConnection", "961622D0-4F5C-4A9A-8E75-936860C22FCB",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 4, IsPage = false, ResultType = ResultType.JsonResult, AuthorityId = "961622D0-4F5C-4A9A-8E75-936860C22FCB")]
        public IActionResult TestStorageConnection(StoragePoolDTO model)
        {
            return GetServiceJsonResult(() =>
            {
                var rootPath = AppDomain.CurrentDomain.BaseDirectory;
                var filePath = rootPath + @"wwwroot/images/banner1.svg";
                return StoragePoolService.TestStorageConnection(model, filePath);
            });
        }
    }
}