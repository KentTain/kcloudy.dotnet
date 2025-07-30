using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KC.Service.DTO
{
    [Serializable, DataContract(IsReference = true)]
    public class ReportGroupDTO : EntityBaseDTO
    {
        [DataMember]
        public string Key1 { get; set; }
        [DataMember]
        public string KeyName1 { get; set; }

        [DataMember]
        public string Key2 { get; set; }
        [DataMember]
        public string KeyName2 { get; set; }

        [DataMember]
        public string Key3 { get; set; }
        [DataMember]
        public string KeyName3 { get; set; }

        [DataMember]
        public string Key4 { get; set; }
        [DataMember]
        public string KeyName4 { get; set; }

        [DataMember]
        public string Key5 { get; set; }
        [DataMember]
        public string KeyName5 { get; set; }

        [DataMember]
        public string Key6 { get; set; }
        [DataMember]
        public string KeyName6 { get; set; }

        [DataMember]
        public decimal Value { get; set; }
    }
}
