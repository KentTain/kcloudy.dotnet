using System;
using System.Runtime.Serialization;

namespace KC.Service.Component.DTO
{
    public abstract class AzureTableEntityDTO : StorageEntityDTO
    {
        [DataMember]
        public string PartitionKey { get; set; }
        [DataMember]
        public string RowKey { get; set; }
        [DataMember]
        public DateTimeOffset? Timestamp { get; set; }
        [DataMember]
        public Azure.ETag ETag { get; set; }
        [DataMember]
        public string Tenant { get; set; }

        protected AzureTableEntityDTO()
            : this(string.Empty, string.Empty)
        {
        }


        protected AzureTableEntityDTO(string patitionKey)
            : this(patitionKey, Guid.NewGuid().ToString())
        {

        }

        protected AzureTableEntityDTO(string patitionKey, string rowKey)
        {
            PartitionKey = patitionKey;
            RowKey = rowKey;
            Timestamp = DateTime.UtcNow;
        }
    }
}
