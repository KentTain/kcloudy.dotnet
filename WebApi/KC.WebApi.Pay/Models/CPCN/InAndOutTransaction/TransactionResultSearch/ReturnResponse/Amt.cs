using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KC.WebApi.Pay.Models.InAndOutTransaction.TransactionResultSearch.ReturnResponse
{
    public class Amt
    {
        /// <summary>
        /// 发生额
        /// </summary>
        
        public decimal AclAmt { get; set; }

        /// <summary>
        /// 转账手续费
        /// </summary>
        
        public decimal FeeAmt { get; set; }

        /// <summary>
        /// 币种，默认“CNY”
        /// </summary>
        
        public string CcyCd { get; set; }
    }
}
