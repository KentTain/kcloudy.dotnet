﻿using System;
using System.Text;

namespace  KC.Framework.SecurityHelper
{
    /// <summary>
    /// 实现Base64编码与其它编码转换的类
    /// </summary>
    public class Base64Provider
    {
        private Base64Provider()
        {
        }
        /// <summary>
        /// 将其它编码的字符串转换成Base64编码的字符串
        /// </summary>
        /// <param name="source">要转换的字符串</param>
        /// <returns></returns>
        public static string EncodeString(string source)
        {
            //如果字符串为空或者长度为0则抛出异常
            if (string.IsNullOrEmpty(source))
                return null;

            //将字符串转换成UTF-8编码的字节数组
            byte[] buffer = Encoding.UTF8.GetBytes(source);
            //将UTF-8编码的字节数组转换成Base64编码的字符串
            string result = Convert.ToBase64String(buffer);
            return result;
        }
        /// <summary>
        /// 将Base64编码的字符串转换成其它编码的字符串
        /// </summary>
        /// <param name="result">要转换的Base64编码的字符串</param>
        /// <returns></returns>
        public static string DecodeString(string result)
        {
            //如果字符串为空或者长度为0则抛出异常
            if (string.IsNullOrEmpty(result))
                return null;

            //将字符串转换成Base64编码的字节数组
            byte[] buffer = Convert.FromBase64String(result);
            //将字节数组转换成UTF-8编码的字符串
            string source = Encoding.UTF8.GetString(buffer);
            return source;
        }
    }
}