using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KC.WebApi.Pay.Models.BoHai
{
    /// <summary>
    /// 账户冻结/解冻
    /// 通过账户资金冻结,可以实时冻结客户账户的可用余额，冻结金额要求大于零并且不大于账户余额
    /// </summary>
    public class VisAccFreezeModel : BoHaiBaseModel
    {
        /// <summary>
        /// 平台会员号 
        /// </summary>
        public string MerUserId { get; set; }

        /// <summary>
        /// 平台会员账户类型 01:基本户 02:保证金户 03:结算户
        /// </summary>
        public string VirlAcctType { get; set; }

        /// <summary>
        /// 冻结解冻标识 1-冻结 2-解冻
        /// </summary>
        public string FrzenFlag { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public string TransAmount { get; set; }

        /// <summary>
        /// 币种CNY 人民币：CNY
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// 交易密码  Flag为非1时必输 注：Des加密密文
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 是否验证密码 非1(默认0)：验密   1：不验密
        /// </summary>
        public string Flag { get; set; }

        /// <summary>
        /// 冻结码 FrzenFlag为2-解冻时必输 
        /// </summary>
        public string FrzenCode { get; set; }
    }
}