using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KC.WebApi.Pay.Models.PayTransaction.OrderPay
{
    public class MSG
    {
        
        public MSGHD MSGHD { get; set; }

        /// <summary>
        /// 支付信息 R
        /// </summary>
        
        public billInfo billInfo { get; set; }

        /// <summary>
        /// 业务标示 R
        /// A00 普通订单支付
        /// B00 收款方收款成功后，再冻结资金
        /// B01 付款方解冻资金后，再支付给收款方
        /// </summary>
        
        public string TrsFlag { get; set; }
    }
}
