using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KC.WebApi.Pay.Models.AccountTransaction.FundAccountBalance.ReturnResponse
{
    public class MSG
    {

        
        public MSGHD MSGHD { get; set; }

        /// <summary>
        /// 成功时返回
        /// </summary>
        
        public AMT Amt { get; set; }
    }
}
