using System;
using System.Runtime.Serialization;
using KC.Framework.Base;

namespace KC.Model.Job.Table
{

    [Serializable]
    [DataContract]
    public enum DatabaseStatus
    {
        [EnumMember]
        Draft = 0,
        [EnumMember]
        InProcess = 1,
        [EnumMember]
        Finished = 2,
        [EnumMember]
        Failed = 3,
    }

    [Serializable]
    [DataContract(IsReference = true)]
    public class DatabaseVersionInfo : AzureTableEntity
    {
        public DatabaseVersionInfo()
            : this("DatabaseVersionInfo", Guid.NewGuid().ToString())
        {
        }


        public DatabaseVersionInfo(string patitionKey)
            : this(patitionKey, Guid.NewGuid().ToString())
        {

        }

        public DatabaseVersionInfo(string patitionKey, string rowKey)
        {
            PartitionKey = patitionKey;
            RowKey = rowKey;
            Timestamp = DateTime.UtcNow;
        }

        [DataMember]
        public string DatabaseName { get; set; }

        [DataMember]
        public int VersionNumber { get; set; }

        [DataMember]
        public DatabaseStatus Status { get; set; }

        [DataMember]
        public DateTime LastModifiedDate { get; set; }
    }
}
