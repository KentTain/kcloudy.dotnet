using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace KC.Framework.SecurityHelper
{
    public class X509CertificateProvider
    {
        private string _thumbprint = string.Empty;
        public string ThumbPrint
        {
            get
            {
                return _thumbprint;
            }
            set
            {
                _thumbprint = value;
            }
        }

        public X509CertificateProvider(string thumbprint)
        {
            _thumbprint = thumbprint;
        }

        public string Encrypt(string encryptString)
        {
            if (string.IsNullOrEmpty(encryptString))
                throw new ArgumentNullException("encryptString", "The encryptString is null or empty.");
            if(string.IsNullOrEmpty(ThumbPrint))
                throw new ArgumentException("ThumbPrint is null.");

            var contentInfo = new ContentInfo(Encoding.UTF8.GetBytes(encryptString));

            var cert = CertificateUtil.GetCertificateByThumbprint(StoreName.My, StoreLocation.CurrentUser, ThumbPrint);
            var env = new EnvelopedCms(contentInfo);
            env.Encrypt(new CmsRecipient(cert));

            return Convert.ToBase64String(env.Encode());
        }

        public string Decrypt(string decryptString)
        {
            var envelop = new EnvelopedCms();
            envelop.Decode(Convert.FromBase64String(decryptString));
            envelop.Decrypt();

            byte[] messageInBytes = envelop.ContentInfo.Content;

            return Encoding.UTF8.GetString(messageInBytes);
        }
    }
}
