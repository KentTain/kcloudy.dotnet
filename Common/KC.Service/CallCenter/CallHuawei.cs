using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using KC.Framework.Extension;
using KC.Framework.Util;
using KC.Service.Base;
using KC.Service.Constants;

namespace KC.Service.CallCenter
{
    public class CallHuawei : AbstractCall
    {
        private const string ServiceName = "KC.Service.Message.CallHuawei";
        public CallHuawei(CallConfig config)
            : base(config)
        {
        }
        public override string IsNotBusy(string phoneNumber)
        {
            throw new NotImplementedException();
        }
        public override string CallContact(string currentUserPhone, string phoneNumber, CallType record, out string sesstionId)
        {
            sesstionId = null;
            var timestamp = DateTime.UtcNow.ToLocalDateTimeStr("yyyyMMddhhmmss");
            var requestData = new RequestForCall();
            var requestHead = new RequestHead();
            var requestBody = new RequestBodyForCall();
            requestData.Head = requestHead;
            requestData.Body = requestBody;
            requestHead.MethodName = "Dial";//Config.MethodName;
            requestHead.Spid = Config.Spid;
            requestHead.Appid = Config.Appid;
            requestHead.Passwd = Sha1Encrypt(Config.Passwd);
            requestHead.Timestamp = timestamp;
            requestHead.Authenticator = Sha1Encrypt(timestamp + requestHead.MethodName + Config.Appid + Config.Passwd);
            requestBody.ChargeNbr = Config.ChargeNbr;
            requestBody.Key = Config.Key;
            requestBody.CallerNbr = currentUserPhone;
            requestBody.CalleeNbr = phoneNumber;
            requestBody.DisplayNbr = Config.DisplayNbr;
            requestBody.Record = ((int)record).ToString();
            requestBody.AnswerOnMedia = "0";
            requestBody.DisplayNbrMode = "1";
            var url = string.Format("{0}?postData={1}", Config.SmsUrl, new XmlSerialize().ModelToXml(requestData));

            try
            {
                var result = GetHtmlData(url);
                LogUtil.LogInfo("SmsHuawei Return data: " + result);

                if (!string.IsNullOrWhiteSpace(result))
                {
                    var response = new Response();
                    response = new XmlSerialize().XmlToModel<Response>(result);
                    if (response == null)
                        return "呼叫中心服务提供商（华为电信）无法解析返回的Xml文件：" + result;

                    var utcNow = DateTime.UtcNow;
                    var content = new SmsContent();
                    content.CallBackSessionId = response.Head.Sessionid;
                    content.ExpiredDateTime = utcNow.AddMinutes(TimeOutConstants.CacheShortTimeOut);

                    //System.Web.HttpContext.Current.Session[phoneNumber] = content;
                    CacheUtil.SetCache(phoneNumber, content);
                    sesstionId = response.Head.Sessionid;

                    var message = string.Format("呼叫中心服务提供商（华为电信）-电话（{0}）拨打电话({1}){2}，接口返回消息：{3}。" + Environment.NewLine + "调用路径：{4}",
                    currentUserPhone,
                    phoneNumber,
                    response.Head != null && response.Head.Result.Equals("0") ? "成功" : "失败",
                    response.Head != null ? response.Head.ResultDesc : string.Empty,
                    url);
                    //LogUtil.LogInfo(message);
                    if (response.Head != null && response.Head.Result.Equals("0"))
                        return null;

                    return message;
                }
                else
                {
                    return "呼叫中心服务提供商（华为电信）返回的结果为空，调用API接口地址：" + url;
                }
            }
            catch (Exception ex)
            {
                string message = "呼叫中心服务提供商（华为电信）调用服务({0})的方法({1})操作{2}。" +
                                 Environment.NewLine + "调用路径：{3}。" +
                                 Environment.NewLine + "错误消息为：{4}";
                message = string.Format(message, ServiceName, MethodBase.GetCurrentMethod().Name, "失败", url, ex.Message);
                LogUtil.LogError(message, ex.StackTrace);
                return message;
            }

        }
        
        public override bool StopCallContact(string phoneNumber,string sessionId)
        {
            var timestamp = DateTime.UtcNow.ToLocalDateTimeStr("yyyyMMddhhmmss");
            var requestData = new RequestForCall();
            var requestHead = new RequestHead();
            var requestBody = new RequestBodyForCall();
            requestData.Head = requestHead;
            requestData.Body = requestBody;
            requestHead.MethodName = "DialStop";//Config.MethodName;
            requestHead.Spid = Config.Spid;
            requestHead.Appid = Config.Appid;
            requestHead.Passwd = Sha1Encrypt(Config.Passwd);
            requestHead.Timestamp = timestamp;
            requestHead.Authenticator = Sha1Encrypt(timestamp + requestHead.MethodName + Config.Appid + Config.Passwd);
            requestBody.Sessionid = sessionId;
            var url = string.Format("{0}?postData={1}", Config.SmsUrl, new XmlSerialize().ModelToXml(requestData));
            try
            {
                var result = GetHtmlData(url);
                LogUtil.LogInfo("SmsHuawei Return data: " + result);
                if (!string.IsNullOrWhiteSpace(result))
                {
                    var response = new Response();
                    response = new XmlSerialize().XmlToModel<Response>(result);
                    return response.Head != null && response.Head.Result.Equals("0");
                }
            }
            catch (Exception ex)
            {
                string message = "呼叫中心服务提供商（华为电信）调用服务({0})的方法({1})操作{2}。" +
                                 Environment.NewLine + "调用路径：{3}。" +
                                 Environment.NewLine + "错误消息为：{4}";
                message = string.Format(message, ServiceName, MethodBase.GetCurrentMethod().Name, "失败", url, ex.Message);
                LogUtil.LogError(message, ex.StackTrace);
                return false;
            }
            return false;
        }

        public override string PopEvent(string phoneNumber)
        {
            throw new NotImplementedException();
        }

        public override string GetCallVoiceFileUrl(string sessionId)
        {
            throw new NotImplementedException();
        }

        public override T GetCallVoiceFileInfo<T>(string sessionId, string caller, string callee, string startTime, string endTime)
        {
            throw new NotImplementedException();
        }
    }
}
