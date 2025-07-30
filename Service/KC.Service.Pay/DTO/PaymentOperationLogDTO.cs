using KC.Service.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace KC.Service.DTO.Pay
{
    public class PaymentOperationLogDTO : ProcessLogBaseDTO
    {
        /// <summary>
        /// 付款编号，系统生成
        /// </summary>
        public string PaymentNumber { get; set; }

        /// <summary>
        /// 关联单号,采购单付款：存入订单编号，充值：存入充值编号
        /// </summary>
        public string ReferenceId { get; set; }
    }
}
