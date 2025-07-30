using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KC.WebApi.Pay.Models.BoHai
{
    /// <summary>
    /// 银行卡绑定支持信息查询
    /// 根据卡号查询该卡所属银行，银行编号，卡类型和是否支持绑定等信息
    /// </summary>
    public class VisCardSupportInfoQryModel : BoHaiBaseModel
    {
        /// <summary>
        /// 绑定卡号
        /// </summary>
        public string AcctNo { get; set; }
    }
}