using KC.Framework.Base;
using KC.Framework.Extension;
using KC.Framework.Tenant;
using KC.IdentityModel;
using KC.Web.Constants;
using KC.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Security.Claims;

namespace KC.Web.Extension
{
    public class GlobalViewModelActionFilter : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            base.OnResultExecuting(filterContext);

            var controller = filterContext.Controller as Controller;
            if (controller == null) return;

            var requestHost = filterContext.HttpContext.Request.Host.Value;
            var tenantName = requestHost.GetTenantNameByHost();
            if (filterContext.Controller != null && filterContext.Controller is TenantWebBaseController)
            {
                var tenantController = filterContext.Controller as TenantWebBaseController;
                if (!string.IsNullOrEmpty(tenantController.TenantName))
                    tenantName = tenantController.TenantName;
            }

            if (string.IsNullOrEmpty(tenantName))
            {
                tenantName = TenantConstant.DbaTenantName;
            }

            if (controller.ViewBag != null && controller.ViewBag.TenantName == null)
            {
                controller.ViewBag.TenantName = tenantName;
            }

            if (!string.IsNullOrEmpty(GlobalConfig.ResWebDomain)
                    && (controller.ViewBag != null && controller.ViewBag.ResWebDomain == null))
            {
                controller.ViewBag.ResWebDomain = GlobalConfig.ResWebDomain;
            }

            if (!string.IsNullOrEmpty(GlobalConfig.SSOWebDomain)
                    && (controller.ViewBag != null && controller.ViewBag.SSOWebDomain == null))
            {
                controller.ViewBag.SSOWebDomain = string.Format("http://{0}.{1}", tenantName, GlobalConfig.SSOWebDomain.Replace("https://", "").Replace("http://", "").TrimEndSlash());
            }

            if (!string.IsNullOrEmpty(GlobalConfig.DocWebDomain)
                    && (controller.ViewBag != null && controller.ViewBag.DocWebApiDomain == null))
            {
                var tenantDocApi = GlobalConfig.GetTenantWebApiDomain(GlobalConfig.DocWebDomain, tenantName);
                controller.ViewBag.DocWebApiDomain = tenantDocApi;
            }

            if (!string.IsNullOrEmpty(GlobalConfig.ApplicationId)
                && (controller.ViewBag != null && controller.ViewBag.AppId == null))
            {
                controller.ViewBag.AppId = GlobalConfig.ApplicationId;
            }

            if (!string.IsNullOrEmpty(GlobalConfig.ApplicationName)
                && (controller.ViewBag != null && controller.ViewBag.AppName == null))
            {
                controller.ViewBag.AppName = GlobalConfig.ApplicationName;
            }

            if (GlobalConfig.CurrentApplication != null
                && (controller.ViewBag != null && controller.ViewBag.AppName == null))
            {
                controller.ViewBag.AppDomain = GlobalConfig.CurrentApplication?.AppDomain;
            }

            if (GlobalConfig.UploadConfig != null
                && (controller.ViewBag != null && controller.ViewBag.UploadConfig == null))
            {
                controller.ViewBag.UploadConfig = GlobalConfig.UploadConfig;
            }

            var user = filterContext.HttpContext.User;
            bool isAuth = user.Identity.IsAuthenticated;
            controller.ViewBag.isAuth = isAuth;
            if (isAuth)
            {
                controller.ViewBag.UserId = GetCurrentUserId(user);
                controller.ViewBag.UserType = GetCurrentUserType(user);
                controller.ViewBag.UserName = GetCurrentUserDisplayName(user);
                controller.ViewBag.UserEmail = GetCurrentUserEmail(user);
                controller.ViewBag.UserPhone = GetCurrentUserPhone(user);
                controller.ViewBag.UserDisplayName = GetCurrentUserDisplayName(user);
                controller.ViewBag.UserTenantName = GetCurrentUserTenant(user);
            }
        }

        /// <summary>
        /// 登录用户Id
        /// </summary>
        protected string GetCurrentUserId(ClaimsPrincipal user)
        {
            var userId = string.Empty;
            if (user != null && user.Identity.IsAuthenticated)
            {
                var identity = user.Identity as ClaimsIdentity;
                if (user.FindFirst(ClaimTypes.NameIdentifier) != null)
                {
                    userId = user.FindFirst(ClaimTypes.NameIdentifier).Value;
                }
                else if (user.FindFirst(KC.IdentityModel.JwtClaimTypes.Subject) != null)
                {
                    userId = user.FindFirst(KC.IdentityModel.JwtClaimTypes.Subject).Value;
                }
                else if (identity != null
                    && identity.FindFirst(ClaimTypes.NameIdentifier) != null)
                {
                    userId = identity.FindFirst(ClaimTypes.NameIdentifier).Value;
                }
                else if (identity != null
                    && identity.FindFirst(KC.IdentityModel.JwtClaimTypes.Subject) != null)
                {
                    userId = identity.FindFirst(KC.IdentityModel.JwtClaimTypes.Subject).Value;
                }
            }
            return userId;
        }
        /// <summary>
        /// 登录用户邮箱
        /// </summary>
        protected string GetCurrentUserEmail(ClaimsPrincipal user)
        {
            if (user != null && user.Identity.IsAuthenticated)
            {
                var identity = user.Identity as ClaimsIdentity;
                if (identity != null && identity.FindFirst(ClaimTypes.Email) != null)
                {
                    return identity.FindFirst(ClaimTypes.Email).Value;
                }
                else if (identity != null && identity.FindFirst(JwtClaimTypes.Email) != null)
                {
                    return identity.FindFirst(JwtClaimTypes.Email).Value;
                }
                else if (identity != null && identity.FindFirst(OpenIdConnectConstants.ClaimTypes_Email) != null)
                {
                    return identity.FindFirst(OpenIdConnectConstants.ClaimTypes_Email).Value;
                }
            }

            return string.Empty;
        }
        /// <summary>
        /// 登录用户手机
        /// </summary>
        protected string GetCurrentUserPhone(ClaimsPrincipal user)
        {
            if (user != null && user.Identity.IsAuthenticated)
            {
                var identity = user.Identity as ClaimsIdentity;
                if (identity != null && identity.FindFirst(ClaimTypes.MobilePhone) != null)
                {
                    return identity.FindFirst(ClaimTypes.MobilePhone).Value;
                }
                else if (identity != null && identity.FindFirst(JwtClaimTypes.PhoneNumber) != null)
                {
                    return identity.FindFirst(JwtClaimTypes.PhoneNumber).Value;
                }
                else if (identity != null && identity.FindFirst(OpenIdConnectConstants.ClaimTypes_Phone) != null)
                {
                    return identity.FindFirst(OpenIdConnectConstants.ClaimTypes_Phone).Value;
                }
            }

            return string.Empty;
        }
        /// <summary>
        /// 登录用户姓名
        /// </summary>
        protected string GetCurrentUserDisplayName(ClaimsPrincipal user)
        {
            var result = string.Empty;
            if (user != null && user.Identity.IsAuthenticated)
            {
                var identity = user.Identity as ClaimsIdentity;
                if (identity != null && identity.FindFirst(ClaimTypes.GivenName) != null)
                {
                    result = identity.FindFirst(ClaimTypes.GivenName).Value;
                }
                else if (identity != null && identity.FindFirst(JwtClaimTypes.GivenName) != null)
                {
                    result = identity.FindFirst(JwtClaimTypes.GivenName).Value;
                }
                else if (identity != null && identity.FindFirst(OpenIdConnectConstants.ClaimTypes_DisplayName) != null)
                {
                    result = identity.FindFirst(OpenIdConnectConstants.ClaimTypes_DisplayName).Value;
                }
            }

            return result.IsNullOrEmpty() ? GetCurrentUserPhone(user).ToHidePhone() : result;
        }

        /// <summary>
        /// 登录用户类型
        /// </summary>
        protected UserType? GetCurrentUserType(ClaimsPrincipal user)
        {
            var identity = user.Identity as ClaimsIdentity;
            if (identity != null && identity.FindFirst(OpenIdConnectConstants.ClaimTypes_UserType) != null)
            {
                var userType = identity.FindFirst(OpenIdConnectConstants.ClaimTypes_UserType).Value;
                if (!userType.IsNullOrEmpty())
                    return Enum.Parse<UserType>(userType);
            }

            return null;
        }

        /// <summary>
        /// 登录用户所在Tenant
        /// </summary>
        protected string GetCurrentUserTenant(ClaimsPrincipal user)
        {
            var identity = user.Identity as ClaimsIdentity;
            if (identity == null || identity.FindFirst(OpenIdConnectConstants.ClaimTypes_TenantName) == null)
                return string.Empty;

            var tenant = identity.FindFirst(OpenIdConnectConstants.ClaimTypes_TenantName).Value;
            return tenant;
        }
    }
}
