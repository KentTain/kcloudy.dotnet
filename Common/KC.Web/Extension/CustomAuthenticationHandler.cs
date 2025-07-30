using KC.IdentityModel.AspNetCore.OAuth2Introspection;
using KC.IdentityModel.Client;
using KC.IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace KC.Web.Extension
{
    public class CustomAuthenticationHandler : AuthenticationHandler<IdentityServerAuthenticationOptions>
    {
        public const string IntrospectionAuthenticationScheme = "IdentityServerAuthenticationIntrospection";
        public const string JwtAuthenticationScheme = "IdentityServerAuthenticationJwt";
        public const string TokenItemsKey = "idsrv4:tokenvalidation:token";
        public const string EffectiveSchemeKey = "idsrv4:tokenvalidation:effective:";

        private readonly ILogger _logger;

        /// <inheritdoc />
        public CustomAuthenticationHandler(
            IOptionsMonitor<IdentityServerAuthenticationOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
            _logger = logger.CreateLogger<CustomAuthenticationHandler>();
        }

        /// <summary>
        /// Tries to validate a token on the current request
        /// </summary>
        /// <returns></returns>
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            _logger.LogTrace("HandleAuthenticateAsync called");

            var jwtScheme = Scheme.Name + JwtAuthenticationScheme;
            var introspectionScheme = Scheme.Name + IntrospectionAuthenticationScheme;

            var token = Options.TokenRetriever(Context.Request);
            bool removeToken = false;

            try
            {
                if (token != null)
                {
                    _logger.LogTrace("Token found: {token}", token);

                    removeToken = true;
                    Context.Items.Add(TokenItemsKey, token);

                    // seems to be a JWT
                    if (token.Contains('.') && Options.SupportsJwt)
                    {
                        _logger.LogTrace("Token is a JWT and is supported.");

                        Context.Items.Add(EffectiveSchemeKey + Scheme.Name, jwtScheme);
                        return await Context.AuthenticateAsync(jwtScheme);
                    }
                    else if (Options.SupportsIntrospection)
                    {
                        _logger.LogTrace("Token is a reference token and is supported.");

                        Context.Items.Add(EffectiveSchemeKey + Scheme.Name, introspectionScheme);
                        return await Context.AuthenticateAsync(introspectionScheme);
                    }
                    else
                    {
                        _logger.LogTrace("Neither JWT nor reference tokens seem to be correctly configured for incoming token.");
                    }
                }

                // set the default challenge handler to JwtBearer if supported
                if (Options.SupportsJwt)
                {
                    Context.Items.Add(EffectiveSchemeKey + Scheme.Name, jwtScheme);
                }

                return AuthenticateResult.NoResult();
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return AuthenticateResult.NoResult();
            }
            finally
            {
                if (removeToken)
                {
                    Context.Items.Remove(TokenItemsKey);
                }
            }
        }

        /// <summary>
        /// Override this method to deal with 401 challenge concerns, if an authentication scheme in question
        /// deals an authentication interaction as part of it's request flow. (like adding a response header, or
        /// changing the 401 result to 302 of a login page or external sign-in location.)
        /// </summary>
        /// <param name="properties"></param>
        /// <returns>
        /// A Task.
        /// </returns>
        protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            if (Context.Items.TryGetValue(EffectiveSchemeKey + Scheme.Name, out object value))
            {
                if (value is string scheme)
                {
                    _logger.LogTrace("Forwarding challenge to scheme: {scheme}", scheme);
                    await Context.ChallengeAsync(scheme);
                }
            }
            else
            {
                await base.HandleChallengeAsync(properties);
            }
        }
    }

    public class ConfigureInternalOptions :
        IConfigureNamedOptions<JwtBearerOptions>,
        IConfigureNamedOptions<OAuth2IntrospectionOptions>
    {
        private readonly IdentityServerAuthenticationOptions _identityServerOptions;
        private string _scheme;

        public ConfigureInternalOptions(
            IdentityServerAuthenticationOptions identityServerOptions, 
            string scheme)
        {
            _identityServerOptions = identityServerOptions;
            _scheme = scheme;
        }

        public void Configure(string name, JwtBearerOptions options)
        {
            if (name == _scheme + CustomAuthenticationHandler.JwtAuthenticationScheme &&
                _identityServerOptions.SupportsJwt)
            {
                ConfigureJwtBearer(options);
            }
        }

        public void Configure(string name, OAuth2IntrospectionOptions options)
        {
            if (name == _scheme + CustomAuthenticationHandler.IntrospectionAuthenticationScheme &&
                _identityServerOptions.SupportsIntrospection)
            {
                ConfigureIntrospection(options);
            }
        }

        public void Configure(JwtBearerOptions options)
        { }

        public void Configure(OAuth2IntrospectionOptions options)
        { }

        static readonly Func<HttpRequest, string> InternalTokenRetriever = request => request.HttpContext.Items[CustomAuthenticationHandler.TokenItemsKey] as string;
        internal void ConfigureJwtBearer(JwtBearerOptions jwtOptions)
        {
            jwtOptions.Authority = _identityServerOptions.Authority;
            jwtOptions.RequireHttpsMetadata = _identityServerOptions.RequireHttpsMetadata;
            jwtOptions.BackchannelTimeout = _identityServerOptions.BackChannelTimeouts;
            jwtOptions.RefreshOnIssuerKeyNotFound = true;
            jwtOptions.SaveToken = _identityServerOptions.SaveToken;

            jwtOptions.Events = new JwtBearerEvents
            {
                OnMessageReceived = e =>
                {
                    e.Token = InternalTokenRetriever(e.Request);
                    return _identityServerOptions.JwtBearerEvents.MessageReceived(e);
                },

                OnTokenValidated = e => _identityServerOptions.JwtBearerEvents.TokenValidated(e),
                OnAuthenticationFailed = e => _identityServerOptions.JwtBearerEvents.AuthenticationFailed(e),
                OnChallenge = e => _identityServerOptions.JwtBearerEvents.Challenge(e)
            };

            if (_identityServerOptions.DiscoveryDocumentRefreshInterval.HasValue)
            {
                var parsedUrl = DiscoveryEndpoint.ParseUrl(_identityServerOptions.Authority);

                var httpClient = new System.Net.Http.HttpClient(_identityServerOptions.JwtBackChannelHandler ?? new System.Net.Http.HttpClientHandler())
                {
                    Timeout = _identityServerOptions.BackChannelTimeouts,
                    MaxResponseContentBufferSize = 1024 * 1024 * 10 // 10 MB
                };

                var manager = new ConfigurationManager<OpenIdConnectConfiguration>(
                    parsedUrl.Url,
                    new OpenIdConnectConfigurationRetriever(),
                    new HttpDocumentRetriever(httpClient) { RequireHttps = _identityServerOptions.RequireHttpsMetadata })
                {
                    AutomaticRefreshInterval = _identityServerOptions.DiscoveryDocumentRefreshInterval.Value
                };

                jwtOptions.ConfigurationManager = manager;
            }

            if (_identityServerOptions.JwtBackChannelHandler != null)
            {
                jwtOptions.BackchannelHttpHandler = _identityServerOptions.JwtBackChannelHandler;
            }

            // if API name is set, do a strict audience check for
            if (!string.IsNullOrWhiteSpace(_identityServerOptions.ApiName) && !_identityServerOptions.LegacyAudienceValidation)
            {
                jwtOptions.Audience = _identityServerOptions.ApiName;
            }
            else
            {
                // no audience validation, rely on scope checks only
                jwtOptions.TokenValidationParameters.ValidateAudience = false;
            }

            //jwtOptions.TokenValidationParameters.ValidateIssuer = false;

            jwtOptions.TokenValidationParameters.NameClaimType = _identityServerOptions.NameClaimType;
            jwtOptions.TokenValidationParameters.RoleClaimType = _identityServerOptions.RoleClaimType;

            if (_identityServerOptions.JwtValidationClockSkew.HasValue)
            {
                jwtOptions.TokenValidationParameters.ClockSkew = _identityServerOptions.JwtValidationClockSkew.Value;
            }

            var handler = new JwtSecurityTokenHandler
            {
                MapInboundClaims = false
            };
            jwtOptions.SecurityTokenValidators.Clear();
            jwtOptions.SecurityTokenValidators.Add(handler);
        }

        internal void ConfigureIntrospection(OAuth2IntrospectionOptions introspectionOptions)
        {
            if (String.IsNullOrWhiteSpace(_identityServerOptions.ApiSecret))
            {
                return;
            }

            if (String.IsNullOrWhiteSpace(_identityServerOptions.ApiName))
            {
                throw new ArgumentException("ApiName must be configured if ApiSecret is set.");
            }

            introspectionOptions.Authority = _identityServerOptions.Authority;
            introspectionOptions.ClientId = _identityServerOptions.ApiName;
            introspectionOptions.ClientSecret = _identityServerOptions.ApiSecret;
            introspectionOptions.NameClaimType = _identityServerOptions.NameClaimType;
            introspectionOptions.RoleClaimType = _identityServerOptions.RoleClaimType;
            introspectionOptions.TokenRetriever = InternalTokenRetriever;
            introspectionOptions.SaveToken = _identityServerOptions.SaveToken;
            //introspectionOptions.DiscoveryPolicy = _identityServerOptions.IntrospectionDiscoveryPolicy;

            introspectionOptions.EnableCaching = _identityServerOptions.EnableCaching;
            introspectionOptions.CacheDuration = _identityServerOptions.CacheDuration;
            introspectionOptions.CacheKeyPrefix = _identityServerOptions.CacheKeyPrefix;

            if (_identityServerOptions.OAuth2IntrospectionEvents != null)
            {
                introspectionOptions.Events = _identityServerOptions.OAuth2IntrospectionEvents;
            }
        }
    }
}
