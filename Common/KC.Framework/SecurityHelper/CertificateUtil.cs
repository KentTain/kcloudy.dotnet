using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace KC.Framework.SecurityHelper
{
    public static class CertificateUtil
    {
        public static X509Certificate2 GetCertificateBySubjectName(StoreName name, StoreLocation location, string subjectName)
        {
            X509Certificate2 result = null;
            X509Certificate2Collection certificates = null;
            var store = new X509Store(name, location);

            try
            {
                store.Open(OpenFlags.ReadOnly);

                certificates = store.Certificates;
                foreach (var cert in certificates)
                {
                    if (string.Equals(cert.SubjectName.Name, subjectName, StringComparison.OrdinalIgnoreCase))
                    {
                        if (result != null)
                            throw new ApplicationException(string.Format("subject Name {0}存在多个证书", subjectName));
                        result = new X509Certificate2(cert);
                    }
                }

                if (result == null)
                {
                    throw new ApplicationException(string.Format("没有找到用于 subject Name {0} 的证书", subjectName));
                }

                return result;
            }
            finally
            {
                if (certificates != null)
                {
                    foreach (X509Certificate2 t in certificates)
                    {
                        t.Reset();
                    }
                }
                store.Close();
            }
        }

        public static X509Certificate2 GetCertificateByThumbprint(StoreName name, StoreLocation location, string subjectName)
        {
            X509Certificate2 result = null;
            X509Certificate2Collection certificates = null;
            var store = new X509Store(name, location);

            try
            {
                store.Open(OpenFlags.ReadOnly);

                certificates = store.Certificates;
                foreach (var cert in certificates)
                {
                    if (string.Equals(cert.Thumbprint, subjectName, StringComparison.OrdinalIgnoreCase))
                    {
                        if (result != null)
                            throw new ApplicationException(string.Format("subject Name {0}存在多个证书", subjectName));
                        result = new X509Certificate2(cert);
                    }
                }

                if (result == null)
                {
                    throw new ApplicationException(string.Format("没有找到用于 subject Name {0} 的证书", subjectName));
                }

                return result;
            }
            finally
            {
                if (certificates != null)
                {
                    foreach (X509Certificate2 t in certificates)
                    {
                        t.Reset();
                    }
                }
                store.Close();
            }
        }

        //用私钥生成签名
        public static string Sign(string unsign, string certPath, string certPwd, ref string errMsg)
        {
            try
            {
                var signeCert = new X509Certificate2(certPath, certPwd);
                RSACryptoServiceProvider rsa = (RSACryptoServiceProvider)signeCert.PrivateKey;
                byte[] data = Encoding.UTF8.GetBytes(unsign);
                byte[] signData = rsa.SignData(data, "SHA1");
                return Convert.ToBase64String(signData);
            }
            catch (Exception e)
            {
                errMsg = e.Message;
            }
            return string.Empty;
        }

        public static bool Verify(string unsign, string signed, string certPath, ref string errMsg)
        {
            byte[] bSource = Encoding.GetEncoding("gb2312").GetBytes(unsign);
            byte[] bSigdat;
            try
            {
                bSigdat = Convert.FromBase64String(signed);
            }
            catch (Exception ex)
            {
                errMsg = "签名格式不正确：" + ex.Message;
                return false;
            }

            X509Certificate2 x509;
            try
            {
                x509 = new X509Certificate2(certPath);
            }
            catch (Exception ex)
            {
                errMsg = "证书异常：" + ex.Message;
                return false;
            }

            try
            {
                RSACryptoServiceProvider rsa = (RSACryptoServiceProvider)x509.PublicKey.Key;
                if (!rsa.VerifyData(bSource, new SHA1CryptoServiceProvider(), bSigdat))
                {
                    errMsg = "验证签名失败";
                    return false;
                }
            }
            catch (Exception ex)
            {
                errMsg = "验证签名异常：" + ex.Message;
                return false;
            }
            return true;
        }

        public static bool VerifyPublicKey(string unsign, string signed, string publicKey, ref string errMsg)
        {
            byte[] bSource = Encoding.GetEncoding("gb2312").GetBytes(unsign);
            byte[] bSigdat;
            try
            {
                bSigdat = Convert.FromBase64String(signed);
            }
            catch (Exception ex)
            {
                errMsg = "签名格式不正确：" + ex.Message;
                return false;
            }
            RSACryptoServiceProvider oRSA4 = new RSACryptoServiceProvider();
            oRSA4.FromXmlString(publicKey);
            if (!oRSA4.VerifyData(bSource, "SHA1", bSigdat))
            {
                errMsg = "验证签名失败";
                return false;
            }
            return true;
        }
    }
}