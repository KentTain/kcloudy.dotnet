using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using KC.Framework.Extension;
using KC.Framework.Tenant;
using KC.Service.WebApiService.Business;
using KC.Service.DTO.Account;
using KC.Service.DTO.Message;
using KC.Service.Enums.Message;

namespace KC.Service.Message
{
    public interface IMessageSender
    {
        /// <summary>
        /// 发送消息给用户--同步方法
        /// </summary>
        /// <param name="title">消息标题</param>
        /// <param name="message">消息内容</param>
        /// <param name="sendToUsers">需要发送到的用户列表</param>
        void SendToUsers(string title, string message, List<SendUserDTO> sendToUsers);

        /// <summary>
        /// 发送消息给用户--异步方法
        /// </summary>
        /// <param name="title">消息标题</param>
        /// <param name="message">消息内容</param>
        /// <param name="sendToUsers">需要发送到的用户列表</param>
        void SendAsyncToUsers(string title, string message, List<SendUserDTO> sendToUsers);

        //List<UserDTO> LoadUserDefaulAll(List<string> userIds = null);
        
    }

    [Serializable]
    [DataContract(IsReference = true)]
    [KnownType(typeof(MailMessageSender))]
    [KnownType(typeof(SmsMessageSender))]
    [KnownType(typeof(InsideMessageSender))]
    public abstract class AbstractMessageSender : IMessageSender
    {
        protected Tenant Tenant { get; private set; }
        protected IMessageApiService MessasgeApiService { get; private set; }
        protected Microsoft.Extensions.Logging.ILogger Logger { get; private set; }

        protected AbstractMessageSender(Tenant tenant,
            IMessageApiService messageApiService,
            Microsoft.Extensions.Logging.ILogger logger)
        {
            Tenant = tenant;
            MessasgeApiService = messageApiService;
            Logger = logger;
        }

        public abstract void SendAsyncToUsers(string title, string message, List<SendUserDTO> sendToUserIds);

        public abstract void SendToUsers(string title, string message, List<SendUserDTO> sendToUserIds);

        protected List<string> GetUserEmailListByUserList(List<SendUserDTO> users)
        {
            var mails = users.Select(u => u.Email);

            return mails.ToList();
        }
        protected List<string> GetUserPhoneListByUserList(List<SendUserDTO> users)
        {
            return users.Select(u => u.PhoneNumber).ToList();
        }
        protected bool AddMessageLogs(string subject, string body, List<string> mailTo, List<string> cc,
            MessageTemplateType type, bool isSuccess, string message, bool isSave = true)
        {
            var log = new MessageLogDTO { Subject = subject, Content = body, ToAddresses = mailTo.ToCommaSeparatedString() };
            if (cc != null && cc.Any())
            {
                log.CCAddresses = cc.ToCommaSeparatedString();
            }
            log.Type = type;
            log.IsSuccess = isSuccess;
            log.ExceptionMessage = message;

            return MessasgeApiService.AddMessageLog(log);
        }

        //public List<UserDTO> LoadUserDefaulAll(List<string> userIds = null)
        //{
        //    return new MessageApiService(Tenant).LoadUserDefaulAll(userIds);
        //}
    }
}
