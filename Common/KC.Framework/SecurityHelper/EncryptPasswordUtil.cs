using System;

namespace KC.Framework.SecurityHelper
{
    public static class EncryptPasswordUtil
    {
        public const string Key = "KCloudy-Microsoft-EncryptKey";
        public static string EncryptPassword(string password)
        {
            return EncryptPassword(password, Key);
        }
        public static string EncryptPassword(string password, string key)
        {
            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException("password", "EncryptPassword：传入参数-需要加密的字符串为空");

            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException("key", "EncryptPassword：传入参数-加密秘钥为空");

            return DESProvider.EncryptString(password, key);
        }

        public static string DecryptPassword(string password)
        {
            return DecryptPassword(password, Key);
        }

        public static string DecryptPassword(string password, string key)
        {
            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException("password", "DecryptPassword：传入参数-需要加密的字符串为空");

            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException("key", "DecryptPassword：传入参数-加密秘钥为空");

            return DESProvider.DecryptString(password, key);
        }

        /// <summary>
        /// 生成随机字符串
        /// </summary>
        /// <param name="len">随机字符串长度（最小值为：8）</param>
        /// <returns></returns>
        public static string GetRandomString(int len = 8)
        {
            if (len < 8)
                len = 8;
            var ra = new Random();
            var first = ((char)ra.Next(65, 90)).ToString();
            var last = ((char)ra.Next(97, 123)).ToString();
            return first + ra.Next(0, 9) + System.Guid.NewGuid().ToString().Substring(0, len - 3) + last;
        }
    }
}
