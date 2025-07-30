using KC.Service.DTO;
using KC.Enums.Pay;
using KC.Framework.Extension;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace KC.Service.DTO.Pay
{
    [Serializable]
    [DataContract(IsReference = true)]
    public class PaymentInfoDTO : EntityDTO
    {
 
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string TenantName { get; set; }

        [DataMember]
        public string TenantDisplayName { get; set; }


        /// <summary>
        /// 交易电话
        /// </summary>
        [DataMember]
        public string Phone { get; set; }

        /// <summary>
        /// 交易密码
        /// </summary>
        [DataMember]
        public string TradePassword { get; set; }


        [DataMember]
        public int State { get; set; }

        /// <summary>
        /// 第三方支付的类型
        /// </summary>
        [DataMember]
        public ThirdPartyType PaymentType { get; set; }

        /// <summary>
        /// 第三方支付的类型
        /// </summary>
        [DataMember]
        public string PaymentTypeStr
        {
            get
            {
                return PaymentType.ToDescription();
            }
        }
        /// <summary>
        /// 第三方支付账号
        /// </summary>
        [DataMember]
        public string PaymentAccount { get; set; }

        /// <summary>
        /// 是否平台的账号
        /// </summary>
        [DataMember]
        public bool IsPlatformAccount { get; set; }

    }
}