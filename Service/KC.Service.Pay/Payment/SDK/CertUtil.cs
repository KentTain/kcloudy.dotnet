using System;
using System.IO;
using System.Numerics;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using KC.Framework.Base;
using KC.Framework.Tenant;
using KC.Storage.Util;

namespace KC.Service.Pay.SDK
{
    public class CertUtil
    {
        /// <summary>
        /// 获取签名证书私钥
        /// </summary>
        /// <returns></returns>
        public static RSACryptoServiceProvider GetSignProviderFromPfx(Tenant tenant, string userId, ConfigStatus state)
        {
            // X509Certificate2 pc = new X509Certificate2(SDKConfig.SignCertPath, SDKConfig.SignCertPwd, X509KeyStorageFlags.MachineKeySet);
            X509Certificate2 pc = new X509Certificate2();
            if (state != 0)
            {
                var blob = BlobUtil.GetBlobById(tenant, userId, SDKConfig.SignCertName);
                pc = new X509Certificate2(blob.Data, SDKConfig.SignCertPwd);

            }
            else
            {
                pc = new X509Certificate2(SDKConfig.SignCertPath, SDKConfig.SignCertPwd);
            }
            Console.WriteLine(pc.PrivateKey);
            return (RSACryptoServiceProvider)pc.PrivateKey;

        }

        /// <summary>
        /// 获取签名证书的证书序列号
        /// </summary>
        /// <returns></returns>
        public static string GetSignCertId(Tenant tenant, string userId, ConfigStatus state)
        {
            X509Certificate2 pc = new X509Certificate2();
            if (state != 0)
            {
                var blob = BlobUtil.GetBlobById(tenant, userId, SDKConfig.SignCertName);
                pc = new X509Certificate2(blob.Data, SDKConfig.SignCertPwd);

            }
            else
            {
                pc = new X509Certificate2(SDKConfig.SignCertPath, SDKConfig.SignCertPwd);
            }
            //return BigNum.ToDecimalStr(BigNum.ConvertFromHex(pc.SerialNumber)); //低于4.0版本的.NET请使用此方法
            return BigInteger.Parse(pc.SerialNumber, System.Globalization.NumberStyles.HexNumber).ToString();
        }

        /// <summary>
        /// 通过证书id，获取验证签名的证书
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="certId"></param>
        /// <returns></returns>
        public static RSACryptoServiceProvider GetValidateProviderFromPath(Tenant tenant, string userId, string encryptCertName, string certId, ConfigStatus state)// 
        {
            //var pc = new X509Certificate2(SDKConfig.EncryptCert);
            //string id = BigNum.ToDecimalStr(BigNum.ConvertFromHex(pc.SerialNumber)); //低于4.0版本的.NET请使用此方法
            //string id = BigInteger.Parse(pc.SerialNumber, System.Globalization.NumberStyles.HexNumber).ToString();
            //if (certId.Equals(id))
            //{
            //    return (RSACryptoServiceProvider)pc.PublicKey.Key;

            //}
            if (state!=0)
            {
                var blob = BlobUtil.GetBlobById(tenant, userId, encryptCertName);
                X509Certificate2 pc = new X509Certificate2(blob.Data, SDKConfig.SignCertPwd);
                string id = BigInteger.Parse(pc.SerialNumber, System.Globalization.NumberStyles.HexNumber).ToString();
                if (certId.Equals(id))
                {
                    return (RSACryptoServiceProvider)pc.PublicKey.Key;

                }
            }
            else
            {
                DirectoryInfo directory = new DirectoryInfo(SDKConfig.ValidateCertDir);
                FileInfo[] files = directory.GetFiles("*.cer");
                if (0 == files.Length)
                {
                    return null;
                }
                foreach (FileInfo file in files)
                {
                    var pc = new X509Certificate2(file.DirectoryName + "\\" + file.Name);
                    //string id = BigNum.ToDecimalStr(BigNum.ConvertFromHex(pc.SerialNumber)); //低于4.0版本的.NET请使用此方法
                    string id = BigInteger.Parse(pc.SerialNumber, System.Globalization.NumberStyles.HexNumber).ToString();
                    if (certId.Equals(id))
                    {
                        return (RSACryptoServiceProvider)pc.PublicKey.Key;

                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 加载密码加密证书
        /// </summary>
        public static RSACryptoServiceProvider InitEncryptCert()
        {

            var pc = new X509Certificate2(SDKConfig.EncryptCert);
            byte[] by = pc.GetPublicKey();


            var pl = new RSACryptoServiceProvider();
            return (RSACryptoServiceProvider)pc.PublicKey.Key;
        }

    }
}
