using KC.Enums.Pay;
using KC.Framework.Extension;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;

namespace KC.Service.DTO.Pay
{
    [Serializable, DataContract(IsReference = true)]
    public class CashUsageDetailDTO : PaymentFlowDTO
    {
        public int Id { get; set; }

        /// <summary>
        /// 采购订单ID
        /// </summary>
        //public string OrderId { get; set; }
        [MaxLength(128), DataMember]
        public string ReferenceId { get; set; }

        [DataMember]
        public decimal ConsumptionMoney { get; set; }

        [DataMember]
        public string ConsumptionDate { get; set; }

        public string ConsumptionMoneyStr
        {
            get
            {
                return ConsumptionMoney.ToString("N");
            }
        }

        [DataMember]
        public CashType CashType { get; set; }

        [DataMember]
        public string CashTypeName
        {
            get
            {
                return CashType.ToDescription();
            }
        }

        [DataMember]
        public string Remark { get; set; }

        /// <summary>
        /// 交易账户
        /// </summary>
        [DataMember]
        public TradingAccount TradingAccount { get; set; }

        [DataMember]
        public string AccountName
        {
            get { return TradingAccount.ToDescription(); }
        }

        /// <summary>
        /// 支付方式
        /// </summary>
        [DataMember]
        public string PaymentMethod { get; set; }

        /// <summary>
        /// 收入
        /// </summary>
        [DataMember]
        public string Income { get; set; }

        /// <summary>
        /// 支出
        /// </summary>
        [DataMember]
        public string Expenditure { get; set; }

        /// <summary>
        /// 第三方支付的类型
        /// </summary>
        [DataMember]
        public ThirdPartyType PaymentType { get; set; }

    }
}
