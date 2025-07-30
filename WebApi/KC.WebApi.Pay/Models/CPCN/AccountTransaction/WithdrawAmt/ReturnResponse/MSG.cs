using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KC.WebApi.Pay.Models.AccountTransaction.WithdrawAmt.ReturnResponse
{
    public class MSG
    {
        
        public MSGHD MSGHD { get; set; }

        
        public CltAcc CltAcc { get; set; }

        /// <summary>
        /// 币种，默认“CNY”
        /// </summary>
        
        public string CcyCd { get; set; }

        /// <summary>
        /// 账户余额-智融资金账户
        /// </summary>
        
        public AcsAmt AcsAmt { get; set; }

        /// <summary>
        /// 可 T1 出金的额度
        /// </summary>
        
        public T1Amt T1Amt { get; set; }

        /// <summary>
        /// 可 T0 出金的额度
        /// </summary>
        
        public T0Amt T0Amt { get; set; }
    }
}
