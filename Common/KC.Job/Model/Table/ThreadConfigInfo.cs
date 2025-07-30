using KC.Framework.Base;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace KC.Model.Job.Table
{
    [Serializable]
    [DataContract(IsReference = true)]
    public class ThreadConfigInfo : AzureTableEntity
    {
        public ThreadConfigInfo()
            : this("ThreadConfigInfo", Guid.NewGuid().ToString())
        {
        }


        public ThreadConfigInfo(string patitionKey)
            : this(patitionKey, Guid.NewGuid().ToString())
        {

        }

        public ThreadConfigInfo(string patitionKey, string rowKey)
        {
            PartitionKey = patitionKey;
            RowKey = rowKey;
            Timestamp = DateTime.UtcNow;

            this.IsEnabled = true;
            this.EnableLiveBackup = false;
            this.EnableEmailService = false;
            this.EnableNotificationService = false;
            this.EnableConversionMonitor = true;
            this.EnableConversionService = true;
            this.EnableUploadProcessing = true;
            this.EnableFullTextSearchIndexer = true;
            this.EnableMetricDatabaseSync = false;
            this.EnableTenantSandboxService = false;
            this.EnableTenantArchiveService = false;
            this.EnableTenantRestoreService = false;
            this.FullTextSearchCheckingIntervalInMinutes = 10;
            this.ConversionTimeoutInMinutes = 30;
            this.MaxRetryTimeForConversion = 3;
            this.ConversionCheckInterval = 1000;
            this.SelectedPdfLibrary = AvailablePdfLibrary[0];
            this.KeepThisConfig = false;
            this.CreateTime = DateTime.UtcNow;
            this.LastModifyTime = DateTime.UtcNow;
        }

        [DataMember]
        public string WorkerRoleId { get; set; }

        [DataMember]
        public string HostName { get; set; }

        [DataMember]
        public bool IsEnabled { get; set; }

        [DataMember]
        public bool EnableLiveBackup { get; set; }

        [DataMember]
        public bool EnableEmailService { get; set; }

        [DataMember]
        public bool EnableNotificationService { get; set; }

        [DataMember]
        public bool EnableConversionService { get; set; }

        [DataMember]
        public bool EnableUploadProcessing { get; set; }

        [DataMember]
        public bool EnableConversionMonitor { get; set; }

        [DataMember]
        public bool EnableTenantSandboxService { get; set; }

        [DataMember]
        public bool EnableTenantArchiveService { get; set; }

        [DataMember]
        public bool EnableTenantRestoreService { get; set; }

        [DataMember]
        public bool EnableMetricDatabaseSync { get; set; }

        [DataMember]
        public bool EnableFullTextSearchIndexer { get; set; }

        [DataMember]
        public int FullTextSearchCheckingIntervalInMinutes { get; set; }

        /// <summary>
        /// timeout include retry 3 times.
        /// </summary>
        [DataMember]
        public int ConversionTimeoutInMinutes { get; set; }

        [DataMember]
        public int MaxRetryTimeForConversion { get; set; }

        [DataMember]
        public int ConversionCheckInterval { get; set; }

        [DataMember]
        public string SelectedPdfLibrary { get; set; }

        [DataMember]
        public bool KeepThisConfig { get; set; }

        [DataMember]
        public DateTime CreateTime { get; set; }

        [DataMember]
        public DateTime LastModifyTime { get; set; }

        [DataMember]
        public DateTime LastAccessTime { get; set; }

        public static List<string> AvailablePdfLibrary
        {
            get
            {
                return new List<string>() {
                    "GhostScript",
                    "PDFTron PDF2Image", 
                    "Aspose.Pdf For .Net", 
                    "O2Solutions PDFView4NET",
                    "Tall Component PDFRasterizer.NET"
                };
            }
        }
    }
}
