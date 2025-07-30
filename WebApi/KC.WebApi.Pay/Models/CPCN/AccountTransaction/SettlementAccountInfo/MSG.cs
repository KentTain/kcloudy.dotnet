using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KC.WebApi.Pay.Models.AccountTransaction.SettlementAccountInfo
{
    /// <summary>
    /// 结算账户信息查询[T1007]
    /// </summary>
    public class MSG
    {
        
        public MSGHD MSGHD { get; set; }

        /// <summary>
        /// 账户信息
        /// </summary>
        
        public CltAcc CltAcc { get; set; }

        /// <summary>
        /// PtnSrl 原 结算账户维护 T1004 交易的合作方交易流水号
        /// </summary>
        
        public Srl Srl { get; set; }
    }
}
