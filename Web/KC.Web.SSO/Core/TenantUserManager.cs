using KC.Framework.Tenant;
using KC.Model.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KC.Web.SSO
{
    public class TenantUserManager : UserManager<User>, IDisposable
    {
        private readonly Tenant _tenant;
        public TenantUserManager(
            IUserStore<User> store,
            IOptions<IdentityOptions> optionsAccessor,
            IPasswordHasher<User> passwordHasher,
            IEnumerable<IUserValidator<User>> userValidators,
            IEnumerable<IPasswordValidator<User>> passwordValidators,
            ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors,
            IServiceProvider services,
            ILogger<TenantUserManager> logger,
            Tenant tenant = null)
            : base(store,
                  optionsAccessor,
                  passwordHasher,
                  userValidators,
                  passwordValidators,
                  keyNormalizer,
                  errors,
                  services,
                  logger)
        {
            _tenant = tenant;
        }

        public override Task<User> FindByIdAsync(string userId)
        {
            Logger.LogDebug(string.Format(Framework.Base.GlobalConfig.CurrentApplication?.AppName + "|Tenant: [{0}-{1}] FindByIdAsync {2} with Permissions in TenantUserManager class.",
                _tenant?.TenantName, _tenant?.TenantDisplayName, userId));

            var user = Users
                .Include(m => m.UserRoles)
                .ThenInclude(r => r.Role)
                //.ThenInclude(p => p.RolePermissions)
                .Include(m => m.UserOrganizations)
                .ThenInclude(r => r.Organization)
                .FirstOrDefaultAsync(m => m.Id == userId);
            return user;
        }

        public override Task<User> FindByEmailAsync(string userEmail)
        {
            Logger.LogDebug(string.Format(Framework.Base.GlobalConfig.CurrentApplication?.AppName + "|Tenant: [{0}-{1}] FindByEmailAsync {2} with Permissions in TenantUserManager class.",
                TenantConstant.DbaTenantName, TenantConstant.DbaTenantName, userEmail));

            var user = Users
                .Include(m => m.UserRoles)
                .ThenInclude(r => r.Role)
                //.ThenInclude(p => p.RolePermissions)
                .Include(m => m.UserOrganizations)
                .ThenInclude(r => r.Organization)
                .FirstOrDefaultAsync(m => m.Email.Equals(userEmail));
            return user;
        }

        public override Task<User> FindByNameAsync(string userName)
        {
            Logger.LogDebug(string.Format(Framework.Base.GlobalConfig.CurrentApplication?.AppName + "|Tenant: [{0}-{1}] FindByNameAsync {2} with Permissions in TenantUserManager class.",
                TenantConstant.DbaTenantName, TenantConstant.DbaTenantName, userName));

            var user = Users
                .Include(m => m.UserRoles)
                .ThenInclude(r => r.Role)
                //.ThenInclude(p => p.RolePermissions)
                .Include(m => m.UserOrganizations)
                .ThenInclude(r => r.Organization)
                .FirstOrDefaultAsync(m => m.UserName.Equals(userName) || m.PhoneNumber.Equals(userName));
            return user;
        }
    }

    public class TenantRoleManager : RoleManager<Role>, IDisposable
    {
        private readonly Tenant _tenant;
        public TenantRoleManager(
            IRoleStore<Role> store,
            IEnumerable<IRoleValidator<Role>> userValidators,
            ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors,
            ILogger<TenantRoleManager> logger,
            Tenant tenant = null)
            : base(store,
                  userValidators,
                  keyNormalizer,
                  errors,
                  logger)
        {
            _tenant = tenant;
        }

        public override Task<Role> FindByIdAsync(string roleId)
        {
            var user = Roles
                .Include(m => m.RolePermissions)
                .ThenInclude(r => r.Permission)
                .FirstOrDefaultAsync(m => m.Id == roleId);
            return user;
        }

        public override Task<Role> FindByNameAsync(string roleName)
        {
            var user = Roles
                .Include(m => m.RolePermissions)
                .ThenInclude(r => r.Permission)
                .FirstOrDefaultAsync(m => m.Name == roleName);
            return user;
        }
    }
}
