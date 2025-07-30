using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

using KC.Framework.Base;
using KC.Framework.Tenant;
using KC.Service.Training;
using KC.Service.Util;
using KC.Service.DTO.Training;
using KC.Service.WebApiService.Business;
using KC.Model.Training;

namespace KC.Web.Training.Controllers
{
    [Web.Extension.MenuFilter("配置管理", "配置管理", "/Course/Index",
        Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType, SmallIcon = "fa fa-list-alt", AuthorityId = "7D931A51-18DF-439D-BBE9-173576711980",
        DefaultRoleId = RoleConstants.AdminRoleId, Order = 1, IsExtPage = false, Level = 2)]
    public class CourseController : TrainingBaseController
    {
        protected ICourseService courseService => ServiceProvider.GetService<ICourseService>();
        protected IConfigApiService ConfigApiService => ServiceProvider.GetService<IConfigApiService>();

        public CourseController(
            Tenant tenant,
            IServiceProvider serviceProvider,
            ILogger<CourseController> logger)
            : base(tenant, serviceProvider, logger)
        {
        }

        [Web.Extension.PermissionFilter("配置管理", "配置管理", "/Course/Index", "7D931A51-18DF-439D-BBE9-173576711980",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 3, IsPage = true, ResultType = ResultType.ActionResult)]
        public IActionResult Index()
        {
            ViewBag.TrainingTypeList = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum(null, new List<ConfigType>()
            {
                ConfigType.WeixinConfig
            });
            return View();
        }

        #region 配置管理：Course
        //[Web.Extension.PermissionFilter("配置管理", "加载配置列表", "/Course/LoadConfigList", "A291F728-B9B3-448B-A793-B3DAFA1BA126",
        //    DefaultRoleId = RoleConstants.AdminRoleId, Order = 1, IsPage = false, ResultType = ResultType.JsonResult)]
        public IActionResult LoadConfigList(int page, int rows, string searchValue = "", int searchType = 0, string sort = "CreatedDate", string order = "desc")
        {
            var result = courseService.GetConfigsByFilter(page, rows, searchValue, searchType, sort, order);
            return Json(result);
        }

        /// <summary>
        /// 获取配置数据（弹窗）
        /// </summary>
        /// <param name="configId"></param>
        /// <returns></returns>
        public PartialViewResult GetConfigForm(int configId)
        {
            var model = new CourseDTO();
            if (configId != 0)
            {
                model = courseService.GetPropertyById(configId);
                model.IsEditMode = true;
                //ViewBag.TrainingTypeList = GetDropDownItemsByEnum(new List<ConfigType>() { ConfigType.UNKNOWN, ConfigType.PaymentMethod, ConfigType.WeixinConfig }, new List<int>() { (int)model.TrainingType });
                //ViewBag.StateList = GetDropDownItemsByEnum<ConfigStatus>(null, new List<int>() { (int)model.State });
            }
            else
            {
                ViewBag.TrainingTypeList = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum(null, new List<ConfigType>() { ConfigType.UNKNOWN, ConfigType.PaymentMethod, ConfigType.WeixinConfig });
                ViewBag.StateList = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum<ConfigStatus>();

            }
            return PartialView("_courseForm", model);
        }

        /// <summary>
        /// 添加/修改配置
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Web.Extension.PermissionFilter("配置管理", "保存配置", "/Course/SaveConfig", "420BFA23-BAC5-4EA2-88D9-A5D060A0C600",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 2, IsPage = false, ResultType = ResultType.JsonResult)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SaveConfig(CourseDTO model)
        {
            return GetServiceJsonResult(() =>
            {
                var success = courseService.SaveCourse(model);
                if (success)
                {
                    if (base.TenantName.Equals(TenantConstant.DbaTenantName, StringComparison.OrdinalIgnoreCase))
                        ConfigApiService.RemoveDefaultEmailConfigCache();
                }
                return success;
            });
        }

        /// <summary>
        /// 移除配置
        /// </summary>
        /// <param name="configId"></param>
        /// <returns></returns>
        [Web.Extension.PermissionFilter("配置管理", "删除配置", "/Course/RemoveConfig", "51DE1887-5C57-4C17-984D-F23456499652",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 3, IsPage = false, ResultType = ResultType.JsonResult)]
        public IActionResult RemoveConfig(int configId)
        {
            return GetServiceJsonResult(() =>
            {
                var result1 = courseService.SoftRemoveCourseById(configId);
                return result1;
            });
        }

        #endregion

    }
}
