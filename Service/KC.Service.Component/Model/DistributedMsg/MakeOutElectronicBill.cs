using System;
using System.Runtime.Serialization;
using KC.Framework.Base;

namespace KC.Model.Component.DistributedMsg
{
    [Serializable]
    [DataContract(IsReference = true)]
    public class MakeOutElectronicBill : Entity
    {
        public MakeOutElectronicBill()
        {
        }
        [DataMember]
        public int? DbElectronicId { get; set; }
        [DataMember]
        public string RequestUrl { get; set; }
        [DataMember]
        public string RequestXml { get; set; }
    }
}
