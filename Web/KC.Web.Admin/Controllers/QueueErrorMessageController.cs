using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KC.Service.Admin;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using KC.Service.Component;
using KC.Model.Component.Table;
using KC.Framework.Tenant;
using KC.Framework.Base;
using KC.Model.Job.Table;
using KC.Service.Admin.WebApiService;

namespace KC.Web.Admin.Controllers
{
    [Web.Extension.MenuFilter("日志管理", "队列日志管理", "/QueueErrorMessage/Index",
        Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType, SmallIcon = "fa fa-archive", AuthorityId = "FE6297B0-EAD5-4A86-AD06-696A5BFD2073",
        DefaultRoleId = RoleConstants.AdminRoleId, Order = 3, IsExtPage = false, Level = 3)]
    public class QueueErrorMessageController : AdminBaseController
    {
        private IJobApiService _jobApiService => ServiceProvider.GetService<IJobApiService>();
        private IStorageQueueService _queueService => ServiceProvider.GetService<IStorageQueueService>();

        public QueueErrorMessageController(
            IServiceProvider serviceProvider,
            ILogger<QueueErrorMessageController> logger)
            : base(serviceProvider, logger)
        {
        }

        [Web.Extension.PermissionFilter("队列日志管理", "队列日志管理", "/QueueErrorMessage/Index", "FE6297B0-EAD5-4A86-AD06-696A5BFD2073",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 1, IsPage = true, ResultType = ResultType.ActionResult, AuthorityId = "FE6297B0-EAD5-4A86-AD06-696A5BFD2073")]
        public IActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 根据条件获取相应的Json日志数据
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="queueName"></param>
        /// <param name="sort"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        //[Web.Extension.PermissionFilter("队列日志管理", "加载队列日志列表数据", "/QueueErrorMessage/GetQueueErrorMessageJson", "0822D027-CCD1-482B-8BA0-D1EF6E100E32",
        //    DefaultRoleId = RoleConstants.AdminRoleId, Order = 1, IsPage = false, ResultType = ResultType.JsonResult, AuthorityId = "0822D027-CCD1-482B-8BA0-D1EF6E100E32")]
        public JsonResult GetQueueErrorMessageJson(int page = 1, int rows = 10, string queueName = "")
        {
            var result = _jobApiService.LoadPagenatedQueueErrors(page, rows, queueName);
            return Json(result);
        }

        /// <summary>
        /// 根据条件删除单条日志或清空日志
        /// </summary>
        /// <param name="rowKey"></param>
        /// <returns></returns>
        [Web.Extension.PermissionFilter("队列日志管理", "删除队列日志", "/QueueErrorMessage/DeleteQueueErrorMessage", "7CA9D1F5-236E-4890-8A68-0C2B504FF79F",
           DefaultRoleId = RoleConstants.AdminRoleId, Order = 3, IsPage = false, ResultType = ResultType.JsonResult, AuthorityId = "7CA9D1F5-236E-4890-8A68-0C2B504FF79F")]
        public JsonResult DeleteQueueErrorMessage(string rowKey = "")
        {
            return GetServiceJsonResult(() =>
            {
                return _jobApiService.RemoveByRowKey(rowKey);
            });
        }

        [HttpPost]
        [Web.Extension.PermissionFilter("队列日志管理", "重置队列", "/QueueErrorMessage/ResetQueue", "91A26722-C66D-4FCE-A631-327054070C93",
           DefaultRoleId = RoleConstants.AdminRoleId, Order = 3, IsPage = false, ResultType = ResultType.JsonResult, AuthorityId = "91A26722-C66D-4FCE-A631-327054070C93")]
        public JsonResult ResetQueue(QueueErrorMessageTable data)
        {
            return GetServiceJsonResult(() =>
            {
                var isSucess = _queueService.ResetQueue(data);
                if (isSucess)
                    _jobApiService.RemoveByRowKey(data.RowKey);

                return true;
            });
        }
    }
}