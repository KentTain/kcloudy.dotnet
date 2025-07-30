using KC.IdentityServer4.Configuration;
using KC.IdentityServer4.Validation;
using KC.Framework.Tenant;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KC.Web.SSO.Services
{
    /// <summary>
    /// 未使用 </br>
    /// </summary>
    public class CustomTokenRequestValidator : ICustomTokenRequestValidator
    {
        private readonly ILogger _logger;
        private readonly IdentityServerOptions _options;
        private readonly ITokenValidator _tokenValidator;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CustomTokenRequestValidator(
            IdentityServerOptions options,
            ITokenValidator tokenValidator,
            IHttpContextAccessor httpContextAccessor,
            ILogger<CustomTokenRequestValidator> logger)
        {
            _options = options;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _tokenValidator = tokenValidator;
        }
        /// <summary>
        /// Custom validation logic for a token request.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>
        /// The validation result
        /// </returns>
        public Task ValidateAsync(CustomTokenRequestValidationContext context)
        {
            TokenRequestValidationResult result = null;
            SaasKit.Multitenancy.TenantContext<Tenant> tenantContext = _httpContextAccessor.HttpContext.GetTenantContext<Tenant>();
            if (tenantContext == null)
            {
                _logger.LogError("No tenant context found in CustomTokenRequestValidator");
                context.Result = Invalid("No tenant context found in CustomTokenRequestValidator");
                return Task.CompletedTask;
            }

            //var clientId = context.Result.ValidatedRequest?.Client?.ClientId;
            //if (!string.IsNullOrEmpty(clientId))
            //{
            //    var tenantName = tenantContext.Tenant.TenantName;
            //    var parsedTenantName = TenantConstant.GetDecodeClientId(clientId);
            //    if (!tenantName.Equals(parsedTenantName, StringComparison.OrdinalIgnoreCase))
            //    {
            //        _logger.LogError("Id or Credential is not the current tenant have in CustomTokenRequestValidator");
            //        context.Result = Invalid("Id or Credential is not the current tenant have in CustomTokenRequestValidator");
            //        return Task.CompletedTask;
            //    }
            //}

            return Task.CompletedTask;
        }

        private TokenRequestValidationResult Invalid(string error, string errorDescription = null, Dictionary<string, object> customResponse = null)
        {
            var validatedRequest = new ValidatedTokenRequest
            {
                Raw = null,
                Options = _options
            };

            return new TokenRequestValidationResult(validatedRequest, error, errorDescription, customResponse);
        }
    }
}
