using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using KC.Framework.Util;
using KC.Framework.Tenant;
using KC.Service.WebApiService.Business;
using KC.Service.DTO.Account;
using KC.Framework.Base;
using KC.Service.DTO.Message;
using KC.Service.Enums.Message;

namespace KC.Service.Message
{
    /// <summary>
    /// 站内信服务
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    public class InsideMessageSender : AbstractMessageSender
    {
        private string _subject;
        private string _body;
        private List<SendUserDTO> _sendToUserList;

        public InsideMessageSender(Tenant tenant,
            IMessageApiService messageApiService,
            Microsoft.Extensions.Logging.ILogger logger)
            : base(tenant, messageApiService, logger)
        {
        }

        protected InsideMessageSender(Tenant tenant, string subject, string body, List<SendUserDTO> mailTo,
            IMessageApiService messageApiService,
            Microsoft.Extensions.Logging.ILogger logger)
            : this(tenant, messageApiService, logger)
        {
            _subject = subject;
            _body = body;
            _sendToUserList = mailTo;
        }

        public override void SendAsyncToUsers(string title, string message, List<SendUserDTO> sendToUserList)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentNullException("title", "The inside message's subject is empty.");

            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentNullException("message", "The inside message's body is empty.");

            if (sendToUserList.Any(u => string.IsNullOrWhiteSpace(u.UserId)))
                throw new ArgumentNullException("sendToUserList", "The inside message's sendTo users' id is empty.");

            //var mailstring = string.Join(";", sendToUserList.Select(m => m.DisplayName).ToList());
            //LogUtil.LogInfo(string.Format("----------Begin to Send Inside Message: subject={0}; body={1}; user email={2}",
            //    title, message, mailstring));

            var mailSender = new InsideMessageSender(Tenant, title, message, sendToUserList, MessasgeApiService, null);

            var thread = new Thread(mailSender.Send);
            thread.Start();
        }

        public override void SendToUsers(string title, string message, List<SendUserDTO> sendToUserList)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentNullException("title", "The inside message's subject is empty.");

            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentNullException("message", "The inside message's body is empty.");

            if (sendToUserList.Any(u => string.IsNullOrWhiteSpace(u.UserId)))
                throw new ArgumentNullException("sendToUserList", "The inside message's sendTo users' id is empty.");

            //var mailstring = string.Join(";", sendToUserIds.Select(m => m).ToList());
            //LogUtil.LogInfo(string.Format("----------Begin to Send Inside Message: subject={0}; body={1}; user Ids={2}",
            //    title, message, mailstring));

            SendInsideMessage(title, message, sendToUserList);
        }

        public void Send()
        {
            SendInsideMessage(this._subject, this._body, this._sendToUserList);
        }

        private void SendInsideMessage(string subject, string body, List<SendUserDTO> mailTo)
        {
            try
            {
                //var mailstring = string.Join(";", mailTo.Select(m => m).ToList());
                //LogUtil.LogInfo(string.Format("----4------Begin to Send Inside Message: subject={0}; body={1}; user email={2}",
                //    subject, body, mailstring));

                var messageClass = MessasgeApiService.GetMessageClassByName(subject);
                var typeId = 0;
                var typeName = string.Empty;
                if (messageClass != null && messageClass.MessageTemplates.Any())
                {
                    typeId = messageClass.Id;
                    typeName = messageClass.Name;
                }

                var messages = new List<MemberRemindMessageDTO>();
                foreach (var user in mailTo)
                {
                    messages.Add(new MemberRemindMessageDTO()
                    {
                        MessageTitle = subject,
                        MessageContent = body,
                        TypeId = typeId,
                        TypeName = typeName,
                        Status = MessageStatus.Unread,
                        UserId = user.UserId,
                        UserName = user.DisplayName,
                        ApplicationId = GlobalConfig.ApplicationGuid,
                        ApplicationName = GlobalConfig.ApplicationName
                    });
                }

                var result = MessasgeApiService.AddRemindMessages(messages);
                var message = result ? "异步调用站内信服务(InsideMessageSender)发送成功。" : "异步调用站内信服务(InsideMessageSender)发送失败。";
                AddMessageLogs(subject, body, mailTo.Select(m => "userId=" + m.UserId + ": " + m.DisplayName).ToList(),
                    null,
                    MessageTemplateType.InsideMessage, result, message);
                //LogUtil.LogInfo("----End to Send Inside Message.");
            }
            catch (Exception ex)
            {
                LogUtil.LogError("异步调用站内信服务失败：CFW.Web.Message.InsideMessageSender--" + ex.Message, ex.StackTrace);

                AddMessageLogs(subject, body, mailTo.Select(m => " userId = " + m.UserId + ": " + m.DisplayName).ToList(), null,
                    MessageTemplateType.InsideMessage, false, "异步调用站内信服务(InsideMessageSender)发送失败：" + ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }


    }
}
