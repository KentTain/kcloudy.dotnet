using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KC.Framework.Tenant;
using KC.Service.Admin;
using KC.Service.DTO.Admin;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using KC.Framework.Base;

namespace KC.Web.Admin.Controllers
{
    [Web.Extension.MenuFilter("资源管理", "服务总线池管理", "/ServiceBusPool/Index",
        Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType, SmallIcon = "fa fa-archive", AuthorityId = "9FF31A46-86DC-4E9E-B51B-685980BE2A5A",
        DefaultRoleId = RoleConstants.AdminRoleId, Order = 6, IsExtPage = false, Level = 3)]
    public class ServiceBusPoolController : AdminBaseController
    {
        protected IServiceBusPoolService ServiceBusPoolService => ServiceProvider.GetService<IServiceBusPoolService>();
        public ServiceBusPoolController(
            IServiceProvider serviceProvider,
            ILogger<ServiceBusPoolController> logger)
            : base(serviceProvider, logger)
        {
        }

        [Web.Extension.PermissionFilter("服务总线池管理", "服务总线池管理", "/ServiceBusPool/Index", "9FF31A46-86DC-4E9E-B51B-685980BE2A5A",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 1, IsPage = true, ResultType = ResultType.ActionResult, AuthorityId = "9FF31A46-86DC-4E9E-B51B-685980BE2A5A")]
        public IActionResult Index()
        {
            return View();
        }


        /// <summary>
        /// 显示服务总线池管理相关数据
        /// </summary>
        /// <returns></returns>
        //[Web.Extension.PermissionFilter("服务总线池管理", "加载服务总线池列表数据", "/ServiceBusPool/LoadServiceBusPoolList", "60C8E7B2-EDAA-4E2E-9831-D45E91217194",
        //    DefaultRoleId = RoleConstants.AdminRoleId, Order = 1, IsPage = false, ResultType = ResultType.ActionResult, AuthorityId = "60C8E7B2-EDAA-4E2E-9831-D45E91217194")]
        public JsonResult LoadServiceBusPoolList(int page, int rows, string searchKey, string searchValue)
        {
            string accessName = string.Empty;
            if (!string.IsNullOrWhiteSpace(searchKey) && !string.IsNullOrWhiteSpace(searchValue))
            {
                accessName = searchValue;
            }

            var result = ServiceBusPoolService.FindPaginatedServiceBusPoolList(page, rows, accessName);
            return Json(result);

        }

        /// <summary>
        /// 跳转到服务总线池管理表单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public PartialViewResult GetServiceBusPoolForm(int id = 0)
        {
            var model = new ServiceBusPoolDTO()
            {
                IsEditMode = false
            };
            if (id != 0)
            {
                model = ServiceBusPoolService.GetServiceBusPoolbyId(id);
                model.IsEditMode = true;
                ViewBag.CloudTypeList = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum<CloudType>((int)model.CloudType);
                ViewBag.ServiceBusTypeList = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum<ServiceBusType>((int)model.ServiceBusType);
            }
            else
            {
                ViewBag.CloudTypeList = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum<CloudType>();
                ViewBag.ServiceBusTypeList = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum<ServiceBusType>();
            }
            return PartialView("_serviceBusPoolForm", model);
        }

        /// <summary>
        /// 添加、编辑服务总线池管理相关数据
        /// </summary>
        /// <param name="userapp"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [Web.Extension.PermissionFilter("服务总线池管理", "保存服务总线连接", "/ServiceBusPool/SaveServiceBusPoolForm", "0AB446BD-F380-43EB-B920-6CAE1A8A6AC0",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 2, IsPage = false, ResultType = ResultType.JsonResult, AuthorityId = "0AB446BD-F380-43EB-B920-6CAE1A8A6AC0")]
        public JsonResult SaveServiceBusPoolForm(ServiceBusPoolDTO model)
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
                return ServiceBusPoolService.SaveServiceBusPool(model);
            });
        }
        /// <summary>
        /// 删除服务总线池管理相关数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Web.Extension.PermissionFilter("服务总线池管理", "删除服务总线连接", "/ServiceBusPool/DeleteServiceBusPool", "A900D687-435B-4748-BF9F-A4D809CAB3E9",
           DefaultRoleId = RoleConstants.AdminRoleId, Order = 3, IsPage = false, ResultType = ResultType.JsonResult, AuthorityId = "A900D687-435B-4748-BF9F-A4D809CAB3E9")]
        public ActionResult DeleteServiceBusPool(int id)
        {
            return GetServiceJsonResult(() =>
            {
                return ServiceBusPoolService.DeleteServiceBusPool(id);
            });
        }

        [Web.Extension.PermissionFilter("服务总线池管理", "测试服务总线连接", "/ServiceBusPool/TestServiceBusPoolConnection", "D360F0A7-989C-4077-88BC-FCDDF0084167",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 5, IsPage = false, ResultType = ResultType.JsonResult, AuthorityId = "D360F0A7-989C-4077-88BC-FCDDF0084167")]
        public IActionResult TestServiceBusPoolConnection(ServiceBusPoolDTO model)
        {
            return GetServiceJsonResult(() =>
            {
                return ServiceBusPoolService.TestServiceBusConnection(model);
            });
        }
    }
}