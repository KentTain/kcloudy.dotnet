using KC.Common;
using KC.Framework.Base;
using KC.Framework.Tenant;
using KC.Service;
using KC.Service.DTO.Account;
using KC.Service.WebApiService.Business;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KC.Web.Extension
{
    /// <summary>
    /// 站点启动时，组装MenuData、PermissionData
    /// </summary>
    public class PermissionFilterModelProvider : IdSrvOAuth2ClientRequestBase, IApplicationModelProvider
    {
        private const string ServiceName = "KC.Web.Extension.PermissionFilterModelProvider";
        public PermissionFilterModelProvider(
            System.Net.Http.IHttpClientFactory httpClient, 
            ILogger<PermissionFilterModelProvider> logger)
            : base(TenantConstant.DbaTenantApiAccessInfo, httpClient, logger)
        {
        }

        public async void OnProvidersExecuted(ApplicationModelProviderContext context)
        {
            var result1 = false;
            var result2 = false;
            if (!MenuData.IsUpgradeDatabase && MenuData.AllMenus.Any())
            {
                //保存应用的所有菜单
                result1 = await SaveMenusAsync(MenuData.AllMenus, GlobalConfig.ApplicationGuid);
                if (result1)
                {
                    MenuData.IsUpgradeDatabase = true;
                }
            }

            if (!PermissionData.IsUpgradeDatabase && PermissionData.AllPermissions.Any())
            {
                //保存应用的所有权限
                result2 = await SavePermissionsAsync(PermissionData.AllPermissions, GlobalConfig.ApplicationGuid);
                if (result2)
                {
                    PermissionData.IsUpgradeDatabase = true;
                }
            }

            //Logger.LogWarning(Common.SerializeHelper.ToJson(MenuData.AllMenus));
            //Logger.LogWarning(Common.SerializeHelper.ToJson(PermissionData.AllPermissions));

            Logger.LogDebug(Framework.Base.GlobalConfig.CurrentApplication?.AppName + "|PermissionFilterModelProvider upgrade PermissionData OnProvidersExecuted is success? " + result2 + ". upgrade MenuData OnProvidersExecuted is success? " + result1);
            Console.WriteLine(Framework.Base.GlobalConfig.CurrentApplication?.AppName + "|PermissionFilterModelProvider upgrade PermissionData OnProvidersExecuted is success? " + result2 + ". upgrade MenuData OnProvidersExecuted is success? " + result1);
        }

        public void OnProvidersExecuting(ApplicationModelProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var menuAttrData = new List<MenuFilterAttribute>();
            var permissionAttrData = new List<PermissionFilterAttribute>();
            //循环获取所有的控制器
            foreach (var controllerModel in context.Result.Controllers)
            {
                var menuData = controllerModel.Attributes.OfType<MenuFilterAttribute>().ToArray();
                if (menuData.Length > 0)
                {
                    menuAttrData.AddRange(menuData);
                }
                var permissionData = controllerModel.Attributes.OfType<PermissionFilterAttribute>().ToArray();
                if (permissionData.Length > 0)
                {
                    permissionAttrData.AddRange(permissionData);
                }
                //循环控制器方法
                foreach (var actionModel in controllerModel.Actions)
                {
                    var menuActionData = actionModel.Attributes.OfType<MenuFilterAttribute>().ToArray();
                    if (menuActionData.Length > 0)
                    {
                        menuAttrData.AddRange(menuActionData);
                    }
                    var permissionActionData = actionModel.Attributes.OfType<PermissionFilterAttribute>().ToArray();
                    if (permissionActionData.Length > 0)
                    {
                        permissionAttrData.AddRange(permissionActionData);
                    }
                }
            }

            #region 筛选重复值
            var duplicatedMenuAuthIds = from menu in menuAttrData
                         group menu by menu.AuthorityId into authId
                         where authId.Count() > 1
                         select authId;
            if(duplicatedMenuAuthIds.Any())
            {
                //遍历分组结果集
                foreach (var item in duplicatedMenuAuthIds)
                {
                    foreach (var menu in item)
                    {
                        if (menu.AuthorityId.Equals(ApplicationConstant.DefaultAuthorityId, StringComparison.OrdinalIgnoreCase))
                            continue;
                        Logger.LogError(string.Format(Framework.Base.GlobalConfig.CurrentApplication?.AppName + "|Duplicate menu: {0}-{1}-{2}", menu.AuthorityId, menu.MenuName, menu.FullUrl));
                    }
                }
            }

            var duplicatedPermissionAuthIds = from permission in permissionAttrData
                                        group permission by permission.AuthorityId into authId
                                        where authId.Count() > 1
                                        select authId;
            if (duplicatedPermissionAuthIds.Any())
            {
                //遍历分组结果集
                foreach (var item in duplicatedPermissionAuthIds)
                {
                    foreach (var permission in item)
                    {
                        if (permission.AuthorityId.Equals(ApplicationConstant.DefaultAuthorityId, StringComparison.OrdinalIgnoreCase))
                            continue;
                        Logger.LogError(string.Format(Framework.Base.GlobalConfig.CurrentApplication?.AppName + "|Duplicate permission: {0}-{1}-{2}", permission.AuthorityId, permission.PermissionName, permission.FullUrl));
                    }
                }
            }
            #endregion

            #region 获取菜单数据
            //一级菜单
            foreach (var item in menuAttrData.Where(m => m.Level == 1))
            {
                MenuData.AddResource(item);
            }

            //二级菜单
            foreach (var item in menuAttrData.Where(m => m.Level == 2))
            {
                var parentItem = MenuData.AllMenus.FirstOrDefault(m => m.Text.Equals(item.ParentMenuName, StringComparison.OrdinalIgnoreCase));
                MenuData.AddResource(item, parentItem);
            }

            //三级菜单
            foreach (var item in menuAttrData.Where(m => m.Level == 3))
            {
                var parentItem = MenuData.AllMenus.SelectMany(m => m.Children).FirstOrDefault(m => m.Text.Equals(item.ParentMenuName, StringComparison.OrdinalIgnoreCase));
                MenuData.AddResource(item, parentItem);
            }
            #endregion

            #region 获取权限数据
            //父节点
            foreach (var item in permissionAttrData.Where(m => m.IsPage))
            {
                PermissionData.AddResource(item);
            }

            //子节点
            foreach (var item in permissionAttrData.Where(m => !m.IsPage))
            {
                var parentItem = PermissionData.AllPermissions.FirstOrDefault(m => m.Level == 1 && m.Text.EndsWith(Framework.Tenant.ApplicationConstant.DefaultAuthoritySplitChar + item.MenuName, StringComparison.OrdinalIgnoreCase));
                PermissionData.AddResource(item, parentItem);
            }
            #endregion
        }

        public int Order { get { return -1000 + 11; } }

        /// <summary>
        /// 保存应用的权限数据
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        private async Task<bool> SavePermissionsAsync(List<PermissionDTO> permissions, Guid appGuid)
        {
            var jsonData = SerializeHelper.ToJson(permissions);
            ServiceResult<bool> result = null;
            await WebSendPostAsync<ServiceResult<bool>>(
                ServiceName + ".SavePermissions",
                AccoutApiServerUrl + "AccountApi/SavePermissions?appId=" + appGuid,
                ApplicationConstant.AccScope,
                jsonData,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<bool>(ServiceResultType.Error, httpStatusCode, errorMessage);
                },
                true);

            if (result.success)
            {
                return result.Result;
            }

            return false;
        }

        /// <summary>
        /// 保存应用的权限数据
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        private async Task<bool> SaveMenusAsync(List<MenuNodeDTO> menus, Guid appGuid)
        {
            var jsonData = SerializeHelper.ToJson(menus);
            ServiceResult<bool> result = null;
            await WebSendPostAsync<ServiceResult<bool>>(
                ServiceName + ".SaveMenusAsync",
                AccoutApiServerUrl + "AccountApi/SaveMenus?appId=" + appGuid,
                ApplicationConstant.AccScope,
                jsonData,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<bool>(ServiceResultType.Error, httpStatusCode, errorMessage);
                },
                true);

            if (result.success)
            {
                return result.Result;
            }

            return false;
        }
    }
}
