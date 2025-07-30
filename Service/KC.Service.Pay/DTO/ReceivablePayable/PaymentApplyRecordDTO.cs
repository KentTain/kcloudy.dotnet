using KC.Service.DTO;
using KC.Enums.Pay;
using KC.Framework.Extension;
using System;
using System.Collections.Generic;
using System.Text;

namespace KC.Service.DTO.Pay
{
    public class PaymentApplyRecordDTO : EntityDTO
    {
        public int Id { get; set; }

        /// <summary>
        /// 应付编号,充值提现时可为空
        /// </summary>
        public string PayableNumber { get; set; }

        /// <summary>
        /// 支付编号,由系统生成
        /// </summary>
        public string PaymentNumber { get; set; }

        /// <summary>
        /// 状态,null：待审核，0:退回，1：通过
        /// </summary>
        public bool? Status { get; set; }

        /// <summary>
        /// 收款方tenant
        /// </summary>
        public string PayeeTenant { get; set; }

        /// <summary>
        /// 收款
        /// </summary>
        public string Payee { get; set; }

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

        public string StatusName
        {
            get
            {
                if (Status.HasValue)
                {
                    return Status.Value ? "已支付" : "取消支付";
                }
                return "待确认";
            }
        }

        public string Remark { get; set; }

        public string CreatedDateStr
        {
            get { return CreatedDate.ToString("yyyy-MM-dd HH:mm:ss"); }
        }

        /// <summary>
        /// 交易账户
        /// </summary>
        public TradingAccount TradingAccount { get; set; }

        /// <summary>
        /// 账户体系
        /// </summary>
        public ThirdPartyType PaymentType { get; set; }

        /// <summary>
        /// 账户体系
        /// </summary>
        public string PaymentTypeName
        {
            get
            {
                return PaymentType.ToDescription();
            }
        }
    }
}
