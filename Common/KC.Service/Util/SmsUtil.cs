using System;
using System.Collections.Generic;
using System.Linq;
using KC.Framework.Extension;
using KC.Service.Base;
using KC.Service.Message;
using KC.Framework.Base;
using KC.Framework.Util;

namespace KC.Service.Util
{
    public static class SmsUtil
    {
        public static string Send(SmsConfig config, List<long> mobiles, string tempCode, string msgContent, SmsType type = SmsType.Notice)
        {
            if (mobiles == null || !mobiles.Any())
            {
                return "没有接收信息的手机号";
            }

            if (string.IsNullOrWhiteSpace(msgContent))
            {
                return "没有信息内容";
            }

            if (config == null)
            {
                LogUtil.LogError("短信服务未配置或未启用", "没有配置创蓝类型的短信平台");
                return "短信服务未配置或未启用";
            }

            ISmsService _smsServiceProvider;
            switch (config.ProviderName.ToLower())
            {
                case "cl":
                    _smsServiceProvider = new SmsCL(config);
                    break;
                case "ali":
                    _smsServiceProvider = new SmsAli(config);
                    break;
                default:
                    _smsServiceProvider = new SmsGeneral(config);
                    break;
            }

            return _smsServiceProvider.Send(mobiles, tempCode, msgContent);
        }
        public static string SendVoice(SmsConfig config, List<long> mobiles, string tempCode, string msgContent, SmsType type = SmsType.Notice)
        {
            if (mobiles == null || !mobiles.Any())
            {
                return "没有接收信息的手机号";
            }

            if (string.IsNullOrWhiteSpace(msgContent))
            {
                return "没有信息内容";
            }

            if (config == null)
            {
                LogUtil.LogError("短信服务未配置或未启用", "没有配置创蓝类型的短信平台");
                return "短信服务未配置或未启用";
            }

            ISmsService smsServiceProvider;
            switch (config.ProviderName.ToLower())
            {
                case "cl":
                    smsServiceProvider = new SmsCL(config);
                    break;
                case "jm":
                    smsServiceProvider = new SmsGeneral(config);
                    break;
                default:
                    return "服务商不支持语音验证码服务";
                    //break;
            }

            return smsServiceProvider.SendVoice(mobiles, tempCode, msgContent);
        }
        
    }
}
