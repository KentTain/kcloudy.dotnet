using System;
using System.Collections.Generic;
using System.IdentityModel;
using System.IdentityModel.Configuration;
using System.IdentityModel.Protocols.WSTrust;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Security.Permissions;
using Com.Framework.SecurityHelper;
using Com.Framework.Util;
using Com.MVC.Core.Base;

namespace Com.MVC.Core.SingleSignOn
{
    public class FederationSecurityTokenService : SecurityTokenService
    {
        private readonly EncryptingCredentials encryptingCreds;

        public FederationSecurityTokenService(SecurityTokenServiceConfiguration config)
            : base(config)
        {
            //if (!string.IsNullOrWhiteSpace(GlobalDataBase.CertThumbprint))
            //{
            //    this.encryptingCreds = new X509EncryptingCredentials(
            //        CertificateUtil.GetCertificateByThumbprint(StoreName.My, StoreLocation.LocalMachine, GlobalDataBase.CertThumbprint));
            //}
        }

        /// <summary>
        /// 此方法返回用于令牌发布请求的配置。配置由Scope类表示。在这里，我们只发布令牌到一个由encryptingCreds字段表示的RP标识        /// </summary>
        /// <param name="principal"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        protected override Scope GetScope(ClaimsPrincipal principal, RequestSecurityToken request)
        {
            this.ValidateAppliesTo(request.AppliesTo);

            // 使用request的AppliesTo属性和RP标识来创建Scope
            var scope = new Scope(request.AppliesTo.Uri.OriginalString, SecurityTokenServiceConfiguration.SigningCredentials);
            if (Uri.IsWellFormedUriString(request.ReplyTo, UriKind.Absolute))
            {
                scope.ReplyToAddress =
                    request.AppliesTo.Uri.Host != new Uri(request.ReplyTo).Host
                        ? request.AppliesTo.Uri.AbsoluteUri
                        : request.ReplyTo;
            }
            else
            {
                Uri resultUri = null;
                scope.ReplyToAddress =
                    Uri.TryCreate(request.AppliesTo.Uri, request.ReplyTo, out resultUri)
                        ? resultUri.AbsoluteUri
                        : request.AppliesTo.Uri.ToString();
            }
            if (this.encryptingCreds != null)
            {
                // 如果STS对应多个RP，要选择证书指定到请求令牌的RP，然后再用 encryptingCreds 
                scope.EncryptingCredentials = this.encryptingCreds;
            }
            else
            {
                scope.TokenEncryptionRequired = false;
                scope.SymmetricKeyEncryptionRequired = false;
            }

            return scope;
        }

        /// <summary>
        /// 此方法返回要发布的令牌内容。内容由一组ClaimsIdentity实例来表示，每一个实例对应了一个要发布的令牌。当前Windows Identity Foundation只支持单个令牌发布，因此返回的集合必须总是只包含单个实例。
        /// </summary>
        /// <param name="principal">调用方的principal</param>
        /// <param name="request">进入的 RST,我们这里不用它</param>
        /// <param name="scope">由之前通过GetScope方法返回的范围</param>
        /// <returns></returns>
        protected override ClaimsIdentity GetOutputClaimsIdentity(ClaimsPrincipal principal, RequestSecurityToken request, Scope scope)
        {
            if (principal == null)
            {
                throw new InvalidRequestException("The caller's principal is null.");
            }

            var input = principal.Identity as ClaimsIdentity;
            if (input == null)
            {
                throw new InvalidRequestException("The caller's principal identity is null.");
            }

            var claim2Remove = input.Claims.FirstOrDefault(claim => claim.Type == "AspNet.Identity.SecurityStamp");
            if (claim2Remove != null)
                input.RemoveClaim(claim2Remove);

            //var output = new ClaimsIdentity();
            //CopyClaims(input, new[] { WSIdentityConstants.ClaimTypes.Name }, output);
            //TransformClaims(input, tenant.ClaimType, tenant.ClaimValue, ClaimTypes.Role, Tailspin.RoleIds.SurveyAdministrator, output);

            //返回一个默认声明集，里面了包含自己想要的声明
            //这里你可以通过ClaimsPrincipal来验证用户，并通过它来返回正确的声明。
            //string identityName = outputIdentity.Name;
            //string[] temp = identityName.Split('|');
            //var outgoingIdentity = new ClaimsIdentity();
            //outgoingIdentity.AddClaim(new Claim(ClaimTypes.Email, temp[0]));
            //outgoingIdentity.AddClaim(new Claim(ClaimTypes.DateOfBirth, temp[1]));
            //outgoingIdentity.AddClaim(new Claim(ClaimTypes.Name, temp[2]));
            //SingleSignOnManager.RegisterRP(scope.AppliesToAddress);
            return input;
        }

        private void ValidateAppliesTo(EndpointReference appliesTo)
        {
            LogUtil.LogDebug(string.Format("Com.MVC.Core.SingleSignOn.FederationSecurityTokenService: SSO Appliess to {0}", appliesTo.Uri));
            if (appliesTo == null)
            {
                throw new InvalidRequestException("The AppliesTo is null.");
            }
        }
    }

}