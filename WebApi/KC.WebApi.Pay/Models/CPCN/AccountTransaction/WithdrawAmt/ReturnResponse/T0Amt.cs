using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KC.WebApi.Pay.Models.AccountTransaction.WithdrawAmt.ReturnResponse
{
    /// <summary>
    /// 可 T0 出金的额度
    /// </summary>
    public class T0Amt
    {
        /// <summary>
        /// 正常出金（A00）时的额度(单位:分)
        /// </summary>
        
        public decimal CtAmtA00 { get; set; }

        /// <summary>
        /// 解冻出金（B01）时的额度(单位:分)
        /// </summary>
        
        public decimal CtAmtB01 { get; set; }
    }
}
