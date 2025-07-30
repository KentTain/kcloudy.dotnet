using System;
using System.Collections.Generic;
using System.Threading;
using KC.Component.Base;
using KC.Framework.Util;
using KC.Job.Core;
using KC.Job.Services;
using KC.Model.Component.Queue;
using KC.Model.Component.Table;
using KC.Service.Component;
using KC.Service.Util;
using KC.Service.WebApiService.Business;
using Microsoft.Extensions.DependencyInjection;

namespace KC.Job.Basic.Jobs
{
    public class SendSmsJob : WorkerRoleJobBase
    {
        public const string THREAD_NAME = "KC.Job.Basic.Jobs.SendSmsJob";
        public const int DEFAULT_SLEEP_TIME = 1 * 1000;//每1秒执行一次

        private IJobService _jobService { get { return ServiceProvider.GetService<IJobService>(); } }
        private IStorageQueueService _queueService { get { return ServiceProvider.GetService<IStorageQueueService>(); } }
        private ITenantUserApiService _tenantApiService { get { return ServiceProvider.GetService<ITenantUserApiService>(); } }
        private IConfigApiService _configService => ServiceProvider.GetService<IConfigApiService>();

        public SendSmsJob(ThreadStatusUpdater threadStatusUpdater, string workerRoleId, IServiceProvider serviceProvider)
            : base(threadStatusUpdater, workerRoleId, THREAD_NAME, DEFAULT_SLEEP_TIME, serviceProvider)
        {
        }

        protected override bool? RunOneJob()
        {
            if (_queueService == null)
                return true;

            //LogUtil.LogInfo(string.Format("Thread Id={0}： Begin to call job: {1}.........JobService is null? {2}，QueueService is null? {3}，TenantApiService is null? {4}，ConfigApiService is null? {5}",
            //    Thread.CurrentThread.ManagedThreadId, 
            //    THREAD_NAME, 
            //    _jobService == null, 
            //    _queueService == null, 
            //    _tenantApiService == null,
            //    _configService == null));

            bool success = _queueService.ProcessSmsQueue(
                result =>
                {
                    try
                    {
                        var smsInfo = result;

                        LogUtil.LogDebug(THREAD_NAME,
                            string.Format("--tenant:{0} begin to send sms message: {1}. Phone number: {2}. Sms Type: {3}.",
                                smsInfo.Tenant, smsInfo.SmsContent, string.Join(",", smsInfo.Phone), smsInfo.Type));

                        var tenant = _tenantApiService.GetTenantByName(smsInfo.Tenant).Result;
                        if (tenant == null )
                        {
                            //LogUtil.LogError(THREAD_NAME, " tenant is null.");
                            return QueueActionType.KeepQueueAction;
                        }

                        var config = _configService.GetTenantSmsConfig(tenant);
                        if (config == null)
                        {
                            LogUtil.LogError(THREAD_NAME, "  Sms config is null.");
                            return QueueActionType.KeepQueueAction;
                        }

                        var message = SmsUtil.Send(config, smsInfo.Phone, smsInfo.SmsCode, smsInfo.SmsContent, smsInfo.Type);

                        LogUtil.LogDebug(THREAD_NAME,
                            string.Format("--tenant:{0} end to send sms message.", smsInfo.Tenant));

                        var isSuccess = string.IsNullOrWhiteSpace(message);
                        if (isSuccess)
                            return QueueActionType.DeleteAfterExecuteAction;
                        else
                            return QueueActionType.FailedRepeatActon;
                    }
                    catch (Exception ex)
                    {
                        LogUtil.LogError(THREAD_NAME + " throw exception: " + ex.Message, ex.StackTrace);
                        return QueueActionType.FailedRepeatActon;
                    }
                },
                failResult =>
                {
                    LogUtil.LogError(THREAD_NAME, " fail to send sms message: " + failResult.GetQueueObjectXml() + Environment.NewLine + ". Phone number: " + string.Join(",", failResult.Phone));
                    var error = new QueueErrorMessageTable()
                    {
                        Tenant = failResult.Tenant,
                        SourceFrom = typeof(SmsInfo).FullName,
                        QueueType = failResult.QueueType.ToString(),
                        QueueName = failResult.QueueName,
                        QueueMessage = failResult.GetQueueObjectXml(),
                        ErrorMessage = failResult.ErrorMessage,
                        ErrorFrom = THREAD_NAME
                    };

                    if (_jobService == null)
                        return;

                    _jobService.AddQueueErrorMessage(error);
                    //var parserObject = new EmailInfo(error.QueueMessage);
                });

            //LogUtil.LogInfo("End to call job: " + THREAD_NAME + ".................");
            return success;
        }

        public void SetConversionAuthToken()
        {
        }
    }
}
