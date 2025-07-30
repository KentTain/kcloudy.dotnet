using KC.Framework.Extension;
using KC.Framework.Tenant;
using KC.Model.Account;
using KC.Service.Admin;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace KC.Web.SSO
{
    public class TenantSignInManager : SignInManager<User>
    {
        private readonly Tenant _tenant;
        private readonly ITenantUserService _tenantService;
        public TenantSignInManager(
            UserManager<User> userManager,
            IHttpContextAccessor contextAccessor,
            IUserClaimsPrincipalFactory<User> claimsFactory,
            IOptions<IdentityOptions> optionsAccessor,
            ILogger<TenantSignInManager> logger,
            IAuthenticationSchemeProvider schemes,
            IUserConfirmation<User> confirmation,
            ITenantUserService tenantService,
            Tenant tenant = null)
            : base(userManager,
                  contextAccessor,
                  claimsFactory,
                  optionsAccessor,
                  logger,
                  schemes,
                  confirmation)
        {
            _tenant = tenant;
            _tenantService = tenantService;
        }

        public override async Task<SignInResult> PasswordSignInAsync(string userName, string password, bool isPersistent, bool lockoutOnFailure)
        {
            User user;
            if (userName.Contains("@"))
            {
                user = await UserManager.FindByEmailAsync(userName);
            }
            else
            {
                //用户名及手机
                user = await UserManager.FindByNameAsync(userName);
            }

            if (user == null)
            {
                return SignInResult.Failed;
            }

            var attempt = await CheckPasswordSignInAsync(user, password, lockoutOnFailure);
            return attempt.Succeeded
                ? await SignInOrTwoFactorAsync(user, isPersistent)
                : attempt;
        }

        public override async Task SignInAsync(User user, AuthenticationProperties authenticationProperties, string authenticationMethod = null)
        {
            var userPrincipal = await ClaimsFactory.CreateAsync(user);
            // Review: should we guard against CreateUserPrincipal returning null?
            if (authenticationMethod != null)
            {
                userPrincipal.Identities.First().AddClaim(new Claim(ClaimTypes.AuthenticationMethod, authenticationMethod));
            }

            Logger.LogDebug(string.Format(Framework.Base.GlobalConfig.CurrentApplication?.AppName + "|Tenant: [{0}-{1}] SignIn Claims: {2} in TenantSignInManager class.",
                _tenant?.TenantName, _tenant?.TenantDisplayName, userPrincipal.Claims.ToCommaSeparatedStringByFilter(m => m.Value)));
            if (authenticationProperties.ExpiresUtc == null)
                authenticationProperties.ExpiresUtc = DateTimeOffset.UtcNow.Add(TimeSpan.FromMinutes(Service.Constants.TimeOutConstants.CookieTimeOut));

            await Context.SignInAsync(IdentityConstants.ApplicationScheme,
                userPrincipal,
                authenticationProperties ?? new AuthenticationProperties());
        }
        
    }
}
