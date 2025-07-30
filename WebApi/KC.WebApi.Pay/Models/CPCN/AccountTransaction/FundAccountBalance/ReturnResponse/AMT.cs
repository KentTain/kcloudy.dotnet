using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KC.WebApi.Pay.Models.AccountTransaction.FundAccountBalance.ReturnResponse
{
    public class AMT
    {
        /// <summary>
        /// 资金余额(单位:分)
        /// </summary>
        
        public decimal BalAmt { get; set; }

        /// <summary>
        /// 可用资金(单位:分)
        /// </summary>
        
        public decimal UseAmt { get; set; }

        /// <summary>
        /// 冻结资金(单位:分)
        /// </summary>
        
        public decimal FrzAmt { get; set; }

        /// <summary>
        /// 币种，默认“CNY”
        /// </summary>
        
        public string CcyCd { get; set; }

    }
}
