
using KC.Framework.Util;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace KC.Service.Pay.SDK
{
    public class CPCNPayment
    {
        /// <summary>
        /// 中金支付获取签名的方法
        /// </summary>
        /// <param name="filePath">私钥文件地址</param>
        /// <param name="password">私钥文件口令</param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static string GetSignature(string filePath, string password, string message)
        {
            var signeCert = new X509Certificate2(filePath, password, X509KeyStorageFlags.MachineKeySet);
            RSACryptoServiceProvider rsa = (RSACryptoServiceProvider)signeCert.PrivateKey;
            byte[] data = Encoding.UTF8.GetBytes(message);
            byte[] signData = rsa.SignData(data, "SHA1");
            return StringToHexString(signData);
        }

        /// <summary>
        /// 根据公钥加密
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static string GetPubSignature(string filePath, string message)
        {
            //var signeCert = new X509Certificate2(filePath);
            //RSACryptoServiceProvider pubkey = (RSACryptoServiceProvider)signeCert.PublicKey.Key;
            //byte[] data = Encoding.UTF8.GetBytes(message);
            //byte[] signByte = Encoding.UTF8.GetBytes(signature);
            //var isSig = pubkey.VerifyData(signByte, "SHA1", data);
            ////byte[] signData = rsa.SignData(data, "SHA1");
            //return isSig;

            X509Certificate2 x509certificate2 = new X509Certificate2(filePath);
            byte[] rsaPublicKey = x509certificate2.PublicKey.EncodedKeyValue.RawData;
            return StringToHexString(rsaPublicKey);
        }

        // 把字节型转换成十六进制字符串  
        private static string StringToHexString(byte[] bytes)
        {
            string returnStr = "";
            if (bytes != null)
            {
                for (int i = 0; i < bytes.Length; i++)
                {
                    returnStr += bytes[i].ToString("X2");
                }
            }
            return returnStr;
        }


        /// <summary>
        /// Model To XML
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public static string ModelToXml<T>(T model, string version)
        {
            XmlSerialize xmlSerialize = new XmlSerialize();
            string xmlStr = xmlSerialize.ModelToXml<T>(model);
            return ModifyXmlNode(xmlStr, version);
        }

        /// <summary>
        /// XML TO Model
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static T XmlToModel<T>(string xml)
        {
            XmlSerialize xmlSerialize = new XmlSerialize();
            return xmlSerialize.XmlToModel<T>(xml);
        }

        /// <summary>
        /// 修改XML属性值
        /// </summary>
        /// <param name="strXml"></param>
        /// <param name="version">版本</param>
        /// <returns></returns>
        public static string ModifyXmlNode(string strXml, string version)
        {
            if (!string.IsNullOrEmpty(strXml))
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(strXml);
                var direction = (XmlDeclaration)xmlDoc.FirstChild;
                direction.Encoding = "GBK";
                XmlElement element = (XmlElement)xmlDoc.SelectSingleNode("MSG");
                element.RemoveAttribute("xmlns:xsi");
                element.RemoveAttribute("xmlns:xsd");
                element.SetAttribute("version", version);
                return xmlDoc.InnerXml;
            }
            return "";
        }


    }
}
