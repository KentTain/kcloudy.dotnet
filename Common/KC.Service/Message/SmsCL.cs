using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using KC.Common.ToolsHelper;
using KC.Framework.Util;
using KC.Service.Base;

namespace KC.Service.Message
{
    /// <summary>
    /// 创蓝平台
    /// </summary>
    public class SmsCL : AbstractSms
    {
        private const string ServiceName = "KC.Service.Account.Message.SmsCL";
        public SmsCL(SmsConfig config)
            : base(config)
        {
        }

        public override string Send(List<long> mobiles, string tempCode, string msgContent)
        {
            var url = string.Format("{0}?account={1}&pswd={2}&mobile={3}&msg={4}&needstatus=true", 
                Config.SmsUrl, Config.UserAccount, Config.Password, string.Join(",", mobiles), msgContent);
            try
            {
                LogUtil.LogInfo("SmsCL call service from url: " + url);
                var result = GetHtmlData(url);
                string clientIp = string.Empty; //IPHelper.GetClientIp();
                LogUtil.LogInfo("SmsCL Return data: " + result + "; client ip: " + clientIp + "; url: " + url);
                if (!string.IsNullOrWhiteSpace(result))
                {
                    var list = result.Split('\n')[0].Split(',');
                    if (list.Any())
                    {
                        var message = GetSendResult(list[1]);
                        if (string.IsNullOrWhiteSpace(message))
                        {
                            return null;
                        }

                        
                        return clientIp + ": 短信服务商（创蓝）发送信息失败，失败信息：" + message + "，调用API接口地址：" + url;
                    }

                    return clientIp + ": 短信服务商（创蓝）返回的结果无法解析，调用API接口地址：" + url;
                }
                else
                {
                    return clientIp + ": 短信服务商（创蓝）返回的结果为空，调用API接口地址：" + url;
                }
            }
            catch (Exception ex)
            {
                string message = "短信服务商（创蓝）调用服务({0})的方法({1})操作{2}。" +
                                 Environment.NewLine + "调用路径：{3}。" +
                                 Environment.NewLine + "错误消息为：{4}";
                message = string.Format(message, ServiceName, MethodBase.GetCurrentMethod().Name, "失败", url, ex.Message);
                LogUtil.LogError(message, ex.StackTrace);
                return message;
            }
        }

        public override string SendVoice(List<long> mobiles, string tempCode, string msgContent)
        {
            var url = string.Format("{0}?account={1}&pswd={2}&mobile={3}&msg={4}&needstatus=true",
                Config.SmsUrl, Config.UserAccount, Config.Password, string.Join(",", mobiles), msgContent);
            try
            {
                var result = GetHtmlData(url);
                string clientIp = string.Empty; //IPHelper.GetClientIp();
                LogUtil.LogInfo("SmsCL Return data: " + result + "; client ip: " + clientIp);
                if (!string.IsNullOrWhiteSpace(result))
                {
                    var list = result.Split('\n')[0].Split(',');
                    if (list.Any())
                    {
                        var message = GetSendResult(list[1]);
                        if (string.IsNullOrWhiteSpace(message))
                        {
                            return null;
                        }


                        return clientIp + ": 短信服务商（创蓝）发送信息失败，失败信息：" + message + "，调用API接口地址：" + url;
                    }

                    return clientIp + ": 短信服务商（创蓝）返回的结果无法解析，调用API接口地址：" + url;
                }
                else
                {
                    return clientIp + ": 短信服务商（创蓝）返回的结果为空，调用API接口地址：" + url;
                }
            }
            catch (Exception ex)
            {
                string message = "短信服务商（创蓝）调用服务({0})的方法({1})操作{2}。" +
                                 Environment.NewLine + "调用路径：{3}。" +
                                 Environment.NewLine + "错误消息为：{4}";
                message = string.Format(message, ServiceName, MethodBase.GetCurrentMethod().Name, "失败", url, ex.Message);
                LogUtil.LogError(message, ex.StackTrace);
                return message;
            }
        }

        private string GetSendResult(string status)
        {
            switch (status)
            {
                case "0":
                    return string.Empty;
                case "101":
                    return "无此用户";
                case "102":
                    return "密码错";
                case "103":
                    return "提交过快（提交速度超过流速限制）";
                case "104":
                    return "系统忙（因平台侧原因，暂时无法处理提交的短信）";
                case "105":
                    return "敏感短信（短信内容包含敏感词）";
                case "106":
                    return "消息长度错（>536或<=0）";
                case "107":
                    return "包含错误的手机号码";
                case "108":
                    return "手机号码个数错（群发>50000或<=0;单发>200或<=0）";
                case "109":
                    return "无发送额度（该用户可用短信数已使用完）";
                case "110":
                    return "不在发送时间内";
                case "111":
                    return "超出该账户当月发送额度限制";
                case "112":
                    return "无此产品，用户没有订购该产品";
                case "113":
                    return "extno格式错（非数字或者长度不对）";
                case "115":
                    return "自动审核驳回";
                case "116":
                    return "签名不合法，未带签名（用户必须带签名的前提下）";
                case "117":
                    return "IP地址认证错,请求调用的IP地址不是系统登记的IP地址";
                case "118":
                    return "用户没有相应的发送权限";
                case "119":
                    return "用户已过期";
            }
            return "未知错误";
        }
    }
}
