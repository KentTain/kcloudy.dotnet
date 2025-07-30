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
    [Web.Extension.MenuFilter("资源管理", "队列池管理", "/QueuePool/Index",
        Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType, SmallIcon = "fa fa-archive", AuthorityId = "E7189232-9899-4E4B-82DC-B3F9EBD7F88C",
        DefaultRoleId = RoleConstants.AdminRoleId, Order = 5, IsExtPage = false, Level = 3)]
    public class QueuePoolController : AdminBaseController
    {
        protected IQueuePoolService QueuePoolService => ServiceProvider.GetService<IQueuePoolService>();
        public QueuePoolController(
            IServiceProvider serviceProvider,
            ILogger<QueuePoolController> logger)
            : base(serviceProvider, logger)
        {
        }

        [Web.Extension.PermissionFilter("队列池管理", "队列池管理", "/QueuePool/Index", "E7189232-9899-4E4B-82DC-B3F9EBD7F88C",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 1, IsPage = true, ResultType = ResultType.ActionResult, AuthorityId = "E7189232-9899-4E4B-82DC-B3F9EBD7F88C")]
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
        //[Web.Extension.PermissionFilter("队列池管理", "加载队列池列表数据", "/QueuePool/LoadQueuePoolList", "4A3B0BBB-9A8A-49FF-9047-406C7FFE8B8A",
        //    DefaultRoleId = RoleConstants.AdminRoleId, Order = 1, IsPage = false, ResultType = ResultType.JsonResult, AuthorityId = "4A3B0BBB-9A8A-49FF-9047-406C7FFE8B8A")]
        public JsonResult LoadQueuePoolList(int page, int rows, string selectname, string order = "desc")
        {
            string accessName = string.Empty;
            if (!string.IsNullOrWhiteSpace(selectname))
            {

                accessName = selectname;

            }

            var model = QueuePoolService.FindQueuePoolsByFilter(page, rows, accessName, order);
            return Json(model);
        }

        /// <summary>
        ///  弹窗
        /// </summary>
        /// <returns></returns>
        public PartialViewResult GetQueuePoolForm(int id = 0)
        {
            var model = new QueuePoolDTO()
            {
                //AccessKeyPasswordHash = EncryptPasswordUtil.EncryptPassword(EncryptPasswordUtil.GetRandomString()),
                IsEditMode = false
            };
            if (id != 0)
            {
                model = QueuePoolService.GetQueuePoolById(id);
                model.IsEditMode = true;
                ViewBag.CloudTypeList = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum<CloudType>((int)model.CloudType);
                ViewBag.QueueTypeList = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum<QueueType>((int)model.QueueType);
            }
            else
            {
                ViewBag.CloudTypeList = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum<CloudType>();
                ViewBag.QueueTypeList = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum<QueueType>();
            }
            return PartialView("_queuePoolForm", model);
        }

        /// <summary>
        /// 添加/新增队列链接
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Web.Extension.PermissionFilter("队列池管理", "保存队列连接", "/QueuePool/SaveQueuePoolForm", "F1823EF3-2726-473E-97D4-2A4D2956E84D",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 2, IsPage = false, ResultType = ResultType.JsonResult, AuthorityId = "F1823EF3-2726-473E-97D4-2A4D2956E84D")]
        public JsonResult SaveQueuePoolForm(QueuePoolDTO model)
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
                return QueuePoolService.SaveQueuePool(model);
            });
        }

        /// <summary>
        /// 删除队列链接
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Web.Extension.PermissionFilter("队列池管理", "删除队列连接", "/QueuePool/RemoveQueuePool", "36497733-7F9B-43D4-A7A6-A31ABC11CB4D",
           DefaultRoleId = RoleConstants.AdminRoleId, Order = 3, IsPage = false, ResultType = ResultType.JsonResult, AuthorityId = "36497733-7F9B-43D4-A7A6-A31ABC11CB4D")]
        public JsonResult RemoveQueuePool(int id)
        {
            return GetServiceJsonResult(() =>
            {
                return QueuePoolService.RemoveQueuePool(id);
            });
        }

        [Web.Extension.PermissionFilter("队列池管理", "测试队列连接", "/QueuePool/TestQueueConnection", "04A2A9A2-5236-47FA-998F-3187B3BAF49C",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 4, IsPage = false, ResultType = ResultType.JsonResult, AuthorityId = "04A2A9A2-5236-47FA-998F-3187B3BAF49C")]
        public JsonResult TestQueueConnection(QueuePoolDTO model)
        {
            return GetServiceJsonResult(() =>
            {
                return QueuePoolService.TestQueueConnection(model);
            });
        }
    }
}