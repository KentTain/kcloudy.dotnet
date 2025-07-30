
using KC.Framework.Base;
using KC.Framework.Extension;
using KC.Framework.Tenant;
using KC.Service.DTO.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace KC.Web.Extension
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class MenuFilterAttribute : Attribute, IEqualityComparer<MenuFilterAttribute>, ICloneable
    {
        /// <summary>
        /// 设置菜单注解
        /// </summary>
        /// <param name="parentName">父级菜单名称</param>
        /// <param name="operName">菜单名称</param>
        /// <param name="url">菜单访问路径</param>
        /// <param name="authorityId">菜单唯一Id号</param>
        public MenuFilterAttribute(string parentName, string menuName, string url)
        {
            if (string.IsNullOrEmpty(parentName))
            {
                throw new ArgumentNullException(nameof(parentName));
            }

            if (string.IsNullOrEmpty(menuName))
            {
                throw new ArgumentNullException(nameof(menuName));
            }

            ParentMenuName = parentName;
            MenuName = menuName;
            IsExtPage = false;
            Version = TenantConstant.DefaultVersion;
            TenantType = TenantConstant.DefaultTenantType;
            SmallIcon = "fa fa-bars";
            Url = url;
            FullUrl = GlobalConfig.CurrentApplication?.AppDomain + url.TrimStart('/');
        }
        /// <summary>
        /// 页面名称，即所属菜单的名称
        /// </summary>
        public string ParentMenuName { get; private set; }
        /// <summary>
        /// 操作名称
        /// </summary>
        public string MenuName { get; private set; }
        /// <summary>
        /// 访问资源地址，形如：/AreaName/ControllerName/ActionName?QueryString=xxx
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int Order { get; set; }
        /// <summary>
        /// 是否为扩展页面
        /// </summary>
        public bool IsExtPage { get; set; }
        /// <summary>
        /// 菜单层级，分为三级（1,2,3）
        /// </summary>
        public int Level { get; set; }
        /// <summary>
        /// 系统版本
        /// </summary>
        public TenantVersion Version { get; set; }
        /// <summary>
        /// 企业类型
        /// </summary>
        public TenantType TenantType { get; set; }
        /// <summary>
        /// 小图标
        /// </summary>
        public string SmallIcon { get; set; }
        /// <summary>
        /// 菜单默认分配的角色
        /// </summary>
        public string DefaultRoleId { get; set; }
        /// <summary>
        /// 访问资源地址，形如：http://domain/AreaName/ControllerName/ActionName?QueryString=xxx
        /// </summary>
        public string FullUrl { get; set; }
        /// <summary>
        /// 菜单的权限控制Id
        /// </summary>
        public string AuthorityId { get; set; }

        public bool Equals(MenuFilterAttribute x, MenuFilterAttribute y)
        {
            return x.AuthorityId == y.AuthorityId;
        }

        public int GetHashCode(MenuFilterAttribute obj)
        {
            return obj.AuthorityId.GetHashCode();
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }

    public class MenuData
    {
        static MenuData()
        {
            IsUpgradeDatabase = false;
            AllMenus = new List<MenuNodeDTO>();
        }

        public static void AddResource(MenuFilterAttribute item, MenuNodeDTO parentItem = null)
        {
            var parentName = item.ParentMenuName;
            var menuName = item.MenuName;
            if (string.IsNullOrEmpty(parentName)
                || string.IsNullOrEmpty(menuName))
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
            if (urlParms.Length == 2)
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

            Expression<Func<MenuNodeDTO, bool>> predicate = m => m.ApplicationId == appId;
            if (!string.IsNullOrEmpty(area))
                predicate = predicate.And(m => m.AreaName.Equals(area, StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrEmpty(controller))
                predicate = predicate.And(m => m.ControllerName.Equals(controller, StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrEmpty(action))
                predicate = predicate.And(m => m.ActionName.Equals(action, StringComparison.OrdinalIgnoreCase));

            if (!AllMenus.Any(predicate.Compile()))
            {
                var node = new MenuNodeDTO()
                {
                    Text = menuName,
                    ApplicationId = appId,
                    ApplicationName = GlobalConfig.CurrentApplication?.AppName,
                    AreaName = area,
                    ControllerName = controller,
                    ActionName = action,
                    Index = item.Order,
                    IsExtPage = item.IsExtPage,
                    Version = item.Version,
                    TenantType = item.TenantType,
                    SmallIcon = item.SmallIcon,
                    Leaf = item.Level == 3 ? true : false,
                    Level = item.Level,
                    Parent = parentItem,
                    DefaultRoleId = item.DefaultRoleId,
                    AuthorityId = item.AuthorityId
                };

                switch (item.Level)
                {
                    case 1:
                        node.Description = string.Format("一级菜单【{0}】", menuName);
                        break;
                    case 2:
                        node.Description = string.Format("一级菜单【{0}】下的二级菜单【{1}】", parentName, menuName);
                        break;
                    case 3:
                        node.Description = string.Format("二级菜单【{0}】下的三级菜单【{1}】", parentName, menuName);
                        break;
                }

                if (parentItem == null)
                {
                    AllMenus.Add(node);
                }
                else
                {
                    parentItem.Children.Add(node);
                }
            }
        }
        public static bool IsUpgradeDatabase { get; set; }

        public static List<MenuNodeDTO> AllMenus { get; set; }
    }
}
