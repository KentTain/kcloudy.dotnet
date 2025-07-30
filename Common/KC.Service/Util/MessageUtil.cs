using System;
using System.Collections.Generic;
using System.Linq;
using KC.Framework.Extension;
using KC.Framework.Util;
using KC.Framework.Tenant;
using KC.Service.Message;
using KC.Service.DTO.Account;
using KC.Service.WebApiService.Business;
using KC.Service.Enums.Message;
using KC.Service.DTO.Message;

namespace KC.Service.Util
{
    public class MessageUtil
    {
        private readonly IMessageGenerator _handler;
        private readonly IConfigApiService _configApiService;
        private readonly IMessageApiService _messageApiService;
        private readonly Microsoft.Extensions.Logging.ILogger _logger;
        public MessageUtil(IMessageGenerator handler,
            IMessageApiService messageApiService,
            IConfigApiService configApiService,
            Microsoft.Extensions.Logging.ILogger<MessageUtil> logger)
        {
            _handler = handler;
            _configApiService = configApiService;
            _messageApiService = messageApiService;
            _logger = logger;
        }

        /// <summary>
        /// 获取消息模板生成器所支持的替换参数
        /// </summary>
        /// <returns>替换参数，例如：{DisplayName}、{Email}</returns>
        public List<string> GetReplaceParameters()
        {
            return _handler.GetReplaceParameters();
        }

        #region 根据消息模板（邮件、短信、站内信）发送消息
        /// <summary>
        /// 根据消息模板（邮件、短信、站内信）发送消息
        /// </summary>
        /// <param name="tenant"></param>
        /// <param name="sendToUsers"></param>
        /// <param name="insideSendAll"></param>
        /// <returns></returns>
        public bool SendMessage(Tenant tenant, string messageCode, Dictionary<string, string> replaceDict, List<SendUserDTO> sendToUsers)
        {
            try
            {
                var messages = _handler.GenerateMessageList(messageCode, replaceDict);
                foreach (var template in messages)
                {
                    IMessageSender sender = null;
                    switch (template.TemplateType)
                    {
                        case MessageTemplateType.EmailMessage:
                            sender = new MailMessageSender(tenant, _messageApiService, _configApiService, _logger);
                            sender.SendToUsers(template.Subject, template.Content, sendToUsers);
                            LogUtil.LogDebug(string.Format("--EmailMessage: subject={0}, content={1}, SendTo={2}.",
                                template.Subject, template.Content,
                                sendToUsers.ToCommaSeparatedStringByFilter(m => m.Email)));
                            break;
                        case MessageTemplateType.InsideMessage:
                            sender = new InsideMessageSender(tenant, _messageApiService, _logger);
                            sender.SendToUsers(template.Subject, template.Content, sendToUsers);
                            LogUtil.LogDebug(string.Format("--InsideMessage: subject={0}, content={1}, SendTo={2}.",
                                template.Subject, template.Content,
                                sendToUsers.ToCommaSeparatedStringByFilter(m => m.DisplayName)));
                            break;
                        case MessageTemplateType.SmsMessage:
                            sender = new SmsMessageSender(tenant, _messageApiService, _configApiService, _logger);
                            sender.SendToUsers(template.Subject, template.Content, sendToUsers);
                            LogUtil.LogDebug(string.Format("--SmsMessage: subject={0}, content={1}, SendTo={2}.",
                                template.Subject, template.Content,
                                sendToUsers.ToCommaSeparatedStringByFilter(m => m.PhoneNumber)));
                            break;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                LogUtil.LogError("异步消息服务失败：KC.Service.Util.MessageUtil--" + ex.Message, ex.StackTrace);
                return false;
            }
        }

        /// <summary>
        /// 根据消息模板（邮件、短信、站内信）发送消息
        /// </summary>
        /// <param name="tenant"></param>
        /// <param name="sendToUsers"></param>
        /// <param name="insideSendAll"></param>
        /// <returns></returns>
        public bool SendAsyncMessage(Tenant tenant, string messageCode, Dictionary<string, string> replaceDict, List<SendUserDTO> sendToUsers)
        {
            try
            {
                var messages = _handler.GenerateMessageList(messageCode, replaceDict);
                foreach (var template in messages)
                {
                    IMessageSender sender = null;
                    switch (template.TemplateType)
                    {
                        case MessageTemplateType.EmailMessage:
                            sender = new MailMessageSender(tenant, _messageApiService, _configApiService, _logger);
                            sender.SendAsyncToUsers(template.Subject, template.Content, sendToUsers);
                            LogUtil.LogDebug(string.Format("--EmailMessage: subject={0}, content={1}, SendTo={2}.",
                                template.Subject, template.Content,
                                sendToUsers.ToCommaSeparatedStringByFilter(m => m.Email)));
                            break;
                        case MessageTemplateType.InsideMessage:
                            sender = new InsideMessageSender(tenant, _messageApiService, _logger);
                            sender.SendAsyncToUsers(template.Subject, template.Content, sendToUsers);
                            LogUtil.LogDebug(string.Format("--InsideMessage: subject={0}, content={1}, SendTo={2}.",
                                template.Subject, template.Content,
                                sendToUsers.ToCommaSeparatedStringByFilter(m => m.DisplayName)));
                            break;
                        case MessageTemplateType.SmsMessage:
                            sender = new SmsMessageSender(tenant, _messageApiService, _configApiService, _logger);
                            sender.SendAsyncToUsers(template.Subject, template.Content, sendToUsers);
                            LogUtil.LogDebug(string.Format("--SmsMessage: subject={0}, content={1}, SendTo={2}.",
                                template.Subject, template.Content,
                                sendToUsers.ToCommaSeparatedStringByFilter(m => m.PhoneNumber)));
                            break;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                LogUtil.LogError("异步消息服务失败：KC.Service.Util.MessageUtil--" + ex.Message, ex.StackTrace);
                return false;
            }
        }
        #endregion
    }
}
