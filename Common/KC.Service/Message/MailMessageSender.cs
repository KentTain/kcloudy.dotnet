using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Runtime.Serialization;
using System.Threading;
using KC.Framework.Util;
using KC.Framework.Tenant;
using KC.Service.Util;
using KC.Service.DTO.Account;
using KC.Framework.Base;
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
    public class MailMessageSender : AbstractMessageSender
    {
        private string _subject;
        private string _body;
        private List<string> _sendToEmails;
        private IConfigApiService _configApiService;

        public MailMessageSender(Tenant tenant,
            IMessageApiService messageApiService,
            IConfigApiService configApiService,
            Microsoft.Extensions.Logging.ILogger logger)
            : base(tenant, messageApiService, logger)
        {
            _configApiService = configApiService;
        }

        protected MailMessageSender(Tenant tenant, string subject, string body, List<string> mailTo,
            IMessageApiService messageApiService,
            IConfigApiService configApiService,
            Microsoft.Extensions.Logging.ILogger logger)
            : this(tenant, messageApiService, configApiService, logger)
        {
            _subject = subject;
            _body = body;
            _sendToEmails = mailTo;
        }
        
        public override void SendAsyncToUsers(string title, string message, List<SendUserDTO> sendToUserList)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentNullException("title", "The email's subject is empty.");

            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentNullException("message", "The email's body is empty.");

            if (sendToUserList.Any(u => string.IsNullOrWhiteSpace(u.Email)))
                throw new ArgumentNullException("sendToUserList", "The email's sendTo users' email is empty.");

            var resConfig = _configApiService.GetTenantEmailConfig(Tenant);
            if (resConfig == null)
                throw new Exception("Load the email cofig from database is failed.");

            //var mailstring = string.Join(";", sendToUserList.Select(m => m.DisplayName).ToList());
            //LogUtil.LogInfo(string.Format("----------Begin to Send Mail Message: subject={0}; body={1}; user email={2}",
            //    title, message, mailstring));

            //判断是否启用邮件通知
            if (resConfig.EnableMail)
            {
                var userList = GetUserEmailListByUserList(sendToUserList);
                var mailSender = new MailMessageSender(Tenant, title, message, userList, MessasgeApiService, _configApiService, null);

                var thread = new Thread(mailSender.Send);
                thread.Start();
            }
            else
            {
                string testInboxs = GlobalConfig.AdminEmails;
                if (!string.IsNullOrWhiteSpace(testInboxs))
                {
                    var inboxArray = testInboxs.Split(',');
                    var testAdresss = inboxArray.Select(m => new MailAddress(m, m)).ToList();

                    //_subject = "(测试邮件)" + title;
                    //_mailTo = testAdresss;

                    var mailSender = new MailMessageSender(Tenant, "(测试邮件)" + title, message, inboxArray.ToList(), MessasgeApiService, _configApiService, null); ;

                    var thread = new Thread(mailSender.Send);
                    thread.Start();

                    var comment = string.Format("邮件通知未启用,邮件将发送给配置的测试邮箱：【{0}】", inboxArray);
                    LogUtil.LogInfo(comment);
                }
            }
        }

        public override void SendToUsers(string title, string message, List<SendUserDTO> sendToUserList)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentNullException("title", "The email's subject is empty.");

            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentNullException("message", "The email's body is empty.");

            if (sendToUserList.Any(u => string.IsNullOrWhiteSpace(u.Email)))
                throw new ArgumentNullException("sendToUserList", "The email's sendTo users' email is empty.");

            var resConfig = _configApiService.GetTenantEmailConfig(Tenant);
            if (resConfig == null)
                throw new Exception("Load the email cofig from database is failed.");

            //var mailstring = string.Join(";", sendToUserIds.Select(m => m).ToList());
            //LogUtil.LogInfo(string.Format("----------Begin to Send Mail Message: subject={0}; body={1}; user email={2}",
            //    title, message, mailstring));

            //判断是否启用邮件通知
            if (resConfig.EnableMail)
            {
                var userList = GetUserEmailListByUserList(sendToUserList);
                SendMail(Tenant, title, message, userList);
            }
            else
            {
                string testInboxs = GlobalConfig.AdminEmails;
                if (!string.IsNullOrWhiteSpace(testInboxs))
                {
                    var inboxArray = testInboxs.Split(',');
                    var testAdresss = inboxArray.Select(m => new MailAddress(m)).ToList();

                    //_subject = "(测试邮件)" + title;
                    //_mailTo = testAdresss;

                    SendMail(Tenant, title, message, inboxArray.ToList());

                    var comment = string.Format("邮件通知未启用,邮件将发送给配置的测试邮箱：【{0}】", inboxArray);
                    LogUtil.LogInfo(comment);
                }
            }
        }

        public void Send()
        {
            SendMail(this.Tenant, this._subject, this._body, this._sendToEmails);
        }

        private void SendMail(Tenant tenant, string subject, string body, List<string> mails)
        {
            try
            {
                //var mailstring = string.Join(";", mails.Select(m => m).ToList());
                //LogUtil.LogInfo(string.Format("----------Begin to Send Mail: subject={0}; body={1}; user email={2}",
                //    subject, body, mailstring));

                var config = _configApiService.GetTenantEmailConfig(tenant);
                EmailUtil.Send(config, subject, body, mails);

                AddMessageLogs(subject, body, mails.Select(m => m).ToList(), null,
                    MessageTemplateType.EmailMessage, true, "异步调用邮件服务(MailMessageSender)发送成功。");

                //LogUtil.LogInfo("----End to Send Email.");
            }
            catch (Exception ex)
            {
                try
                {
                    LogUtil.LogError("异步调用邮件服务失败：CFW.Web.Message.MailMessageSender--" + ex.Message, ex.StackTrace);

                    EmailUtil.SendAdministratorMail("异步调用邮件服务失败",
                                    string.Format(
                                        "异步调用邮件服务失败：CFW.Web.Message.MailMessageSender--。" + Environment.NewLine +
                                        "错误消息：{0}；消息详情：{1}", ex.Message, ex.StackTrace));

                    AddMessageLogs(subject, body, mails.Select(m => m).ToList(), null,
                        MessageTemplateType.EmailMessage, false, "异步调用邮件服务(MailMessageSender)发送失败：" + ex.Message + Environment.NewLine + ex.StackTrace);
                }
                catch
                {

                }
            }
        }
    }
}
