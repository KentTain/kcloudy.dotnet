using KC.Model.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace KC.Web.SSO.Core
{
    /// <summary>Copy of Microsoft.AspNetCore.Identity UserValidator with added SiteId checking</summary>
    public class TenantUserValidator<TUser> : UserValidator<TUser> where TUser : User
    {
        public TenantUserValidator(IOptions<IdentityOptions> optionsAccessor, IdentityErrorDescriber errors = null)
        {
            Options = optionsAccessor?.Value ?? new IdentityOptions();
            Describer = errors ?? new IdentityErrorDescriber();
        }

        protected internal IdentityOptions Options { get; set; }

        public new IdentityErrorDescriber Describer { get; private set; }
    }

    public class TenantUserStore
        //: UserStore<User, Role, Microsoft.EntityFrameworkCore.DbContext, string>
        : UserStore<User, Role, DbContext, string, IdentityUserClaim<string>, UsersInRoles, IdentityUserLogin<string>, IdentityUserToken<string>, IdentityRoleClaim<string>>
    {
        private readonly ILogger _logger;
        public TenantUserStore(
            DataAccess.Account.ComAccountContext context,
            ILogger<TenantUserStore> logger,
            IdentityErrorDescriber describer = null)
            : base(context, describer)
        {
            _logger = logger;
        }

        public override Task<User> FindByIdAsync(string userId, CancellationToken cancellationToken = default(CancellationToken))
        {
            _logger.LogDebug(string.Format(Framework.Base.GlobalConfig.CurrentApplication?.AppName + "|Tenant: {0} FindByIdAsync {1} with Permissions in TenantUserStore class.",
                (Context as DataAccess.Account.ComAccountContext)?.TenantName, userId));

            return Users
                .Include(m => m.UserRoles)
                .ThenInclude(r => r.Role)
                .ThenInclude(p => p.Permissions)
                .FirstOrDefaultAsync(m => m.Id == userId);
        }
    }
}
