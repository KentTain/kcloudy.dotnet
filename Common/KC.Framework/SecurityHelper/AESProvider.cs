using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace KC.Framework.SecurityHelper
{
    /// <summary>
    /// 为了和java统一，key必须是16个字节
    /// </summary>
    public class AESProvider
    {
        public static byte[] Encrypt(byte[] bytes, string keyStr, string ivStr)
        {
            var md5 = new MD5CryptoServiceProvider();
            var sha256 = new SHA256CryptoServiceProvider();
            byte[] ivBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(ivStr));
            var keySize = 16;
            byte[] keyBytes = new byte[keySize];
            var shaKey = sha256.ComputeHash(Encoding.UTF8.GetBytes(keyStr));
            Array.Copy(shaKey, 0, keyBytes, 0, keySize);
            return Encrypt(bytes, keyBytes, ivBytes);
        }

        static byte[] Encrypt(byte[] bytes, byte[] keyBytes, byte[] ivBytes)
        {
            var aes = new AesCryptoServiceProvider();
            aes.IV = ivBytes;
            aes.Key = keyBytes;

            ICryptoTransform transform = aes.CreateEncryptor();
            return transform.TransformFinalBlock(ivBytes.Concat(bytes).ToArray(), 0, bytes.Length + ivBytes.Length);
        }

        public static byte[] Decrypt(byte[] bytes, string keyStr, string ivStr)
        {
            var md5 = new MD5CryptoServiceProvider();
            var sha256 = new SHA256CryptoServiceProvider();
            byte[] ivBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(ivStr));
            byte[] keyBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(keyStr));

            return decrypt(bytes, keyBytes, ivBytes);
        }

        private static byte[] decrypt(byte[] bytes, byte[] keyBytes, byte[] ivBytes)
        {
            var aes = new AesCryptoServiceProvider();
            aes.IV = ivBytes;
            aes.Key = keyBytes;
            ICryptoTransform transform = aes.CreateDecryptor();
            return transform.TransformFinalBlock(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// 把iv数据插入到待加密数据前面
        /// </summary>
        /// <param name="enStr"></param>
        /// <param name="keyStr"></param>
        /// <returns></returns>
        public static string EncryptStrAndToBase64(string enStr, string keyStr)
        {
            var random=new Random();
            var ivStr = random.Next(9999999, 9999999);
            byte[] bytes = Encrypt(Encoding.GetEncoding("UTF-8").GetBytes(enStr), keyStr, ivStr.ToString());
            return Convert.ToBase64String(bytes);
        }

        public static string DecryptStrAndFromBase64(string deStr, string keyStr)
        {
            var sha256 = new SHA256CryptoServiceProvider();
            var ivSizes = 16;
            var keySize = 16;
            var ivBytes = new byte[ivSizes];
            byte[] keyBytes = new byte[keySize];
            var deBytes = Convert.FromBase64String(deStr);
            Array.Copy(deBytes, 0, ivBytes, 0, ivSizes);
            int encryptedSize = deBytes.Length - ivSizes;
            var data = new byte[encryptedSize];
            Array.Copy(deBytes, ivSizes, data, 0, encryptedSize);
            var shaKey = sha256.ComputeHash(Encoding.UTF8.GetBytes(keyStr));
            Array.Copy(shaKey, 0, keyBytes, 0, keySize);
            byte[] bytes = decrypt(data, keyBytes, ivBytes);
            return Encoding.GetEncoding("UTF-8").GetString(bytes);
        }
    }
}
