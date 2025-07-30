using System;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Threading.Tasks;

namespace KC.Framework.SecurityHelper
{
    /// <summary>
    /// DES算法加密解密
    /// </summary>
    public class DESProvider
    {
        //默认的初始化密钥
        private const string DefaultKey = "tkGGRmBErvc=";
        private const string DefaultKeyIV = "Kl7ZgtM1dvQ=";

        #region 加密
        #region 加密：同步方式
        
        /// <summary>
        /// DES加密
        /// </summary>
        /// <param name="encryptString">要加密的字符串</param>
        /// <returns></returns>
        public static string EncryptString(string encryptString)
        {
            return EncryptString(encryptString, DefaultKey, DefaultKeyIV);
        }
        /// <summary>
        /// 采用DES算法对字符串加密
        /// </summary>
        /// <param name="encryptString">要加密的字符串</param>
        /// <param name="key">加密的密钥</param>
        /// <returns></returns>
        public static string EncryptString(string encryptString, string key)
        {
            return EncryptString(encryptString, key, DefaultKeyIV);
        }

        /// <summary>
        /// 采用DES算法对字符串加密
        /// </summary>
        /// <param name="encryptString">要加密的字符串</param>
        /// <param name="key">算法的密钥</param>
        /// <param name="vi">算法的初始化向量</param>
        /// <returns></returns>
        private static string EncryptString(string encryptString, string key, string vi)
        {
            //加密加密字符串是否为空
            if (string.IsNullOrEmpty(encryptString))
            {
                throw new ArgumentNullException("encryptString", "要加密的字符串不能为空");
            }
            //加查密钥是否为空
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key", "算法的密钥不能为空");
            }
            //加查密钥是否为空
            if (string.IsNullOrEmpty(vi))
            {
                throw new ArgumentNullException("vi", "算法的初始化向量不能为空");
            }

            //将密钥转换成字节数组
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            //设置初始化向量
            byte[] keyIV = Encoding.UTF8.GetBytes(vi);
            //将加密字符串转换成UTF8编码的字节数组
            byte[] inputByteArray = Encoding.UTF8.GetBytes(encryptString);
            //调用EncryptBytes方法加密
            byte[] resultByteArray = EncryptBytes(inputByteArray, keyBytes, keyIV);
            //将字节数组转换成字符串并返回
            return Convert.ToBase64String(resultByteArray);
        }

        /// <summary>
        /// 采用DES算法对字节数组加密
        /// </summary>
        /// <param name="sourceBytes">要加密的字节数组</param>
        /// <param name="keyBytes">算法的密钥，长度为8的倍数，最大长度64</param>
        /// <param name="keyIV">算法的初始化向量，长度为8的倍数，最大长度64</param>
        /// <returns></returns>
        private static byte[] EncryptBytes(byte[] sourceBytes, byte[] keyBytes, byte[] keyIV)
        {
            // Check arguments.
            if (sourceBytes == null || sourceBytes.Length <= 0)
                throw new ArgumentNullException("sourceBytes");
            if (keyBytes == null || keyBytes.Length <= 0)
                throw new ArgumentNullException("keyBytes");
            if (keyIV == null || keyIV.Length <= 0)
                throw new ArgumentNullException("keyIV");

            //检查密钥数组长度是否是8的倍数并且长度是否小于64
            keyBytes = CheckByteArrayLength(keyBytes);
            //检查初始化向量数组长度是否是8的倍数并且长度是否小于64
            keyIV = CheckByteArrayLength(keyIV);
            var provider = new DESCryptoServiceProvider();
            // java 默认的是ECB模式，PKCS5padding；c#默认的CBC模式，PKCS7padding 所以这里我们默认使用ECB方式
            //provider.Mode = CipherMode.ECB;
            //实例化内存流MemoryStream
            var mStream = new MemoryStream();
            //实例化CryptoStream
            var cStream = new CryptoStream(mStream, provider.CreateEncryptor(keyBytes, keyIV), CryptoStreamMode.Write);
            cStream.Write(sourceBytes, 0, sourceBytes.Length);
            cStream.FlushFinalBlock();
            //将内存流转换成字节数组
            byte[] buffer = mStream.ToArray();
            mStream.Close();//关闭流
            cStream.Close();//关闭流
            return buffer;
        }
        #endregion

        #region 加密：异步方式
        /// <summary>
        /// DES加密
        /// </summary>
        /// <param name="encryptString">要加密的字符串</param>
        /// <returns></returns>
        public static async Task<string> EncryptStringAsync(string encryptString)
        {
            return await EncryptStringAsync(encryptString, DefaultKey, DefaultKeyIV);
        }
        /// <summary>
        /// 采用DES算法对字符串加密
        /// </summary>
        /// <param name="encryptString">要加密的字符串</param>
        /// <param name="key">加密的密钥</param>
        /// <returns></returns>
        public static async Task<string> EncryptStringAsync(string encryptString, string key)
        {
            return await EncryptStringAsync(encryptString, key, DefaultKeyIV);
        }

        /// <summary>
        /// 采用DES算法对字符串加密
        /// </summary>
        /// <param name="encryptString">要加密的字符串</param>
        /// <param name="key">算法的密钥</param>
        /// <param name="vi">算法的初始化向量</param>
        /// <returns></returns>
        private static async Task<string> EncryptStringAsync(string encryptString, string key, string vi)
        {
            //加密加密字符串是否为空
            if (string.IsNullOrEmpty(encryptString))
            {
                throw new ArgumentNullException("encryptString", "要加密的字符串不能为空");
            }
            //加查密钥是否为空
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key", "算法的密钥不能为空");
            }
            //加查密钥是否为空
            if (string.IsNullOrEmpty(vi))
            {
                throw new ArgumentNullException("vi", "算法的初始化向量不能为空");
            }

            //将密钥转换成字节数组
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            //设置初始化向量
            byte[] keyIV = Encoding.UTF8.GetBytes(vi);
            //将加密字符串转换成UTF8编码的字节数组
            byte[] inputByteArray = Encoding.UTF8.GetBytes(encryptString);
            //调用EncryptBytes方法加密
            byte[] resultByteArray = await EncryptBytesAsync(inputByteArray, keyBytes, keyIV);
            //将字节数组转换成字符串并返回
            return Convert.ToBase64String(resultByteArray);
        }

        /// <summary>
        /// 采用DES算法对字节数组加密
        /// </summary>
        /// <param name="sourceBytes">要加密的字节数组</param>
        /// <param name="keyBytes">算法的密钥，长度为8的倍数，最大长度64</param>
        /// <param name="keyIV">算法的初始化向量，长度为8的倍数，最大长度64</param>
        /// <returns></returns>
        private static async Task<byte[]> EncryptBytesAsync(byte[] sourceBytes, byte[] keyBytes, byte[] keyIV)
        {
            // Check arguments.
            if (sourceBytes == null || sourceBytes.Length <= 0)
                throw new ArgumentNullException("sourceBytes");
            if (keyBytes == null || keyBytes.Length <= 0)
                throw new ArgumentNullException("keyBytes");
            if (keyIV == null || keyIV.Length <= 0)
                throw new ArgumentNullException("keyIV");

            //检查密钥数组长度是否是8的倍数并且长度是否小于64
            keyBytes = CheckByteArrayLength(keyBytes);
            //检查初始化向量数组长度是否是8的倍数并且长度是否小于64
            keyIV = CheckByteArrayLength(keyIV);
            using (var provider = new DESCryptoServiceProvider())
            {
                // java 默认的是ECB模式，PKCS5padding；c#默认的CBC模式，PKCS7padding 所以这里我们默认使用ECB方式
                //provider.Mode = CipherMode.ECB;
                using (var ct = provider.CreateEncryptor(keyBytes, keyIV))
                {
                    //实例化内存流MemoryStream
                    using (var mStream = new MemoryStream())
                    {
                        //实例化CryptoStream
                        using (var cStream = new CryptoStream(mStream, ct, CryptoStreamMode.Write))
                        {
                            await cStream.WriteAsync(sourceBytes, 0, sourceBytes.Length);
                            await cStream.FlushAsync();
                            cStream.Close();
                        }
                        //将内存流转换成字节数组
                        return mStream.ToArray(); ;
                    }
                }
            } 
        }
        #endregion
        #endregion

        #region 解密
        #region 解密：同步方式
        /// <summary>
        /// 采用DES算法对字符串解密
        /// </summary>
        /// <param name="decryptString">要解密的字符串</param>
        /// <returns></returns>
        public static string DecryptString(string decryptString)
        {
            return DecryptString(decryptString, DefaultKey, DefaultKeyIV);
        }
        /// <summary>
        /// 采用DES算法对字符串解密
        /// </summary>
        /// <param name="decryptString">要解密的字符串</param>
        /// <param name="key">算法的密钥</param>
        /// <returns></returns>
        public static string DecryptString(string decryptString, string key)
        {
            return DecryptString(decryptString, key, DefaultKeyIV);
        }
        /// <summary>
        /// 采用DES算法对字符串解密
        /// </summary>
        /// <param name="decryptString">要解密的字符串</param>
        /// <param name="key">算法的密钥</param>
        /// <param name="vi">算法的初始化向量</param>
        /// <returns></returns>
        private static string DecryptString(string decryptString, string key, string vi)
        {
            if (string.IsNullOrEmpty(decryptString))
            {
                throw new ArgumentNullException("decryptString", "要解密的字符串不能为空");
            }
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key", "算法的密钥不能为空");
            }
            //加查密钥是否为空
            if (string.IsNullOrEmpty(vi))
            {
                throw new ArgumentNullException("vi", "算法的初始化向量不能为空");
            }

            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] keyIV = Encoding.UTF8.GetBytes(vi);
            //将解密字符串转换成Base64编码字节数组
            byte[] inputByteArray = Convert.FromBase64String(decryptString);
            //调用DecryptBytes方法解密
            byte[] resultByteArray = DecryptBytes(inputByteArray, keyBytes, keyIV);
            //将字节数组转换成UTF8编码的字符串
            return Encoding.UTF8.GetString(resultByteArray);
        }

        /// <summary>
        /// 采用DES算法对字节数组解密
        /// </summary>
        /// <param name="sourceBytes">要加密的字节数组</param>
        /// <param name="keyBytes">算法的密钥，长度为8的倍数，最大长度64</param>
        /// <param name="keyIV">算法的初始化向量，长度为8的倍数，最大长度64</param>
        /// <returns></returns>
        private static byte[] DecryptBytes(byte[] sourceBytes, byte[] keyBytes, byte[] keyIV)
        {
            // Check arguments.
            if (sourceBytes == null || sourceBytes.Length <= 0)
                throw new ArgumentNullException("sourceBytes");
            if (keyBytes == null || keyBytes.Length <= 0)
                throw new ArgumentNullException("keyBytes");
            if (keyIV == null || keyIV.Length <= 0)
                throw new ArgumentNullException("keyIV");

            //检查密钥数组长度是否是8的倍数并且长度是否小于64
            keyBytes = CheckByteArrayLength(keyBytes);
            //检查初始化向量数组长度是否是8的倍数并且长度是否小于64
            keyIV = CheckByteArrayLength(keyIV);
            var provider = new DESCryptoServiceProvider();
            // java 默认的是ECB模式，PKCS5padding；c#默认的CBC模式，PKCS7padding 所以这里我们默认使用ECB方式
            //provider.Mode = CipherMode.ECB;
            var mStream = new MemoryStream();
            var cStream = new CryptoStream(mStream, provider.CreateDecryptor(keyBytes, keyIV), CryptoStreamMode.Write);
            cStream.Write(sourceBytes, 0, sourceBytes.Length);
            cStream.FlushFinalBlock();
            //将内存流转换成字节数组
            byte[] buffer = mStream.ToArray();
            mStream.Close();//关闭流
            cStream.Close();//关闭流
            return buffer;
        }
        #endregion

        #region 解密：异步方式
        /// <summary>
        /// 采用DES算法对字符串解密
        /// </summary>
        /// <param name="decryptString">要解密的字符串</param>
        /// <returns></returns>
        public static async Task<string> DecryptStringAsync(string decryptString)
        {
            return await DecryptStringAsync(decryptString, DefaultKey, DefaultKeyIV);
        }
        /// <summary>
        /// 采用DES算法对字符串解密
        /// </summary>
        /// <param name="decryptString">要解密的字符串</param>
        /// <param name="key">算法的密钥</param>
        /// <returns></returns>
        public static async Task<string> DecryptStringAsync(string decryptString, string key)
        {
            return await DecryptStringAsync(decryptString, key, DefaultKeyIV);
        }
        /// <summary>
        /// 采用DES算法对字符串解密
        /// </summary>
        /// <param name="decryptString">要解密的字符串</param>
        /// <param name="key">算法的密钥</param>
        /// <param name="vi">算法的初始化向量</param>
        /// <returns></returns>
        private static async Task<string> DecryptStringAsync(string decryptString, string key, string vi)
        {
            if (string.IsNullOrEmpty(decryptString))
            {
                throw new ArgumentNullException("decryptString", "要解密的字符串不能为空");
            }
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key", "算法的密钥不能为空");
            }
            //加查密钥是否为空
            if (string.IsNullOrEmpty(vi))
            {
                throw new ArgumentNullException("vi", "算法的初始化向量不能为空");
            }

            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] keyIV = Encoding.UTF8.GetBytes(vi);
            //将解密字符串转换成Base64编码字节数组
            byte[] inputByteArray = Convert.FromBase64String(decryptString);
            //调用DecryptBytes方法解密
            byte[] resultByteArray = await DecryptBytesAsync(inputByteArray, keyBytes, keyIV);
            //将字节数组转换成UTF8编码的字符串
            return Encoding.UTF8.GetString(resultByteArray);
        }
        private static async Task<byte[]> DecryptBytesAsync(byte[] sourceBytes, byte[] keyBytes, byte[] keyIV)
        {
            // Check arguments.
            if (sourceBytes == null || sourceBytes.Length <= 0)
                throw new ArgumentNullException("sourceBytes");
            if (keyBytes == null || keyBytes.Length <= 0)
                throw new ArgumentNullException("keyBytes");
            if (keyIV == null || keyIV.Length <= 0)
                throw new ArgumentNullException("keyIV");

            //检查密钥数组长度是否是8的倍数并且长度是否小于64
            keyBytes = CheckByteArrayLength(keyBytes);
            //检查初始化向量数组长度是否是8的倍数并且长度是否小于64
            keyIV = CheckByteArrayLength(keyIV);
            using (var provider = new DESCryptoServiceProvider())
            {
                // java 默认的是ECB模式，PKCS5padding；c#默认的CBC模式，PKCS7padding 所以这里我们默认使用ECB方式
                //provider.Mode = CipherMode.ECB;
                using (var ct = provider.CreateDecryptor(keyBytes, keyIV))
                {
                    //实例化内存流MemoryStream
                    using (var mStream = new MemoryStream())
                    {
                        //实例化CryptoStream
                        using (var cStream = new CryptoStream(mStream, ct, CryptoStreamMode.Write))
                        {
                            await cStream.WriteAsync(sourceBytes, 0, sourceBytes.Length);
                            await cStream.FlushAsync();
                            cStream.Close();
                        }
                        //将内存流转换成字节数组
                        return mStream.ToArray(); ;
                    }
                }
            }
        }
        #endregion
        #endregion

        /// <summary>
        /// 检查密钥或初始化向量的长度，如果不是8的倍数或长度大于64则截取前8个元素
        /// </summary>
        /// <param name="byteArray">要检查的数组</param>
        /// <returns></returns>
        private static byte[] CheckByteArrayLength(byte[] byteArray)
        {
            var resultBytes = new byte[8];
            //如果数组长度小于8
            if (byteArray.Length < 8)
            {
                var defaultByteArry = Encoding.UTF8.GetBytes(DefaultKey);
                byteArray = MergeBytes(byteArray, defaultByteArry);
            }

            Array.Copy(byteArray, 0, resultBytes, 0, 8);
            return resultBytes;
        }

        private static byte[] MergeBytes(byte[] data1, byte[] data2)
        {
            var all = new byte[data1.Length + data2.Length];
            Buffer.BlockCopy(data1, 0, all, 0, data1.Length);
            Buffer.BlockCopy(data2, 0, all, data1.Length * sizeof(byte), data2.Length);

            return all;
        }
    }
}