using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KC.WebApi.Pay.Models.CPCN.InAndOutTransaction.BankOutApply
{
    /// <summary>
    /// 银商_出金-申请[T2004]
    /// </summary>
    public class MSG
    {
        
        public MSGHD MSGHD { get; set; }

        /// <summary>
        /// 账户信息
        /// </summary>
        
        public CltAcc CltAcc { get; set; }

        /// <summary>
        /// 结算账户信息
        /// </summary>
        
        public BkAcc BkAcc { get; set; }

        /// <summary>
        /// 转账信息
        /// </summary>
        
        public Amt Amt { get; set; }

        /// <summary>
        /// 资金用途(附言) 128
        /// </summary>
        
        public string Usage { get; set; }

        /// <summary>
        /// 业务标示 R
        /// A00 正常出金
        /// A01 还款出金
        /// B01 解冻资金后，再出金
        /// </summary>
        
        public string TrsFlag { get; set; }

        /// <summary>
        /// 结算方式标示T0=T0 结算出金T1=T1 结算出金(默认)
        /// </summary>
        
        public string BalFlag { get; set; }

        /// <summary>
        /// 流水
        /// </summary>
        
        public Srl Srl { get; set; }
    }
}
