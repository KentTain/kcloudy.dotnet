using System;
using System.IdentityModel;
using System.IdentityModel.Configuration;
using System.IdentityModel.Metadata;
using System.IdentityModel.Protocols.WSTrust;
using System.IdentityModel.Tokens;
using System.IO;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Security.Permissions;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using Com.Framework.SecurityHelper;
using Com.Framework.Tenant;
using Com.MVC.Core.Base;
using Com.MVC.Core.Constants;
using Com.Service.Core.Base;

namespace Com.MVC.Core.SingleSignOn
{
    public class SecurityTokenServiceConfiguration<T> : SecurityTokenServiceConfiguration where T : SecurityTokenService
    {
        private static readonly object syncObj = new object();
        private static X509Certificate2 _certification = null;
        private static X509Certificate2 Certification
        {
            get
            {
                lock (syncObj)
                {
                    if (_certification == null)
                    {
                        _certification = CertificateUtil.GetCertificateByThumbprint(StoreName.My, StoreLocation.LocalMachine, GlobalDataBase.CertThumbprint);
                    }
                }

                return _certification;
            }
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        private SecurityTokenServiceConfiguration()
            //: base(ConfigUtil.GetConfigItem(CustomConstants.IssuerName))
        {
            this.TokenIssuerName = CustomConstants.IssuerName;

            //由于Cert是不变的，所以只需要将它存到静态变量里面
            this.SigningCredentials = new X509SigningCredentials(Certification);
            this.ServiceCertificate = Certification;

            this.SecurityTokenService = typeof(T);
            this.DefaultTokenType = "http://docs.oasis-open.org/wss/oasis-wss-saml-token-profile-1.1#SAMLV2.0";
        }

        private static readonly object syncRoot = new object();
        public static SecurityTokenServiceConfiguration<T> Current
        {
            get
            {
                var app = HttpContext.Current.Application;
                var keyName = typeof(T).Name + "_Configuration";

                var config = app.Get(keyName) as SecurityTokenServiceConfiguration<T>;
                if (config != null)
                    return config;
                lock (syncRoot)
                {
                    config = app.Get(keyName) as SecurityTokenServiceConfiguration<T>;
                    if (config == null)
                    {
                        config = new SecurityTokenServiceConfiguration<T>();
                        app.Add(keyName, config);
                    }

                    return config;
                }
            }
        }

        public XElement GetFederationMetadata()
        {
            // hostname
            EndpointReference passiveEndpoint = new EndpointReference(CustomConstants.HttpLocalhost + CustomConstants.Port + CustomConstants.WSFedStsIssue);
            EndpointReference activeEndpoint = new EndpointReference(CustomConstants.HttpLocalhost + CustomConstants.Port + CustomConstants.WSTrustSTS);

            // metadata document 
            EntityDescriptor entity = new EntityDescriptor(new EntityId(CustomConstants.IssuerName));
            SecurityTokenServiceDescriptor sts = new SecurityTokenServiceDescriptor();

            entity.RoleDescriptors.Add(sts);

            // signing key
            KeyDescriptor signingKey = new KeyDescriptor(this.SigningCredentials.SigningKeyIdentifier);
            signingKey.Use = KeyType.Signing;
            sts.Keys.Add(signingKey);

            // claim types
            sts.ClaimTypesOffered.Add(new DisplayClaim(ClaimTypes.Email, "Email Address", "User email address"));
            sts.ClaimTypesOffered.Add(new DisplayClaim(ClaimTypes.Surname, "Surname", "User last name"));
            sts.ClaimTypesOffered.Add(new DisplayClaim(ClaimTypes.Name, "Name", "User name"));
            sts.ClaimTypesOffered.Add(new DisplayClaim(ClaimTypes.Role, "Role", "RoleIds user are in"));
            sts.ClaimTypesOffered.Add(new DisplayClaim("http://schemas.xmlsoap.org/claims/Group", "Group", "Groups users are in"));

            // passive federation endpoint
            sts.PassiveRequestorEndpoints.Add(passiveEndpoint);

            // supported protocols

            //Inaccessable due to protection level
            //sts.ProtocolsSupported.Add(new Uri(WSFederationConstants.Namespace));
            sts.ProtocolsSupported.Add(new Uri("http://docs.oasis-open.org/wss/oasis-wss-saml-token-profile-1.1#SAMLV2.0"));
            sts.ProtocolsSupported.Add(new Uri("http://docs.oasis-open.org/wsfed/federation/200706"));

            // add passive STS endpoint
            sts.SecurityTokenServiceEndpoints.Add(activeEndpoint);

            // metadata signing
            entity.SigningCredentials = this.SigningCredentials;

            // serialize 
            var serializer = new MetadataSerializer();
            XElement federationMetadata = null;

            using (var stream = new MemoryStream())
            {
                serializer.WriteMetadata(stream, entity);
                stream.Flush();
                stream.Seek(0, SeekOrigin.Begin);

                XmlReaderSettings readerSettings = new XmlReaderSettings
                {
                    DtdProcessing = DtdProcessing.Prohibit, // prohibit DTD processing

                    XmlResolver = null, // disallow opening any external resources
                    // no need to do anything to limit the size of the input, given the input is crafted internally and it is of small size
                };

                XmlReader xmlReader = XmlTextReader.Create(stream, readerSettings);
                federationMetadata = XElement.Load(xmlReader);

            }

            return federationMetadata;
        }
    }

}