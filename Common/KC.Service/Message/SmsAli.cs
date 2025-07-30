using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Exceptions;
using Aliyun.Acs.Core.Http;
using Aliyun.Acs.Core.Profile;
using KC.Common;
using KC.Common.ToolsHelper;
using KC.Framework.Extension;
using KC.Framework.Util;
using KC.Service.Base;

namespace KC.Service.Message
{
    /// <summary>
    /// 阿里平台
    /// </summary>
    public class SmsAli : AbstractSms
    {
        private const string ServiceName = "KC.Service.Account.Message.SmsAli";
        public SmsAli(SmsConfig config)
            : base(config)
        {
            
        }

        private DefaultAcsClient GetDefaultAcsClient()
        {
            IClientProfile profile = DefaultProfile.GetProfile(Config.SmsUrl, Config.UserAccount, Config.Password);
            return new DefaultAcsClient(profile);
        }

        public override string Send(List<long> mobiles, string tempCode, string msgContent)
        {
            
            try
            {
                var client = GetDefaultAcsClient();
                var request = new CommonRequest();
                request.Method = MethodType.POST;
                request.Domain = "dysmsapi.aliyuncs.com";
                request.Version = "2017-05-25";
                request.Action = "SendSms";
                // request.Protocol = ProtocolType.HTTP;

                request.AddQueryParameters("PhoneNumbers", mobiles.ToCommaSeparatedStringByFilter(m => m.ToString()));
                request.AddQueryParameters("SignName", Config.Signature);
                request.AddQueryParameters("TemplateCode", tempCode);
                request.AddQueryParameters("TemplateParam", msgContent);
                request.AddQueryParameters("OutId", "");

                var response = client.GetCommonResponse(request);
                var content = System.Text.Encoding.Default.GetString(response.HttpResponse.Content);
                var result = SerializeHelper.FromJson<AliSendSmsResponse>(content);
                if (result.Code != "OK")
                {
                    return "短信服务商（阿里云）返回的结果出错，错误消息：" + content;
                }

                return null;
            }
            catch (ServerException ex)
            {
                LogUtil.LogError(ex.Message, ex.StackTrace);
                return ex.Message;
            }
            catch (ClientException ex)
            {
                LogUtil.LogError(ex.Message, ex.StackTrace);
                return ex.Message;
            }
            catch (Exception ex)
            {
                string message = "短信服务商（阿里云）调用服务({0})的方法({1})操作{2}。" +
                                 Environment.NewLine + "调用路径：{3}。" +
                                 Environment.NewLine + "错误消息为：{4}";
                message = string.Format(message, ServiceName, MethodBase.GetCurrentMethod().Name, "失败", Config.SmsUrl, ex.Message);
                LogUtil.LogError(message, ex.StackTrace);
                return message;
            }
        }

        public override string SendVoice(List<long> mobiles, string tempCode, string msgContent)
        {

            try
            {
                var client = GetDefaultAcsClient();
                var request = new CommonRequest();
                request.Method = MethodType.POST;
                request.Domain = "dysmsapi.aliyuncs.com";
                request.Version = "2017-05-25";
                request.Action = "SendSms";
                // request.Protocol = ProtocolType.HTTP;

                request.AddQueryParameters("PhoneNumbers", mobiles.ToCommaSeparatedStringByFilter(m => m.ToString()));
                request.AddQueryParameters("SignName", Config.Signature);
                request.AddQueryParameters("TemplateCode", tempCode);
                request.AddQueryParameters("TemplateParam", msgContent);
                request.AddQueryParameters("OutId", "");

                var response = client.GetCommonResponse(request);
                var content = System.Text.Encoding.Default.GetString(response.HttpResponse.Content);
                var result = SerializeHelper.FromJson<AliSendSmsResponse>(content);
                if (result.Code != "OK")
                {
                    return "短信服务商（阿里云）返回的结果出错，错误消息：" + content;
                }

                return null;
            }
            catch (ServerException ex)
            {
                LogUtil.LogError(ex.Message, ex.StackTrace);
                return ex.Message;
            }
            catch (ClientException ex)
            {
                LogUtil.LogError(ex.Message, ex.StackTrace);
                return ex.Message;
            }
            catch (Exception ex)
            {
                string message = "短信服务商（阿里云）调用服务({0})的方法({1})操作{2}。" +
                                 Environment.NewLine + "调用路径：{3}。" +
                                 Environment.NewLine + "错误消息为：{4}";
                message = string.Format(message, ServiceName, MethodBase.GetCurrentMethod().Name, "失败", Config.SmsUrl, ex.Message);
                LogUtil.LogError(message, ex.StackTrace);
                return message;
            }
        }

        public class AliSendSmsResponse
        {
            public string Message { get; set; }
            public string RequestId { get; set; }
            public string BizId { get; set; }
            public string Code { get; set; }
        }
    }
}
