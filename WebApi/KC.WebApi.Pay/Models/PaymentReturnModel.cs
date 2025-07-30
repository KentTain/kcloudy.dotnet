using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace KC.WebApi.Pay.Models.Models
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

        public string ReturnXmlStr { get; set; }

        public string PostXmlStr { get; set; }

        [DataMember]
        public object ReturnData { get; set; }
        /// <summary>
        /// 返回金额相关的数据
        /// </summary>
        [DataMember]
        public ReturnAmtDTO ReturnAmtData { get; set; }

    }
}
