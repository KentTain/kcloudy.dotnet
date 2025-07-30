using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KC.Service.DTO.Account;
using KC.Framework.Base;
using KC.Framework.Extension;
using System.Linq.Expressions;

namespace KC.Web.Extension
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class PermissionFilterAttribute : AuthorizeAttribute, IEqualityComparer<PermissionFilterAttribute>, ICloneable
    {
        /// <summary>
        /// 设置权限注解
        /// </summary>
        /// <param name="name">
        /// 如果为页面(IsPage==true)时，值为：归属菜单的名称        <br/>
        /// 如果为页面(IsPage==false)时，值为：归属权限页面的名称    <br/>
        /// </param>
        /// <param name="operName">权限名称</param>
        /// <param name="url">权限访问路径</param>
        /// <param name="authorityId">权限唯一Id号</param>
        public PermissionFilterAttribute(string name, string operName, string url, string authorityId)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (string.IsNullOrEmpty(operName))
            {
                throw new ArgumentNullException(nameof(operName));
            }

            MenuName = name;
            PermissionName = operName;
            IsPage = false;
            ResultType = ResultType.ActionResult;
            Url = url;
            FullUrl = GlobalConfig.CurrentApplication?.AppDomain + url.TrimStart('/');
            AuthorityId = authorityId;
            Policy = authorityId;
            IsDeleted = false;
            //把资源名称设置为Policy名称
            //Policy = name + Framework.Tenant.ApplicationConstant.DefaultAuthoritySplitChar + operName;
        }
        /// <summary>
        /// 页面名称，即所属菜单的名称
        /// </summary>
        public string MenuName { get; private set; }
        /// <summary>
        /// 操作名称
        /// </summary>
        public string PermissionName { get; private set; }
        /// <summary>
        /// 访问资源地址，形如：/AreaName/ControllerName/ActionName?QueryString=xxx
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int Order { get; set; }
        /// <summary>
        /// 是否为页面权限
        ///     true：页面权限，即所属菜单的名称
        ///     false：页面下的操作
        /// </summary>
        public bool IsPage { get; set; }
        /// <summary>
        /// 操作结果类型：KC.ResultType
        /// </summary>
        public ResultType ResultType { get; set; }
        /// <summary>
        /// 权限默认分配的角色
        /// </summary>
        public string DefaultRoleId { get; set; }
        /// <summary>
        /// 访问资源地址，形如：http://domain/AreaName/ControllerName/ActionName?QueryString=xxx
        /// </summary>
        public string FullUrl { get; set; }
        /// <summary>
        /// 权限的权限控制Id
        /// </summary>
        public string AuthorityId { get; set; }

        public bool IsDeleted { get; set; }

        public bool Equals(PermissionFilterAttribute x, PermissionFilterAttribute y)
        {
            return x.AuthorityId == y.AuthorityId;
        }

        public int GetHashCode(PermissionFilterAttribute obj)
        {
            return obj.AuthorityId.GetHashCode();
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }

    public class PermissionData
    {
        static PermissionData()
        {
            IsUpgradeDatabase = false;
            AllPermissions = new List<PermissionDTO>();
        }

        public static void AddResource(PermissionFilterAttribute item, PermissionDTO parentItem = null)
        {
            var menuName = item.MenuName;
            var permissionName = item.PermissionName;
            if (string.IsNullOrEmpty(menuName) 
                || string.IsNullOrEmpty(permissionName))
            {
                return;
            }

            var url = item.Url;
            //Url: /ControllerName/ActionName?QueryString=xxxx
            //Url: /AreaName/ControllerName/ActionName?QueryString=xxxx
            url = url.StartsWith('/') ? url.ReplaceFirst("/", "") : url;
            url = url.EndsWith('/') ? url.ReplaceLast("/", "") : url;

            var queryParms = url.Split('?');
            var urlParms = queryParms[0].Split('/');

            var query = string.Empty;
            if (queryParms.Length == 2)
                query = queryParms[1];

            var appId = GlobalConfig.ApplicationGuid;
            var area = string.Empty;
            var controller = string.Empty;
            var action = string.Empty;
            if(urlParms.Length == 2)
            {
                controller = urlParms[0];
                action = urlParms[1];
            }
            else if (urlParms.Length == 3)
            {
                area = urlParms[0];
                controller = urlParms[1];
                action = urlParms[2];
            }

            if (string.IsNullOrEmpty(area) 
                && string.IsNullOrEmpty(controller) 
                && string.IsNullOrEmpty(action))
                return;

            Expression<Func<PermissionDTO, bool>> predicate = m => m.ApplicationId == appId;
            if (!string.IsNullOrEmpty(area))
                predicate = predicate.And(m => m.AreaName.Equals(area, StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrEmpty(controller))
                predicate = predicate.And(m => m.ControllerName.Equals(controller, StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrEmpty(action))
                predicate = predicate.And(m => m.ActionName.Equals(action, StringComparison.OrdinalIgnoreCase));

            if (!AllPermissions.Any(predicate.Compile()))
            {
                var node = new PermissionDTO()
                {
                    Text = menuName + Framework.Tenant.ApplicationConstant.DefaultAuthoritySplitChar + permissionName,
                    ApplicationId = appId,
                    ApplicationName = GlobalConfig.CurrentApplication.AppName,
                    AreaName = area,
                    ControllerName = controller,
                    ActionName = action,
                    Parameters = query,
                    ResultType = item.ResultType,
                    Description = item.IsPage 
                        ? string.Format("页面【{0}】的权限", menuName)
                        : string.Format("页面【{0}】下的权限【{1}】", menuName, permissionName),
                    Index = item.Order,
                    Leaf = parentItem == null ? false : true,
                    Level = item.IsPage ? 1 : 2,
                    Parent = parentItem,
                    DefaultRoleId = item.DefaultRoleId,
                    AuthorityId = item.AuthorityId,
                    IsDeleted = item.IsDeleted
                };
                
                if (parentItem == null)
                {
                    AllPermissions.Add(node);
                }
                else
                {
                    parentItem.Children.Add(node);
                }
            }
        }
        public static bool IsUpgradeDatabase { get; set; }

        public static List<PermissionDTO> AllPermissions { get; set; }
    }
}
