// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using KC.IdentityModel;
using KC.IdentityServer4.Configuration;
using KC.IdentityServer4.Extensions;
using KC.IdentityServer4.Services;
using KC.IdentityServer4.Stores;
using KC.IdentityServer4.Validation;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace KC.IdentityServer4.ResponseHandling
{
    /// <summary>
    /// Default implementation of the discovery endpoint response generator
    /// </summary>
    /// <seealso cref="KC.IdentityServer4.ResponseHandling.IDiscoveryResponseGenerator" />
    public class DiscoveryResponseGenerator : IDiscoveryResponseGenerator
    {
        /// <summary>
        /// The options
        /// </summary>
        protected readonly IdentityServerOptions Options;

        /// <summary>
        /// The extension grants validator
        /// </summary>
        protected readonly ExtensionGrantValidator ExtensionGrants;

        /// <summary>
        /// The key material service
        /// </summary>
        protected readonly IKeyMaterialService Keys;

        /// <summary>
        /// The resource owner validator
        /// </summary>
        protected readonly IResourceOwnerPasswordValidator ResourceOwnerValidator;

        /// <summary>
        /// The resource store
        /// </summary>
        protected readonly IResourceStore ResourceStore;

        /// <summary>
        /// The secret parsers
        /// </summary>
        protected readonly SecretParser SecretParsers;

        /// <summary>
        /// The logger
        /// </summary>
        protected readonly ILogger Logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="DiscoveryResponseGenerator"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="resourceStore">The resource store.</param>
        /// <param name="keys">The keys.</param>
        /// <param name="extensionGrants">The extension grants.</param>
        /// <param name="secretParsers">The secret parsers.</param>
        /// <param name="resourceOwnerValidator">The resource owner validator.</param>
        /// <param name="logger">The logger.</param>
        public DiscoveryResponseGenerator(
            IdentityServerOptions options,
            IResourceStore resourceStore,
            IKeyMaterialService keys,
            ExtensionGrantValidator extensionGrants,
            SecretParser secretParsers,
            IResourceOwnerPasswordValidator resourceOwnerValidator,
            ILogger<DiscoveryResponseGenerator> logger)
        {
            Options = options;
            ResourceStore = resourceStore;
            Keys = keys;
            ExtensionGrants = extensionGrants;
            SecretParsers = secretParsers;
            ResourceOwnerValidator = resourceOwnerValidator;
            Logger = logger;
        }

        /// <summary>
        /// Creates the discovery document.
        /// </summary>
        /// <param name="baseUrl">The base URL.</param>
        /// <param name="issuerUri">The issuer URI.</param>
        public virtual async Task<Dictionary<string, object>> CreateDiscoveryDocumentAsync(string baseUrl, string issuerUri)
        {
            var entries = new Dictionary<string, object>
            {
                { OidcConstants.Discovery.Issuer, issuerUri }
            };

            // jwks
            if (Options.Discovery.ShowKeySet)
            {
                if ((await Keys.GetValidationKeysAsync()).Any())
                {
                    entries.Add(OidcConstants.Discovery.JwksUri, baseUrl + Constants.ProtocolRoutePaths.DiscoveryWebKeys);
                }
            }

            // endpoints
            if (Options.Discovery.ShowEndpoints)
            {
                if (Options.Endpoints.EnableAuthorizeEndpoint)
                {
                    entries.Add(OidcConstants.Discovery.AuthorizationEndpoint, baseUrl + Constants.ProtocolRoutePaths.Authorize);
                }

                if (Options.Endpoints.EnableTokenEndpoint)
                {
                    entries.Add(OidcConstants.Discovery.TokenEndpoint, baseUrl + Constants.ProtocolRoutePaths.Token);
                }

                if (Options.Endpoints.EnableUserInfoEndpoint)
                {
                    entries.Add(OidcConstants.Discovery.UserInfoEndpoint, baseUrl + Constants.ProtocolRoutePaths.UserInfo);
                }

                if (Options.Endpoints.EnableEndSessionEndpoint)
                {
                    entries.Add(OidcConstants.Discovery.EndSessionEndpoint, baseUrl + Constants.ProtocolRoutePaths.EndSession);
                }

                if (Options.Endpoints.EnableCheckSessionEndpoint)
                {
                    entries.Add(OidcConstants.Discovery.CheckSessionIframe, baseUrl + Constants.ProtocolRoutePaths.CheckSession);
                }

                if (Options.Endpoints.EnableTokenRevocationEndpoint)
                {
                    entries.Add(OidcConstants.Discovery.RevocationEndpoint, baseUrl + Constants.ProtocolRoutePaths.Revocation);
                }

                if (Options.Endpoints.EnableIntrospectionEndpoint)
                {
                    entries.Add(OidcConstants.Discovery.IntrospectionEndpoint, baseUrl + Constants.ProtocolRoutePaths.Introspection);
                }

                if (Options.Endpoints.EnableDeviceAuthorizationEndpoint)
                {
                    entries.Add(OidcConstants.Discovery.DeviceAuthorizationEndpoint, baseUrl + Constants.ProtocolRoutePaths.DeviceAuthorization);
                }

                if (Options.MutualTls.Enabled)
                {
                    var mtlsEndpoints = new Dictionary<string, string>();

                    if (Options.Endpoints.EnableTokenEndpoint)
                    {
                        mtlsEndpoints.Add(OidcConstants.Discovery.TokenEndpoint, baseUrl + Constants.ProtocolRoutePaths.MtlsToken);
                    }
                    if (Options.Endpoints.EnableTokenRevocationEndpoint)
                    {
                        mtlsEndpoints.Add(OidcConstants.Discovery.RevocationEndpoint, baseUrl + Constants.ProtocolRoutePaths.MtlsRevocation);
                    }
                    if (Options.Endpoints.EnableIntrospectionEndpoint)
                    {
                        mtlsEndpoints.Add(OidcConstants.Discovery.IntrospectionEndpoint, baseUrl + Constants.ProtocolRoutePaths.MtlsIntrospection);
                    }
                    if (Options.Endpoints.EnableDeviceAuthorizationEndpoint)
                    {
                        mtlsEndpoints.Add(OidcConstants.Discovery.DeviceAuthorizationEndpoint, baseUrl + Constants.ProtocolRoutePaths.MtlsDeviceAuthorization);
                    }

                    if (mtlsEndpoints.Any())
                    {
                        entries.Add(OidcConstants.Discovery.MtlsEndpointAliases, mtlsEndpoints);
                    }
                }
            }

            // logout
            if (Options.Endpoints.EnableEndSessionEndpoint)
            {
                entries.Add(OidcConstants.Discovery.FrontChannelLogoutSupported, true);
                entries.Add(OidcConstants.Discovery.FrontChannelLogoutSessionSupported, true);
                entries.Add(OidcConstants.Discovery.BackChannelLogoutSupported, true);
                entries.Add(OidcConstants.Discovery.BackChannelLogoutSessionSupported, true);
            }

            // scopes and claims
            if (Options.Discovery.ShowIdentityScopes ||
                Options.Discovery.ShowApiScopes ||
                Options.Discovery.ShowClaims)
            {
                var resources = await ResourceStore.GetAllEnabledResourcesAsync();
                var scopes = new List<string>();

                // scopes
                if (Options.Discovery.ShowIdentityScopes)
                {
                    scopes.AddRange(resources.IdentityResources.Where(x => x.ShowInDiscoveryDocument).Select(x => x.Name));
                }

                if (Options.Discovery.ShowApiScopes)
                {
                    var apiScopes = from api in resources.ApiResources
                                    from scope in api.Scopes
                                    where scope.ShowInDiscoveryDocument
                                    select scope.Name;

                    scopes.AddRange(apiScopes);
                    scopes.Add(IdentityServerConstants.StandardScopes.OfflineAccess);
                }

                if (scopes.Any())
                {
                    entries.Add(OidcConstants.Discovery.ScopesSupported, scopes.ToArray());
                }

                // claims
                if (Options.Discovery.ShowClaims)
                {
                    var claims = new List<string>();

                    // add non-hidden identity scopes related claims
                    claims.AddRange(resources.IdentityResources.Where(x => x.ShowInDiscoveryDocument).SelectMany(x => x.UserClaims));

                    // add non-hidden api scopes related claims
                    foreach (var resource in resources.ApiResources)
                    {
                        claims.AddRange(resource.UserClaims);

                        foreach (var scope in resource.Scopes)
                        {
                            if (scope.ShowInDiscoveryDocument)
                            {
                                claims.AddRange(scope.UserClaims);
                            }
                        }
                    }

                    entries.Add(OidcConstants.Discovery.ClaimsSupported, claims.Distinct().ToArray());
                }
            }

            // grant types
            if (Options.Discovery.ShowGrantTypes)
            {
                var standardGrantTypes = new List<string>
                {
                    OidcConstants.GrantTypes.AuthorizationCode,
                    OidcConstants.GrantTypes.ClientCredentials,
                    OidcConstants.GrantTypes.RefreshToken,
                    OidcConstants.GrantTypes.Implicit
                };

                if (!(ResourceOwnerValidator is NotSupportedResourceOwnerPasswordValidator))
                {
                    standardGrantTypes.Add(OidcConstants.GrantTypes.Password);
                }

                if (Options.Endpoints.EnableDeviceAuthorizationEndpoint)
                {
                    standardGrantTypes.Add(OidcConstants.GrantTypes.DeviceCode);
                }

                var showGrantTypes = new List<string>(standardGrantTypes);

                if (Options.Discovery.ShowExtensionGrantTypes)
                {
                    showGrantTypes.AddRange(ExtensionGrants.GetAvailableGrantTypes());
                }

                entries.Add(OidcConstants.Discovery.GrantTypesSupported, showGrantTypes.ToArray());
            }

            // response types
            if (Options.Discovery.ShowResponseTypes)
            {
                entries.Add(OidcConstants.Discovery.ResponseTypesSupported, Constants.SupportedResponseTypes.ToArray());
            }

            // response modes
            if (Options.Discovery.ShowResponseModes)
            {
                entries.Add(OidcConstants.Discovery.ResponseModesSupported, Constants.SupportedResponseModes.ToArray());
            }

            // misc
            if (Options.Discovery.ShowTokenEndpointAuthenticationMethods)
            {
                var types = SecretParsers.GetAvailableAuthenticationMethods().ToList();
                if (Options.MutualTls.Enabled)
                {
                    types.Add(OidcConstants.EndpointAuthenticationMethods.TlsClientAuth);
                    types.Add(OidcConstants.EndpointAuthenticationMethods.SelfSignedTlsClientAuth);
                }

                entries.Add(OidcConstants.Discovery.TokenEndpointAuthenticationMethodsSupported, types);
            }
            
            var signingCredentials = await Keys.GetSigningCredentialsAsync();
            if (signingCredentials != null)
            {
                var algorithm = signingCredentials.Algorithm;
                entries.Add(OidcConstants.Discovery.IdTokenSigningAlgorithmsSupported, new[] { algorithm });
            }

            entries.Add(OidcConstants.Discovery.SubjectTypesSupported, new[] { "public" });
            entries.Add(OidcConstants.Discovery.CodeChallengeMethodsSupported, new[] { OidcConstants.CodeChallengeMethods.Plain, OidcConstants.CodeChallengeMethods.Sha256 });

            if (Options.Endpoints.EnableAuthorizeEndpoint)
            {
                entries.Add(OidcConstants.Discovery.RequestParameterSupported, true);

                if (Options.Endpoints.EnableJwtRequestUri)
                {
                    entries.Add(OidcConstants.Discovery.RequestUriParameterSupported, true);
                }
            }

            if (Options.MutualTls.Enabled)
            {
                entries.Add(OidcConstants.Discovery.TlsClientCertificateBoundAccessTokens, true);
            }

            // custom entries
            if (!Options.Discovery.CustomEntries.IsNullOrEmpty())
            {
                foreach (var customEntry in Options.Discovery.CustomEntries)
                {
                    if (entries.ContainsKey(customEntry.Key))
                    {
                        Logger.LogError("Discovery custom entry {key} cannot be added, because it already exists.", customEntry.Key);
                    }
                    else
                    {
                        if (customEntry.Value is string customValueString)
                        {
                            if (customValueString.StartsWith("~/") && Options.Discovery.ExpandRelativePathsInCustomEntries)
                            {
                                entries.Add(customEntry.Key, baseUrl + customValueString.Substring(2));
                                continue;
                            }
                        }

                        entries.Add(customEntry.Key, customEntry.Value);
                    }
                }
            }

            return entries;
        }

        /// <summary>
        /// Creates the JWK document.
        /// </summary>
        public virtual async Task<IEnumerable<Models.JsonWebKey>> CreateJwkDocumentAsync()
        {
            var webKeys = new List<Models.JsonWebKey>();
            
            foreach (var key in await Keys.GetValidationKeysAsync())
            {
                if (key.Key is X509SecurityKey x509Key)
                {
                    var cert64 = Convert.ToBase64String(x509Key.Certificate.RawData);
                    var thumbprint = Base64Url.Encode(x509Key.Certificate.GetCertHash());

                    if (x509Key.PublicKey is RSA rsa)
                    {
                        var parameters = rsa.ExportParameters(false);
                        var exponent = Base64Url.Encode(parameters.Exponent);
                        var modulus = Base64Url.Encode(parameters.Modulus);

                        var rsaJsonWebKey = new Models.JsonWebKey
                        {
                            kty = "RSA",
                            use = "sig",
                            kid = x509Key.KeyId,
                            x5t = thumbprint,
                            e = exponent,
                            n = modulus,
                            x5c = new[] { cert64 },
                            alg = key.SigningAlgorithm
                        };
                        webKeys.Add(rsaJsonWebKey);
                    }
                    else if (x509Key.PublicKey is ECDsa ecdsa)
                    {
                        var parameters = ecdsa.ExportParameters(false);
                        var x = Base64Url.Encode(parameters.Q.X);
                        var y = Base64Url.Encode(parameters.Q.Y);

                        var ecdsaJsonWebKey = new Models.JsonWebKey
                        {
                            kty = "EC",
                            use = "sig",
                            kid = x509Key.KeyId,
                            x5t = thumbprint,
                            x = x,
                            y = y,
                            crv = CryptoHelper.GetCrvValueFromCurve(parameters.Curve),
                            x5c = new[] { cert64 },
                            alg = key.SigningAlgorithm
                        };
                        webKeys.Add(ecdsaJsonWebKey);
                    }
                    else
                    {
                        throw new InvalidOperationException($"key type: {x509Key.PublicKey.GetType().Name} not supported.");
                    }
                }
                else if (key.Key is RsaSecurityKey rsaKey)
                {
                    var parameters = rsaKey.Rsa?.ExportParameters(false) ?? rsaKey.Parameters;
                    var exponent = Base64Url.Encode(parameters.Exponent);
                    var modulus = Base64Url.Encode(parameters.Modulus);

                    var webKey = new Models.JsonWebKey
                    {
                        kty = "RSA",
                        use = "sig",
                        kid = rsaKey.KeyId,
                        e = exponent,
                        n = modulus,
                        alg = key.SigningAlgorithm
                    };

                    webKeys.Add(webKey);
                }
                else if (key.Key is ECDsaSecurityKey ecdsaKey)
                {
                    var parameters = ecdsaKey.ECDsa.ExportParameters(false);
                    var x = Base64Url.Encode(parameters.Q.X);
                    var y = Base64Url.Encode(parameters.Q.Y);

                    var ecdsaJsonWebKey = new Models.JsonWebKey
                    {
                        kty = "EC",
                        use = "sig",
                        kid = ecdsaKey.KeyId,
                        x = x,
                        y = y,
                        crv = CryptoHelper.GetCrvValueFromCurve(parameters.Curve),
                        alg = key.SigningAlgorithm
                    };
                    webKeys.Add(ecdsaJsonWebKey);
                }
                else if (key.Key is JsonWebKey jsonWebKey)
                {
                    var webKey = new Models.JsonWebKey
                    {
                        kty = jsonWebKey.Kty,
                        use = jsonWebKey.Use ?? "sig",
                        kid = jsonWebKey.Kid,
                        x5t = jsonWebKey.X5t,
                        e = jsonWebKey.E,
                        n = jsonWebKey.N,
                        x5c = jsonWebKey.X5c?.Count == 0 ? null : jsonWebKey.X5c.ToArray(),
                        alg = jsonWebKey.Alg,
                        crv = jsonWebKey.Crv,
                        x = jsonWebKey.X,
                        y = jsonWebKey.Y
                    };

                    webKeys.Add(webKey);
                }
            }

            return webKeys;
        }
    }
}