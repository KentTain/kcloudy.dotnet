using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KC.Service.DTO.Pay
{
    [Serializable, DataContract(IsReference = true)]
    public class BF_ZD_PAYBANKDTO
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string FQHHO2 { get; set; }

        [DataMember]
        public int ZHUANT { get; set; }

        [DataMember]
        public string JIGULB { get; set; }

        [DataMember]
        public string HANBDM { get; set; }

        [DataMember]
        public string C2ZCHH { get; set; }

        [DataMember]
        public string QSHHO2 { get; set; }

        [DataMember]
        public string JIEDDM { get; set; }

        [DataMember]
        public string FKHMC1 { get; set; }

        [DataMember]
        public string CXUMC1 { get; set; }

        [DataMember]
        public string SUSDDM { get; set; }

        [DataMember]
        public string YOUZBM { get; set; }

        [DataMember]
        public string ROWIDD { get; set; }

        [DataMember]
        public int JILUZT { get; set; }

        [DataMember]
        public string SPEC16 { get; set; }

        [DataMember]
        public string SPEC32 { get; set; }
    }
}

