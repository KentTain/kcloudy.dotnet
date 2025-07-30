using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KC.WebApi.Pay.Models.PayTransaction.FreezeAmt
{
    public class Amt
    {
        /// <summary>
        /// 发生额 R
        /// </summary>
        
        public decimal AclAmt { get; set; }

        /// <summary>
        /// 币种，默认“CNY” R
        /// </summary>
        
        public string CcyCd { get; set; }

    }
}
