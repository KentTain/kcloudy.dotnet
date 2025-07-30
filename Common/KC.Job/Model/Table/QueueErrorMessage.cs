using System;
using System.Runtime.Serialization;
using KC.Framework.Base;

namespace KC.Model.Job.Table
{
    [Serializable]
    [DataContract(IsReference = true)]
    public class QueueErrorMessage : AzureTableEntity
    {
        public QueueErrorMessage()
            : this("QueueErrorMessage", Guid.NewGuid().ToString())
        {
        }


        public QueueErrorMessage(string patitionKey)
            : this(patitionKey, Guid.NewGuid().ToString())
        {

        }

        public QueueErrorMessage(string patitionKey, string rowKey)
        {
            PartitionKey = patitionKey;
            RowKey = rowKey;
            Timestamp = DateTime.UtcNow;
        }

        [DataMember]
        public string QueueType { get; set; }
        [DataMember]
        public string QueueName { get; set; }
        [DataMember]
        public string QueueMessage { get; set; }
        [DataMember]
        public string SourceFrom { get; set; }
        [DataMember]
        public string ErrorMessage { get; set; }
        [DataMember]
        public string ErrorFrom { get; set; }
    }
}
