using System;
using System.Collections.Generic;
//using System.Messaging;
using System.Threading;
using System.Threading.Tasks;
using KC.Component.Base;
using KC.Framework.Extension;
using KC.Model.Component.Queue;
using KC.Service.Component;
using KC.UnitTest;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;

namespace KC.UnitTest.Storage.Component
{
    /// <summary>
    /// 通过设置Web.config中的：BlobStorage节点，控制文件存储的方式
    ///     FileSystem：本地存储
    ///     azure：微软云Azure存储
    ///     cmb：招行云/AWS中的存储
    /// </summary>
    
    public class AllQueueTest : KC.UnitTest.Storage.StorageTestBase
    {
        private static IStorageQueueService storageQueueService;
        private static IStorageQueueService devdbStorageQueueService;
        private ILogger _logger;
        public AllQueueTest(CommonFixture data)
            : base(data)
        {
            _logger = LoggerFactory.CreateLogger(nameof(AllQueueTest));
        }

        protected override void SetUp()
        {
            base.SetUp();

            storageQueueService = Services.BuildServiceProvider().GetService<IStorageQueueService>();//返回调用者

            InjectTenant(TestTenant);
            Services.AddScoped<ITopicService, TopicService>();
            var devdbTopicService = Services.BuildServiceProvider().GetService<ITopicService>();

            devdbStorageQueueService = Services.BuildServiceProvider().GetService<IStorageQueueService>();//返回调用者
        }

        [Xunit.Fact]
        public void InserEmailQueue_Test()
        {
            var email = new EmailInfo()
            {
                Tenant = TestTenant.TenantName,
                SendTo = new List<string>() { "chenlinfei929@126.com" },
                EmailTitle = "[测试邮件]测试EmailQueue",
                EmailBody = "[测试邮件]测试EmailQueue",
                SendFrom = "tianchangjun@cfwin.com",
                IsBodyHtml = false,
            };

            var result = storageQueueService.InsertEmailQueue(email);
            Assert.True(result);

            //email.EmailTitle = email.EmailTitle + "--Tenant: DevDB";
            //email.EmailBody = email.EmailBody + "--Tenant: DevDB";
            //var result1 = BuyStorageQueueService.InserEmailQueue(email);
            //Assert.True(result1);
        }

        [Xunit.Fact]
        public void ProcessEmailQueue_Test()
        {
            var r = Parallel.For(0, 3, act =>
            {
                var threadId = Thread.CurrentThread.ManagedThreadId;
                var mailOperateResult = storageQueueService.ProcessEmailQueue(callback =>
                {
                    var emailResult = callback;
                    if (emailResult != null)
                    {
                        _logger.LogInformation(
                            string.Format("---email object likes threadId ({0}) send emailId ({1}) to users: {2}",
                                threadId, emailResult.Id, emailResult.SendTo.ToCommaSeparatedString()));
                    }
                    return QueueActionType.DeleteAfterExecuteAction;
                }, failCallback =>
                {
                    _logger.LogInformation("---get email is failed. " + failCallback.ErrorMessage);
                });

                Assert.True(mailOperateResult);
            });
        }

        [Xunit.Fact]
        public async Task ProcessEmailQueueAsync_Test()
        {
            //var r = Parallel.For(0, 3, act =>
            //{
            var threadId = Thread.CurrentThread.ManagedThreadId;
            var mailOperateResult = await storageQueueService.ProcessEmailQueueAsync(callback =>
            {
                var emailResult = callback;
                if (emailResult != null)
                {
                    _logger.LogInformation(
                        string.Format("---email object likes threadId ({0}) send emailId ({1}) to users: {2}",
                            threadId, emailResult.Id, emailResult.SendTo.ToCommaSeparatedString()));
                }
                return QueueActionType.DeleteAfterExecuteAction;
            }, failCallback =>
            {
                _logger.LogInformation("---get email is failed. " + failCallback.ErrorMessage);
            });

            Assert.True(mailOperateResult);
            //});
        }

        [Xunit.Fact]
        public void InserSmsQueue_Test()
        {
            var sms = new SmsInfo()
            {
                Tenant = TestTenant.TenantName,
                Phone = new List<long>() { 17744949695 },
                SmsContent = "测试短信--测试SmsQueue",
            };

            var result = storageQueueService.InsertSmsQueue(sms);
            Assert.True(result);

            sms.SmsContent = sms.SmsContent + "--Tenant: DevDB";
            var result1 = devdbStorageQueueService.InsertSmsQueue(sms);
            Assert.True(result1);
        }

        [Xunit.Fact]
        public void ClearAllEmailQueue_Test()
        {
            var result = storageQueueService.ClearAllEmailQueue();
            Assert.True(result);

            var result1 = devdbStorageQueueService.ClearAllEmailQueue();
            Assert.True(result1);
        }

        [Xunit.Fact]
        public void ClearAllSmsQueue_Test()
        {
            var result = storageQueueService.ClearAllSmsQueue();
            Assert.True(result);

            var result1 = devdbStorageQueueService.ClearAllSmsQueue();
            Assert.True(result1);
        }

        //[Xunit.Fact]
        //public void Msmq_Test()
        //{
        //    var serializerSettings = new Newtonsoft.Json.JsonSerializerSettings
        //    {
        //        MaxDepth = 5,
        //        ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore,
        //        MissingMemberHandling = Newtonsoft.Json.MissingMemberHandling.Ignore,
        //        NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore,
        //        ObjectCreationHandling = Newtonsoft.Json.ObjectCreationHandling.Reuse,
        //        DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.Ignore,
        //        //ContractResolver = new CustomResolver(),
        //        Formatting = Formatting.Indented
        //    };
        //    var entity = new EmailInfo()
        //    {
        //        Tenant = TestTenant.TenantName,
        //        SendTo = new List<string>() { "chenlinfei929@126.com" },
        //        EmailTitle = "[测试邮件]测试EmailQueue",
        //        EmailBody = "[测试邮件]测试EmailQueue",
        //        SendFrom = "tianchangjun@cfwin.com",
        //        IsBodyHtml = false,
        //    };
            
        //    var queuePath = @".\private$\test";
        //    using (var mq = CreateIfNotExist(queuePath))
        //    {
        //        entity.CreatedDate = DateTime.UtcNow;
        //        var jsonObject = JsonConvert.SerializeObject(entity, serializerSettings);

        //        mq.Label = "LearningHardPrivateQueue";
        //        mq.Send(jsonObject, "Leaning Hard"); // 发送消息
        //    }
        //}

        //private MessageQueue CreateIfNotExist(string queuePath)
        //{
        //    try
        //    {
        //        if (!MessageQueue.Exists(queuePath))
        //        {
        //            var result = MessageQueue.Create(queuePath);  //创建事务性的专用消息队列
        //            //result.QueueName = QueueName;
        //            //result.DefaultPropertiesToSend = new DefaultPropertiesToSend()
        //            //{
        //            //    AttachSenderId = false,
        //            //    UseAuthentication = false,
        //            //    UseEncryption = false,
        //            //    AcknowledgeType = AcknowledgeTypes.None,
        //            //    UseJournalQueue = false
        //            //};
        //            //result.SetPermissions("Everyone", System.Messaging.MessageQueueAccessRights.FullControl);
        //            return result;
        //        }
        //        else
        //        {
        //            //设置当应用程序向消息对列发送消息时默认情况下使用的消息属性值
        //            var result = new MessageQueue(queuePath);
        //            //result.QueueName = QueueName;
        //            //result.DefaultPropertiesToSend = new DefaultPropertiesToSend()
        //            //{
        //            //    AttachSenderId = false,
        //            //    UseAuthentication = false,
        //            //    UseEncryption = false,
        //            //    AcknowledgeType = AcknowledgeTypes.None,
        //            //    UseJournalQueue = false
        //            //};
        //            //result.SetPermissions("Everyone", System.Messaging.MessageQueueAccessRights.FullControl);
        //            return result;
        //        }
        //    }
        //    catch (MessageQueueException e)
        //    {
        //        _logger.LogError(string.Format("{0}: 创建队列失败，路径为：{1}。", "CreateIfNotExist", queuePath), e.Message);

        //        throw;
        //    }
        //}
    }
}
