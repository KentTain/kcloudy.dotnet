using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

using KC.Framework.Tenant;
using KC.Service.Training;
using KC.Service.DTO.Training;
using KC.Web.Training.Controllers;
using KC.Framework.Base;

namespace KC.Web.Training.Controllers
{
    [Web.Extension.MenuFilter("配置管理", "ID生成管理", "/SysSequence/Index",
        Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType, SmallIcon = "fa fa-list-alt", AuthorityId = "09FAFF66-5BE6-4A0D-BBB7-DDFD5C6FD501",
        DefaultRoleId = RoleConstants.AdminRoleId, Order = 2, IsExtPage = false, Level = 2)]
    public class TeacherController : TrainingBaseController
    {
        protected ITeacherService TeacherService => ServiceProvider.GetService<ITeacherService>();

        public TeacherController(
            Tenant tenant,
            IServiceProvider serviceProvider,
            ILogger<TeacherController> logger)
            : base(tenant, serviceProvider, logger)
        {
        }

        [Web.Extension.PermissionFilter("ID生成管理", "ID生成管理", "/SysSequence/Index", "09FAFF66-5BE6-4A0D-BBB7-DDFD5C6FD501",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 1, IsPage = true, ResultType = ResultType.ActionResult)]
        public IActionResult Index()
        {
            return View();
        }

        //[Web.Extension.PermissionFilter("ID生成管理", "加载ID生成配置列表", "/SysSequence/LoadSysSequenceList", "46E7A81E-629A-4629-BB33-50E1FEBCE85F",
        //    DefaultRoleId = RoleConstants.AdminRoleId, Order = 1, IsPage = false, ResultType = ResultType.JsonResult)]
        public IActionResult LoadSysSequenceList(int page, int rows, string name = "")
        {
            var result = TeacherService.GetPaginatedTeachersByName(page, rows, name);
            return Json(result);
        }

        /// <summary>
        /// 获取配置数据（弹窗）
        /// </summary>
        /// <param name="SysSequenceId"></param>
        /// <returns></returns>
        public PartialViewResult GetSysSequenceForm(string name)
        {
            var model = new TeacherDTO();
            if (!string.IsNullOrEmpty(name))
            {
                model = TeacherService.GetTeacherByName(name);
                model.IsEditMode = true;
            }
            return PartialView("_sysSequenceForm", model);
        }

        /// <summary>
        /// 添加/修改配置
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Web.Extension.PermissionFilter("ID生成管理", "保存ID生成配置", "/SysSequence/SaveSysSequence", "8AC77361-DD8B-4AF1-9AA8-A8C3518168BF",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 2, IsPage = false, ResultType = ResultType.JsonResult)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SaveSysSequence(TeacherDTO model)
        {
            return GetServiceJsonResult(() =>
            {
                var success = TeacherService.SaveTeacher(model);
                
                return success;
            });
        }

        /// <summary>
        /// 移除配置
        /// </summary>
        /// <param name="SysSequenceId"></param>
        /// <returns></returns>
        [Web.Extension.PermissionFilter("ID生成管理", "删除ID生成配置", "/SysSequence/RemoveSysSequence", "17D6B1DA-4C72-4668-95CD-5380DAC38E16",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 3, IsPage = false, ResultType = ResultType.JsonResult)]
        public IActionResult RemoveSysSequence(string name)
        {
            return GetServiceJsonResult(() =>
            {
                var result1 = TeacherService.RemoveTeacherById(name);
                return result1 == true;
            });
        }
    }
}
