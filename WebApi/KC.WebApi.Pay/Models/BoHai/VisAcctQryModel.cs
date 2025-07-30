using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KC.WebApi.Pay.Models.BoHai
{
    /// <summary>
    /// 开户信息查询
    /// 商户上送开户时返回的平台会员号和账户类型，
    /// 通过开户信息查询交易可以查询平台会员在资金监管系统所有开立的互联网合作平台账户和类型以及互联网合作平台账户状态及绑定卡等信息。
    /// </summary>
    public class VisAcctQryModel : BoHaiBaseModel
    {
        /// <summary>
        /// 平台会员号
        /// </summary>
        public string MerUserId { get; set; }
    }
}