﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KC.IdentityServer4.Events;
using KC.IdentityServer4.Services;
using KC.IdentityServer4.Stores;
using KC.IdentityServer4.Validation;
using KC.Framework.Tenant;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace KC.Web.SSO.Services
{
    /// <summary>
    /// 未使用
    /// Validates API secrets using the registered secret validators and parsers
    /// </summary>
    public class CustomApiSecretValidator : IApiSecretValidator
    {
        private readonly ILogger _logger;
        private readonly IResourceStore _resources;
        private readonly IEventService _events;
        private readonly SecretParser _parser;
        private readonly SecretValidator _validator;
        protected readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomApiSecretValidator"/> class.
        /// </summary>
        /// <param name="resources">The resources.</param>
        /// <param name="parsers">The parsers.</param>
        /// <param name="validator">The validator.</param>
        /// <param name="events">The events.</param>
        /// <param name="logger">The logger.</param>
        public CustomApiSecretValidator(
            IResourceStore resources, 
            SecretParser parsers, 
            SecretValidator validator, 
            IEventService events,
            IHttpContextAccessor httpContextAccessor,
            ILogger<CustomApiSecretValidator> logger)
        {
            _resources = resources;
            _parser = parsers;
            _validator = validator;
            _events = events;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Validates the secret on the current request.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public async Task<ApiSecretValidationResult> ValidateAsync(HttpContext context)
        {
            var fail = new ApiSecretValidationResult
            {
                IsError = true
            };

            var returnUrl = context.Request.Path;

            _logger.LogDebug(Framework.Base.GlobalConfig.CurrentApplication?.AppName + "|CustomApiSecretValidator Start client validation：" + returnUrl);

            SaasKit.Multitenancy.TenantContext<Tenant> tenantContext = _httpContextAccessor.HttpContext.GetTenantContext<Tenant>();
            if (tenantContext == null)
            {
                _logger.LogError("No tenant context found");
                return fail;
            }

            var parsedSecret = await _parser.ParseAsync(context);
            if (parsedSecret == null)
            {
                await RaiseFailureEventAsync("unknown", "No API id or secret found");

                _logger.LogError("No API secret found");
                return fail;
            }

            var tenantName = tenantContext.Tenant.TenantName;
            var parsedTenantName = TenantConstant.GetDecodeClientId(parsedSecret.Id);
            if (!tenantName.Equals(parsedTenantName, StringComparison.OrdinalIgnoreCase))
            {
                await RaiseFailureEventAsync("unknown", "Can't use other tenant api's Secret Id");

                _logger.LogError("Can't use other tenant api's Secret Id");
                return fail;
            }

            // load API resource
            var api = await _resources.FindApiResourceAsync(parsedSecret.Id);
            if (api == null)
            {
                await RaiseFailureEventAsync(parsedSecret.Id, "Unknown API resource");

                _logger.LogError("No API resource with that name found. aborting");
                return fail;
            }

            if (api.Enabled == false)
            {
                await RaiseFailureEventAsync(parsedSecret.Id, "API resource not enabled");

                _logger.LogError("API resource not enabled. aborting.");
                return fail;
            }

            var result = await _validator.ValidateAsync(parsedSecret, api.ApiSecrets);
            if (result.Success)
            {
                _logger.LogDebug("API resource validation success");

                var success = new ApiSecretValidationResult
                {
                    IsError = false,
                    Resource = api
                };

                await RaiseSuccessEventAsync(api.Name, parsedSecret.Type);
                return success;
            }

            await RaiseFailureEventAsync(api.Name, "Invalid API secret");
            _logger.LogError("API validation failed.");

            return fail;
        }

        private Task RaiseSuccessEventAsync(string clientId, string authMethod)
        {
            return _events.RaiseAsync(new ApiAuthenticationSuccessEvent(clientId, authMethod));
        }

        private Task RaiseFailureEventAsync(string clientId, string message)
        {
            return _events.RaiseAsync(new ApiAuthenticationFailureEvent(clientId, message));
        }
    }
}
