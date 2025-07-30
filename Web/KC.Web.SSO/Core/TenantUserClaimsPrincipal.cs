using KC.Model.Account;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using KC.Framework.Extension;
using KC.IdentityServer4.Validation;
using Microsoft.AspNetCore.Authentication;
using KC.IdentityServer4.Models;
using KC.IdentityModel;
using KC.IdentityServer4.Services;
using KC.IdentityServer4.Extensions;
using Microsoft.Extensions.Options;
using KC.Framework.Tenant;
using KC.Web.Constants;
using Microsoft.AspNetCore.Http;

namespace KC.Web.SSO
{
    /// <summary>
    /// Provides methods to create a claims principal for a given user.
    /// </summary>
    /// <typeparam name="User">The type used to represent a user.</typeparam>
    /// <typeparam name="TRole">The type used to represent a role.</typeparam>
    public class TenantUserClaimsPrincipal : UserClaimsPrincipalFactory<User, Role>
    {
        private readonly Tenant _tenant;
        private readonly ILogger _logger;
        /// <summary>
        /// Initializes a new instance of the <see cref="UserClaimsPrincipalFactory{User, TRole}"/> class.
        /// </summary>
        /// <param name="userManager">The <see cref="UserManager{User}"/> to retrieve user information from.</param>
        /// <param name="roleManager">The <see cref="RoleManager{TRole}"/> to retrieve a user's roles from.</param>
        /// <param name="optionsAccessor">The configured <see cref="IdentityOptions"/>.</param>
        public TenantUserClaimsPrincipal(
            UserManager<User> userManager,
            RoleManager<Role> roleManager,
            IOptions<IdentityOptions> optionsAccessor,
            ILogger<TenantUserClaimsPrincipal> logger,
            Tenant tenant = null)
            : base(userManager, roleManager, optionsAccessor)
        {
            _logger = logger;
            _tenant = tenant;
        }

        /// <summary>
        /// Creates a <see cref="ClaimsPrincipal"/> from an user asynchronously.
        /// </summary>
        /// <param name="user">The user to create a <see cref="ClaimsPrincipal"/> from.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous creation operation, containing the created <see cref="ClaimsPrincipal"/>.</returns>
        public override async Task<ClaimsPrincipal> CreateAsync(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            return new ClaimsPrincipal(await GenerateClaimsAsync(user));
        }
        /// <summary>
        /// Generate the claims for a user.
        /// </summary>
        /// <param name="user">The user to create a <see cref="ClaimsIdentity"/> from.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous creation operation, containing the created <see cref="ClaimsIdentity"/>.</returns>
        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(User user)
        {
            var id = await base.GenerateClaimsAsync(user);
            var idClaims = id.Claims;

            var claims = new List<Claim>();
            if (_tenant != null && !string.IsNullOrEmpty(_tenant?.TenantName)
                && !idClaims.Any(m => m.Type.Equals(OpenIdConnectConstants.ClaimTypes_TenantName, StringComparison.OrdinalIgnoreCase)))
                claims.Add(new Claim(OpenIdConnectConstants.ClaimTypes_TenantName, _tenant?.TenantName));
            if (!string.IsNullOrEmpty(user.Email)
                && !idClaims.Any(m => m.Type.Equals(JwtClaimTypes.Email, StringComparison.OrdinalIgnoreCase)))
                claims.Add(new Claim(JwtClaimTypes.Email, user.Email));
            if (!string.IsNullOrEmpty(user.PhoneNumber)
                && !idClaims.Any(m => m.Type.Equals(JwtClaimTypes.PhoneNumber, StringComparison.OrdinalIgnoreCase)))
                claims.Add(new Claim(JwtClaimTypes.PhoneNumber, user.PhoneNumber));
            if (!string.IsNullOrEmpty(user.DisplayName)
                && !idClaims.Any(m => m.Type.Equals(JwtClaimTypes.GivenName, StringComparison.OrdinalIgnoreCase)))
                claims.Add(new Claim(JwtClaimTypes.GivenName, user.DisplayName));
            if (!idClaims.Any(m => m.Type.Equals(OpenIdConnectConstants.ClaimTypes_UserType, StringComparison.OrdinalIgnoreCase)))
                claims.Add(new Claim(OpenIdConnectConstants.ClaimTypes_UserType, user.UserType.ToString()));
            
            //if (UserManager.SupportsUserClaim)
            //{
            //    var userClaims = await UserManager.GetClaimsAsync(user);
            //    foreach (var userClaim in userClaims)
            //    {
            //        claims.Add(userClaim);
            //    }
            //}

            if (user.UserOrganizations.Any())
            {
                foreach (var org in user.UserOrganizations)
                    claims.Add(new Claim(OpenIdConnectConstants.ClaimTypes_OrgId, org.OrganizationId.ToString()));
            }

            if (user.Organizations.Any())
            {
                foreach (var org in user.Organizations)
                {
                    claims.Add(new Claim(OpenIdConnectConstants.ClaimTypes_OrgCode, org.OrganizationCode));
                    //claims.Add(new Claim(OpenIdConnectConstants.ClaimTypes_OrgName, org.Name));
                }
            }

            //默认系统管理角色所属权限
            claims.Add(new Claim(OpenIdConnectConstants.ClaimTypes_AuthorityIds, ApplicationConstant.DefaultAuthorityId));
            if (user.UserRoles.Any())
            {
                foreach (var userRole in user.UserRoles)
                {
                    var roleId = userRole.RoleId;
                    claims.Add(new Claim(OpenIdConnectConstants.ClaimTypes_RoleId, roleId));
                    var role = await RoleManager.FindByIdAsync(roleId);
                    if (role != null)
                    {
                        claims.Add(new Claim(OpenIdConnectConstants.ClaimTypes_RoleName, role.Name));
                        if (role.Permissions == null)
                            continue;

                        var rolePermissions = role.Permissions;
                        foreach (var roleClaim in rolePermissions)
                        {
                            claims.Add(new Claim(OpenIdConnectConstants.ClaimTypes_AuthorityIds, roleClaim.AuthorityId.IsNullOrEmpty() ? ApplicationConstant.DefaultAuthorityId : roleClaim.AuthorityId));

                            if (!roleClaim.AuthorityId.IsNullOrEmpty()
                                && !claims.Any(c => c.Value.Equals(roleClaim.AuthorityId)))
                            {
                                claims.Add(new Claim(OpenIdConnectConstants.ClaimTypes_AuthorityIds, roleClaim.AuthorityId));
                            }
                        }

                        //if (RoleManager.SupportsRoleClaims)
                        //{
                        //    var roleClaims = await RoleManager.GetClaimsAsync(role);
                        //    foreach (var roleClaim in roleClaims)
                        //    {
                        //        claims.Add(roleClaim);
                        //    }
                        //}
                    }
                }
            }

            id.AddClaims(claims);

            _logger.LogDebug(string.Format(Framework.Base.GlobalConfig.CurrentApplication?.AppName + "|Tenant: [{0}-{1}] Generate Claims: {2} in TenantUserClaimsPrincipal class.",
                _tenant?.TenantName, _tenant?.TenantDisplayName, idClaims.ToCommaSeparatedStringByFilter(m => m.Type + ":" + m.Value)));

            return id;
        }
    }

    /// <summary>
    /// 自定义 Resource owner password 验证器
    /// </summary>
    public class TenantResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        private readonly UserManager<User> _userManager;
        private readonly ILogger _logger;
        private readonly ISystemClock _clock;
        private readonly Tenant _tenant;
        public TenantResourceOwnerPasswordValidator(
            UserManager<User> userManager,
            ISystemClock clock,
            IHttpContextAccessor httpContextAccessor,
            ILogger<TenantResourceOwnerPasswordValidator> logger,
            Tenant tenant = null)
        {
            _userManager = userManager;
            _clock = clock;
            _logger = logger;
            _tenant = tenant;
        }

        /// <summary>
        /// 验证
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            User user;
            if (context.UserName.Contains("@"))
            {
                user = await _userManager.FindByEmailAsync(context.UserName);
            }
            else
            {
                user = await _userManager.FindByNameAsync(context.UserName);
            }

            if (user == null)
            {
                //验证失败
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "invalid custom credential");
                return;
            }
            else if (await _userManager.CheckPasswordAsync(user, context.Password))
            {
                var claims = await _userManager.GetClaimsAsync(user);
                claims.Add(new Claim(OpenIdConnectConstants.ClaimTypes_TenantName, _tenant?.TenantName));
                //验证通过返回结果 
                //subjectId 为用户唯一标识 一般为用户id
                //authenticationMethod 描述自定义授权类型的认证方法 
                //authTime 授权时间
                //claims 需要返回的用户身份信息单元 此处应该根据我们从数据库读取到的用户信息 添加Claims 如果是从数据库中读取角色信息，那么我们应该在此处添加 此处只返回必要的Claim
                context.Result = new GrantValidationResult(
                    user.Id ?? throw new ArgumentException("Subject ID not set", nameof(user.Id)),
                    OidcConstants.AuthenticationMethods.Password,
                    _clock.UtcNow.UtcDateTime,
                    claims);

                _logger.LogDebug(string.Format(Framework.Base.GlobalConfig.CurrentApplication?.AppName + "|Tenant: [{0}-{1}] Validate Claims: {2} in TenantResourceOwnerPasswordValidator class.",
                    _tenant?.TenantName, _tenant?.TenantDisplayName, claims.ToCommaSeparatedStringByFilter(m => m.Type)));
            }
            else
            {
                //验证失败
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "invalid custom credential");
            }
            return;
        }
    }

    public class TenantProfileService : DefaultProfileService
    {
        private readonly Tenant _tenant;

        /// <summary>
        /// The users
        /// </summary>
        private readonly UserManager<User> _userManager;

        private readonly RoleManager<Role> _roleManager;
        /// <summary>
        /// Initializes a new instance of the <see cref="TesUserProfileService"/> class.
        /// </summary>
        /// <param name="tenant">The tenant.</param>
        /// <param name="logger">The logger.</param>
        public TenantProfileService(
            UserManager<User> userManager, 
            RoleManager<Role> roleManager,
            ILogger<TenantProfileService> logger,
            Tenant tenant = null)
            : base(logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _tenant = tenant;
        }

        /// <summary>
        /// 只要有关用户的身份信息单元被请求（例如在令牌创建期间或通过用户信息终点），就会调用此方法
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public override async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            await base.GetProfileDataAsync(context);

            var idClaims = context.IssuedClaims;

            var clientId = !string.IsNullOrEmpty(context.Client.ClientId)
                        ? TenantConstant.GetDecodeClientId(context.Client.ClientId)
                        : context.Client.ClientName;

            var claims = new List<Claim>();
            if (!string.IsNullOrEmpty(clientId)
                && !idClaims.Any(m => m.Type.Equals(OpenIdConnectConstants.ClaimTypes_TenantName, StringComparison.OrdinalIgnoreCase)))
                claims.Add(new Claim(OpenIdConnectConstants.ClaimTypes_TenantName, clientId));

            //根据用户唯一标识查找用户信息
            var user = await _userManager.FindByIdAsync(context.Subject.GetSubjectId());
            if (user != null)
            {
                if (!idClaims.Any(m => m.Type.Equals(OpenIdConnectConstants.ClaimTypes_UserType, StringComparison.OrdinalIgnoreCase)))
                    claims.Add(new Claim(OpenIdConnectConstants.ClaimTypes_UserType, user.UserType.ToString()));
                if (!string.IsNullOrEmpty(user.DisplayName)
                    && !idClaims.Any(m => m.Type.Equals(OpenIdConnectConstants.ClaimTypes_DisplayName, StringComparison.OrdinalIgnoreCase)))
                    claims.Add(new Claim(OpenIdConnectConstants.ClaimTypes_DisplayName, user.DisplayName));
                if (!string.IsNullOrEmpty(user.Email)
                    && !idClaims.Any(m => m.Type.Equals(OpenIdConnectConstants.ClaimTypes_Email, StringComparison.OrdinalIgnoreCase)))
                    claims.Add(new Claim(OpenIdConnectConstants.ClaimTypes_Email, user.Email));
                if (!string.IsNullOrEmpty(user.PhoneNumber)
                    && !idClaims.Any(m => m.Type.Equals(OpenIdConnectConstants.ClaimTypes_Phone, StringComparison.OrdinalIgnoreCase)))
                    claims.Add(new Claim(OpenIdConnectConstants.ClaimTypes_Phone, user.PhoneNumber));

                if (user.UserOrganizations.Any())
                {
                    foreach (var org in user.UserOrganizations)
                        claims.Add(new Claim(OpenIdConnectConstants.ClaimTypes_OrgId, org.OrganizationId.ToString()));
                }

                if (user.Organizations.Any())
                {
                    foreach (var org in user.Organizations)
                    {
                        claims.Add(new Claim(OpenIdConnectConstants.ClaimTypes_OrgCode, org.OrganizationCode));
                        //claims.Add(new Claim(OpenIdConnectConstants.ClaimTypes_OrgName, org.Name));
                    }
                }
            }

            //if (_userManager.SupportsUserClaim)
            //{
            //    var userClaims = await _userManager.GetClaimsAsync(user);
            //    foreach (var userClaim in userClaims)
            //    {
            //        claims.Add(userClaim);
            //    }
            //}

            claims.Add(new Claim(OpenIdConnectConstants.ClaimTypes_AuthorityIds, ApplicationConstant.DefaultAuthorityId));
            if (_userManager.SupportsUserRole)
            {
                var roles = await _userManager.GetRolesAsync(user);
                foreach (var roleName in roles)
                {
                    if (_roleManager.SupportsRoleClaims)
                    {
                        var role = await _roleManager.FindByNameAsync(roleName);
                        if (role != null)
                        {
                            claims.Add(new Claim(OpenIdConnectConstants.ClaimTypes_RoleId, role.Id));
                            claims.Add(new Claim(OpenIdConnectConstants.ClaimTypes_RoleName, role.Name));
                            if (role.Permissions == null)
                                continue;

                            var rolePermissions = role.Permissions;
                            foreach (var roleClaim in rolePermissions)
                            {
                                if(!roleClaim.AuthorityId.IsNullOrEmpty()
                                    && !claims.Any(c => c.Value.Equals(roleClaim.AuthorityId)))
                                {
                                    claims.Add(new Claim(OpenIdConnectConstants.ClaimTypes_AuthorityIds, roleClaim.AuthorityId));
                                }
                                
                            }

                            //var roleClaims = await _roleManager.GetClaimsAsync(role);
                            //foreach (var roleClaim in roleClaims)
                            //{
                            //    claims.Add(roleClaim);
                            //}
                        }
                    }
                }
            }

            idClaims.AddRange(claims);

            Logger.LogDebug(string.Format(Framework.Base.GlobalConfig.CurrentApplication?.AppName + "|Tenant: [{0}-{1}] Profile Claims: {2} in TenantProfileService class.",
                _tenant?.TenantName, _tenant?.TenantDisplayName, idClaims.ToCommaSeparatedStringByFilter(m => m.Type + ":" + m.Value)));
            
            return;
        }

        /// <summary>
        /// 验证用户是否有效 例如：token创建或者验证
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public override async Task IsActiveAsync(IsActiveContext context)
        {
            var user = await _userManager.FindByIdAsync(context.Subject.GetSubjectId());
            context.IsActive = user?.Status == Framework.Base.WorkflowBusStatus.Approved;

            return;
        }

        private List<Claim> FilterClaims(ProfileDataRequestContext context, IEnumerable<Claim> claims)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (claims == null) throw new ArgumentNullException(nameof(claims));

            return claims.Where(x => context.RequestedClaimTypes.Contains(x.Type)).ToList();
        }
    }
}
