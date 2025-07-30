using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using KC.Common.ToolsHelper;
using KC.Framework.Util;
using KC.Service.Base;
using KC.Service.Util;

namespace KC.Service.Message
{
    /// <summary>
    /// 通用短信平台，比如：联合力量
    /// </summary>
    public class SmsGeneral : AbstractSms
    {
        private const string ServiceName = "KC.Service.Account.Message.SmsGeneral";
        public SmsGeneral(SmsConfig config)
            : base(config)
        {
        }

        public override string Send(List<long> mobiles, string tempCode, string msgContent)
        {
            var url =
                    string.Format(
                        "{0}?action={1}&userid={2}&account={3}&password={4}&mobile={5}&content={6}&sendTime={7}&extno={8}",
                        Config.SmsUrl, "send", Config.UserId, Config.UserAccount, Config.Password,
                        string.Join(",", mobiles), msgContent + Config.Signature, string.Empty, string.Empty);

            try
            {
                var temp = GetHtmlData(url);
                string clientIp = string.Empty; //IPHelper.GetClientIp();
                LogUtil.LogInfo("SmsGeneral Return xml: " + temp + "; client ip: " + clientIp);
                var returnSms = Deserialize(typeof(SmsResult), temp) as SmsResult;

                if (returnSms == null)
                    return clientIp + ": 通用短信商-无法解析返回的Xml文件：" + temp;

                var message = string.Format(clientIp + ": 通用短信商-手机号码（{0}）发送消息({1}){2}，接口返回消息：{3}。" + Environment.NewLine + "调用路径：{4}",
                    string.Join(",", mobiles), msgContent,
                    returnSms.returnstatus.Equals("faild", StringComparison.CurrentCultureIgnoreCase) ? "失败" : "成功",
                    returnSms.message,
                    url);
                //LogUtil.LogInfo(message);
                if (returnSms.returnstatus.Equals("faild", StringComparison.CurrentCultureIgnoreCase))
                    return message;

                return null;
            }
            catch (Exception ex)
            {
                string message = "通用短信商-调用服务({0})的方法({1})操作{2}。" +
                                 Environment.NewLine + "调用路径：{3}。" +
                                 Environment.NewLine + "错误消息为：{4}";
                message = string.Format(message, ServiceName, MethodBase.GetCurrentMethod().Name, "失败", url, ex.Message);
                LogUtil.LogError(message, ex.StackTrace);
                return message;
            }
        }

        public override string SendVoice(List<long> mobiles, string tempCode, string msgContent)
        {
            throw new NotImplementedException();
        }
    }
}
