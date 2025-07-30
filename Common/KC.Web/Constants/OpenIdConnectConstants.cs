using KC.Framework.Base;
using KC.Framework.Extension;
using KC.Framework.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace KC.Web.Constants
{
    public class OpenIdConnectConstants
    {
        public const string ClientCookieName = "sso.idrserver";
        public const string AuthScheme = "Cookies";
        //public const string AuthScheme = "sso.oauth.cookies";
        public const string ChallengeScheme = "oidc";
        public const string ApiAuthScheme = "Bearer";
        public const string ClientAuthScheme = "idsrv";

        public const string JavaSwaggerCallbackPath = "/webjars/springfox-swagger-ui/oauth2-redirect.html";
        public const string JavaCallbackPath = "/login/oauth2/code/idsrv";
        public const string CallbackPath = "/oidc/signin-callback";
        public const string SignOutPath = "/oidc/signout-callback";

        public const string ClaimTypes_TenantName = "tenantname";
        public const string ClaimTypes_TargetTenantName = "targettenantname";

        public const string ClaimTypes_UserType = "type";
        public const string ClaimTypes_Email = "email";
        public const string ClaimTypes_Phone = "phone";
        public const string ClaimTypes_OrgId = "orgid";
        public const string ClaimTypes_OrgCode = "orgcode";
        public const string ClaimTypes_OrgName = "orgname";
        public const string ClaimTypes_RoleId = "roleid";
        public const string ClaimTypes_RoleName = "rolename";
        public const string ClaimTypes_DisplayName = "disiplayname";

        public const string ClaimTypes_AuthorityIds = "authid";

        public const string TestClient_ClientId = "testclient";
        public const string TestClient_ClientSecret = "secret";
        public const string WebApiTestClient_ClientId = "9BEF4002-442D-45DA-8079-28E11E3C8721";

        /// <summary>
        /// 根据租户代码：TenantName，获取OAuth的Authority <br/>
        ///     TenantName为空时，返回SSOWebDomain，例如：http://localhost:1001 <br/>
        ///     否则返回，例如：http://cTest.localhost:1001
        /// </summary>
        /// <param name="tenantName">租户代码</param>
        /// <returns></returns>
        public static string GetAuthUrlByConfig(string tenantName)
        {
            var protocol = GlobalConfig.SSOWebDomain.StartsWith("https") 
                ? "https" 
                :　"http";
            var baseDomain = GlobalConfig.SSOWebDomain.TrimEndSlash()
                .Replace("https://", "").Replace("http://", "");

            var result = string.IsNullOrEmpty(tenantName)
                ? $"{protocol}://{baseDomain}"
                : $"{protocol}://{tenantName}.{baseDomain}";
            LogUtil.LogDebug($"-----GetAuthUrlByConfig by tenant: {tenantName}，result：{result}");
            return result;
        }
    }
}
