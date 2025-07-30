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
    [Web.Extension.MenuFilter("资源管理", "Vod池管理", "/VodPool/Index",
        Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType, 
        SmallIcon = "fa fa-archive", AuthorityId = "3945FAA9-7340-4753-B175-1A54B623F3F9",
        DefaultRoleId = RoleConstants.AdminRoleId, Order = 4, IsExtPage = false, Level = 3)]
    public class VodPoolController : AdminBaseController
    {
        protected IVodPoolService VodPoolService => ServiceProvider.GetService<IVodPoolService>();
        public VodPoolController(
            IServiceProvider serviceProvider,
            ILogger<VodPoolController> logger)
            : base(serviceProvider, logger)
        {
        }

        [Web.Extension.PermissionFilter("Vod池管理", "Vod池管理", "/VodPool/Index", 
            "3945FAA9-7340-4753-B175-1A54B623F3F9", DefaultRoleId = RoleConstants.AdminRoleId, 
            Order = 1, IsPage = true, ResultType = ResultType.ActionResult)]
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
        public JsonResult LoadVodPoolList(int page, int rows, string selectname, string order = "desc")
        {
            string accessName = string.Empty;
            if (!string.IsNullOrWhiteSpace(selectname))
            {
                accessName = selectname;
            }

            var model = VodPoolService.FindVodPoolsByFilter(page, rows, accessName, order);
            return Json(model);
        }

        public IActionResult LoadTenantUserListByVodPool(int vodPoolId, int CloudType)
        {
            var result = TenantService.FindTenantUserByVodPoolId(vodPoolId);
            return Json(result);
        }
        /// <summary>
        ///  弹窗
        /// </summary>
        /// <returns></returns>
        public PartialViewResult GetVodPoolForm(int id = 0)
        {
            var model = new VodPoolDTO()
            {
                //AccessKeyPasswordHash = EncryptPasswordUtil.EncryptPassword(EncryptPasswordUtil.GetRandomString()),
                IsEditMode = false
            };
            if (id != 0)
            {
                model = VodPoolService.GetVodPoolById(id);
                model.IsEditMode = true;
                ViewBag.CloudTypeList = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum<CloudType>((int)model.CloudType);
                ViewBag.VodTypeList = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum<VodType>((int)model.VodType);
            }
            else
            {
                ViewBag.CloudTypeList = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum<CloudType>();
                ViewBag.VodTypeList = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum<VodType>();
            }
            return PartialView("_vodPoolForm", model);
        }

        /// <summary>
        /// 添加/新增队列链接
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Web.Extension.PermissionFilter("Vod池管理", "保存Vod连接", "/VodPool/SaveVodPoolForm",
            "425B3088-9E5A-4A32-B232-02F38E6C1D52", DefaultRoleId = RoleConstants.AdminRoleId, 
            Order = 2, IsPage = false, ResultType = ResultType.JsonResult)]
        public JsonResult SaveVodPoolForm(VodPoolDTO model)
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
                return VodPoolService.SaveVodPool(model);
            });
        }

        /// <summary>
        /// 删除队列链接
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Web.Extension.PermissionFilter("Vod池管理", "删除Vod连接", "/VodPool/RemoveVodPool",
            "770C10D1-EE87-40E3-8CAA-B31EBD4A8CA0", DefaultRoleId = RoleConstants.AdminRoleId, 
            Order = 3, IsPage = false, ResultType = ResultType.JsonResult)]
        public JsonResult RemoveVodPool(int id)
        {
            return GetServiceJsonResult(() =>
            {
                return VodPoolService.RemoveVodPool(id);
            });
        }

        [Web.Extension.PermissionFilter("Vod池管理", "测试Vod连接", "/VodPool/TestVodConnection",
            "9CE265C3-D088-4B00-9665-F845D5243ECF", DefaultRoleId = RoleConstants.AdminRoleId, 
            Order = 4, IsPage = false, ResultType = ResultType.JsonResult)]
        public JsonResult TestVodConnection(VodPoolDTO model)
        {
            return GetServiceJsonResult(() =>
            {
                return VodPoolService.TestVodConnection(model);
            });
        }
    }
}