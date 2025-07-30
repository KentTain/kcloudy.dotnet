using KC.Enums.Pay;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;

namespace KC.Model.Pay
{
    [Serializable, DataContract(IsReference = true)]
    public class CashUsageDetail : PaymentFlow
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
        public DateTime ConsumptionDate { get; set; }

        /// <summary>
        /// 交易类型
        /// </summary>
        [DataMember]
        public CashType CashType { get; set; }

        /// <summary>
        /// 是否对账，1、交易时，买家支付后，状态为未对账，入卖家账户锁定金额，交易完成后，状态改为已对账，入卖家账户可用金额；
        /// 2、充值：充值成功为已对账，用户账户可用余额；
        /// 3、提现：用户提现后，流水状态为未对账，入用户账户锁定金额，提现成功后，状态改为已对账，用户账户锁定金额减去相应金额；
        /// 4、交易费：默认为已对账;
        /// 充值时有三种状态，空则为等待充值，1则为充值成功，0则为充值失败;交易、提现、交易费三种只有两种状态：0、1
        /// </summary>
        public bool? CashStatus { get; set; }

        /// <summary>
        /// 交易账户
        /// </summary>
        [DataMember]
        public TradingAccount TradingAccount { get; set; }

        /// <summary>
        /// 支付方式
        /// </summary>
        [MaxLength(128), DataMember]
        public string PaymentMethod { get; set; }

        [DataMember]
        public string Remark { get; set; }

        /// <summary>
        /// 第三方支付的类型
        /// </summary>
        [DataMember]
        public ThirdPartyType PaymentType { get; set; }
    }
}
