using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.Enums.Pay;
using KC.Framework.Base;
using KC.Model.Pay.Constants;

namespace KC.Model.Pay
{
    /// <summary>
    /// 付款单(订单付款、平台服务费、充值时需要审核，先插入本记录，支付成功后再生成付款记录)
    /// </summary>
    [Table(Tables.PaymentRecord)]
    public class PaymentRecord : Entity
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// 应付编号
        /// </summary>
        [MaxLength(128)]
        public string PayableNumber { get; set; }

        /// <summary>
        /// 支付编号,系统生成
        /// </summary>
        [MaxLength(128)]
        public string PaymentNumber { get; set; }

        /// <summary>
        /// 状态,null：待审核，0:退回，1：通过
        /// </summary>
        public bool? Status { get; set; }

        /// <summary>
        /// 收款方tenant
        /// </summary>
        [MaxLength(128)]
        public string PayeeTenant { get; set; }

        /// <summary>
        /// 收款
        /// </summary>
        [MaxLength(128)]
        public string Payee{ get; set; }

        /// <summary>
        /// 采购付款时，存入采购订单编号，提现：存入提现编号，充值：存入充值编号
        /// </summary>
        public string OrderNumber { get; set; }

        /// <summary>
        /// 订单类型，充值为空
        /// </summary>
        public PayableSource? Source { get; set; }

        /// <summary>
        /// 本次支付金额
        /// </summary>
        public decimal PaymentAmount { get; set; }

        public string Remark { get; set; }

        /// <summary>
        /// 交易账户
        /// </summary>
        public TradingAccount TradingAccount{ get; set; }


        /// <summary>
        /// 账户体系
        /// </summary>
        public ThirdPartyType PaymentType { get; set; }
    }
}
