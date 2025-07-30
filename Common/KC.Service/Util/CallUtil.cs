using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.Service.Base;
using KC.Service.CallCenter;

namespace KC.Service.Util
{
    public class CallUtil
    {
        private static ICallService _smsServiceProvider;
        public static string IsNotBusy(CallConfig config, string callerPhone)
        {
            if (callerPhone == "")
            {
                return "拨打号码为空";
            }

            switch (config.ProviderName.ToLower())
            {
                case "dxnlkf":
                    _smsServiceProvider = new CallHuawei(config);
                    break;
                case "shj":
                    _smsServiceProvider = new CallSHJ(config);
                    break;
                case "uncall":
                    //_smsServiceProvider = new CallUncallWebService(config);
                    _smsServiceProvider = new CallUncallWebApi(config);
                    break;
                default:
                    return "不支持的短信或语音平台。";
                    //break;
            }

            return _smsServiceProvider.IsNotBusy(callerPhone);
        }
        /// <summary>
        /// 拨打电话
        /// </summary>
        /// <param name="config">拨打电话的配置</param>
        /// <param name="callerPhone">主叫人的电话（座机格式：82680051-808）</param>
        /// <param name="calleePhone">被叫人的电话：一般为手机</param>
        /// <param name="type">
        /// 0：主叫人使用座机呼叫；
        /// 1：主叫人使用手机呼叫</param>
        /// <param name="sesstionId">通话记录的SessionId</param>
        /// <returns>
        /// 呼叫成功时，无返回值；
        /// 呼叫出错时，返回错误信息
        /// </returns>
        public static string CallContact(CallConfig config, string callerPhone, string calleePhone, CallType type, out string sesstionId)
        {
            sesstionId = null;
            if (calleePhone == "")
            {
                return "拨打号码为空";
            }

            switch (config.ProviderName.ToLower())
            {
                case "dxnlkf":
                    _smsServiceProvider = new CallHuawei(config);
                    break;
                case "shj":
                    _smsServiceProvider = new CallSHJ(config);
                    break;
                case "uncall":
                    //_smsServiceProvider = new CallUncallWebService(config);
                    _smsServiceProvider = new CallUncallWebApi(config);
                    break;
                default:
                    return "不支持的短信或语音平台。";
                    //break;
            }

            return _smsServiceProvider.CallContact(callerPhone, calleePhone, type, out sesstionId);
        }
        public static string PopEvent(CallConfig config, string phoneNumber)
        {
            switch (config.ProviderName.ToLower())
            {
                case "dxnlkf":
                    _smsServiceProvider = new CallHuawei(config);
                    break;
                case "shj":
                    _smsServiceProvider = new CallSHJ(config);
                    break;
                case "uncall":
                    //_smsServiceProvider = new CallUncallWebService(config);
                    _smsServiceProvider = new CallUncallWebApi(config);
                    break;
                default:
                    return string.Empty;
                    throw new Exception("不支持的短信或语音平台。");
                    //break;
            }
            return _smsServiceProvider.PopEvent(phoneNumber);
        }
        public static bool StopCallContact(CallConfig config, string phoneNumber, string message)
        {
            if (phoneNumber == "")
            {
                return false;
                throw new Exception("拨打号码为空");
            }
            switch (config.ProviderName.ToLower())
            {
                case "dxnlkf":
                    _smsServiceProvider = new CallHuawei(config);
                    break;
                case "shj":
                    _smsServiceProvider = new CallSHJ(config);
                    break;
                case "uncall":
                    //_smsServiceProvider = new CallUncallWebService(config);
                    _smsServiceProvider = new CallUncallWebApi(config);
                    break;
                default:
                    return false;
                    throw new Exception("不支持的短信或语音平台。");
                    //break;
            }
            return _smsServiceProvider.StopCallContact(phoneNumber, message);
        }
        public static string GetCallVoiceFileUrl(CallConfig config, string sessionId)
        {
            switch (config.ProviderName.ToLower())
            {
                case "dxnlkf":
                    _smsServiceProvider = new CallHuawei(config);
                    break;
                case "shj":
                    _smsServiceProvider = new CallSHJ(config);
                    break;
                case "uncall":
                    //_smsServiceProvider = new CallUncallWebService(config);
                    _smsServiceProvider = new CallUncallWebApi(config);
                    break;
                default:
                    return string.Empty;
                    throw new Exception("不支持的短信或语音平台。");
                    //break;
            }
            return _smsServiceProvider.GetCallVoiceFileUrl(sessionId);
        }

        public static T GetCallVoiceFileInfo<T>(CallConfig config, string sessionId,string caller, string callee, string startTime, string endTime) where T:class 
        {
            switch (config.ProviderName.ToLower())
            {
                case "dxnlkf":
                    _smsServiceProvider = new CallHuawei(config);
                    break;
                case "shj":
                    _smsServiceProvider = new CallSHJ(config);
                    break;
                case "uncall":
                    //_smsServiceProvider = new CallUncallWebService(config);
                    _smsServiceProvider = new CallUncallWebApi(config);
                    break;
                default:
                    return null;
            }
            return _smsServiceProvider.GetCallVoiceFileInfo<T>(sessionId, caller, callee, startTime, endTime);
        }
    }
}
