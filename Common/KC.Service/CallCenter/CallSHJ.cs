using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using KC.Common;
using KC.Framework.Extension;
using KC.Framework.Util;
using KC.Service.Base;

namespace KC.Service.CallCenter
{
    /// <summary>
    /// 深海捷呼叫平台
    /// </summary>
    public class CallSHJ : AbstractCall
    {
        private const string ServiceName = "KC.Service.Message.CallSHJ";
        public CallSHJ(CallConfig config) 
            : base(config)
        {
        }

        public override string IsNotBusy(string phoneNumber)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 拨打电话
        ///     3.4 电话呼出
        /// </summary>
        /// <param name="currentUserPhone">主叫号码</param>
        /// <param name="phoneNumber">被叫号码</param>
        /// <param name="type">
        /// 主叫号码类型
        ///     0: 座机；1: 手机
        /// </param>
        /// <param name="sesstionId"></param>
        /// <returns></returns>
        public override string CallContact(string currentUserPhone, string phoneNumber, CallType type, out string sesstionId)
        {
            var url = string.Empty;
            sesstionId = Guid.NewGuid().ToString();

            try
            {
                // 0: 座机；1: 手机
                if (type == CallType.Telephone)
                {
                    var extNum = currentUserPhone.GetExtensionNumber();
                    url = string.Format("{0}admin/?m=interface&c=api&a=dial&extension={1}&extensionDst={2}&account={3}&variable=__autocallcustomfield={4}",
                    Config.SmsUrl, extNum, phoneNumber, Config.TenantName, sesstionId);
                }
                else
                {
                    url = string.Format("{0}admin/?m=interface&c=api&a=mobiletransfer&mobile={1}&mobileDst={2}&account={3}&variable=__autocallcustomfield={4}",
                    Config.SmsUrl, currentUserPhone, phoneNumber, Config.TenantName, sesstionId);
                }

                var result = GetHtmlData(url);
                LogUtil.LogInfo("SmsSHJ Return data: " + result);

                if (!string.IsNullOrWhiteSpace(result))
                {
                    var response = SerializeHelper.FromJson<ShjResult>(result);
                    if (response == null)
                        return "呼叫中心服务提供商（深海捷）无法解析返回的Json文件：" + result;

                    var message = string.Format("呼叫中心服务提供商（深海捷）-电话（{0}）拨打电话({1}){2}，接口返回消息：{3}。" + Environment.NewLine + "调用路径：{4}",
                    currentUserPhone,
                    phoneNumber,
                    response.error == 0 ? "成功" : "失败",
                    response.result,
                    url);
                    LogUtil.LogDebug(message);
                    if (response.error == 0)
                        return null;

                    return message;
                }
                else
                {
                    return "呼叫中心服务提供商（深海捷）返回的结果为空，调用API接口地址：" + url;
                }
            }
            catch (Exception ex)
            {
                string message = "呼叫中心服务提供商（深海捷）调用服务({0})的方法({1})操作{2}。" +
                                 Environment.NewLine + "调用路径：{3}。" +
                                 Environment.NewLine + "错误消息为：{4}";
                message = string.Format(message, ServiceName, MethodBase.GetCurrentMethod().Name, "失败", url, ex.Message);
                LogUtil.LogError(message, ex.StackTrace);
                return message;
            }
        }
        /// <summary>
        /// 强制挂机
        ///     3.5 电话挂断
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public override bool StopCallContact(string phoneNumber, string sessionId)
        {
            var url = string.Format("{0}admin/?m=interface&c=api&a=hangup&extension={1}", Config.SmsUrl, phoneNumber);
            try
            {
                var result = GetHtmlData(url);
                LogUtil.LogInfo("SmsSHJ Return data: " + result);
                if (!string.IsNullOrWhiteSpace(result))
                {
                    var response = SerializeHelper.FromJson<ShjResult>(result);
                    if (response == null)
                        return false;

                    return response.error == 0;
                }
            }
            catch (Exception ex)
            {
                string message = "呼叫中心服务提供商（深海捷）调用服务({0})的方法({1})操作{2}。" +
                                 Environment.NewLine + "调用路径：{3}。" +
                                 Environment.NewLine + "错误消息为：{4}";
                message = string.Format(message, ServiceName, MethodBase.GetCurrentMethod().Name, "失败", url, ex.Message);
                LogUtil.LogError(message, ex.StackTrace);
                return false;
            }
            return false;
        }
        /// <summary>
        /// 获取通话录音的下载地址
        ///     3.37：获取呼叫中心数据，如通话记录、客户评价、语音留言 + 
        ///     3.27：通话录音下载
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public override string GetCallVoiceFileUrl(string sessionId)
        {
            var url = string.Format("{0}interface/api/?action=getmsg", Config.SmsUrl);
            try
            {
                var result = GetHtmlData(url);
                LogUtil.LogInfo("SmsSHJ Return data: " + result);
                if (!string.IsNullOrWhiteSpace(result))
                {
                    var response = SerializeHelper.FromJson<ShjResult>(result);
                    if (response.error != 0 || response.data == null)
                        return string.Empty;

                    var callLogs = response.data.msg.Where(m => m.Event.Equals("cdr", StringComparison.OrdinalIgnoreCase)).ToList();
                    var callLog = callLogs.FirstOrDefault(
                        m => !string.IsNullOrEmpty(m.MixExtVar)
                             && m.MixExtVar.EndsWith(sessionId, StringComparison.OrdinalIgnoreCase)
                             && m.Disposition.Equals("ANSWERED", StringComparison.OrdinalIgnoreCase));
                    if (callLog == null)
                        return string.Empty;

                    var filePath = Config.Value1.EndWithSlash() + callLog.UserField;
                    return Config.SmsUrl.EndWithSlash() + "admin/?m=interface&c=api&a=record_download&filename=" + filePath; ;
                }
            }
            catch (Exception ex)
            {
                string message = "呼叫中心服务提供商（深海捷）调用服务({0})的方法({1})操作{2}。" +
                                 Environment.NewLine + "调用路径：{3}。" +
                                 Environment.NewLine + "错误消息为：{4}";
                message = string.Format(message, ServiceName, MethodBase.GetCurrentMethod().Name, "失败", url, ex.Message);
                LogUtil.LogError(message, ex.StackTrace);
                return string.Empty;
            }

            return string.Empty;
        }
        /// <summary>
        /// 3.2 来电弹屏
        /// </summary>
        /// <param name="currentUserPhone"></param>
        /// <returns></returns>
        public override string PopEvent(string currentUserPhone)
        {
            var extNum = currentUserPhone.GetExtensionNumber();
            var url = string.Format("{0}interface/api/?action=popscreen&extension={1}", Config.SmsUrl, extNum);
            try
            {
                var result = GetHtmlData(url);
                LogUtil.LogInfo("SmsSHJ Return data: " + result);
                if (!string.IsNullOrWhiteSpace(result))
                {
                    var response = SerializeHelper.FromJson<ShjResult>(result);
                    if (response == null || response.error != 0 || response.data == null)
                        return string.Empty;

                    return response.result;
                }
            }
            catch (Exception ex)
            {
                string message = "呼叫中心服务提供商（深海捷）调用服务({0})的方法({1})操作{2}。" +
                                 Environment.NewLine + "调用路径：{3}。" +
                                 Environment.NewLine + "错误消息为：{4}";
                message = string.Format(message, ServiceName, MethodBase.GetCurrentMethod().Name, "失败", url, ex.Message);
                LogUtil.LogError(message, ex.StackTrace);
                return string.Empty;
            }
            return string.Empty;
        }

        public override T GetCallVoiceFileInfo<T>(string sessionId, string caller, string callee, string startTime, string endTime)
        {
            throw new NotImplementedException();
        }
    }
}
