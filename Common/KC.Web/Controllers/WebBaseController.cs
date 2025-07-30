using KC.Framework.Base;
using KC.Framework.Extension;
using KC.Framework.Tenant;
using KC.Web.Constants;
using KC.Service;
using KC.Service.Constants;
using KC.Service.WebApiService.Business;
using KC.Service.DTO.Account;

using KC.IdentityModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;


namespace KC.Web.Controllers
{
    public abstract class WebBaseController : Controller
    {
        protected string ServiceName = "KC.Web.Controllers.WebBaseController";
        protected readonly ILogger Logger;
        protected readonly IServiceProvider ServiceProvider;

        public WebBaseController(
            IServiceProvider serviceProvider,
            ILogger logger)
        {
            Logger = logger;
            ServiceProvider = serviceProvider;
        }

        #region 文件地址
        private string _serverPath;
        protected string ServerPath
        {
            get
            {
                return Directory.GetCurrentDirectory().Replace("\\", "/"); 

                //var exePath = Path.GetDirectoryName(System.Reflection
                //   .Assembly.GetExecutingAssembly().CodeBase);
                //Regex appPathMatcher = new Regex(@"(?<!fil)[A-Za-z]:\\+[\S\s]*?(?=\\+bin)");
                //_serverPath = appPathMatcher.Match(exePath).Value;
                //return _serverPath;
            }
            set { _serverPath = value; }
        }

        private string _tempDir;
        //上传使用
        protected string TempDir
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_tempDir))
                {
                    _tempDir = ServerPath + "/TempDir";
                    if (!Directory.Exists(_tempDir))
                    {
                        try
                        {
                            Directory.CreateDirectory(_tempDir);
                        }
                        catch (SecurityException ex)
                        {
                            Logger.LogError(ex, ex.Message);
                        }
                    }
                }
                return _tempDir;
            }
            set { _tempDir = value; }
        }
        #endregion

        #region CurrentTenant
        protected Tenant CurrentUserTenant
        {
            get
            {
                var tenantName = CurrentUserTenantName.ToLower();
                //var cacheKey = CacheKeyConstants.Prefix.TenantName + tenantName;
                //var cache = Service.CacheUtil.GetCache<Tenant>(cacheKey);
                //if (cache != null) return cache;

                using (var scope = ServiceProvider.CreateScope())
                {
                    var tenantApiService = scope.ServiceProvider.GetRequiredService<ITenantUserApiService>();
                    var result = tenantApiService.GetTenantByName(tenantName).Result;
                    if (result == null) return null;

                    //Service.CacheUtil.SetCache(cacheKey, result, TimeSpan.FromMinutes(TimeOutConstants.CacheTimeOut));

                    return result;
                }
            }
        }

        /// <summary>
        /// 登录用户所属企业的TenantName
        /// </summary>
        protected string CurrentUserTenantName
        {
            get
            {
                if (Identity != null && Identity.FindFirst(OpenIdConnectConstants.ClaimTypes_TenantName) != null)
                {
                    return Identity.FindFirst(OpenIdConnectConstants.ClaimTypes_TenantName).Value;
                }

                return string.Empty;
            }
        }
        /// <summary>
        /// 当前登录用户的企业别名
        /// </summary>
        protected string CurrentUserTenantNickName
        {
            get
            {
                try
                {
                    if (Identity == null)
                        return string.Empty;

                    return string.IsNullOrWhiteSpace(CurrentUserTenant?.NickName)
                        ? CurrentUserTenant?.TenantName
                        : CurrentUserTenant?.NickName;
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, ex.Message);
                    return string.Empty;
                }
            }
        }
        /// <summary>
        /// 登录企业所属企业名称
        /// </summary>
        protected string CurrentUserTenantDisplayName
        {
            get
            {
                try
                {
                    return CurrentUserTenant?.TenantDisplayName;
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, ex.Message);
                    return string.Empty;
                }
            }
        }
        #endregion

        #region CurrentUser
        private ClaimsIdentity _identity;
        protected ClaimsIdentity Identity
        {
            get
            {
                if (_identity != null)
                    return _identity;

                if (User != null && User.Identity.IsAuthenticated)
                {
                    _identity = User.Identity as ClaimsIdentity;
                }

                return _identity;
            }
        }

        protected CurrentUser CurrentUser
        {
            get
            {
                return GetCachedCurrentUser()?.Result;
            }
        }

        protected void RemoveCurrentUserCache()
        {
            var lowerTenant = CurrentUserTenantName?.ToLower();
            var userCacheKey = lowerTenant + "-" + CacheKeyConstants.Prefix.CurrentUserId + CurrentUserId;
            var menuCacheKey = lowerTenant + "-CurrentUserMenus-" + CurrentUserId;
            var permissionCacheKey = lowerTenant + "-CurrentUserPermissions-" + CurrentUserId;

            CacheUtil.RemoveCache(userCacheKey);
            CacheUtil.RemoveCache(menuCacheKey);
            CacheUtil.RemoveCache(permissionCacheKey);

            var apiMethod1 = typeof(IAccountApiService).GetMethod("GetUserWithOrgsAndRolesByUserId");
            var cacheKey1 = KC.Service.Util.CacheKeyUtil.CacheKeyGenerator(lowerTenant, apiMethod1, new object[] { CurrentUserId });
            if (!string.IsNullOrEmpty(cacheKey1))
                Service.CacheUtil.RemoveCache(cacheKey1);

            var apiMethod2 = typeof(IAccountApiService).GetMethod("GetUserContactInfoByUserId");
            var cacheKey2 = KC.Service.Util.CacheKeyUtil.CacheKeyGenerator(lowerTenant, apiMethod2, new object[] { CurrentUserId });
            if (!string.IsNullOrEmpty(cacheKey2))
                Service.CacheUtil.RemoveCache(cacheKey2);
        }

        protected void RemoveCachedCurrentUser(string tenant, string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("用户Id为空");//必须要用户验证后才能使用CurrentUser
            }

            var lowerTenant = tenant.ToLower();
            var cacheKey = lowerTenant + "-" + CacheKeyConstants.Prefix.CurrentUserId + userId;
            Service.CacheUtil.RemoveCache(cacheKey);

            cacheKey = lowerTenant + "-CurrentUserMenus-" + userId;
            Service.CacheUtil.RemoveCache(cacheKey);

            cacheKey = lowerTenant + "-CurrentUserPermissions-" + userId;
            Service.CacheUtil.RemoveCache(cacheKey);

            var apiMethod1 = typeof(IAccountApiService).GetMethod("GetUserWithOrgsAndRolesByUserId");
            var cacheKey1 = KC.Service.Util.CacheKeyUtil.CacheKeyGenerator(lowerTenant, apiMethod1, new object[] { CurrentUserId });
            if (!string.IsNullOrEmpty(cacheKey1))
                Service.CacheUtil.RemoveCache(cacheKey1);

            var apiMethod2 = typeof(IAccountApiService).GetMethod("GetUserContactInfoByUserId");
            var cacheKey2 = KC.Service.Util.CacheKeyUtil.CacheKeyGenerator(lowerTenant, apiMethod2, new object[] { CurrentUserId });
            if (!string.IsNullOrEmpty(cacheKey2))
                Service.CacheUtil.RemoveCache(cacheKey2);
        }

        protected UserType? CurrentUserType
        {
            get
            {
                if (Identity != null && Identity.FindFirst(OpenIdConnectConstants.ClaimTypes_UserType) != null)
                {
                    var userType = Identity.FindFirst(OpenIdConnectConstants.ClaimTypes_UserType).Value;
                    if (!userType.IsNullOrEmpty())
                        return Enum.Parse<UserType>(userType);
                } 
                else if(CurrentUser != null)
                {
                    return CurrentUser.UserType;
                }

                return null;
            }
        }

        /// <summary>
        /// 登录用户Id
        /// </summary>
        protected string CurrentUserId
        {
            get
            {
                var userId = string.Empty;
                if (this.User != null
                    && this.User.Identity.IsAuthenticated)
                {
                    if (this.User.FindFirst(ClaimTypes.NameIdentifier) != null)
                    {
                        userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                    }
                    else if (this.User.FindFirst(KC.IdentityModel.JwtClaimTypes.Subject) != null)
                    {
                        userId = this.User.FindFirst(KC.IdentityModel.JwtClaimTypes.Subject).Value;
                    }
                    else if (Identity != null
                        && Identity.FindFirst(ClaimTypes.NameIdentifier) != null)
                    {
                        userId = Identity.FindFirst(ClaimTypes.NameIdentifier).Value;
                    }
                    else if (Identity != null
                        && Identity.FindFirst(KC.IdentityModel.JwtClaimTypes.Subject) != null)
                    {
                        userId = Identity.FindFirst(KC.IdentityModel.JwtClaimTypes.Subject).Value;
                    }
                }
                return userId;
            }
        }
        /// <summary>
        /// 登录用户名
        /// </summary>
        protected string CurrentUserName
        {
            get
            {
                if (Identity != null && Identity.FindFirst(ClaimTypes.Name) != null)
                {
                    return Identity.FindFirst(ClaimTypes.Name).Value;
                }
                else if (Identity != null && Identity.FindFirst(JwtClaimTypes.Name) != null)
                {
                    return Identity.FindFirst(JwtClaimTypes.Name).Value;
                }

                return string.Empty;
            }
        }
        /// <summary>
        /// 用户显示名
        /// </summary>
        protected string CurrentUserDisplayName
        {
            get
            {
                if (Identity != null && Identity.FindFirst(ClaimTypes.GivenName) != null)
                {
                    return Identity.FindFirst(ClaimTypes.GivenName).Value;
                }
                else if (Identity != null && Identity.FindFirst(JwtClaimTypes.GivenName) != null)
                {
                    return Identity.FindFirst(JwtClaimTypes.GivenName).Value;
                }
                else if (Identity != null && Identity.FindFirst(OpenIdConnectConstants.ClaimTypes_DisplayName) != null)
                {
                    return Identity.FindFirst(OpenIdConnectConstants.ClaimTypes_DisplayName).Value;
                }

                return string.Empty;
            }
        }
        /// <summary>
        /// 登录用户邮箱
        /// </summary>
        protected string CurrentUserEmail
        {
            get
            {
                if (Identity != null && Identity.FindFirst(ClaimTypes.Email) != null)
                {
                    return Identity.FindFirst(ClaimTypes.Email).Value;
                }
                else if (Identity != null && Identity.FindFirst(JwtClaimTypes.Email) != null)
                {
                    return Identity.FindFirst(JwtClaimTypes.Email).Value;
                }
                else if (Identity != null && Identity.FindFirst(OpenIdConnectConstants.ClaimTypes_Email) != null)
                {
                    return Identity.FindFirst(OpenIdConnectConstants.ClaimTypes_Email).Value;
                }

                return string.Empty;
            }
        }
        /// <summary>
        /// 登录用户手机
        /// </summary>
        protected string CurrentUserPhone
        {
            get
            {
                if (Identity != null && Identity.FindFirst(ClaimTypes.MobilePhone) != null)
                {
                    return Identity.FindFirst(ClaimTypes.MobilePhone).Value;
                }
                else if (Identity != null && Identity.FindFirst(JwtClaimTypes.PhoneNumber) != null)
                {
                    return Identity.FindFirst(JwtClaimTypes.PhoneNumber).Value;
                }
                else if (Identity != null && Identity.FindFirst(OpenIdConnectConstants.ClaimTypes_Phone) != null)
                {
                    return Identity.FindFirst(OpenIdConnectConstants.ClaimTypes_Phone).Value;
                }
                return string.Empty;
            }
        }

        /// <summary>
        /// 登录用户座机
        /// </summary>
        protected string CurrentUserTelephone
        {
            get
            {
                if (CurrentUser != null)
                {
                    return CurrentUser.UserTelephone;
                }
                return string.Empty;
            }
        }
        /// <summary>
        /// 登陆用户职位
        /// </summary>
        protected PositionLevel CurrentUserPosition
        {
            get
            {
                return CurrentUser.UserPositionLevel;
            }
        }

        /// <summary>
        /// 登录用户角色Id列表
        /// </summary>
        protected List<string> CurrentUserRoleIds
        {
            get
            {
                if (Identity != null && Identity.FindAll(ClaimTypes.Role).Any())
                {
                    return Identity.FindAll(ClaimTypes.Role).Select(r => r.Value).ToList();
                }
                else if (Identity != null && Identity.FindAll(JwtClaimTypes.Role).Any())
                {
                    return Identity.FindAll(JwtClaimTypes.Role).Select(r => r.Value).ToList();
                }
                else if (Identity != null && Identity.FindAll(OpenIdConnectConstants.ClaimTypes_RoleId).Any())
                {
                    return Identity.FindAll(OpenIdConnectConstants.ClaimTypes_RoleId).Select(r => r.Value).ToList();
                }

                return new List<string>();
            }
        }

        /// <summary>
        /// 登录用户组织Id列表
        /// </summary>
        protected List<int> CurrentUserOrgIds
        {
            get
            {
                if (Identity != null && Identity.FindAll(OpenIdConnectConstants.ClaimTypes_OrgId).Any())
                {
                    return Identity.FindAll(OpenIdConnectConstants.ClaimTypes_OrgId).Select(r => int.Parse(r.Value)).ToList();
                }

                return new List<int>();
            }
        }

        /// <summary>
        /// 登录用户组织Code列表
        /// </summary>
        protected List<string> CurrentUserOrgCodes
        {
            get
            {
                if (Identity != null && Identity.FindAll(OpenIdConnectConstants.ClaimTypes_OrgCode).Any())
                {
                    return Identity.FindAll(OpenIdConnectConstants.ClaimTypes_OrgCode).Select(r => r.Value).ToList();
                }

                return new List<string>();
            }
        }
        

        #region 应用相关Id及地址
        private Guid _appId;
        protected Guid CurrentApplicationId
        {
            get
            {
                if (_appId != Guid.Empty)
                    return _appId;
                return GlobalConfig.ApplicationGuid;
            }

            set
            {
                _appId = value;
            }
        }

        private string _appName;
        protected string CurrentApplicationName
        {
            get
            {
                if (!_appName.IsNullOrEmpty())
                    return _appName;
                return GlobalConfig.ApplicationName;
            }

            set
            {
                _appName = value;
            }
        }
        #endregion

        /// <summary>
        /// 是否为公司系统管理员
        /// </summary>
        protected bool IsSystemAdmin
        {
            get
            {
                //管理员角色
                return CurrentUserRoleIds.Contains(RoleConstants.AdminRoleId);
            }
        }
        /// <summary>
        /// 是否为公司高管
        /// </summary>
        protected bool IsManager
        {
            get
            {
                //如果是顶级组织机构的管理员，则获取全公司用户的数据
                if (CurrentUserPosition == PositionLevel.Mananger)
                    return true;

                return false;
            }
        }
        #endregion

        #region CurrentUser's Menu & Permission Data
        /// <summary>
        /// 根据菜单的Url，返回菜单Id
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetMenuIdByUrl(string url)
        {
            if (!base.User.Identity.IsAuthenticated)
                return new ChallengeResult();

            return await GetServiceJsonResultAsync(async () =>
            {
                var allmenus = await GetCachedCurrentUserMenus();

                var menu = allmenus.FirstOrDefault(m => m.URL.Contains(url));
                return menu != null ? menu.Id : 0;
            });
        }

        /// <summary>
        /// 获取当前用户的基本信息及组织结构信息
        /// </summary>
        /// <param name="container"></param>
        /// <param name="tenant"></param>
        /// <returns></returns>
        protected async Task<CurrentUser> GetCachedCurrentUser()
        {
            if (!this.User.Identity.IsAuthenticated)
            {
                Response.StatusCode = (int)System.Net.HttpStatusCode.Unauthorized;
                throw new UnauthorizedAccessException("用户未登录");//必须要用户验证后才能使用CurrentUser
            }
            var userId = CurrentUserId;
            if (string.IsNullOrEmpty(userId))
            {
                Response.StatusCode = (int)System.Net.HttpStatusCode.Unauthorized;
                throw new UnauthorizedAccessException("用户Id为空");//必须要用户验证后才能使用CurrentUser
            }

            var lowerTenant = CurrentUserTenantName?.ToLower();
            var cacheKey = lowerTenant + "-" + CacheKeyConstants.Prefix.CurrentUserId + userId;

            //Logger.LogDebug(string.Format("---Tenant：[{0}] User:[{1}] get Claims: " , lowerTenant, userId, User.Claims.ToList().ToCommaSeparatedStringByFilter(m => m.Type)));

            var cache = await Service.CacheUtil.GetCacheAsync<CurrentUser>(cacheKey);
            if (cache == null)
            {
                var currentUser = new CurrentUser();
                currentUser.UserId = userId;
                currentUser.UserName = CurrentUserName;
                currentUser.UserPhone = CurrentUserPhone;
                currentUser.UserEmail = CurrentUserEmail;
                currentUser.UserDisplayName = CurrentUserDisplayName;
                currentUser.RoleIds = CurrentUserRoleIds;
                currentUser.OrganizationIds = CurrentUserOrgIds;
                currentUser.OrganizationCodes = CurrentUserOrgCodes;

                //Logger.LogDebug("--" + CurrentUserName + "--" + CurrentUserPhone + "--" + CurrentUserEmail + "--" + CurrentUserDisplayName + "--" + CurrentUserRoleIds.ToCommaSeparatedString());

                currentUser.CurrentTenantName = CurrentUserTenant?.TenantName;
                currentUser.CurrentTenantNickName = CurrentUserTenant?.NickName;
                currentUser.CurrentTenantDisplayName = CurrentUserTenant?.TenantDisplayName;
                currentUser.CurrentTenantTenantType = CurrentUserTenant != null ? CurrentUserTenant.TenantType : TenantType.Enterprise;

                using (var scope = ServiceProvider.CreateScope())
                {
                    var accountService = scope.ServiceProvider.GetRequiredService<IAccountApiService>();

                    //后台用户需要组织架构
                    var user = await accountService.GetUserWithOrgsAndRolesByUserId(userId);
                    if (user == null)
                        return currentUser;

                    currentUser.UserName = user.UserName;
                    currentUser.UserPhone = user.PhoneNumber;
                    currentUser.UserEmail = user.Email;
                    currentUser.UserDisplayName = user.DisplayName;
                    currentUser.UserCode = user.ReferenceId1;
                    currentUser.UserTelephone = user.Telephone;
                    currentUser.UserType = user.UserType;
                    currentUser.UserPositionLevel = user.PositionLevel;
                    currentUser.OrganizationIds = user.UserOrgIds;
                    currentUser.RoleIds = user.UserRoleIds;

                    await Service.CacheUtil.SetCacheAsync(cacheKey, currentUser, TimeSpan.FromMinutes(TimeOutConstants.CacheTimeOut));
                }

                return currentUser;
            }
            else
            {
                return cache;
            }
        }
        /// <summary>
        /// 获取当前登录用户的菜单数据
        /// </summary>
        /// <param name="container"></param>
        /// <param name="tenant"></param>
        /// <returns></returns>
        protected async Task<List<MenuNodeSimpleDTO>> GetCachedCurrentUserMenus()
        {
            var currentUser = await GetCachedCurrentUser();
            if (currentUser == null)
            {
                throw new UnauthorizedAccessException("用户未登录");//必须要用户验证后才能使用CurrentUser
            }

            var lowerTenant = CurrentUserTenantName?.ToLower();
            var cacheKey = lowerTenant + "-CurrentUserMenus-" + currentUser.UserId;
            var cache = await Service.CacheUtil.GetCacheAsync<List<MenuNodeSimpleDTO>>(cacheKey);
            //Logger.LogDebug(string.Format("-----Get CurrentUser cache is null? {0}", cache == null ? "True" : "Flase"));
            if (cache == null || !cache.Any())
            {
                var currentUserMenus = new List<MenuNodeSimpleDTO>();
                using (var scope = ServiceProvider.CreateScope())
                {
                    var accountService = scope.ServiceProvider.GetRequiredService<IAccountApiService>();

                    #region 后台用户需要权限、角色、组织、菜单等信息

                    var menus = await accountService.LoadUserMenusByRoleIds(currentUser.RoleIds);
                    if (menus == null)
                        return new List<MenuNodeSimpleDTO>();

                    #endregion

                    currentUserMenus = menus;
                    await Service.CacheUtil.SetCacheAsync(cacheKey, currentUserMenus, TimeSpan.FromMinutes(TimeOutConstants.CacheTimeOut));
                }
                return currentUserMenus;
            }
            else
            {
                return cache;
            }
        }

        /// <summary>
        /// 获取当前登录用户的权限数据
        /// </summary>
        /// <param name="container"></param>
        /// <param name="tenant"></param>
        /// <returns></returns>
        protected async Task<List<PermissionSimpleDTO>> GetCachedCurrentUserPermissions()
        {
            var currentUser = await GetCachedCurrentUser();
            if (currentUser == null)
            {
                throw new UnauthorizedAccessException("用户未登录");//必须要用户验证后才能使用CurrentUser
            }

            var lowerTenant = CurrentUserTenantName?.ToLower();
            var cacheKey = lowerTenant + "-CurrentUserPermissions-" + currentUser.UserId;
            var cache = await Service.CacheUtil.GetCacheAsync<List<PermissionSimpleDTO>>(cacheKey);
            //Logger.LogInformation(string.Format("-----Get CurrentUser cache is null? {0}", cache == null ? "True" : "Flase"));
            if (cache == null)
            {
                var currentUserPermissions = new List<PermissionSimpleDTO>();
                using (var scope = ServiceProvider.CreateScope())
                {
                    var accountService = scope.ServiceProvider.GetRequiredService<IAccountApiService>();

                    #region 后台用户需要权限、角色、组织、菜单等信息

                    var permissions = await accountService.LoadUserPermissionsByRoleIds(currentUser.RoleIds);
                    if (permissions == null)
                        return new List<PermissionSimpleDTO>();

                    #endregion

                    currentUserPermissions = permissions;
                    await Service.CacheUtil.SetCacheAsync(cacheKey, currentUserPermissions, TimeSpan.FromMinutes(TimeOutConstants.CacheTimeOut));
                }
                return currentUserPermissions;
            }
            else
            {
                return cache;
            }
        }

        /// <summary>
        /// 获取当前用户的菜单树结构
        /// </summary>
        /// <param name="predict"></param>
        /// <returns></returns>
        protected async Task<List<MenuNodeSimpleDTO>> GetCurrentUserMenuTree(Func<MenuNodeSimpleDTO, bool> predict)
        {
            var result = new List<MenuNodeSimpleDTO>();
            var allmenus = await GetCachedCurrentUserMenus();
            var menus = allmenus.Where(predict).OrderBy(m => m.Index).ToList();
            foreach (var parent in menus.Where(m => m.ParentId == null))
            {
                SetUrlWithDomain(parent, menus);
                result.Add(parent);
            }
            return result;
        }

        /// <summary>
        /// 获取当前用户的权限树结构
        /// </summary>
        /// <param name="name">权限名称</param>
        /// <returns></returns>
        protected async Task<List<PermissionSimpleDTO>> GetCurrentPermissionTree(string name = "")
        {
            Func<PermissionSimpleDTO, bool> predict = m => true;
            if (!string.IsNullOrWhiteSpace(name))
            {
                predict = m => (m.Text.Contains(name));
            }

            var result = new List<PermissionSimpleDTO>();
            var userPermissions = await GetCachedCurrentUserPermissions();
            var menus = userPermissions.Where(predict).ToList();
            //查询将父级菜单也带出来
            if (!string.IsNullOrWhiteSpace(name))
            {
                var menusParentId = menus.Where(c => c.ParentId != null).Select(c => c.ParentId).Distinct().ToList();
                for (int i = 0; i < menusParentId.Count(); i++)
                {
                    if (menus.Count(c => c.Id == menusParentId[i]) == 0)
                    {
                        menus.Add(userPermissions.FirstOrDefault(c => c.Id == menusParentId[i]));
                    }
                }
            }
            foreach (var parent in menus.Where(m => m.ParentId == null))
            {
                GetActionPermissionWithChild(parent, menus);
                result.Add(parent);
            }
            return result;
        }

        /// <summary>
        /// 拼凑权限树
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="allOrgs"></param>
        private void GetActionPermissionWithChild(PermissionSimpleDTO parent, List<PermissionSimpleDTO> allOrgs)
        {
            var child = allOrgs.Where(m => m.ParentId.Equals(parent.Id)).ToList();
            parent.Children = child.ToList();
            foreach (var children in child)
            {
                children.ParentName = parent.Text;
                GetActionPermissionWithChild(children, allOrgs);
            }
        }
        /// <summary>
        /// 拼装菜单对象的URL，以full Url的方式，例如：http://tenantName.sso.domain.com/area/controller/action?query=
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="allOrgs"></param>
        private void SetUrlWithDomain(MenuNodeSimpleDTO parent, List<MenuNodeSimpleDTO> allOrgs)
        {
            var child = allOrgs.Where(m => m.ParentId.Equals(parent.Id)).OrderBy(m => m.Index).ToList();
            parent.Children = child.ToList();
            var app = GlobalConfig.Applications.FirstOrDefault(m => m.AppId == parent.ApplicationId);
            if (app != null && !string.IsNullOrEmpty(app.AppDomain))
            {
                var domain = app.AppDomain.Replace(TenantConstant.SubDomain, CurrentUserTenantName);
                if (app.AppId == ApplicationConstant.AdminAppId)
                    domain = app.AppDomain.Replace("localhost", CurrentUserTenantName + ".localhost");
                parent.URL = domain.EndsWith("/") ? domain.ReplaceLast("/", "") + parent.URL : domain + parent.URL;
            }

            foreach (var children in child)
            {
                SetUrlWithDomain(children, allOrgs);
            }
        }
        #endregion

        #region Get JsonResult
        /// <summary>
        /// 返回boolean类型的快捷方式
        /// </summary>
        /// <param name="success">是否成功</param>
        /// <param name="message">失败后的消息</param>
        /// <returns></returns>
        protected JsonResult ThrowErrorJsonMessage(bool success, string message)
        {
            return Json(new { success = success, message = message });
        }

        /// <summary>
        /// 获取ServiceResult<T>对象的JsonResult
        /// </summary>
        /// <typeparam name="T">所需返回的对象</typeparam>
        /// <param name="func">Lamdba表达式</param>
        /// <returns>ServiceResult<T></returns>
        protected JsonResult GetServiceJsonResult<T>(Func<T> func)
        {
            var result = ServiceWrapper.Invoke(
                ServiceName,
                func.Method.Name,
                func,
                Logger);
            return Json(result);
        }

        /// <summary>
        /// 获取ServiceResult<T>对象的JsonResult（异步方法）
        /// </summary>
        /// <typeparam name="T">所需返回的对象</typeparam>
        /// <param name="func">Lamdba表达式</param>
        /// <returns>ServiceResult<T></returns>
        protected async Task<JsonResult> GetServiceJsonResultAsync<T>(Func<Task<T>> func)
        {
            var result = await ServiceWrapper.InvokeAsync<T>(
                ServiceName,
                func.Method.Name,
                func,
                Logger);
            return Json(result);
        }
        /// <summary>
        /// 获取ServiceResult<T>对象
        /// </summary>
        /// <typeparam name="T">所需返回的对象</typeparam>
        /// <param name="func">Lamdba表达式</param>
        /// <returns>ServiceResult<T></returns>
        protected ServiceResult<T> GetServiceResult<T>(Func<T> func)
        {
            return ServiceWrapper.Invoke(
                ServiceName,
                func.Method.Name,
                func,
                Logger);
        }
        /// <summary>
        /// 获取ServiceResult<T>对象（异步方法）
        /// </summary>
        /// <typeparam name="T">所需返回的对象</typeparam>
        /// <param name="func">Lamdba表达式</param>
        /// <returns>ServiceResult<T></returns>
        protected async Task<ServiceResult<T>> GetServiceResultAsync<T>(Func<Task<T>> func)
        {
            var result = await ServiceWrapper.InvokeAsync<T>(
                ServiceName,
                func.Method.Name,
                func,
                Logger);
            return result;
        }

        #endregion

        protected string GetAllModelErrorMessages()
        {
            var errors = new StringBuilder();
            ModelState.Remove("Id");
            ModelState.Remove("IsEditMode");
            ModelState.Remove("TreeCode");
            ModelState.Remove("Leaf");
            ModelState.Remove("Level");
            ModelState.Remove("IsDeleted");
            ModelState.Remove("CreatedBy");
            ModelState.Remove("CreatedName");
            ModelState.Remove("CreatedDate");
            ModelState.Remove("ModifiedBy");
            ModelState.Remove("ModifiedName");
            ModelState.Remove("ModifiedDate");
            if (!ModelState.IsValid)
            {
                foreach (var key in ModelState.Keys)
                {
                    var error = string.Empty;
                    var state = ModelState[key];
                    if (state.Errors.Any())
                    {
                        foreach(var errorInfo in state.Errors)
                        {
                            var message = !errorInfo.ErrorMessage.IsNullOrEmpty()
                                ? errorInfo.ErrorMessage 
                                : errorInfo.Exception != null 
                                    ? errorInfo.Exception.ToString()
                                    : string.Empty;

                            if(!message.IsNullOrEmpty())
                                error += message + "；";
                        }
                    }

                    if (!error.IsNullOrEmpty())
                        errors.Append(string.Format("对象属性【{0}】，验证错误：{1}</br>", key, error));
                }
            }

            return errors.ToString();
        }
    }
}
