using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using KC.Common;
using KC.Framework.Extension;
using KC.Common.HttpHelper;
using KC.Framework.Util;
using KC.Service.Base;
using KC.Service.Base.CallResult;
using KC.Service.Constants;

namespace KC.Service.CallCenter
{
    /// <summary>
    /// 长鑫盛通呼叫平台：Http方式
    /// </summary>
    public class CallUncall : AbstractCall
    {
        private const string ServiceName = "KC.Service.Message.CallUncall";
        public CallUncall(CallConfig config)
            : base(config)
        {
        }
        public override string IsNotBusy(string phoneNumber)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 拨打电话（不支持手机呼出的功能--参数record=1时的方式）
        ///     2.6 点击呼叫
        /// </summary>
        /// <param name="callerPhone">主叫号码</param>
        /// <param name="calleePhone">被叫号码</param>
        /// <param name="type">
        /// 主叫号码类型
        ///     0: 座机；1: 手机
        /// </param>
        /// <param name="sesstionId"></param>
        /// <returns></returns>
        public override string CallContact(string callerPhone, string calleePhone, CallType type, out string sesstionId)
        {
            var url = string.Empty;
            sesstionId = Guid.NewGuid().ToString();

            try
            {
                var extNum = callerPhone.GetExtensionNumber();
                var preTelNum = Config.Value2;//加拨9
                if (type == CallType.Telephone)
                {
                    url = string.Format("{0}http_uncall_api.php?model=OnClickCall&agent={1}&toTel={2}",
                    Config.SmsUrl, extNum, preTelNum + calleePhone, Config.TenantName, sesstionId);
                }
                else
                {
                    url = string.Format("{0}http_uncall_api.php?model=OnClickBridge&agent={1}&toTel={2}",
                    Config.SmsUrl, preTelNum + callerPhone, calleePhone, Config.TenantName, sesstionId);
                }

                var result = GetHtmlData(url);
                LogUtil.LogInfo("CallUncall Return data: " + result);

                if (!string.IsNullOrWhiteSpace(result))
                {
                    var response = SerializeHelper.FromJson<UncallResult>(result);
                    if (response == null)
                        return "呼叫中心服务提供商（长鑫盛通）无法解析返回的Json文件：" + result;

                    var message = string.Format("呼叫中心服务提供商（长鑫盛通）-电话（{0}）拨打电话({1}){2}，接口返回消息：{3}。" + Environment.NewLine + "调用路径：{4}",
                    callerPhone,
                    calleePhone,
                    response.status ? "成功" : "失败",
                    response.msg,
                    url);
                    LogUtil.LogInfo(message);
                    if (response.status)
                        return null;

                    return message;
                }
                else
                {
                    return "呼叫中心服务提供商（长鑫盛通）返回的结果为空，调用API接口地址：" + url;
                }
            }
            catch (Exception ex)
            {
                string message = "呼叫中心服务提供商（长鑫盛通）调用服务({0})的方法({1})操作{2}。" +
                                 Environment.NewLine + "调用路径：{3}。" +
                                 Environment.NewLine + "错误消息为：{4}";
                message = string.Format(message, ServiceName, MethodBase.GetCurrentMethod().Name, "失败", url, ex.Message);
                LogUtil.LogError(message, ex.StackTrace);
                return message;
            }
        }
        /// <summary>
        /// 强制挂机
        ///     2.5 强制挂机
        /// </summary>
        /// <param name="callerPhone">需要挂机的分机号</param>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public override bool StopCallContact(string callerPhone, string sessionId)
        {
            var extNum = callerPhone.GetExtensionNumber();
            var url = string.Format("{0}http_uncall_api.php?model=hangUp&agent={1}", Config.SmsUrl, extNum);
            try
            {
                var result = GetHtmlData(url);
                LogUtil.LogInfo("CallUncall Return data: " + result);

                if (!string.IsNullOrWhiteSpace(result))
                {
                    var response = SerializeHelper.FromJson<UncallResult>(result);
                    if (response == null || !response.status)
                        return false;

                    //response.msg：1、 挂机成功；2、 挂机失败
                    return response.msg == "1";
                }
            }
            catch (Exception ex)
            {
                string message = "呼叫中心服务提供商（长鑫盛通）调用服务({0})的方法({1})操作{2}。" +
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
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public override string GetCallVoiceFileUrl(string sessionId)
        {
            var url = string.Format("{0}http_uncall_api.php?model=getRecording&uid={1}", Config.SmsUrl, sessionId);
            try
            {
                var result = GetHtmlData(url);
                LogUtil.LogInfo("CallUncall Return data: " + result);
                if (!string.IsNullOrWhiteSpace(result))
                {
                    var response = SerializeHelper.FromJson<UncallResult>(result);
                    if (!response.status)
                        return string.Empty;

                    var filePath = Config.Value1.EndWithSlash() + response.msg;
                    return Config.SmsUrl.EndWithSlash() + Config.Value1 + response.msg;
                }
            }
            catch (Exception ex)
            {
                string message = "呼叫中心服务提供商（长鑫盛通）调用服务({0})的方法({1})操作{2}。" +
                                 Environment.NewLine + "调用路径：{3}。" +
                                 Environment.NewLine + "错误消息为：{4}";
                message = string.Format(message, ServiceName, MethodBase.GetCurrentMethod().Name, "失败", url, ex.Message);
                LogUtil.LogError(message, ex.StackTrace);
                return string.Empty;
            }

            return string.Empty;
        }

        /// <summary>
        /// 2.3 来电弹屏
        /// </summary>
        /// <param name="callerPhone">分机号</param>
        /// <returns></returns>
        public override string PopEvent(string callerPhone)
        {
            var extNum = callerPhone.GetExtensionNumber();
            var url = string.Format("{0}http_uncall_api.php?model=popEvent&agent={1}",
                    Config.SmsUrl, extNum);

            try
            {
                var result = GetHtmlData(url);
                LogUtil.LogInfo("CallUncall Return data: " + result);

                if (!string.IsNullOrWhiteSpace(result))
                {
                    var response = SerializeHelper.FromJson<UncallResult>(result);
                    if (response == null)
                        return "呼叫中心服务提供商（长鑫盛通）无法解析返回的Json文件：" + result;
                    var message =
                        string.Format("呼叫中心服务提供商（长鑫盛通）-分机号（{0}）来电弹屏{1}，接口返回消息：{2}。" + Environment.NewLine + "调用路径：{3}",
                            callerPhone,
                            response.status ? "成功" : "失败",
                            response.msg,
                            url);
                    LogUtil.LogInfo(message);
                    if (response.status)
                        return response.msg;

                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                string message = "呼叫中心服务提供商（长鑫盛通）调用服务({0})的方法({1})操作{2}。" +
                                 Environment.NewLine + "调用路径：{3}。" +
                                 Environment.NewLine + "错误消息为：{4}";
                message = string.Format(message, ServiceName, MethodBase.GetCurrentMethod().Name, "失败", url, ex.Message);
                LogUtil.LogError(message, ex.StackTrace);
                return message;
            }

            return string.Empty;
        }

        public override T GetCallVoiceFileInfo<T>(string sessionId, string caller, string callee, string startTime, string endTime)
        {
            throw new NotImplementedException();
        }
    }

    public class CallUncallWebApi : AbstractCall
    {
        private const string ServiceName = "KC.Service.Message.CallUncallWebApi";
        private int _maxRunCount = 5;
        private int _sleepMilliseconds = 3000;

        public CallUncallWebApi(CallConfig config)
            : base(config)
        {
        }

        public override string IsNotBusy(string callerPhone)
        {
            string url = Config.WebApiUrl + "juzkf/getListExten.html";
            string token = GetAccessToken(Config);
            var extNum = callerPhone.GetExtensionNumber();

            var data = new Dictionary<string, string>
            {
                {"access_token", token},
                {"exten", extNum}
            };
            var result = HttpWebRequestHelper.DoPost(url, data);
            LogUtil.LogInfo("CallUncallWebAPI getListExten Return data: " + result);

            bool isSuccess = GetErrorCode(result) == 200;
            var message =
                string.Format(
                    "呼叫中心服务提供商（长鑫盛通: {3}）-查询电话（{0}）状态{1}. " + Environment.NewLine + "接口返回消息：{2}。",
                    callerPhone,
                    isSuccess ? "成功" : "失败",
                    result,
                    Config.WebApiUrl);
            LogUtil.LogInfo(message);

            if (isSuccess)
            {
                var dataResult = GetData(result);
                var exten =
                    SerializeHelper
                        .FromJson<List<Exten>>(dataResult).FirstOrDefault(o => o.extension == extNum);
                if (exten != null)
                {
                    switch (exten.status)
                    {
                        case "1":
                            return "通话中";
                        case "4":
                            return "未注册";
                        case "8":
                            return "振铃中";
                        default:
                            return null;
                    }
                }
            }
            return message;
        }

        public override string CallContact(string callerPhone, string calleePhone, CallType type, out string sesstionId)
        {
            sesstionId = Guid.NewGuid().ToString();

            string url = Config.WebApiUrl + "juzkf/onClickCall.html";
            string token = GetAccessToken(Config);
            try
            {
                var extNum = callerPhone.GetExtensionNumber(); //分机号
                var data = new Dictionary<string, string>
                {
                    {"access_token", token},
                    {"exten", extNum},
                    {"phone", calleePhone}
                };
                var result = HttpWebRequestHelper.DoPost(url, data);
                LogUtil.LogInfo("CallUncallWebAPI OnClickCall Return data: " + result);

                var isSuccess = GetErrorCode(result) == 200;
                var message =
                    string.Format(
                        "呼叫中心服务提供商（长鑫盛通: {4}）-电话（{0}）拨打电话({1}){2}. " + Environment.NewLine + "接口返回消息：{3}。",
                        callerPhone,
                        calleePhone,
                        isSuccess ? "成功" : "失败",
                        result,
                        Config.WebApiUrl);
                LogUtil.LogInfo(message);

                if (isSuccess)
                    return null;
                return message;
            }
            catch (Exception ex)
            {
                var msg = string.Format("电话（{0}）呼叫电话（{1}）", callerPhone, calleePhone);
                string message = "呼叫中心服务提供商（长鑫盛通）调用服务({0})的方法({1})操作{2}。" +
                                 Environment.NewLine + "调用路径：{3}。" +
                                 Environment.NewLine + "错误消息为：{4}。";
                message = string.Format(message, ServiceName, MethodBase.GetCurrentMethod().Name, "失败", msg, ex.Message);
                LogUtil.LogError(message, ex.StackTrace);
                return message;
            }
        }

        public override bool StopCallContact(string callerPhone, string sessionId)
        {
            try
            {
                string url = Config.WebApiUrl + "juzkf/hangUp.html";
                string token = GetAccessToken(Config);
                var extNum = callerPhone.GetExtensionNumber();
                var data = new Dictionary<string, string>
                {
                    {"access_token", token},
                    {"exten", extNum}
                };
                var result = HttpWebRequestHelper.DoPost(url, data);
                LogUtil.LogInfo("CallUncallWebAPI hangUp Return data: " + result);

                var isSuccess = GetErrorCode(result) == 200;
                var message =
                    string.Format(
                        "呼叫中心服务提供商（长鑫盛通）-电话（{0}）挂断电话{1}. " + Environment.NewLine + "接口返回消息：{2}。",
                        callerPhone,
                        isSuccess ? "成功" : "失败",
                        result);
                LogUtil.LogInfo(message);

                return isSuccess;
            }
            catch (Exception ex)
            {
                string message = "呼叫中心服务提供商（长鑫盛通）调用服务({0})的方法({1})操作{2}。" +
                                 Environment.NewLine + "错误消息为：{3}";
                message = string.Format(message, ServiceName, MethodBase.GetCurrentMethod().Name, "失败", ex.Message);
                LogUtil.LogError(message, ex.StackTrace);
                return false;
            }
        }

        public override string PopEvent(string callerPhone)
        {
            try
            {
                var i = 0;
                var extUid = string.Empty;
                string url = Config.WebApiUrl + "juzkf/popEvent.html";
                string token = GetAccessToken(Config);
                var extNum = callerPhone.GetExtensionNumber();
                var data = new Dictionary<string, string>
                {
                    {"access_token", token},
                    {"exten", extNum}
                };
                while (i <= _maxRunCount)
                {
                    LogUtil.LogInfo("----start----子线程id：" + Thread.CurrentThread.ManagedThreadId +
                                     " 开始调用 3.3 来电弹屏-----分机号: " + extNum);
                    var result = HttpWebRequestHelper.DoPost(url, data);
                    LogUtil.LogInfo("弹屏幕结果(result):" + result);
                    int errorCode = GetErrorCode(result);
                    if (errorCode == 0)
                    {
                        var popEvent = SerializeHelper.GetMapValueByKey(result, "data");
                        if (!string.IsNullOrEmpty(popEvent))
                        {
                            var src = SerializeHelper.GetMapValueByKey(popEvent, "src");
                            var dst = SerializeHelper.GetMapValueByKey(popEvent, "dst");
                            var uid = SerializeHelper.GetMapValueByKey(popEvent, "uid");
                            var stats = SerializeHelper.GetMapValueByKey(popEvent, "Stats") == "0" ? "成功" : "失败";
                            var messge = string.Format(
                                "----end----3.3 来电弹屏 返回结果：呼叫方={0}; 接听方={1}; uid={2}; status={3}", src, dst, uid, stats);
                            LogUtil.LogInfo(messge);
                            extUid = uid;
                            break;
                        }
                    }
                    i++;
                    Thread.Sleep(_sleepMilliseconds);
                }
                return extUid;
            }
            catch (Exception ex)
            {
                string message = "呼叫中心服务提供商（长鑫盛通）调用服务({0})的方法({1})操作{2}。" +
                                 Environment.NewLine + "错误消息为：{3}";
                message = string.Format(message, ServiceName, MethodBase.GetCurrentMethod().Name, "失败", ex.Message);
                LogUtil.LogError(message, ex.StackTrace);
                return string.Empty;
            }
        }

        public override string GetCallVoiceFileUrl(string sessionId)
        {
            throw new NotImplementedException();
        }

        public override T GetCallVoiceFileInfo<T>(string sessionId, string caller, string callee, string startTime,
            string endTime)
        {
            try
            {
                var i = 0;
                var callRecording = new CallRecording();
                string url = Config.WebApiUrl + "juzkf/cdr.html";
                string token = GetAccessToken(Config);
                var extNum = caller.GetExtensionNumber();
                var data = new Dictionary<string, string>
                {
                    {"access_token", token},
                    {"stime", startTime},
                    {"etime", endTime},
                    {"src", extNum},
                    {"dst", callee}
                };
                while (i <= 2*_maxRunCount) //10次
                {
                    LogUtil.LogInfo(Thread.CurrentThread.ManagedThreadId + " 开始调用 3.3 获取录音文件名--uid: " + sessionId);
                    var result = HttpWebRequestHelper.DoPost(url, data);
                    int errorCode = GetErrorCode(result);
                    if (errorCode == 200)
                    {
                        var dataResult = SerializeHelper.GetMapValueByKey(result, "data");
                        callRecording =
                            SerializeHelper
                                .FromJson<List<CallRecording>>(dataResult)
                                .FirstOrDefault(o => o.uniqueid == sessionId);
                        if (callRecording != null)
                        {
                            var fileName = callRecording.userfield.ReplaceFirst("audio:", string.Empty);
                            LogUtil.LogInfo("----start----3.16 下载录音文件--录音文件名称：" + fileName);
                            callRecording.vRecordUrl = Config.WebApiUrl.EndWithSlash() + Config.Value1 + fileName;
                            break;
                        }
                    }
                    i++;
                    Thread.Sleep(20*_sleepMilliseconds); //每分钟调用一次
                }
                return callRecording as T;
            }
            catch (Exception ex)
            {
                string message = "呼叫中心服务提供商（长鑫盛通）调用服务({0})的方法({1})操作{2}。" +
                                 Environment.NewLine + "错误消息为：{3}";
                message = string.Format(message, ServiceName, MethodBase.GetCurrentMethod().Name, "失败", ex.Message);
                LogUtil.LogError(message, ex.StackTrace);
                return null;
            }
        }

        private string GetAccessToken(CallConfig config)
        {
            string cacheKey = config.TenantName + config.AppId + config.Token;
            var accessToken = CacheUtil.GetCache<string>(cacheKey);
            if (string.IsNullOrEmpty(accessToken))
            {
                string url = config.WebApiUrl + "juzkf/getAccessToken.html";
                var data = new Dictionary<string, string>
                {
                    {"appid", config.AppId},
                    {"token", config.Token}
                };
                string result = HttpWebRequestHelper.DoPost(url, data);
                int errorCode = GetErrorCode(result);

                if (errorCode == 200)
                {
                    accessToken = SerializeHelper.GetMapValueByKey(GetData(result), "access_token");
                    CacheUtil.SetCache(cacheKey, accessToken, TimeSpan.FromMinutes(TimeOutConstants.AccessTokenTimeOut));
                }
            }
            return accessToken;
        }
    }


    [Serializable]
    public class uncall
    {
        public int result { get; set; }
        public OnClickCall OnClickCall { get; set; }
        public OnClickBridge OnClickBridge { get; set; }
        public whisper whisper { get; set; }
        public popEvent popEvent { get; set; }
        public getRecording getRecording { get; set; }
        public whisper hangUp { get; set; }
        public getExtenStatus getExtenStatus { get; set; }
    }

    public class Exten
    {
        /// <summary>
        /// 分机号
        /// </summary>
        public string extension { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string password { get; set; }
        /// <summary>
        /// 是否示忙
        /// </summary>
        public string dnd { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public string status { get; set; }
    }
    /// <summary>
    /// 长鑫盛通通话记录
    /// </summary>
    public class CallRecording
    {
        /// <summary>
        /// 通话时间
        /// </summary>
        public string calldate { get; set; }
        /// <summary>
        /// 主叫
        /// </summary>
        public string src { get; set; }
        /// <summary>
        /// 被叫
        /// </summary>
        public string dst { get; set; }
        /// <summary>
        /// 通话时长
        /// </summary>
        public string billsec { get; set; }
        /// <summary>
        /// 接听类型
        /// </summary>
        public string disposition { get; set; }
        /// <summary>
        /// 记录编号
        /// </summary>
        public string uniqueid { get; set; }
        /// <summary>
        /// 录音名称
        /// </summary>
        public string userfield { get; set; }
        /// <summary>
        /// 录音地址
        /// </summary>
        public string vRecordUrl { get; set; }
    }

    [Serializable]
    public class popEvent
    {
        public string calla { get; set; }
        public string callb { get; set; }
        public string uid { get; set; }
        public string status { get; set; }
    }
    [Serializable]
    public class OnClickCall
    {
        public string Response { get; set; }
        public string ActionID { get; set; }
        public string Message { get; set; }
    }
    [Serializable]
    public class OnClickBridge
    {
        public string Response { get; set; }
        public string ActionID { get; set; }
        public string Message { get; set; }
    }
    [Serializable]
    public class whisper
    {
        public string Response { get; set; }
        public string Message { get; set; }
    }
    [Serializable]
    public class getRecording
    {
        public string files { get; set; }
    }
    [Serializable]
    public class getExtenStatus
    {
        public string extension { get; set; }
        /// <summary>
        /// status:状态、其中（0 空闲、8 振铃、4 不在线 、1 通话中 11 示忙）
        /// </summary>
        public int status { get; set; }
    }
}
