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
    public class SendEmailJob : WorkerRoleJobBase
    {
        public const string THREAD_NAME = "KC.Job.Basic.Jobs.SendEmailJob";
        public const int DEFAULT_SLEEP_TIME = 60 * 1000;//每60秒执行一次

        private IConfigApiService _configService => ServiceProvider.GetService<IConfigApiService>();

        private IJobService _jobService { get { return ServiceProvider.GetService<IJobService>(); } }
        private IStorageQueueService _queueService { get { return ServiceProvider.GetService<IStorageQueueService>(); } }
        private ITenantUserApiService _tenantApiService { get { return ServiceProvider.GetService<ITenantUserApiService>(); } }

        public SendEmailJob(ThreadStatusUpdater threadStatusUpdater, string workerRoleId, IServiceProvider serviceProvider)
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

            bool success = _queueService.ProcessEmailQueue(
                result =>
                {
                    try
                    {
                        var emailInfo = result;

                        LogUtil.LogDebug(THREAD_NAME,
                            string.Format("--tenant:{0} begin to send email message: {1} to Email address: {2}",
                                emailInfo.Tenant, emailInfo.EmailBody, emailInfo.SendTo));

                        var tenant = _tenantApiService.GetTenantByName(emailInfo.Tenant).Result;
                        if (tenant == null)
                        {
                            LogUtil.LogError(THREAD_NAME, " tenant is null.");
                            return QueueActionType.KeepQueueAction;
                        }

                        var config = _configService.GetTenantEmailConfig(tenant);
                        if (config == null)
                        {
                            LogUtil.LogError(THREAD_NAME, " Email Config is null.");
                            return QueueActionType.KeepQueueAction;
                        }

                        var isSuccess = EmailUtil.Send(config, emailInfo.EmailTitle, emailInfo.EmailBody,
                            emailInfo.SendTo, emailInfo.CcTo);

                        LogUtil.LogDebug(THREAD_NAME,
                            string.Format("--tenant:{0} end to send email message.", emailInfo.Tenant));
                        if (isSuccess)
                            return QueueActionType.DeleteAfterExecuteAction;
                        else
                            return QueueActionType.KeepQueueAction;
                    }
                    catch (Exception ex)
                    {
                        LogUtil.LogError(THREAD_NAME + " throw exception: " + ex.Message, ex.StackTrace);
                        return QueueActionType.FailedRepeatActon;
                    }
                },
                failResult =>
                {
                    LogUtil.LogInfo(THREAD_NAME, " fail to send email message: " + failResult.GetQueueObjectXml() + Environment.NewLine + ". Email address: " + failResult.SendTo);
                    var error = new QueueErrorMessageTable()
                    {
                        Tenant = failResult.Tenant,
                        SourceFrom = typeof(EmailInfo).FullName,
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
