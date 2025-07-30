using KC.Framework.Base;
using System;
using System.Runtime.Serialization;

namespace KC.Model.Job.Table
{
    [Serializable]
    [DataContract(IsReference = true)]
    public class ThreadStatusInfo : AzureTableEntity
    {
        public ThreadStatusInfo()
            : this("ThreadStatusInfo", Guid.NewGuid().ToString())
        {
        }


        public ThreadStatusInfo(string patitionKey)
            : this(patitionKey, Guid.NewGuid().ToString())
        {

        }

        public ThreadStatusInfo(string patitionKey, string rowKey)
        {
            PartitionKey = patitionKey;
            RowKey = rowKey;
            Timestamp = DateTime.UtcNow;
        }

        [DataMember]
        public string Id { get; set; }
        [DataMember]
        public string ThreadName { get; set; }
        [DataMember]
        public string WorkerRoleId { get; set; }
        [DataMember]
        public DateTime UpdateTime { get; set; }
        [DataMember]
        public int Successes { get; set; }
        [DataMember]
        public int Failures { get; set; }
    }
}
