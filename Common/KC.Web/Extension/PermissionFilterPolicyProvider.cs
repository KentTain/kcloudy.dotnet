using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace KC.Web.Extension
{
    /// <summary>
    /// 权限验证策略（策略名为：MenuName-PermissionName）
    /// </summary>
    public class PermissionFilterPolicyProvider : IAuthorizationPolicyProvider
    {
        private AuthorizationOptions _options;
        private ILogger<PermissionFilterPolicyProvider> _logger;
        public PermissionFilterPolicyProvider(IOptions<AuthorizationOptions> options, ILogger<PermissionFilterPolicyProvider> logger)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            _options = options.Value;
            _logger = logger;
        }
        public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
        {
            return Task.FromResult(_options.DefaultPolicy);
        }

        public Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            AuthorizationPolicy policy = _options.GetPolicy(policyName);
            //_logger.LogDebug(Framework.Base.GlobalConfig.CurrentApplication?.AppName + "|PermissionFilterPolicyProvider: " + policyName);

            //因为我们policy的名称其实就是对应的权限名称，所以可以用下列逻辑返回需要的验证规则
            if (policy == null)
            {
                string[] resourceValues = policyName.Split(new char[] { Framework.Tenant.ApplicationConstant.DefaultAuthoritySplitChar }, StringSplitOptions.None);
                if (resourceValues.Length == 1)
                {
                    _options.AddPolicy(policyName, builder =>
                    {
                        //builder.AddRequirements(new ClaimsAuthorizationRequirement(resourceValues[0], null));
                        builder.AddRequirements(new ClaimsAuthorizationRequirement(Constants.OpenIdConnectConstants.ClaimTypes_AuthorityIds, new string[] { resourceValues[0] }));
                    });
                }
                else
                {
                    _options.AddPolicy(policyName, builder =>
                    {
                        builder.AddRequirements(new ClaimsAuthorizationRequirement(resourceValues[0], new string[] { resourceValues[1] }));
                    });
                }
            }
            return Task.FromResult(_options.GetPolicy(policyName));
        }

        Task<AuthorizationPolicy> IAuthorizationPolicyProvider.GetFallbackPolicyAsync()
        {
            return Task.FromResult<AuthorizationPolicy>(null);
        }
    }
}
