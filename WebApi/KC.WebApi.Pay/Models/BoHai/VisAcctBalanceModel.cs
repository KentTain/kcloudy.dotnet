using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KC.WebApi.Pay.Models.BoHai
{
    /// <summary>
    /// 账户余额查询
    /// 商户上送平台会员号，通过余额查询交易，可以查询账户的份额、可用余额、冻结金额，待结算金额
    /// </summary>
    public class VisAcctBalanceModel : BoHaiBaseModel
    {
        /// <summary>
        /// 平台会员号   客户账户必输平台账户送空
        /// </summary>
        public string MerUserId { get; set; }

        /// <summary>
        /// 平台会员账户类型  客户账户类型或者平台账户类型 客户类型：01:普通账户 03:结算户    平台账户： 众信：80:众信消费
        /// </summary>
        public string VirlAcctType { get; set; }

    }
}