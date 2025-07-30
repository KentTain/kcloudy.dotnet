using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KC.Service.DTO.Pay
{
    [Serializable, DataContract(IsReference = true)]
    public class PaymentReturnModel
    {
        [DataMember]
        public bool Success { get; set; }

        [DataMember]
        public string ErrorMessage { get; set; }

        [DataMember]
        public string ErrorCode { get; set; }

        [DataMember]
        public string HtmlStr { get; set; }

        [DataMember]
        public object ReturnData { get; set; }

        [DataMember]
        public ReturnAmtDTO ReturnAmtData { get; set; }
    }
}
