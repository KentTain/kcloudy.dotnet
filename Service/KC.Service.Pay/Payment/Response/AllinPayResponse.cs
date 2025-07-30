using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KC.Service.Pay.Response
{
    public class AllinPayResponse
    {
        [DataMember]
        public string MerchantId { get; set; }
        [DataMember]
        public string Version { get; set; }
        [DataMember]
        public string Language { get; set; }
        [DataMember]
        public string SignType { get; set; }
        [DataMember]
        public string PayType { get; set; }
        [DataMember]
        public string PaymentOrderId { get; set; }
        [DataMember]
        public string OrderNo { get; set; }
        [DataMember]
        public string OrderDatetime { get; set; }
        [DataMember]
        public string OrderAmount { get; set; }
        [DataMember]
        public string PayDatetime { get; set; }
        [DataMember]
        public string PayAmount { get; set; }
        [DataMember]
        public string PayResult { get; set; }
        [DataMember]
        public string ErrorCode { get; set; }
        [DataMember]

        public string ReturnDatetime { get; set; }
        [DataMember]
        public string SignMsg { get; set; }
    }


}