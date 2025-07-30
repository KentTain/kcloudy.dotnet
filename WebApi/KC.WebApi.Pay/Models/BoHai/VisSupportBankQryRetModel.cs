using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KC.WebApi.Pay.Models.BoHai
{
    public class VisSupportBankQryRetModel : BoHaiReturnModel
    {
        /// <summary>
        /// 支持的银行列表
        /// </summary>
        public List<SupportBankList> List { get; set; }
    }

    public class SupportBankList
    {
        /// <summary>
        /// 银行编号
        /// </summary>
        public string BankNo { get; set; }

        /// <summary>
        /// 银行名称
        /// </summary>
        public string BankName { get; set; }
    }
}