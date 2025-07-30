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
using KC.Service.Config;
using KC.Service.Util;
using KC.Service.DTO.Config;
using KC.Service.WebApiService.Business;

namespace KC.Web.Config.Controllers
{
    public class ConfigManagerController : ConfigBaseController
    {
        protected IConfigService ConfigService => ServiceProvider.GetService<IConfigService>();
        protected IConfigApiService ConfigApiService => ServiceProvider.GetService<IConfigApiService>();

        public ConfigManagerController(
            Tenant tenant,
            IServiceProvider serviceProvider,
            ILogger<ConfigManagerController> logger)
            : base(tenant, serviceProvider, logger)
        {
        }

        /// <summary>
        /// 三级菜单：配置管理/配置管理/配置列表
        /// </summary>
        [Web.Extension.MenuFilter("配置管理", "配置列表", "/ConfigManager/Index",
            Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType,
            SmallIcon = "fa fa-list-alt", AuthorityId = "7D931A51-18DF-439D-BBE9-173576711980",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 1, IsExtPage = false, Level = 3)]
        [Web.Extension.PermissionFilter("配置列表", "配置列表", "/ConfigManager/Index", 
            "7D931A51-18DF-439D-BBE9-173576711980", DefaultRoleId = RoleConstants.AdminRoleId, 
            Order = 3, IsPage = true, ResultType = ResultType.ActionResult)]
        public IActionResult Index()
        {
            ViewBag.ConfigTypeList = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum(null, new List<ConfigType>()
            {
                ConfigType.WeixinConfig
            });
            return View();
        }

        #region 配置管理：ConfigEntity
        public IActionResult LoadConfigList(int page, int rows, string searchValue = "", int searchType = 0, string sort = "CreatedDate", string order = "desc")
        {
            var result = ConfigService.FindConfigsByFilter(page, rows, searchValue, searchType, sort, order);
            return Json(result);
        }

        [Web.Extension.PermissionFilter("配置列表", "删除配置", "/ConfigManager/RemoveConfig",
            "51DE1887-5C57-4C17-984D-F23456499652", DefaultRoleId = RoleConstants.AdminRoleId,
            Order = 3, IsPage = false, ResultType = ResultType.JsonResult)]
        public async Task<IActionResult> RemoveConfig(int id)
        {
            return await GetServiceJsonResultAsync(async () =>
            {
                var result = await ConfigService.RemoveConfigByIdAsync(id, CurrentUserId, CurrentUserDisplayName);
                return result;
            });
        }

        /// <summary>
        /// 三级菜单：配置管理/配置定义/新增/编辑配置
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Web.Extension.MenuFilter("配置管理", "新增/编辑配置", "/ConfigManager/GetConfigForm",
            Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType,
            SmallIcon = "fa fa-sitemap", AuthorityId = "448C6519-A303-404F-9EB6-6B15601E7E6A",
            DefaultRoleId = RoleConstants.ExecutorManagerRoleId, Order = 3, IsExtPage = true, Level = 3)]
        [Web.Extension.PermissionFilter("新增/编辑配置", "新增/编辑配置", "/ConfigManager/GetConfigForm",
            "448C6519-A303-404F-9EB6-6B15601E7E6A", DefaultRoleId = RoleConstants.AdminRoleId,
            Order = 3, IsPage = true, ResultType = ResultType.ActionResult)]
        public IActionResult GetConfigForm(int id)
        {
            var model = new ConfigEntityDTO();
            if (id != 0)
            {
                model = ConfigService.GetConfigById(id);
                model.IsEditMode = true;
                ViewBag.ConfigTypeList = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum((int)model.ConfigType, new List<ConfigType>() { ConfigType.UNKNOWN, ConfigType.PaymentMethod, ConfigType.WeixinConfig });
                ViewBag.StateList = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum<ConfigStatus>((int)model.State);
            }
            else
            {
                ViewBag.ConfigTypeList = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum(null, new List<ConfigType>() { ConfigType.UNKNOWN, ConfigType.PaymentMethod, ConfigType.WeixinConfig });
                ViewBag.StateList = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum<ConfigStatus>();
            }
            return View("ConfigForm", model);
        }

        [Web.Extension.PermissionFilter("新增/编辑配置", "保存配置", "/ConfigManager/SaveConfig", 
            "420BFA23-BAC5-4EA2-88D9-A5D060A0C600", DefaultRoleId = RoleConstants.AdminRoleId, 
            Order = 2, IsPage = false, ResultType = ResultType.JsonResult)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveConfig(ConfigEntityDTO model)
        {
            return await GetServiceJsonResultAsync(async () =>
            {
                model.ModifiedName = CurrentUserDisplayName;
                model.ModifiedBy = CurrentUserId;
                model.ModifiedDate = DateTime.UtcNow;
                if (!model.IsEditMode)
                {
                    model.CreatedName = CurrentUserDisplayName;
                    model.CreatedBy = CurrentUserId;
                    model.CreatedDate = DateTime.UtcNow;
                }

                var success = await ConfigService.SaveConfigWithAttributesAsync(model);
                if (success)
                {
                    //清除相应的缓存
                    switch (model.ConfigType)
                    {
                        case ConfigType.EmailConfig:
                            ConfigApiService.RemoveTenantEmailConfigCache(base.Tenant);
                            if (base.TenantName.Equals(TenantConstant.DbaTenantName, StringComparison.OrdinalIgnoreCase))
                                ConfigApiService.RemoveDefaultEmailConfigCache();
                            break;
                        case ConfigType.SmsConfig:
                            ConfigApiService.RemoveTenantSmsConfigCache(base.Tenant);
                            if (base.TenantName.Equals(TenantConstant.DbaTenantName, StringComparison.OrdinalIgnoreCase))
                                ConfigApiService.RemoveDefaultSmsConfigCache();
                            break;
                        case ConfigType.CallConfig:
                            ConfigApiService.RemoveTenantCallConfigCache(base.Tenant);
                            if (base.TenantName.Equals(TenantConstant.DbaTenantName, StringComparison.OrdinalIgnoreCase))
                                ConfigApiService.RemoveDefaultCallConfigCache();
                            break;
                        case ConfigType.WeixinConfig:
                            if (base.TenantName.Equals(TenantConstant.DbaTenantName, StringComparison.OrdinalIgnoreCase))
                                WeChatUtil.RemoveDefaultWeixinConfigCache();
                            break;
                    }

                    if (base.TenantName.Equals(TenantConstant.DbaTenantName, StringComparison.OrdinalIgnoreCase))
                        ConfigApiService.RemoveDefaultEmailConfigCache();
                }
                return success;
            });
        }

        #endregion

        #region 配置属性管理：ConfigAttribute

        //[Web.Extension.PermissionFilter("配置列表", "加载配置属性列表", "/ConfigManager/LoadPropertyList", "E50A0E97-1107-44D9-93C0-CD75120DDCF2",
        //    DefaultRoleId = RoleConstants.AdminRoleId, Order = 4, IsPage = false, ResultType = ResultType.JsonResult)]
        public IActionResult LoadPropertyList(int id)
        {
            var result = ConfigService.FindConfigAttributesByConfigId(id);

            return Json(result);
        }

        [Web.Extension.PermissionFilter("新增/编辑配置", "删除配置属性", "/ConfigManager/RemoveConfigAttribute", 
            "8542C34A-C616-4534-A17F-615B873C5A46", DefaultRoleId = RoleConstants.AdminRoleId, 
            Order = 5, IsPage = false, ResultType = ResultType.JsonResult)]
        public IActionResult RemoveConfigAttribute(int id)
        {
            return GetServiceJsonResult(() =>
            {
                return ConfigService.SoftRemoveConfigAttributeById(id);
            });
        }

        #endregion

        #region 配置日志
        /// <summary>
        /// 三级菜单：配置管理/配置管理/配置日志
        /// </summary>
        /// <returns></returns>
        [Web.Extension.MenuFilter("配置管理", "配置日志", "/ConfigManager/ConfigLog",
            Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType,
            SmallIcon = "fa fa-list-alt", AuthorityId = "06A84E58-54EC-44DC-A816-CFF2ED36BDD0",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 2, IsExtPage = false, Level = 3)]
        [Web.Extension.PermissionFilter("配置日志", "配置日志", "/ConfigManager/ConfigLog",
            "06A84E58-54EC-44DC-A816-CFF2ED36BDD0", DefaultRoleId = RoleConstants.AdminRoleId,
            Order = 2, IsPage = true, ResultType = ResultType.ActionResult)]
        public IActionResult ConfigLog()
        {
            return View();
        }

        public JsonResult LoadConfigLogList(int page = 1, int rows = 10, string selectname = null)
        {
            var result = ConfigService.FindPaginatedConfigLogs(page, rows, selectname);

            return Json(result);
        }
        #endregion
    }
}
