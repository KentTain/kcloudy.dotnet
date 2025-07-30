// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Threading.Tasks;
using KC.IdentityServer4.Validation;
using KC.IdentityServer4.Hosting;
using Microsoft.AspNetCore.Http;
using KC.IdentityServer4.Configuration;
using Microsoft.Extensions.DependencyInjection;
using KC.IdentityServer4.Models;
using KC.IdentityServer4.Stores;
using KC.IdentityServer4.Extensions;
using System;
using Microsoft.AspNetCore.Authentication;

namespace KC.IdentityServer4.Endpoints.Results
{
    /// <summary>
    /// Result for endsession
    /// </summary>
    /// <seealso cref="KC.IdentityServer4.Hosting.IEndpointResult" />
    public class EndSessionResult : IEndpointResult
    {
        private readonly EndSessionValidationResult _result;

        /// <summary>
        /// Initializes a new instance of the <see cref="EndSessionResult"/> class.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <exception cref="System.ArgumentNullException">result</exception>
        public EndSessionResult(EndSessionValidationResult result)
        {
            _result = result ?? throw new ArgumentNullException(nameof(result));
        }

        internal EndSessionResult(
            EndSessionValidationResult result,
            IdentityServerOptions options,
            ISystemClock clock,
            IMessageStore<LogoutMessage> logoutMessageStore)
            : this(result)
        {
            _options = options;
            _clock = clock;
            _logoutMessageStore = logoutMessageStore;
        }

        private IdentityServerOptions _options;
        private ISystemClock _clock;
        private IMessageStore<LogoutMessage> _logoutMessageStore;

        private void Init(HttpContext context)
        {
            _options = _options ?? context.RequestServices.GetRequiredService<IdentityServerOptions>();
            _clock = _clock ?? context.RequestServices.GetRequiredService<ISystemClock>();
            _logoutMessageStore = _logoutMessageStore ?? context.RequestServices.GetRequiredService<IMessageStore<LogoutMessage>>();
        }

        /// <summary>
        /// Executes the result.
        /// </summary>
        /// <param name="context">The HTTP context.</param>
        /// <returns></returns>
        public async Task ExecuteAsync(HttpContext context)
        {
            Init(context);
            
            var redirect = _options.UserInteraction.LogoutUrl;
            if (redirect.IsLocalUrl())
            {
                redirect = context.GetIdentityServerRelativeUrl(redirect);
            }

            var validatedRequest = _result.IsError ? null : _result.ValidatedRequest;
            if (validatedRequest != null)
            {
                var logoutMessage = new LogoutMessage(validatedRequest);
                if (logoutMessage.ContainsPayload)
                {
                    var msg = new Message<LogoutMessage>(logoutMessage, _clock.UtcNow.UtcDateTime);
                    var id = await _logoutMessageStore.WriteAsync(msg);
                    if (id != null)
                    {
                        redirect = redirect.AddQueryString(_options.UserInteraction.LogoutIdParameter, id);
                    }
                }

                //Multi-tenant: for different tenant logout
                var client = validatedRequest.Client;
                var tenantName = client.ClientName;
                if (!tenantName.IsNullOrEmpty() && !redirect.Contains(tenantName))
                {
                    if (redirect.StartsWith("http"))
                    {
                        redirect = redirect.Replace("http://", "http://" + tenantName + ".");
                    }
                    else
                    {
                        redirect = redirect.Replace("https://", "https://" + tenantName + ".");
                    }
                }
            }

            context.Response.Redirect(redirect);
        }
    }
}
