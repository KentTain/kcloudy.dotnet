using KC.Enums.Pay;
using KC.Framework.Extension;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace KC.Service.DTO.Pay
{
    [Serializable]
    [DataContract(IsReference = true)]
    public class PaymentDTO
    {
        [DataMember]
        public decimal Consumption { set; get; }
        [DataMember]
        public string PayDate { set; get; }
        [DataMember]
        public PaymentType PayType { set; get; }

        public string PayTypeName
        {
            get
            {
                return PayType.ToDescription();
            }
        }

        public string BillTypeName { get; set; }

        [DataMember]
        public string Remark { set; get; }
        [DataMember]
        public string Number { get; set; }

        [DataMember]
        public string Status { get; set; }

        /// <summary>
        /// 票据会有多种状态，有些状态不能算作已经支付
        /// </summary>
        [DataMember]
        public bool ConfirmPay { get; set; }

        public string FileUrl { get; set; }

        [DataMember]
        public string PayNumber { get; set; }

    }
}
