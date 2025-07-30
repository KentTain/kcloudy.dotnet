using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using KC.Framework.Base;
using KC.Framework.Tenant;

namespace KC.Component.Base
{
    //[Serializable]
    //[DataContract]
    //public enum TopicType
    //{
    //    [EnumMember]
    //    [Description("ServiceBus队列")]
    //    ServiceBus = 0,
    //    [EnumMember]
    //    [Description("Kafka队列")]
    //    Kafka = 1,
    //    [EnumMember]
    //    [Description("Redis")]
    //    Redis = 2,
    //    [EnumMember]
    //    [Description("未知")]
    //    UNKNOWN = 99,
    //}
    [Serializable]
    [DataContract(IsReference = true)]
    public class TopicEntity<T> : EntityBase where T :EntityBase
    {
        public TopicEntity()
        {
            Id = Guid.NewGuid().ToString();
            CreatedDate = DateTime.UtcNow;
        }
        [DataMember]
        public string Id { get; set; }
        [DataMember]
        public string TopicName { get; set; }
        [DataMember]
        public ServiceBusType TopicType { get; set; }
        [DataMember]
        public DateTime CreatedDate { get; set; }
        [DataMember]
        public string Tenant { get; set; }
        [DataMember]
        public string CallBackUrl { get; set; }

        [DataMember]
        public T TopicContext { get; set; }
    }
}
