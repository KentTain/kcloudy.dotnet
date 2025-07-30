using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace KC.Framework.Base
{
    [Serializable, DataContract(IsReference = true)]
    public class AzureTableEntity : Entity
    {
        [Key]
        [DataMember]
        public string RowKey { get; set; }

        [DataMember]
        public string PartitionKey { get; set; }

        [DataMember]
        public DateTimeOffset? Timestamp { get; set; }

        [DataMember]
        public string Tenant { get; set; }

        protected AzureTableEntity()
            : this(string.Empty, Guid.NewGuid().ToString())
        {

        }


        protected AzureTableEntity(string patitionKey)
            : this(patitionKey, Guid.NewGuid().ToString())
        {

        }

        protected AzureTableEntity(string patitionKey, string rowKey)
        {
            PartitionKey = patitionKey;
            RowKey = rowKey;
            Timestamp = DateTime.UtcNow;
        }
    }
}