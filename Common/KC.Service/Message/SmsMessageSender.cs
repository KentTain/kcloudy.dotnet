using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using KC.Framework.Util;
using KC.Framework.Base;
using KC.Framework.Tenant;
using KC.Service.Util;
using KC.Service.DTO.Account;
using KC.Service.WebApiService.Business;
using KC.Service.Enums.Message;
using KC.Service.DTO.Message;

namespace KC.Service.Message
{
    /// <summary>
    /// 短信服务
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    public class SmsMessageSender : AbstractMessageSender
    {
        private string _subject;
        private string _body;
        private List<long> _sendToPhoneNumbers;
        private IConfigApiService _configApiService;

        public SmsMessageSender(Tenant tenant,
            IMessageApiService messageApiService,
            IConfigApiService configApiService,
            Microsoft.Extensions.Logging.ILogger logger)
            : base(tenant, messageApiService, logger)
        {
            _configApiService = configApiService;
        }

        protected SmsMessageSender(Tenant tenant, string subject, string body, List<long> mailTo,
            IMessageApiService messageApiService,
            IConfigApiService configApiService,
            Microsoft.Extensions.Logging.ILogger logger)
            : this(tenant, messageApiService, configApiService, logger)
        {
            _subject = subject;
            _body = body;
            _sendToPhoneNumbers = mailTo;
        }

        public override void SendAsyncToUsers(string title, string message, List<SendUserDTO> sendToUserList)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentNullException("title", "The sms's subject is empty.");

            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentNullException("message", "The sms's body is empty.");

            if (sendToUserList.Any(u => string.IsNullOrWhiteSpace(u.PhoneNumber)))
                throw new ArgumentNullException("sendToUserList", "The sms's sendTo users' phone is empty.");

            //var mailstring = string.Join(";", sendToUserList.Select(m => m.DisplayName).ToList());
            //LogUtil.LogInfo(string.Format("----------Begin to Send Sms Message: subject={0}; body={1}; user email={2}",
            //    title, message, mailstring));

            var userList = GetUserPhoneListByUserList(sendToUserList).Select(long.Parse).ToList();
            var mailSender = new SmsMessageSender(Tenant, title, message, userList, MessasgeApiService, _configApiService, null);

            var thread = new Thread(mailSender.Send);
            thread.Start();
        }

        public override void SendToUsers(string title, string message, List<SendUserDTO> sendToUserList)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentNullException("title", "The sms's subject is empty.");

            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentNullException("message", "The sms's body is empty.");

            if (sendToUserList.Any(u => string.IsNullOrWhiteSpace(u.PhoneNumber)))
                throw new ArgumentNullException("sendToUserList", "The sms's sendTo users' phone is empty.");

            var config = _configApiService.GetTenantSmsConfig(Tenant);
            if(config == null)
                throw new Exception("The sms's config is null.");

            //var mailstring = string.Join(";", sendToUserIds.Select(m => m).ToList());
            //LogUtil.LogInfo(string.Format("----------Begin to Send Sms Message: subject={0}; body={1}; user email={2}",
            //    title, message, mailstring));

            var userList = GetUserPhoneListByUserList(sendToUserList).Select(long.Parse).ToList();
            SendSms(Tenant, title, message, userList);
        }

        public void Send()
        {
            SendSms(Tenant, this._subject, this._body, this._sendToPhoneNumbers);
        }

        /// <summary>
        /// 默认只发送营销短信
        /// </summary>
        private void SendSms(Tenant tenant, string subject, string body, List<long> mailTo)
        {
            try
            {
                //var mailstring = string.Join(";", mailTo.Select(m => m.ToString()).ToList());
                //LogUtil.LogInfo(string.Format("----------Begin to Send Sms: subject={0}; body={1}; user email={2}",
                //    subject, body, mailstring));

                var config = _configApiService.GetTenantSmsConfig(tenant);
                var msg = SmsUtil.Send(config, mailTo, subject, body);
                if (string.IsNullOrWhiteSpace(msg))
                {
                    AddMessageLogs(subject, body, mailTo.Select(m => m.ToString()).ToList(), null,
                        MessageTemplateType.SmsMessage, true, "异步调用短信服务(SmsMessageSender)发送成功。");
                }
                else
                {
                    EmailUtil.SendAdministratorMail("异步调用短信服务失败", string.Format(
                        "异步调用短信服务失败：CFW.Web.Message.SmsMessageSender--。" + Environment.NewLine + "错误消息：{0}", msg));

                    AddMessageLogs(subject, body, mailTo.Select(m => m.ToString()).ToList(), null,
                        MessageTemplateType.SmsMessage, false, "异步调用短信服务(SmsMessageSender)发送失败：" + msg);
                }


                //LogUtil.LogInfo("----End to Send Sms.");
            }
            catch (Exception ex)
            {
                try
                {
                    LogUtil.LogError("异步调用短信服务失败：CFW.Web.Message.SmsMessageSender--" + ex.Message, ex.StackTrace);

                    EmailUtil.SendAdministratorMail("异步调用短信服务失败",
                        string.Format(
                            "异步调用短信服务失败：CFW.Web.Message.SmsMessageSender--。" + Environment.NewLine +
                            "错误消息：{0}；消息详情：{1}", ex.Message, ex.StackTrace));

                    AddMessageLogs(subject, body, mailTo.Select(m => m.ToString()).ToList(), null,
                        MessageTemplateType.SmsMessage, false, "异步调用短信服务(SmsMessageSender)发送失败：" + ex.Message + Environment.NewLine + ex.StackTrace);
                }
                catch
                {
                }
            }
        }
    }
}
