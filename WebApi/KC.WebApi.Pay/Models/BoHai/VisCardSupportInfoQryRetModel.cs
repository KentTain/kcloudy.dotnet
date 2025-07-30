using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KC.WebApi.Pay.Models.BoHai
{
    public class VisCardSupportInfoQryRetModel : BoHaiReturnModel
    {
        /// <summary>
        /// 银行编号
        /// </summary>
        public string BankNo { get; set; }

        /// <summary>
        /// 行名
        /// </summary>
        public string BankName { get; set; }

        /// <summary>
        /// 卡类型 1:借记卡 2：信用卡
        /// </summary>
        public string CardType { get; set; }

        /// <summary>
        /// 支持绑定标志 1：支持  0：不支持 
        /// </summary>
        public string SupportFlag { get; set; }
    }
}