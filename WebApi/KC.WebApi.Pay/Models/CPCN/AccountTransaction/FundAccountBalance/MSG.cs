using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KC.WebApi.Pay.Models.AccountTransaction.FundAccountBalance
{
    /// <summary>
    /// 资金账户余额查询[T1005]
    /// </summary>
    public class MSG
    {
        /// <summary>
        /// 
        /// </summary>
        
        public MSGHD MSGHD { get; set; }

        /// <summary>
        /// 账户信息
        /// </summary>
        
        public CltAcc CltAcc { get; set; }
    }
}
