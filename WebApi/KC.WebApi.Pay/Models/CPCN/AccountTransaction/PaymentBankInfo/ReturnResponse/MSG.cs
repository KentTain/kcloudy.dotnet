using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KC.WebApi.Pay.Models.AccountTransaction.PaymentBankInfo.ReturnResponse
{
    public class MSG
    {
        
        public MSGHD MSGHD { get; set; }

        /// <summary>
        /// 支付银行信息
        /// </summary>
        
        public List<PayBnk> PayBnks { get; set; }
    }
}
