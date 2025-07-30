using KC.IdentityModel;
using KC.IdentityServer4;
using KC.IdentityServer4.Extensions;
using KC.IdentityServer4.Models;
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
    public class CustomHashedSharedSecretValidator : ISecretValidator
    {
        private readonly ILogger _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        /// <summary>
        /// Initializes a new instance of the <see cref="HashedSharedSecretValidator"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public CustomHashedSharedSecretValidator(
            IHttpContextAccessor httpContextAccessor,
            ILogger<CustomHashedSharedSecretValidator> logger
            )
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        /// <summary>
        /// Validates a secret
        /// </summary>
        /// <param name="secrets">The stored secrets.</param>
        /// <param name="parsedSecret">The received secret.</param>
        /// <returns>
        /// A validation result
        /// </returns>
        /// <exception cref="System.ArgumentNullException">Id or cedential</exception>
        public Task<SecretValidationResult> ValidateAsync(IEnumerable<Secret> secrets, ParsedSecret parsedSecret)
        {
            var fail = Task.FromResult(new SecretValidationResult { Success = false });
            var success = Task.FromResult(new SecretValidationResult { Success = true });

            if (parsedSecret.Type != IdentityServerConstants.ParsedSecretTypes.SharedSecret)
            {
                _logger.LogDebug("Hashed shared secret validator cannot process {type}", parsedSecret.Type ?? "null");
                return fail;
            }

            var sharedSecrets = secrets.Where(s => s.Type == IdentityServerConstants.SecretTypes.SharedSecret);
            if (!sharedSecrets.Any())
            {
                _logger.LogDebug("No shared secret configured for client.");
                return fail;
            }

            var sharedSecret = parsedSecret.Credential as string;

            if (string.IsNullOrWhiteSpace(parsedSecret.Id) || string.IsNullOrWhiteSpace(sharedSecret))
            {
                throw new ArgumentException("Id or Credential is missing.");
            }

            SaasKit.Multitenancy.TenantContext<Tenant> tenantContext = _httpContextAccessor.HttpContext.GetTenantContext<Tenant>();
            if (tenantContext == null)
            {
                _logger.LogError("No tenant context found in CustomHashedSharedSecretValidator");
                return fail;
            }

            var tenantName = tenantContext.Tenant.TenantName;
            var parsedTenantName = TenantConstant.GetDecodeClientId(parsedSecret.Id);
            if (!tenantName.Equals(parsedTenantName, StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogError("Id or Credential is not the current tenant have in CustomHashedSharedSecretValidator");
                return fail;
            }

            var secretSha256 = sharedSecret.Sha256();
            var secretSha512 = sharedSecret.Sha512();

            foreach (var secret in sharedSecrets)
            {
                var secretDescription = string.IsNullOrEmpty(secret.Description) ? "no description" : secret.Description;

                bool isValid = false;
                byte[] secretBytes;

                try
                {
                    secretBytes = Convert.FromBase64String(secret.Value);
                }
                catch (FormatException)
                {
                    _logger.LogInformation("Secret: {description} uses invalid hashing algorithm.", secretDescription);
                    return fail;
                }
                
                if (secretBytes.Length == 32)
                {
                    isValid = TimeConstantComparer.IsEqual(secret.Value, secretSha256);
                }
                else if (secretBytes.Length == 64)
                {
                    isValid = TimeConstantComparer.IsEqual(secret.Value, secretSha512);
                }
                else
                {
                    _logger.LogInformation("Secret: {description} uses invalid hashing algorithm.", secretDescription);
                    return fail;
                }

                if (isValid)
                {
                    return success;
                }
            }

            _logger.LogDebug("No matching hashed secret found.");
            return fail;
        }
    }
}
