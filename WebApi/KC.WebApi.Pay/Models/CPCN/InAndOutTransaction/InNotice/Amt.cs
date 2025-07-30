using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KC.WebApi.Pay.Models.CPCN.InAndOutTransaction.InNotice
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
        /// 总金额(发生额+转账手续费)
        /// </summary>
        public decimal TAmt { get; set; }

        /// <summary>
        /// 币种，默认“CNY“
        /// </summary>

        public string CcyCd { get; set; }
    }
}
