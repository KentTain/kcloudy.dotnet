using KC.Enums.Pay;
using KC.Framework.Base;
using KC.Model.Pay.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KC.Model.Pay
{
    [Serializable, DataContract(IsReference = true)]
    [Table(Tables.PaymentInfo)]

    public class PaymentInfo : Entity
    {
        [Key]
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string TenantName { get; set; }

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
        /// 第三方支付账号
        /// </summary>
        [DataMember]
        public string PaymentAccount { get; set; }

    }
}
