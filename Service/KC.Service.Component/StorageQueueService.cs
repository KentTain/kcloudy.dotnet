using System;
using System.Collections.Generic;
using KC.Common.FileHelper;
using KC.Framework.Util;
using KC.Framework;
using KC.Framework.Tenant;
using KC.Component.Base;
using KC.Model.Component.Table;
using KC.Component.IRepository;
using KC.Component.QueueRepository;
using System.Threading.Tasks;
using KC.Model.Component.Queue;

namespace KC.Service.Component
{
    public interface IStorageQueueService
    {
        bool InsertReportQueue(ReportInfo report);
        bool ProcessReportQueue(Func<ReportInfo, QueueActionType> callback, Action<ReportInfo> failCallback = null);
        bool ClearAllReportQueue();

        bool InsertEmailQueue(EmailInfo email);
        bool ProcessEmailQueue(Func<EmailInfo, QueueActionType> callback, Action<EmailInfo> failCallBack = null);
        Task<bool> ProcessEmailQueueAsync(Func<EmailInfo, QueueActionType> callback, Action<EmailInfo> failCallBack = null);
        bool ClearAllEmailQueue();

        bool InsertSmsQueue(SmsInfo sms);
        bool ProcessSmsQueue(Func<SmsInfo, QueueActionType> callback, Action<SmsInfo> failCallBack = null);
        bool ClearAllSmsQueue();

        bool InsertThumbnailQueue(ThumbnailInfo thumbnail);
        bool ProcessThumbnailQueue(Func<ThumbnailInfo, QueueActionType> callback, Action<ThumbnailInfo> failCallback = null);
        bool ClearAllThumbnailQueue();

        bool InsertUncallQueue(UncallEntity thumbnail);
        bool ProcessUncallQueue(Func<UncallEntity, QueueActionType> callback, Action<UncallEntity> failCallback = null);
        bool ClearAllUncallQueue();

        bool InsertTenantApplicationsQueue(TenantApplications thumbnail);
        bool ProcessTenantApplicationsQueue(Func<TenantApplications, QueueActionType> callback, Action<TenantApplications> failCallback = null);
        bool RemoveTenantApplicationsQueue(TenantApplications thumbnail);
        bool ClearAllTenantApplicationsQueue();

        bool InsertPenaltyInterestInfoQueue(PenaltyInterestInfo thumbnail);
        bool RemovePenaltyInterestInfoQueue(PenaltyInterestInfo thumbnail);
        bool ProcessPenaltyInterestInfoQueue(Func<PenaltyInterestInfo, QueueActionType> callback, Action<PenaltyInterestInfo> failCallback = null);
        bool ClearAllPenaltyInterestInfoQueue();

        bool ResetQueue(QueueErrorMessageTable data);

        string TestQueueConnection(QueueType storageType, string connectionString);
    }

    public class StorageQueueService : IStorageQueueService
    {
        private const string ServiceName = "KC.Database.Services.Queue.StorageQueueService";
        protected Tenant Tenant { get; private set; }
        protected IQueueRepository<ReportInfo> ReportQueueRepository { get; private set; }
        protected IQueueRepository<ThumbnailInfo> ThumbnailQueueRepository { get; private set; }
        protected IQueueRepository<EmailInfo> EmailQueueRepository { get; private set; }
        protected IQueueRepository<SmsInfo> SmsQueueRepository { get; private set; }
        protected IQueueRepository<DownloadVoiceInfo> DownloadVoiceQueueRepository { get; private set; }
        protected IQueueRepository<UncallEntity> UncallEntityRepository { get; private set; }
        protected IQueueRepository<TenantApplications> TenantApplicationsRepository { get; private set; }
        protected IQueueRepository<PenaltyInterestInfo> PenaltyInterestInfoRepository { get; private set; }

        public StorageQueueService()
        {
            var storageType = KC.Framework.Base.GlobalConfig.BlobStorage.ToLower();
            switch (storageType)
            {
                case "azure":
                    {
                        ReportQueueRepository = new AzureCommonQueueRepository<ReportInfo>();
                        ThumbnailQueueRepository = new AzureCommonQueueRepository<ThumbnailInfo>();
                        EmailQueueRepository = new AzureCommonQueueRepository<EmailInfo>();
                        SmsQueueRepository = new AzureCommonQueueRepository<SmsInfo>();
                        DownloadVoiceQueueRepository = new AzureCommonQueueRepository<DownloadVoiceInfo>();
                        UncallEntityRepository = new AzureCommonQueueRepository<UncallEntity>();
                        TenantApplicationsRepository = new AzureCommonQueueRepository<TenantApplications>();
                        PenaltyInterestInfoRepository = new AzureCommonQueueRepository<PenaltyInterestInfo>();
                    } break;
                case "cmb":
                    {
                        ReportQueueRepository = new KafkaCommonQueueRepository<ReportInfo>();
                        ThumbnailQueueRepository = new KafkaCommonQueueRepository<ThumbnailInfo>();
                        EmailQueueRepository = new KafkaCommonQueueRepository<EmailInfo>();
                        SmsQueueRepository = new KafkaCommonQueueRepository<SmsInfo>();
                        DownloadVoiceQueueRepository = new KafkaCommonQueueRepository<DownloadVoiceInfo>();
                        UncallEntityRepository = new KafkaCommonQueueRepository<UncallEntity>();
                        TenantApplicationsRepository = new KafkaCommonQueueRepository<TenantApplications>();
                        PenaltyInterestInfoRepository = new KafkaCommonQueueRepository<PenaltyInterestInfo>();
                    } break;
                default:
                    {
                        ReportQueueRepository = new RedisCommonQueueRepository<ReportInfo>();
                        ThumbnailQueueRepository = new RedisCommonQueueRepository<ThumbnailInfo>();
                        EmailQueueRepository = new RedisCommonQueueRepository<EmailInfo>();
                        SmsQueueRepository = new RedisCommonQueueRepository<SmsInfo>();
                        DownloadVoiceQueueRepository = new RedisCommonQueueRepository<DownloadVoiceInfo>();
                        UncallEntityRepository = new RedisCommonQueueRepository<UncallEntity>();
                        TenantApplicationsRepository = new RedisCommonQueueRepository<TenantApplications>();
                        PenaltyInterestInfoRepository = new RedisCommonQueueRepository<PenaltyInterestInfo>();
                    } break;
            }
        }

        protected StorageQueueService(Tenant tenant)
        {
            Tenant = tenant;
            var storageType = tenant.QueueType;
            switch (storageType)
            {
                case QueueType.AzureQueue:
                    {
                        ReportQueueRepository = new AzureCommonQueueRepository<ReportInfo>(tenant);
                        ThumbnailQueueRepository = new AzureCommonQueueRepository<ThumbnailInfo>(tenant);
                        EmailQueueRepository = new AzureCommonQueueRepository<EmailInfo>(tenant);
                        SmsQueueRepository = new AzureCommonQueueRepository<SmsInfo>(tenant);
                        DownloadVoiceQueueRepository = new AzureCommonQueueRepository<DownloadVoiceInfo>(tenant);
                        UncallEntityRepository = new AzureCommonQueueRepository<UncallEntity>(tenant);
                        TenantApplicationsRepository = new AzureCommonQueueRepository<TenantApplications>(tenant);
                        PenaltyInterestInfoRepository = new AzureCommonQueueRepository<PenaltyInterestInfo>(tenant);
                    } break;
                case QueueType.Kafka:
                    {
                        ReportQueueRepository = new KafkaCommonQueueRepository<ReportInfo>(tenant);
                        ThumbnailQueueRepository = new KafkaCommonQueueRepository<ThumbnailInfo>(tenant);
                        EmailQueueRepository = new KafkaCommonQueueRepository<EmailInfo>(tenant);
                        SmsQueueRepository = new KafkaCommonQueueRepository<SmsInfo>(tenant);
                        DownloadVoiceQueueRepository = new KafkaCommonQueueRepository<DownloadVoiceInfo>(tenant);
                        UncallEntityRepository = new KafkaCommonQueueRepository<UncallEntity>(tenant);
                        TenantApplicationsRepository = new KafkaCommonQueueRepository<TenantApplications>(tenant);
                        PenaltyInterestInfoRepository = new KafkaCommonQueueRepository<PenaltyInterestInfo>(tenant);
                    } break;
                case QueueType.Redis:
                default:
                    {
                        ReportQueueRepository = new RedisCommonQueueRepository<ReportInfo>(tenant);
                        ThumbnailQueueRepository = new RedisCommonQueueRepository<ThumbnailInfo>(tenant);
                        EmailQueueRepository = new RedisCommonQueueRepository<EmailInfo>(tenant);
                        SmsQueueRepository = new RedisCommonQueueRepository<SmsInfo>(tenant);
                        DownloadVoiceQueueRepository = new RedisCommonQueueRepository<DownloadVoiceInfo>(tenant);
                        UncallEntityRepository = new RedisCommonQueueRepository<UncallEntity>(tenant);
                        TenantApplicationsRepository = new RedisCommonQueueRepository<TenantApplications>(tenant);
                        PenaltyInterestInfoRepository = new RedisCommonQueueRepository<PenaltyInterestInfo>(tenant);
                    } break;
            }
        }

        #region 报表队列操作

        public bool InsertReportQueue(ReportInfo report)
        {
            try
            {
                ReportQueueRepository.AddMessage(report);
                return true;
            }
            catch (Exception ex)
            {
                LogUtil.LogError(ServiceName + " throw exception: " + ex.Message, ex.StackTrace);
                return false;
            }
        }

        public bool ProcessReportQueue(Func<ReportInfo, QueueActionType> callback, Action<ReportInfo> failCallback = null)
        {
            try
            {
                return ReportQueueRepository.ProcessQueueList(callback, failCallback);
            }
            catch (Exception ex)
            {
                LogUtil.LogError(ServiceName + " throw exception: " + ex.Message, ex.StackTrace);
                return false;
            }
        }

        public bool ClearAllReportQueue()
        {
            try
            {
                ReportQueueRepository.RemoveAllMessage();
                return true;
            }
            catch (Exception ex)
            {
                LogUtil.LogError(ServiceName + " throw exception: " + ex.Message, ex.StackTrace);
                return false;
            }
        }

        #endregion

        #region Email Queue

        public bool InsertEmailQueue(EmailInfo email)
        {
            try
            {
                EmailQueueRepository.AddMessage(email);

                return true;
            }
            catch (Exception ex)
            {
                LogUtil.LogError(ServiceName + " throw exception: " + ex.Message, ex.StackTrace);
                return false;
            }
        }

        public bool ProcessEmailQueue(Func<EmailInfo, QueueActionType> callback, Action<EmailInfo> failCallBack = null)
        {
            try
            {
                return EmailQueueRepository.ProcessQueueList(callback, failCallBack);
            }
            catch (Exception ex)
            {
                LogUtil.LogError(ServiceName + " throw exception: " + ex.Message, ex.StackTrace);
                return false;
            }
        }

        public async Task<bool> ProcessEmailQueueAsync(Func<EmailInfo, QueueActionType> callback, Action<EmailInfo> failCallBack = null)
        {
            try
            {
                return await EmailQueueRepository.ProcessQueueListAsync(callback, failCallBack);
            }
            catch (Exception ex)
            {
                LogUtil.LogError(ServiceName + " throw exception: " + ex.Message, ex.StackTrace);
                return false;
            }
        }

        public bool ClearAllEmailQueue()
        {
            try
            {
                EmailQueueRepository.RemoveAllMessage();
                return true;
            }
            catch (Exception ex)
            {
                LogUtil.LogError(ServiceName + " throw exception: " + ex.Message, ex.StackTrace);
                return false;
            }
        }

        #endregion

        #region Sms Queue

        public bool InsertSmsQueue(SmsInfo sms)
        {
            try
            {
                //LogUtil.LogInfo(ServiceName, ": begin to insert sms queue.............");
                SmsQueueRepository.AddMessage(sms);
                //LogUtil.LogInfo(ServiceName, ": end to insert sms queue.............");
                return true;
            }
            catch (Exception ex)
            {
                LogUtil.LogError(ServiceName + " throw exception: " + ex.Message, ex.StackTrace);
                return false;
            }
        }

        public bool ProcessSmsQueue(Func<SmsInfo, QueueActionType> callback, Action<SmsInfo> failCallBack = null)
        {
            try
            {
                //LogUtil.LogInfo(ServiceName, "begin to Process sms queue.............");
                bool result = SmsQueueRepository.ProcessQueueList(callback, failCallBack);
                //LogUtil.LogInfo(ServiceName, "begin to Process sms queue.............");
                return result;
            }
            catch (Exception ex)
            {
                LogUtil.LogError(ServiceName + " throw exception: " + ex.Message, ex.StackTrace);
                return false;
            }
        }

        public bool ClearAllSmsQueue()
        {
            try
            {
                SmsQueueRepository.RemoveAllMessage();
                return true;
            }
            catch (Exception ex)
            {
                LogUtil.LogError(ServiceName + " throw exception: " + ex.Message, ex.StackTrace);
                return false;
            }
        }

        #endregion

        #region 缩略图队列操作
        public bool InsertThumbnailQueue(ThumbnailInfo thumbnail)
        {
            try
            {
                ThumbnailQueueRepository.AddMessage(thumbnail);
                return true;
            }
            catch (Exception ex)
            {
                LogUtil.LogError(ServiceName + " throw exception: " + ex.Message, ex.StackTrace);
                return false;
            }
        }

        public bool ProcessThumbnailQueue(Func<ThumbnailInfo, QueueActionType> callback, Action<ThumbnailInfo> failCallback = null)
        {
            try
            {
                return ThumbnailQueueRepository.ProcessQueueList(callback, failCallback);
            }
            catch (Exception ex)
            {
                LogUtil.LogError(ServiceName + " throw exception: " + ex.Message, ex.StackTrace);
                return false;
            }
        }

        public bool ClearAllThumbnailQueue()
        {
            try
            {
                ThumbnailQueueRepository.RemoveAllMessage();
                return true;
            }
            catch (Exception ex)
            {
                LogUtil.LogError(ServiceName + " throw exception: " + ex.Message, ex.StackTrace);
                return false;
            }
        }

        #endregion

        #region 语音下载

        public bool InserDownloadVoiceInfo(DownloadVoiceInfo thumbnail)
        {
            try
            {
                DownloadVoiceQueueRepository.AddMessage(thumbnail);
                return true;
            }
            catch (Exception ex)
            {
                LogUtil.LogError(ServiceName + " throw exception: " + ex.Message, ex.StackTrace);
                return false;
            }
        }

        public bool ProcessDownloadVoiceInfoQueue(Func<DownloadVoiceInfo, QueueActionType> callback, Action<DownloadVoiceInfo> failCallback = null)
        {
            try
            {
                return DownloadVoiceQueueRepository.ProcessQueueList(callback, failCallback);
            }
            catch (Exception ex)
            {
                LogUtil.LogError(ServiceName + " throw exception: " + ex.Message, ex.StackTrace);
                return false;
            }
        }

        public bool ClearAllDownloadVoiceInfoQueue()
        {
            try
            {
                DownloadVoiceQueueRepository.RemoveAllMessage();
                return true;
            }
            catch (Exception ex)
            {
                LogUtil.LogError(ServiceName + " throw exception: " + ex.Message, ex.StackTrace);
                return false;
            }
        }

        #endregion

        #region uncall job

        public bool InsertUncallQueue(UncallEntity thumbnail)
        {
            try
            {
                UncallEntityRepository.AddMessage(thumbnail);
                return true;
            }
            catch (Exception ex)
            {
                LogUtil.LogError(ServiceName + " throw exception: " + ex.Message, ex.StackTrace);
                return false;
            }
        }

        public bool ProcessUncallQueue(Func<UncallEntity, QueueActionType> callback, Action<UncallEntity> failCallback = null)
        {
            try
            {
                return UncallEntityRepository.ProcessQueue(callback, failCallback);
            }
            catch (Exception ex)
            {
                LogUtil.LogError(ServiceName + " throw exception: " + ex.Message, ex.StackTrace);
                return false;
            }
        }

        public bool ClearAllUncallQueue()
        {
            try
            {
                UncallEntityRepository.RemoveAllMessage();
                return true;
            }
            catch (Exception ex)
            {
                LogUtil.LogError(ServiceName + " throw exception: " + ex.Message, ex.StackTrace);
                return false;
            }
        }

        #endregion

        #region TenantApplications job

        public bool InsertTenantApplicationsQueue(TenantApplications thumbnail)
        {
            try
            {
                TenantApplicationsRepository.AddMessage(thumbnail);
                return true;
            }
            catch (Exception ex)
            {
                LogUtil.LogError(ServiceName + " throw exception: " + ex.Message, ex.StackTrace);
                return false;
            }
        }
        public bool ProcessTenantApplicationsQueue(Func<TenantApplications, QueueActionType> callback, Action<TenantApplications> failCallback = null)
        {
            try
            {
                return TenantApplicationsRepository.ProcessQueueList(callback, failCallback);
            }
            catch (Exception ex)
            {
                LogUtil.LogError(ServiceName + " throw exception: " + ex.Message, ex.StackTrace);
                return false;
            }
        }
        public bool RemoveTenantApplicationsQueue(TenantApplications thumbnail)
        {
            try
            {
                TenantApplicationsRepository.RemoveMessage(thumbnail);
                return true;
            }
            catch (Exception ex)
            {
                LogUtil.LogError(ServiceName + " throw exception: " + ex.Message, ex.StackTrace);
                return false;
            }
        }
        public bool ClearAllTenantApplicationsQueue()
        {
            try
            {
                TenantApplicationsRepository.RemoveAllMessage();
                return true;
            }
            catch (Exception ex)
            {
                LogUtil.LogError(ServiceName + " throw exception: " + ex.Message, ex.StackTrace);
                return false;
            }
        }

        #endregion

        #region PenaltyInterestInfo job

        public bool InsertPenaltyInterestInfoQueue(PenaltyInterestInfo thumbnail)
        {
            try
            {
                PenaltyInterestInfoRepository.AddMessage(thumbnail);
                return true;
            }
            catch (Exception ex)
            {
                LogUtil.LogError(ServiceName + " throw exception: " + ex.Message, ex.StackTrace);
                return false;
            }
        }
        public bool RemovePenaltyInterestInfoQueue(PenaltyInterestInfo thumbnail)
        {
            try
            {
                PenaltyInterestInfoRepository.RemoveMessage(thumbnail);
                return true;
            }
            catch (Exception ex)
            {
                LogUtil.LogError(ServiceName + " throw exception: " + ex.Message, ex.StackTrace);
                return false;
            }
        }
        public bool ProcessPenaltyInterestInfoQueue(Func<PenaltyInterestInfo, QueueActionType> callback, Action<PenaltyInterestInfo> failCallback = null)
        {
            try
            {
                return PenaltyInterestInfoRepository.ProcessQueueList(callback, failCallback);
            }
            catch (Exception ex)
            {
                LogUtil.LogError(ServiceName + " throw exception: " + ex.Message, ex.StackTrace);
                return false;
            }
        }
        public bool ClearAllPenaltyInterestInfoQueue()
        {
            try
            {
                PenaltyInterestInfoRepository.RemoveAllMessage();
                return true;
            }
            catch (Exception ex)
            {
                LogUtil.LogError(ServiceName + " throw exception: " + ex.Message, ex.StackTrace);
                return false;
            }
        }

        #endregion

        public bool ResetQueue(QueueErrorMessageTable data)
        {
            var assembly = data.SourceFrom;
            if (assembly.Equals(typeof(ReportInfo).FullName, StringComparison.OrdinalIgnoreCase))
            {
                var objXml = data.QueueMessage;
                var obj = new ReportInfo(objXml);
                obj.ErrorCount = 0;
                return InsertReportQueue(obj);
            }
            else if (assembly.Equals(typeof(ThumbnailInfo).FullName, StringComparison.OrdinalIgnoreCase))
            {
                var objXml = data.QueueMessage;
                var obj = new ThumbnailInfo(objXml);
                obj.ErrorCount = 0;
                return InsertThumbnailQueue(obj);
            }
            else if (assembly.Equals(typeof(EmailInfo).FullName, StringComparison.OrdinalIgnoreCase))
            {
                var objXml = data.QueueMessage;
                var obj = new EmailInfo(objXml);
                obj.ErrorCount = 0;
                return InsertEmailQueue(obj);
            }
            else if (assembly.Equals(typeof(SmsInfo).FullName, StringComparison.OrdinalIgnoreCase))
            {
                var objXml = data.QueueMessage;
                var obj = new SmsInfo(objXml);
                obj.ErrorCount = 0;
                return InsertSmsQueue(obj);
            }
            else if (assembly.Equals(typeof(UncallEntity).FullName, StringComparison.OrdinalIgnoreCase))
            {
                var objXml = data.QueueMessage;
                var obj = new UncallEntity(objXml);
                obj.ErrorCount = 0;
                return InsertUncallQueue(obj);
            }
            else if (assembly.Equals(typeof(TenantApplications).FullName, StringComparison.OrdinalIgnoreCase))
            {
                var objXml = data.QueueMessage;
                var obj = new TenantApplications(objXml);
                obj.ErrorCount = 0;
                return InsertTenantApplicationsQueue(obj);
            }
            else if (assembly.Equals(typeof(PenaltyInterestInfo).FullName, StringComparison.OrdinalIgnoreCase))
            {
                var objXml = data.QueueMessage;
                var obj = new PenaltyInterestInfo(objXml);
                obj.ErrorCount = 0;
                return InsertPenaltyInterestInfoQueue(obj);
            }
            return true;
        }

        public string TestQueueConnection(QueueType storageType, string connectionString)
        {
            IQueueRepository<EmailInfo> emailQueueRepository;
            switch (storageType)
            {
                case QueueType.AzureQueue:
                    {
                        emailQueueRepository = new AzureCommonQueueRepository<EmailInfo>(connectionString);
                    }
                    break;
                case QueueType.Kafka:
                    {
                        emailQueueRepository = new KafkaCommonQueueRepository<EmailInfo>(connectionString);
                    }
                    break;
                case QueueType.Redis:
                default:
                    {
                        emailQueueRepository = new RedisCommonQueueRepository<EmailInfo>(connectionString);
                    }
                    break;
            }

            var email = new EmailInfo()
            {
                Tenant = TenantConstant.DbaTenantName,
                SendTo = new List<string>() { "tianchangjun@cfwin.com,rayxuan@cfwin.com" },
                EmailTitle = "[测试邮件]测试EmailQueue",
                EmailBody = "[测试邮件]测试EmailQueue",
                SendFrom = "tianchangjun@cfwin.com,rayxuan@cfwin.com",
                IsBodyHtml = false,
            };
            try
            {
                emailQueueRepository.AddMessage(email);
                return string.Empty;
            }
            catch (Exception ex)
            {
                var errorMsg = string.Format(
                    "使用队列[队列：{0}--{1}]出错; " + Environment.NewLine +
                    "错误消息：{2}; " + Environment.NewLine +
                    "错误详情：{3}",
                    storageType, connectionString, ex.Message, ex.StackTrace);
                LogUtil.LogError(errorMsg);
                return errorMsg;
            }
        }

        
    }

    public class TenantStorageQueueService : StorageQueueService
    {
        private const string ServiceName = "KC.Database.Services.Queue.TenantServiceBusQueueService";
        public new Tenant Tenant { get; private set; }
        public TenantStorageQueueService(Tenant tenant)
            : base(tenant)
        {
            Tenant = tenant;
        }
    }
}
