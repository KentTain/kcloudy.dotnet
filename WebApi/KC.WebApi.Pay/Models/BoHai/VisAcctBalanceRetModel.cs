using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KC.WebApi.Pay.Models.BoHai
{
    public class VisAcctBalanceRetModel : BoHaiReturnModel
    {
        /// <summary>
        /// 总可用余额
        /// </summary>
        public string AviAmt { get; set; }

        /// <summary>
        /// 待结算余额
        /// </summary>
        public string SettAmt { get; set; }

        /// <summary>
        /// 份额
        /// </summary>
        public string Share { get; set; }

        /// <summary>
        /// 冻结金额
        /// </summary>
        public string FreezeAmt { get; set; }

        /// <summary>
        /// 可提现余额 可提现余额=现金+可用份额额度
        /// </summary>
        public string TxAviAmt { get; set; }

        /// <summary>
        /// 积分
        /// </summary>
        public string Integral { get; set; }
    }
}