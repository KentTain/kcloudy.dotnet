using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.Framework.Base;
using KC.Model.Pay.Constants;

namespace KC.Model.Pay
{
    /// <summary>
    /// 付款提现充值操作日志
    /// </summary>
    [Table(Tables.PaymentOperationLog)]
    public class PaymentOperationLog : ProcessLogBase
    {
        /// <summary>
        /// 付款编号，系统生成
        /// </summary>
        [MaxLength(128)]
        public string PaymentNumber { get; set; }

        /// <summary>
        /// 关联单号,采购单付款：存入订单编号，充值：存入充值编号
        /// </summary>
        [MaxLength(128)]
        public string ReferenceId { get; set; }

    }
}
