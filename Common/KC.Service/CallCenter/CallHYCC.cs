using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Com.Framework.Extension;
using Com.Framework.Util;
using Com.Service.Core.Base;
using Newtonsoft.Json;

namespace Com.Service.Core.CallCenter
{
    /// <summary>
    /// 华宇天下呼叫平台
    /// </summary>
    public class CallHYCC : AbstractCall
    {
        private const string ServiceName = "Com.Service.Core.Message.CallHYCC";
        public CallHYCC(CallConfig config) 
            : base(config)
        {
        }


        /// <summary>
        /// 拨打电话
        /// </summary>
        /// <param name="currentUserPhone">主叫号码</param>
        /// <param name="phoneNumber">被叫号码</param>
        /// <param name="record">
        /// 主叫号码类型
        ///     0: 座机；1: 手机
        /// </param>
        /// <param name="sesstionId"></param>
        /// <returns></returns>
        public override string CallContact(string currentUserPhone, string phoneNumber, string record, out string sesstionId)
        {
            var url = string.Empty;
            sesstionId = Guid.NewGuid().ToString();

            try
            {
                if (record.Equals("1"))
                {
                    url =
                        string.Format(
                            "{0}asterccinterface/?EVENT=MAKECALL&targetdn={2}&targettype=exter&agentgroupid=agentgroupid" +
                            "&usertype=usertype&user=user&orgidentity=orgidentity&pwdtype=pwdtype&password=password" +
                            "&modeltype=BusinessApp&model_id=model_id&userdata=userdata&agentexten=agentexten" +
                            "&callerid=callerid&callername=callername&trunkidentity=trunkidentity&cidtype=cidtype",
                            Config.SmsUrl, currentUserPhone, phoneNumber, sesstionId);
                }
                else
                {
                    var extNum = currentUserPhone.GetExtensionNumber();
                    url = string.Format("{0}admin/?m=interface&c=api&a=dial&extension={1}&extensionDst={2}&account={3}",
                    Config.SmsUrl, extNum, phoneNumber, sesstionId);
                }

                var result = GetHtmlData(url);
                LogUtil.LogInfo("SmsSHJ Return data: " + result);

                if (!string.IsNullOrWhiteSpace(result))
                {
                    var response = JsonConvert.DeserializeObject<ShjResult>(result);
                    if (response == null)
                        return "短信服务商（深海捷）无法解析返回的Json文件：" + result;

                    var message = string.Format("短信服务商（深海捷）-电话（{0}）拨打电话({1}){2}，接口返回消息：{3}。" + Environment.NewLine + "调用路径：{4}",
                    currentUserPhone,
                    phoneNumber,
                    response.error == 0 ? "成功" : "失败",
                    response.result,
                    url);
                    //LogUtil.LogInfo(message);
                    if (response.error == 0)
                        return null;

                    return message;
                }
                else
                {
                    return "短信服务商（深海捷）返回的结果为空，调用API接口地址：" + url;
                }
            }
            catch (Exception ex)
            {
                string message = "短信服务商（华为）调用服务({0})的方法({1})操作{2}。" +
                                 Environment.NewLine + "调用路径：{3}。" +
                                 Environment.NewLine + "错误消息为：{4}";
                message = string.Format(message, ServiceName, MethodBase.GetCurrentMethod().Name, "失败", url, ex.Message);
                LogUtil.LogError(message, ex.StackTrace);
                return message;
            }
        }

        public override bool StopCallContact(string phoneNumber, string sessionId)
        {
            var url = string.Format("{0}admin/?m=interface&c=api&a=hangup&extension={1}", Config.SmsUrl, phoneNumber);
            try
            {
                var result = GetHtmlData(url);
                LogUtil.LogInfo("SmsSHJ Return data: " + result);
                if (!string.IsNullOrWhiteSpace(result))
                {
                    var response = JsonConvert.DeserializeObject<ShjResult>(result);
                    if (response == null)
                        return false;

                    return response.error == 0;
                }
            }
            catch (Exception ex)
            {
                string message = "短信服务商（华为）调用服务({0})的方法({1})操作{2}。" +
                                 Environment.NewLine + "调用路径：{3}。" +
                                 Environment.NewLine + "错误消息为：{4}";
                message = string.Format(message, ServiceName, MethodBase.GetCurrentMethod().Name, "失败", url, ex.Message);
                LogUtil.LogError(message, ex.StackTrace);
                return false;
            }
            return false;
        }
    }
}
