using KC.Framework.Util;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace KC.Service.Util
{
    /// <summary>
    /// 用于和Java语言进行加密通信的帮助类
    /// </summary>
    public class DESSercurityUtil
    {
        #region DESEnCode DES加密
        public static string DESEnCode(string pToEncrypt, string sKey)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            byte[] inputByteArray = Encoding.GetEncoding("UTF-8").GetBytes(pToEncrypt);

            des.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
            des.IV = ASCIIEncoding.ASCII.GetBytes(sKey);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);

            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();

            StringBuilder ret = new StringBuilder();
            foreach (byte b in ms.ToArray())
            {
                ret.AppendFormat("{0:X2}", b);
            }

            return ret.ToString();
        }
        #endregion

        #region DESDeCode DES解密
        public static string DESDeCode(string pToDecrypt, string sKey)
        {
            //    HttpContext.Current.Response.Write(pToDecrypt + "<br>" + sKey);   
            //    HttpContext.Current.Response.End();   
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();

            byte[] inputByteArray = new byte[pToDecrypt.Length / 2];
            for (int x = 0; x < pToDecrypt.Length / 2; x++)
            {
                int i = (Convert.ToInt32(pToDecrypt.Substring(x * 2, 2), 16));
                inputByteArray[x] = (byte)i;
            }

            des.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
            des.IV = ASCIIEncoding.ASCII.GetBytes(sKey);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();

            StringBuilder ret = new StringBuilder();

            return System.Text.Encoding.UTF8.GetString(ms.ToArray());
        }
        #endregion

        #region RSA

        /// <summary>
        /// RSA的加密函数  string
        /// </summary>
        /// <param name="publicKey">公钥XML</param>
        /// <param name="plaintext">明文</param>
        /// <returns></returns>
        public static string Encrypt(string publicKey, string plaintext)
        {
            return Encrypt(publicKey, Encoding.UTF8.GetBytes(plaintext));
        }

        /// <summary>
        /// RSA的加密函数  string
        /// </summary>
        /// <param name="publicKey">公钥XML</param>
        /// <param name="plainbytes">明文字节数组</param>
        /// <returns></returns>
        public static string Encrypt(string publicKey, byte[] plainbytes)
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                try
                {
                    rsa.FromXmlString(publicKey);
                    var bufferSize = (rsa.KeySize / 8 - 11);
                    byte[] buffer = new byte[bufferSize];//待加密块

                    using (MemoryStream msInput = new MemoryStream(plainbytes))
                    {
                        using (MemoryStream msOutput = new MemoryStream())
                        {
                            while (true)
                            {
                                int readLine = msInput.Read(buffer, 0, bufferSize);
                                if (readLine <= 0)
                                {
                                    break;
                                }
                                byte[] temp = new byte[readLine];
                                Array.Copy(buffer, 0, temp, 0, readLine);
                                byte[] encrypt = rsa.Encrypt(temp, false);
                                msOutput.Write(encrypt, 0, encrypt.Length);
                            }
                            return Convert.ToBase64String(msOutput.ToArray());
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogUtil.LogException(ex);
                    return string.Empty;
                }
            }
        }

        /// <summary>
        /// RSA的解密函数  stirng
        /// </summary>
        /// <param name="privateKey">私钥XML</param>
        /// <param name="ciphertext">密文字符串</param>
        /// <returns></returns>
        public static string Decrypt(string privateKey, string ciphertext)
        {
            return Decrypt(privateKey, Convert.FromBase64String(ciphertext.Contains("_") ? JavaUrlBase64ToNet(ciphertext) : ciphertext));
        }

        /// <summary>
        /// RSA的解密函数  byte
        /// </summary>
        /// <param name="privateKey">私钥XML</param>
        /// <param name="cipherbytes">密文字节数组</param>
        /// <returns></returns>
        public static string Decrypt(string privateKey, byte[] cipherbytes)
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                try
                {
                    rsa.FromXmlString(privateKey);
                    int keySize = rsa.KeySize / 8;
                    byte[] buffer = new byte[keySize];
                    using (MemoryStream msInput = new MemoryStream(cipherbytes))
                    {
                        using (MemoryStream msOutput = new MemoryStream())
                        {

                            while (true)
                            {
                                int readLine = msInput.Read(buffer, 0, keySize);
                                if (readLine <= 0)
                                {
                                    break;
                                }
                                byte[] temp = new byte[readLine];
                                Array.Copy(buffer, 0, temp, 0, readLine);
                                byte[] decrypt = rsa.Decrypt(temp, false);
                                msOutput.Write(decrypt, 0, decrypt.Length);
                            }
                            return Encoding.UTF8.GetString(msOutput.ToArray());
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogUtil.LogException(ex);
                    return string.Empty;
                }
            }
        }

        #endregion


        /// <summary>
        /// PKCS7验签,
        /// </summary>
        /// <param name="strSource">签名前字符串</param>
        /// <param name="strSignedData">签名后字符串(带公钥)</param>
        /// <param name="detached">是否带原文</param>
        /// <param name="pubKeyPEM">证书PEM</param>
        /// <param name="pubKeyFile">公钥证书文件</param>
        /// <returns></returns>
        public static bool VerifyPKCS7Signed(string strSource, string strSignedData, bool detached, string pubKeyPEM = null, string pubKeyFile = null)
        {
            try
            {
                byte[] signedDataByte = Convert.FromBase64String(!strSignedData.Contains("_") ? strSignedData : JavaUrlBase64ToNet(strSignedData));
                var byteConverter = Encoding.UTF8;
                byte[] dataToVerify = byteConverter.GetBytes(strSource);
                ContentInfo signedData = new ContentInfo(dataToVerify);
                SignedCms cms = new SignedCms(signedData, !detached);
                cms.Decode(signedDataByte);
                cms.CheckSignature(true);
                if (cms.Certificates.Count > 0)
                {
                    X509Certificate2 cert = cms.Certificates[cms.Certificates.Count - 1];
                    if (string.IsNullOrWhiteSpace(pubKeyPEM) && !string.IsNullOrWhiteSpace(pubKeyFile))
                    {
                        X509Certificate2 cerDecrypt = new X509Certificate2(pubKeyFile);
                        return cert.Thumbprint == cerDecrypt.Thumbprint;
                    }
                    else if (!string.IsNullOrWhiteSpace(pubKeyPEM))
                    {
                        var cerDecrypt = LoadCertificateFile(pubKeyPEM);
                        return cerDecrypt != null ? cert.Thumbprint == cerDecrypt.Thumbprint : false;
                    }
                    else
                    {
                        var pubKey = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Cert", "cfwin.crt");
                        X509Certificate2 cerDecrypt = new X509Certificate2(pubKey);
                        return cert.Thumbprint == cerDecrypt.Thumbprint;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                LogUtil.LogException(ex);
                return false;
            }
        }

        /// <summary>
        /// 验证签名
        /// </summary>
        /// <param name="unsign"></param>
        /// <param name="sign"></param>
        /// <param name="cert"></param>
        /// <returns></returns>
        public static bool VerifyPKCS1(string unsign, string sign, CertDataFromSignedData cert)
        {
            var sha1Result = VerifyPKCS1WithSHA1(unsign, sign, cert);
            if (sha1Result)
                return true;
            return VerifyPKCS1WithSHA256(unsign, sign, cert);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="unsign"></param>
        /// <param name="sign"></param>
        /// <param name="cert"></param>
        /// <returns></returns>
        public static bool VerifyPKCS1WithSHA1(string unsign, string sign, CertDataFromSignedData cert)
        {
            //实例化验签对象.
            var f2 = new RSAPKCS1SignatureDeformatter(cert.RSAProvider);

            //设置哈昔算法为sha1
            f2.SetHashAlgorithm("SHA1");
            SHA1Managed sha1 = new SHA1Managed();
            //计算源串的哈昔值,生成字节数组.
            var name = sha1.ComputeHash(System.Text.UnicodeEncoding.Default.GetBytes(unsign));

            //把签名串转化为字节数组
            byte[] key = Convert.FromBase64String(!sign.Contains("_") ? sign : JavaUrlBase64ToNet(sign));
            return f2.VerifySignature(name, key);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="unsign"></param>
        /// <param name="sign"></param>
        /// <param name="cert"></param>
        /// <returns></returns>
        public static bool VerifyPKCS1WithSHA256(string unsign, string sign, CertDataFromSignedData cert)
        {
            //实例化验签对象.
            var f2 = new RSAPKCS1SignatureDeformatter(cert.RSAProvider);

            //设置哈昔算法为sha1
            f2.SetHashAlgorithm("SHA256");
            SHA256Managed sha256 = new SHA256Managed();
            //计算源串的哈昔值,生成字节数组.
            var name = sha256.ComputeHash(System.Text.UnicodeEncoding.Default.GetBytes(unsign));
            //把签名串转化为字节数组
            byte[] key = Convert.FromBase64String(!sign.Contains("_") ? sign : JavaUrlBase64ToNet(sign));
            return f2.VerifySignature(name, key);
        }


        /// <summary>
        /// 从签名信息中获取证书信息
        /// </summary>
        /// <param name="strSignedData"></param>
        /// <returns></returns>
        public static CertDataFromSignedData GetCertFromSignedData(string strSignedData)
        {
            try
            {
                byte[] signedDataByte = Convert.FromBase64String(strSignedData.Contains("_") ? JavaUrlBase64ToNet(strSignedData) : strSignedData);
                SignedCms cms = new SignedCms();
                cms.Decode(signedDataByte);
                if (cms.Certificates.Count > 0)
                {
                    X509Certificate2 cert = cms.Certificates[cms.Certificates.Count - 1];
                    var f = cert.GetEffectiveDateString();
                    byte[] encoded = cert.GetRawCertData();
                    RSACryptoServiceProvider rsa = (RSACryptoServiceProvider)cert.PublicKey.Key;
                    var certData = new CertDataFromSignedData
                    {
                        PublicKey = cert.GetPublicKey(),
                        PublicKeyString = cert.GetPublicKeyString(),
                        RawCertData = cert.GetRawCertData(),
                        RawCertDataString = cert.GetRawCertDataString(),
                        PublicKeyXml = rsa.ToXmlString(false),
                        RSAProvider = rsa,
                        EffectiveDateString = cert.GetEffectiveDateString(),
                        ExpirationDateString = cert.GetExpirationDateString(),
                        Issuer = cert.GetNameInfo(X509NameType.SimpleName, true),
                        Owner = cert.GetNameInfo(X509NameType.SimpleName, false),
                        Thumbprint = cert.Thumbprint,
                        SignatureAlgorithm = cert.SignatureAlgorithm.FriendlyName
                    };
                    return certData;
                }
            }
            catch (Exception ex)
            {
                LogUtil.LogException(ex);
            }

            return null;
        }

        public static CertDataFromSignedData GetCertFromCertFile(string certFilePath)
        {
            try
            {
                X509Certificate2 cert = new X509Certificate2(certFilePath);
                var f = cert.GetEffectiveDateString();
                byte[] encoded = cert.GetRawCertData();
                RSACryptoServiceProvider rsa = (RSACryptoServiceProvider)cert.PublicKey.Key;
                var certData = new CertDataFromSignedData
                {
                    PublicKey = cert.GetPublicKey(),
                    PublicKeyString = cert.GetPublicKeyString(),
                    RawCertData = cert.GetRawCertData(),
                    RawCertDataString = cert.GetRawCertDataString(),
                    PublicKeyXml = rsa.ToXmlString(false),
                    RSAProvider = rsa,
                    EffectiveDateString = cert.GetEffectiveDateString(),
                    ExpirationDateString = cert.GetExpirationDateString(),
                    Issuer = cert.GetNameInfo(X509NameType.SimpleName, true),
                    Owner = cert.GetNameInfo(X509NameType.SimpleName, false)
                };
                return certData;
            }
            catch (Exception ex)
            {
                LogUtil.LogException(ex);
                return null;
            }
        }

        /// <summary>
        /// 通过证书给字符串签名,返回base64
        /// </summary>
        /// <param name="originalTextToSign"></param>
        /// <param name="pwd"></param>
        /// <param name="pfxPath"></param>
        /// <returns></returns>
        public static string CreateSignature(string originalTextToSign, string pwd, string pfxPath = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(pfxPath))
                    pfxPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Cert", "starlu.com.pfx");
                X509Certificate2 certEncrypt = new X509Certificate2(pfxPath, pwd, X509KeyStorageFlags.Exportable);
                var contentInfo = new ContentInfo(Encoding.UTF8.GetBytes(originalTextToSign));
                SignedCms cms = new SignedCms(contentInfo, true);
                var cmsSigner = new CmsSigner(certEncrypt);
                cms.ComputeSignature(cmsSigner);
                var signData = cms.Encode();

                return Convert.ToBase64String(signData);
            }
            catch (Exception ex)
            {
                LogUtil.LogException(ex);
                return string.Empty;
            }
        }

        /// <summary>
        /// ToBase64String之后进行URl编码
        /// </summary>
        /// <param name="unsign"></param>
        /// <param name="pwd"></param>
        /// <param name="pfxPath"></param>
        /// <param name="isSHA1"></param>
        /// <returns></returns>
        public static string CreatePKCS1Signature(string unsign, string pwd, string pfxPath = null, bool isSHA1 = true)
        {
            X509Certificate2 certEncrypt = GetX509Certificate2(pwd, pfxPath);
            RSACryptoServiceProvider rsa = (RSACryptoServiceProvider)certEncrypt.PrivateKey;
            // 加密对象 
            RSAPKCS1SignatureFormatter f = new RSAPKCS1SignatureFormatter(rsa);
            //把要签名的源串转化成字节数组
            byte[] source = System.Text.UnicodeEncoding.Default.GetBytes(unsign);
            byte[] result = null;
            if (isSHA1)
            {
                f.SetHashAlgorithm("SHA1");
                SHA1Managed sha = new SHA1Managed();
                //对签名源串做哈昔算法,为sha1.
                result = sha.ComputeHash(source);
            }
            else
            {
                f.SetHashAlgorithm("SHA256");
                SHA256Managed sha = new SHA256Managed();
                //对签名源串做哈昔算法,为sha1.
                result = sha.ComputeHash(source);
            }
            //对哈昔算法后的字符串进行签名.
            byte[] b = f.CreateSignature(result);
            //生成签名对象sign
            return Convert.ToBase64String(b);
        }

        /// <summary>
        /// 得到证书
        /// </summary>
        /// <param name="pwd"></param>
        /// <param name="pfxPath"></param>
        /// <returns></returns>
        public static X509Certificate2 GetX509Certificate2(string pwd, string pfxPath = null)
        {
            if (string.IsNullOrWhiteSpace(pfxPath))
                pfxPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Cert", "starlu.com.pfx");
            return new X509Certificate2(pfxPath, pwd, X509KeyStorageFlags.Exportable);
        }

        public static string ToJavaUrlBase64(string str)
        {
            return str.Replace('+', '-').Replace('/', '_');
        }

        public static string JavaUrlBase64ToNet(string str)
        {
            var source = str.Replace('_', '/').Replace('-', '+');
            switch (str.Length % 4)
            {
                case 2: source += "=="; break;
                case 3: source += "="; break;
            }
            return source;
        }

        public static string PostForFormData(string url, NameValueCollection input, Encoding endoding)
        {
            string boundary = "----------------------------" + DateTime.Now.Ticks.ToString("x");
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.ContentType = "multipart/form-data; boundary=" + boundary;
            request.Method = "POST";
            request.KeepAlive = true;
            request.Expect = "";
            MemoryStream stream = new MemoryStream();
            byte[] line = Encoding.ASCII.GetBytes("--" + boundary + "\r\n");
            byte[] enterER = Encoding.ASCII.GetBytes("\r\n");
            //提交文本字段
            if (input != null)
            {
                string format = "--" + boundary + "\r\nContent-Disposition:form-data;name=\"{0}\"\r\n\r\n{1}\r\n";
                foreach (string key in input.Keys)
                {
                    string s = string.Format(format, key, input[key]);
                    byte[] data = Encoding.UTF8.GetBytes(s);
                    stream.Write(data, 0, data.Length);
                }
            }

            byte[] foot_data = Encoding.UTF8.GetBytes("--" + boundary + "--\r\n");      //项目最后的分隔符字符串需要带上--  
            stream.Write(foot_data, 0, foot_data.Length);
            request.ContentLength = stream.Length;
            Stream requestStream = request.GetRequestStream(); //写入请求数据
            stream.Position = 0L;
            stream.CopyTo(requestStream);
            stream.Close();
            requestStream.Close();
            try
            {
                HttpWebResponse response;
                try
                {
                    response = (HttpWebResponse)request.GetResponse();
                    try
                    {
                        using (var responseStream = response.GetResponseStream())
                        using (var mstream = new MemoryStream())
                        {
                            responseStream.CopyTo(mstream);
                            string message = endoding.GetString(mstream.ToArray());
                            return message;
                        }
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
                catch (WebException)
                {
                    throw;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        private static byte[] PEM(string type, string pem)
        {
            string header = String.Format("-----BEGIN {0}-----", type);
            string footer = String.Format("-----END {0}-----", type);
            pem = !pem.Contains("_") ? pem : JavaUrlBase64ToNet(pem);
            string base64 = pem.Replace(header, "").Replace(footer, "");
            return Convert.FromBase64String(base64);
        }

        private static X509Certificate2 LoadCertificateFile(string cerPEM)
        {
            X509Certificate2 x509 = null;
            try
            {
                var data = Encoding.UTF8.GetBytes(cerPEM);

                if (data[0] != 0x30)
                {
                    data = PEM("CERTIFICATE", cerPEM);
                }
                if (data != null)
                    x509 = new X509Certificate2(data);
            }
            catch (Exception ex) {
                LogUtil.LogException(ex);
            }
            return x509;
        }
    }

    [Serializable, DataContract(IsReference = true)]
    public class CertDataFromSignedData
    {
        /// <summary> 摘要:
        ///     返回 X.509v3 证书的公钥。
        ///
        /// 返回结果:
        ///     字节数组形式的 X.509 证书的公钥。
        ///</summary>
        [DataMember]
        public byte[] PublicKey { get; set; }

        /// <summary>
        /// 摘要:
        ///     返回整个 X.509v3 证书的原始数据。
        ///
        /// 返回结果:
        ///     一个包含 X.509 证书数据的字节数组。
        /// </summary>
        [DataMember]
        public byte[] RawCertData { get; set; }

        /// <summary>
        /// 摘要:
        ///     返回整个 X.509v3 证书的原始数据。
        ///
        /// 返回结果:
        ///     十六进制字符串形式的 X.509 证书数据。
        /// </summary>
        [DataMember]
        public string RawCertDataString { get; set; }

        /// <summary>
        ///  摘要:
        ///     返回 X.509v3 证书的公钥。
        ///
        /// 返回结果:
        ///     十六进制字符串形式的 X.509 证书的公钥。
        /// </summary>
        [DataMember]
        public string PublicKeyString { get; set; }

        [DataMember]
        public string PublicKeyXml { get; set; }

        [DataMember]
        public string RawCertDataPEM
        {
            get
            {
                string certOutString = Convert.ToBase64String(RawCertData);
                return "-----BEGIN CERTIFICATE-----" + certOutString + "-----END CERTIFICATE-----";
            }
        }

        public RSACryptoServiceProvider RSAProvider { get; set; }

        /// <summary>
        /// 摘要:
        ///     返回此 X.509v3 证书的生效日期。
        ///
        /// 返回结果:
        ///     该 X.509 证书的生效日期。
        /// </summary>
        [DataMember]
        public string EffectiveDateString { get; set; }

        /// <summary>
        /// 返回此 X.509v3 证书的到期日期。
        /// </summary>
        [DataMember]
        public string ExpirationDateString { get; set; }

        /// <summary>
        /// 有效期
        /// </summary>
        public string TermOfValidity
        {
            get
            {
                return EffectiveDateString + " - " + ExpirationDateString;
            }
        }
        /// <summary>
        /// 颁发者
        /// </summary>
        [DataMember]
        public string Issuer { get; set; }

        /// <summary>
        /// 拥有者
        /// </summary>
        [DataMember]
        public string Owner { get; set; }

        /// <summary>
        /// 指纹
        /// </summary>
        [DataMember]
        public string Thumbprint { get; set; }

        [DataMember]
        public string SignatureAlgorithm { get; set; }
    }

}
