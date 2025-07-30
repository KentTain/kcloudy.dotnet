using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace KC.Common.ToolsHelper
{
    /// <summary>
    /// 共用工具类
    /// </summary>
    public static class OtherUtilHelper
    {
        private static readonly object Locker = new object();

        /// <summary>
        /// 根据 User Agent 获取操作系统名称
        /// </summary>
        public static string GetHoverTreeOSName(string userAgent)
        {
            string m_hvtOsVersion = "未知";
            if (userAgent.Contains("NT 6.4"))
            {
                m_hvtOsVersion = "Windows 10";
            }
            else
            if (userAgent.Contains("NT 6.3"))
            {
                m_hvtOsVersion = "Windows 8.1";
            }
            else
            if (userAgent.Contains("NT 6.2"))
            {
                m_hvtOsVersion = "Windows 8";
            }
            else
            if (userAgent.Contains("NT 6.1"))
            {
                m_hvtOsVersion = "Windows 7";
            }
            else
            if (userAgent.Contains("NT 6.0"))
            {
                m_hvtOsVersion = "Windows Vista/Server 2008";
            }
            else if (userAgent.Contains("NT 5.2"))
            {
                m_hvtOsVersion = "Windows Server 2003";
            }
            else if (userAgent.Contains("NT 5.1"))
            {
                m_hvtOsVersion = "Windows XP";
            }
            else if (userAgent.Contains("NT 5"))
            {
                m_hvtOsVersion = "Windows 2000";
            }
            else if (userAgent.Contains("NT 4"))
            {
                m_hvtOsVersion = "Windows NT4";
            }
            else if (userAgent.Contains("Me"))
            {
                m_hvtOsVersion = "Windows Me";
            }
            else if (userAgent.Contains("98"))
            {
                m_hvtOsVersion = "Windows 98";
            }
            else if (userAgent.Contains("95"))
            {
                m_hvtOsVersion = "Windows 95";
            }
            else if (userAgent.Contains("Mac"))
            {
                m_hvtOsVersion = "Mac";
            }
            else if (userAgent.Contains("Unix"))
            {
                m_hvtOsVersion = "UNIX";
            }
            else if (userAgent.Contains("Linux"))
            {
                m_hvtOsVersion = "Linux";
            }
            else if (userAgent.Contains("SunOS"))
            {
                m_hvtOsVersion = "SunOS";
            }
            return m_hvtOsVersion;
        }

        /// <summary>
        /// 返回流水号，时间+随机数
        /// </summary>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public static string GetSerialNumber(string prefix)
        {
            lock (Locker)
            {
                Random r = new Random(BitConverter.ToInt32(Guid.NewGuid().ToByteArray(), 0));
                var nowDate = DateTime.UtcNow.AddHours(8).ToString("yyyyMMddHHmmss");
                return prefix + nowDate + r.Next(99999, 1000000);
            }
        }
        public static string GetRandomNumber(string prefix)
        {
            lock (Locker)
            {
                Random r = new Random(BitConverter.ToInt32(Guid.NewGuid().ToByteArray(), 0));
                return prefix + r.Next(99999, 1000000);
            }
        }

          /// <summary>
        /// 字符串编码转换
        /// </summary>
        /// <param name="srcEncoding">原编码</param>
        /// <param name="dstEncoding">目标编码</param>
        /// <param name="srcStr">原字符串</param>
        /// <returns>字符串</returns>
        public static string TransferEncoding(Encoding srcEncoding, Encoding dstEncoding, string srcStr)
        {
            byte[] srcBytes = srcEncoding.GetBytes(srcStr);
            byte[] bytes = Encoding.Convert(srcEncoding, dstEncoding, srcBytes);
            return dstEncoding.GetString(bytes);
        }

        /// <summary>
        /// 根据一个Model拼成一个string后加密
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public static string GetStrByModel<T>(T model, string key)
        {
            StringBuilder sb = new StringBuilder();
            foreach (System.Reflection.PropertyInfo p in model.GetType().GetProperties())
            {
                var attributes = p.GetCustomAttributesData().ToList();
                foreach (var attribute in attributes)
                {
                    if (attribute.AttributeType.Name == "RequiredAttribute")
                    {
                        var value = p.GetValue(model, null);
                        if (value != null)
                        {
                            if (p.PropertyType.Name == "Decimal")
                            {
                                sb.Append(decimal.Parse(value.ToString()).ToString("F"));
                            }
                            else
                            {
                                sb.Append(value);
                            }
                        }
                    }
                }
            }
            return GetEncryptionString(sb.ToString(), key);
        }

        /// <summary>
        /// 拼post的参数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="paramDTO"></param>
        /// <returns></returns>
        public static string GetPostData<T>(T paramDTO)
        {
            string retStr = string.Empty;
            foreach (System.Reflection.PropertyInfo p in paramDTO.GetType().GetProperties())
            {
                var value = p.GetValue(paramDTO, null);
                if (value != null)
                {
                    if (!string.IsNullOrEmpty(retStr))
                    {

                        retStr = retStr + "&" + p.Name + "=" + System.Web.HttpUtility.UrlEncode(value.ToString());
                    }
                    else
                    {
                        retStr = p.Name + "=" + HttpUtility.UrlEncode(value.ToString());
                    }
                }
            }
            return retStr;
        }

        public static string GetEncryptionString(string password, string privateEncryptKey)
        {
            return KC.Framework.SecurityHelper.MD5Provider.Hash(password + privateEncryptKey);
        }

        /// <summary>  
        /// 将c# DateTime时间格式转换为Unix时间戳格式  
        /// </summary>  
        /// <param name="time">时间</param>  
        /// <returns>long</returns>  
        public static long ConvertDateTimeToTimeStamp(System.DateTime time)
        {
            var startTime = TimeZoneInfo.ConvertTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0), TimeZoneInfo.Local);
            long t = (time.Ticks - startTime.Ticks) / 10000;   //除10000调整为13位      
            return t;
        }

        /// <summary>
        /// 时间戳转为C#格式时间
        /// </summary>
        /// <param name="timeStamp">Unix时间戳格式,13位</param>
        /// <returns>C#格式时间</returns>
        public static DateTime GetTimeByTimeStamp(string timeStamp)
        {
            DateTime dtStart = TimeZoneInfo.ConvertTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0), TimeZoneInfo.Local); 
            long lTime = long.Parse(timeStamp + "0000");
            TimeSpan toNow = new TimeSpan(lTime);
            return dtStart.Add(toNow);
        }
    }
}
