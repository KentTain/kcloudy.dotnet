using System;
using System.Runtime.Serialization;

namespace KC.Service.Component.DTO
{
    [Serializable]
    [DataContract(IsReference = true)]
    public class QueueErrorMessageDTO : AzureTableEntityDTO
    {
        public QueueErrorMessageDTO()
            : base("QueueErrorMessageDTO", Guid.NewGuid().ToString())
        {
        }

        public QueueErrorMessageDTO(string patitionKey)
            : base(patitionKey)
        {
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
