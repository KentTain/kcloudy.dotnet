using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KC.WebApi.Pay.Models.AccountTransaction.WithdrawAmt
{
    /// <summary>
    /// 查询可 T0/T1 出金额度[T1018]
    /// 此交易主要是配合 T2004、 T2009 交易使用。
    /// 通过该交易告知合作方 T2004、 T2009 交易中 T0/T1 出金时允许的出金额度情况。
    /// </summary>
    public class MSG
    {
        
        public MSGHD MSGHD { get; set; }
        /// <summary>
        /// 资金账户
        /// </summary>
        
        public CltAcc CltAcc { get; set; }


    }
}
