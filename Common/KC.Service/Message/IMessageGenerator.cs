using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using KC.Common;
using KC.Framework.Tenant;
using KC.Service.WebApiService.Business;
using KC.Service.DTO.Message;
using Microsoft.Extensions.Logging;

namespace KC.Service.Message
{
    public interface IMessageGenerator
    {
        /// <summary>
        /// 获取消息生成器中的可替换参数列表
        /// </summary>
        /// <returns></returns>
        List<string> GetReplaceParameters();

        /// <summary>
        /// 获取消息内容对象
        /// </summary>
        /// <param name="messageCode">消息模板代码</param>
        /// <param name="replaceDict">可替换参数对照字典表（key：可替换参数；value：替换参数值）
        ///     例如：'code':'1001', 'name':'test'</param>
        /// <returns></returns>
        List<MessageDTO> GenerateMessageList(string messageCode, Dictionary<string, string> replaceDict);
    }

    public class CommonMessageGenerator : IMessageGenerator
    {
        protected Tenant Tenant { get; private set; }

        public char Prefix_ReplaceChar = '{';
        public char Suffix_ReplaceChar = '}';
        protected ILogger Logger { get; set; }
        protected IMessageApiService MessasgeApiService;

        /// <summary>
        /// 可替换参数列表
        /// </summary>
        protected List<string> ReplaceParameters { get; set; }

        /// <summary>
        /// 可替换参数对照字典表（key：可替换参数；value：替换参数值）
        ///     例如：'code':'1001', 'name':'test'
        /// </summary>
        protected Dictionary<string, string> ReplaceContent { get; set; }

        public CommonMessageGenerator(
            Tenant tenant,
            IMessageApiService messageApiService,
            ILogger<CommonMessageGenerator> logger)
        {
            Tenant = tenant;
            MessasgeApiService = messageApiService;
            Logger = logger;
        }

        /// <summary>
        /// 获取消息生成器中的可替换参数列表
        /// </summary>
        /// <returns></returns>
        public virtual List<string> GetReplaceParameters()
        {
            return ReplaceParameters;
        }

        /// <summary>
        /// 获取消息内容对象
        /// </summary>
        /// <param name="messageCode">消息模板代码</param>
        /// <param name="replaceDict">可替换参数对照字典表（key：可替换参数；value：替换参数值）
        ///     例如：'code':'1001', 'name':'test'</param>
        /// <returns></returns>
        public virtual List<MessageDTO> GenerateMessageList(string messageCode, Dictionary<string, string> replaceDict)
        {
            var messageClass = MessasgeApiService.GetMessageClassByCode(messageCode);
            var result = new List<MessageDTO>();
            if (messageClass == null)
            {
                Logger.LogError(string.Format("不支持的消息类型（MessageCode={0}）", messageCode));
                return result;
            }

            ReplaceContent = replaceDict;
            ReplaceParameters = messageClass.ReplaceParametersString
                .Split(',')
                .Where(m => !string.IsNullOrEmpty(m))
                .Select(m => Prefix_ReplaceChar + m + Suffix_ReplaceChar)
                .ToList();

            foreach (var template in messageClass.MessageTemplates
                .Where(o => !o.IsDeleted && !string.IsNullOrEmpty(o.Content)))
            {
                var message = new MessageDTO();
                message.MessageClassCode = messageClass.Code;
                message.MessageClassName = messageClass.Name;
                message.TemplateId = template.Id;
                message.TemplateType = template.TemplateType;
                message.TemplateId = template.Id;
                message.Name = template.Name;
                message.Subject = template.Subject;
                message.Content = SerializeHelper.ToJson(replaceDict);
                message.Content = GetMessageContentWithDict(template.Content, replaceDict);
            }

            return result;
        }

        /// <summary>
        /// 根据可替换参数列表，提取{source}对象与可替换参数相同属性的值，保存至可替换参数对照字典表中（ReplaceContent）
        /// </summary>
        /// <param name="source"></param>
        protected void SetReplaceContent(object source)
        {
            var replaceParameter = GetReplaceParameters();
            foreach (var item in replaceParameter)
            {
                try
                {
                    var attribute = item.TrimStart(Prefix_ReplaceChar).TrimEnd(Suffix_ReplaceChar);
                    Type type = source.GetType();
                    PropertyInfo field = type.GetProperty(attribute);
                    var attributeValue = field.GetValue(source);
                    var value = string.Empty;
                    if (attributeValue != null)
                    {
                        value = attributeValue.ToString();
                    }
                    if (!ReplaceContent.ContainsKey(item))
                    {
                        ReplaceContent.Add(item, value);
                    }
                    else
                    {
                        ReplaceContent[item] = value;
                    }
                }
                catch (Exception ex)
                {
                    ReplaceContent[item] = string.Empty;
                    Logger.LogError(string.Format("调用KC.Service.Message.AbstractMessageGenerator中的SetReplaceContent方法体报错,参数如下：source={0}{1}错误消息：{2}", SerializeHelper.ToJson(source), Environment.NewLine, ex.Message));
                }

            }
        }

        /// <summary>
        /// 将消息模板内容，替换为传入替换字典类内容
        /// </summary>
        /// <param name="messageContent"></param>
        /// <returns></returns>
        private string GetMessageContentWithDict(string messageContent, Dictionary<string, string> replaceDictionay)
        {
            //TODO: 使用模板引擎
            var result = messageContent;
            if (replaceDictionay != null && replaceDictionay.Any())
            {
                result = replaceDictionay.Aggregate(messageContent,
                    (current, keyValue) => current.Replace(Prefix_ReplaceChar + keyValue.Key + Suffix_ReplaceChar, keyValue.Value));
            }

            return result;
        }
    }
}
