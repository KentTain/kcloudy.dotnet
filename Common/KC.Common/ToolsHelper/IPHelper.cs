using System;
using System.Text;
using System.Text.RegularExpressions;
using Com.Framework.Util;

namespace  Com.Common.ToolsHelper
{
    /// <summary>
    /// 共用工具类
    /// </summary>
    public static class IPHelper
    {
        #region 获得用户IP
        /// <summary>
        /// 获得用户IP
        /// </summary>
        public static string GetUserIp()
        {
            string ip;
            string[] temp;
            bool isErr = false;
            if (System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_ForWARDED_For"] == null)
                ip = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"].ToString();
            else
                ip = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_ForWARDED_For"].ToString();
            if (ip.Length > 15)
                isErr = true;
            else
            {
                temp = ip.Split('.');
                if (temp.Length == 4)
                {
                    for (int i = 0; i < temp.Length; i++)
                    {
                        if (temp[i].Length > 3) isErr = true;
                    }
                }
                else
                    isErr = true;
            }

            if (isErr)
                return "1.1.1.1";
            else
                return ip;
        }

        /// <summary>
        /// 获取客户端Ip
        /// </summary>
        /// <returns></returns>
        public static string GetClientIp()
        {
            String clientIP = "";
            try
            {
                if (System.Web.HttpContext.Current != null && System.Web.HttpContext.Current.Request != null)
                {
                    clientIP = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                    if (string.IsNullOrEmpty(clientIP) || (clientIP.ToLower() == "unknown"))
                    {
                        clientIP = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_REAL_IP"];
                        if (string.IsNullOrEmpty(clientIP))
                        {
                            clientIP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                        }
                    }
                    else
                    {
                        clientIP = clientIP.Split(',')[0];
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtil.LogException(ex);
            }
            
            return clientIP;
        }
        #endregion
    }
}
