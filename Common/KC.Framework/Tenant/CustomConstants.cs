//----------------------------------------------------------------------------------------------
//    Copyright 2012 Microsoft Corporation
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//      http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
//----------------------------------------------------------------------------------------------

namespace KC.Framework.Tenant
{
    public class CustomConstants
    {
        public const string Port = "13436";
        public const string IssuerName = "CFWinSTS";
        //public const string SigningCertificate = "LocalSTS.pfx";
        //public const string SigningCertificatePassword = "LocalSTS";
        //public const string SigningCertificateThumbprint = GlobalDataBase.CertThumbprint;
        //public const string EncryptingCertificate = "LocalSTS.pfx";
        //public const string EncryptingCertificatePassword = "LocalSTS";
        //public const string EncryptingCertificateThumbprint = "EncryptingCertificateThumbprint";

        public const string HttpLocalhost = "http://localhost:";
        public const string WSFedSts = "/WSFederation/";
        public const string WSFedStsIssue = WSFedSts + "Issue/";
        public const string FederationMetadataAddress = "Metadata";
        public const string FederationMetadataEndpoint = WSFedSts + FederationMetadataAddress;
        public const string WSTrustSTS = "/wsTrustSTS/";

        public const string TenantName = "MemberId";
        public const string TenantNickName = "TenantNickName";
        public const string CurrentUserDomain = "CurrentUserDomain";

        public const string TenantRoute = "tenant";
        public const string TenantDisplayName = "tenantDisplayName";

        public const string Area = "area";
        public const string ControllerRoute = "controller";
        public const string ActionRoute = "action";
    }

    public static class Cfwin
    {
        public static class ClaimTypes
        {
            public const string Tenant = "http://schemas.kcloudy.com/claims/2010/06/tenant";
        }

        public static class Federation
        {
            public const string HomeRealm = "http://cfwin/trust";
        }
    }
}