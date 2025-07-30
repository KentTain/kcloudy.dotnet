using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using KC.IdentityServer4.Services;
using KC.IdentityServer4.Events;
using KC.IdentityServer4.Stores;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using KC.IdentityServer4.Validation;
using KC.Framework.Tenant;

namespace KC.Web.SSO.Services
{
    /// <summary>
    /// 未使用 </br>
    /// Validates a client secret using the registered secret validators and parsers
    /// </summary>
    public class CustomClientSecretValidator : IClientSecretValidator
    {
        private readonly ILogger _logger;
        private readonly IClientStore _clients;
        private readonly IEventService _events;
        private readonly SecretValidator _validator;
        private readonly SecretParser _parser;
        protected readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomClientSecretValidator"/> class.
        /// </summary>
        /// <param name="clients">The clients.</param>
        /// <param name="parser">The parser.</param>
        /// <param name="validator">The validator.</param>
        /// <param name="events">The events.</param>
        /// <param name="logger">The logger.</param>
        public CustomClientSecretValidator(
            IClientStore clients,
            SecretParser parser,
            SecretValidator validator,
            IEventService events,
            IHttpContextAccessor httpContextAccessor,
            ILogger<CustomClientSecretValidator> logger)
        {
            _clients = clients;
            _parser = parser;
            _validator = validator;
            _events = events;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Validates the current request.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public async Task<ClientSecretValidationResult> ValidateAsync(HttpContext context)
        {
            var fail = new ClientSecretValidationResult
            {
                IsError = true
            };

            SaasKit.Multitenancy.TenantContext<Tenant> tenantContext = _httpContextAccessor.HttpContext.GetTenantContext<Tenant>();
            if (tenantContext == null)
            {
                _logger.LogError("No tenant context found");
                return fail;
            }

            var returnUrl = context.Request.Path;
            
            var parsedSecret = await _parser.ParseAsync(context);
            if (parsedSecret == null)
            {
                await RaiseFailureEventAsync("unknown", "No client id found");

                _logger.LogError("No client identifier found");
                return fail;
            }

            //var tenantName = tenantContext.Tenant.TenantName;
            //var parsedTenantName = TenantConstant.GetDecodeClientId(parsedSecret.Id);
            //if (!tenantName.Equals(parsedTenantName, StringComparison.OrdinalIgnoreCase))
            //{
            //    await RaiseFailureEventAsync("unknown", "Can't use other tenant's Secret Id");

            //    _logger.LogError("Can't use other tenant's Secret Id");
            //    return fail;
            //}

            _logger.LogDebug("CustomClientSecretValidator Start client validation：" + returnUrl);

            // load client
            var client = await _clients.FindEnabledClientByIdAsync(parsedSecret.Id);
            if (client == null)
            { 
                await RaiseFailureEventAsync(parsedSecret.Id, "Unknown client");

                _logger.LogError("No client with id '{clientId}' found. aborting", parsedSecret.Id);
                return fail;
            }

            SecretValidationResult secretValidationResult = null;
            if (!client.RequireClientSecret /*|| client.IsImplicitOnly()*/)
            {
                _logger.LogDebug("Public Client - skipping secret validation success");
            }
            else
            {
                var secret = parsedSecret.Credential as string;
                var secretSha256 = Framework.Tenant.TenantConstant.Sha256(secret);
                //_logger.LogWarning("-----ClientSecret: {Secret}; ClientSecret Sha256: {secretSha256}.", secret, secretSha256);

                secretValidationResult = await _validator.ValidateAsync(parsedSecret, client.ClientSecrets);
                if (secretValidationResult.Success == false)
                {
                    await RaiseFailureEventAsync(client.ClientId, "Invalid client secret");
                    _logger.LogError("Client secret validation failed for client: {clientId}.", client.ClientId);

                    return fail;
                }
            }

            _logger.LogDebug("Client validation success");

            var success = new ClientSecretValidationResult
            {
                IsError = false,
                Client = client,
                Secret = parsedSecret,
                //Confirmation = secretValidationResult?.Confirmation
            };

            await RaiseSuccessEventAsync(client.ClientId, parsedSecret.Type);
            return success;
        }

        private Task RaiseSuccessEventAsync(string clientId, string authMethod)
        {
            return _events.RaiseAsync(new ClientAuthenticationSuccessEvent(clientId, authMethod));
        }

        private Task RaiseFailureEventAsync(string clientId, string message)
        {
            return _events.RaiseAsync(new ClientAuthenticationFailureEvent(clientId, message));
        }
    }
}
