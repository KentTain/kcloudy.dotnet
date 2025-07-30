using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KC.WebApi.Pay.Models.BoHai
{
    public class VisAcctOpenRetModel : BoHaiReturnModel
    {
        /// <summary>
        /// 添金宝标志 0：未开通 1：开通成功
        /// </summary>
        public string FundFlag { get; set; }

        /// <summary>
        /// 虚拟账号
        /// </summary>
        public string EleAcctNo { get; set; }

        /// <summary>
        /// 17位虚拟账号
        /// </summary>
        public string EleVisAcctNo { get; set; }
    }
}