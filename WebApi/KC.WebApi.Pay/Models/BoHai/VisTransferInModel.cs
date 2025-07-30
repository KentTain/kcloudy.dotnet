using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KC.WebApi.Pay.Models.BoHai
{
    /// <summary>
    /// 充值
    /// 通过线上充值交易可对账户余额账户进行充值，充值的实体账户只能是互联网合作平台账户的绑定账户，
    /// 互联网合作平台账户的可用额度和总额度实时生效。
    /// </summary>
    public class VisTransferInModel : BoHaiBaseModel
    {
        /// <summary>
        /// 平台会员号
        /// </summary>
        public string MerUserId { get; set; }

        /// <summary>
        /// 平台会员账户类型 01:基本户02:保证金户03:结算户
        /// </summary>
        public string VirlAcctType { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public string TransAmount { get; set; }

        /// <summary>
        /// 币种 人民币：CNY
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// 交易密码 Flag为非1时必输 注：Des加密密文
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 是否验证密码 非1(默认0)：验密   1：不验密(Flag或VerifyFlag必须有一个要验证)
        /// </summary>
        public string Flag { get; set; }

        /// <summary>
        /// 待结算标志 是否入待结算金额 0或空：入 ，1：不入
        /// </summary>
        public string SettleFlag { get; set; }

        /// <summary>
        /// 短信验证码 
        /// </summary>
        public string VerifyNo { get; set; }

        /// <summary>
        /// 是否验证短息 0：不验证 1：验证(Flag或VerifyFlag必须有一个要验证)
        /// </summary>
        public string VerifyFlag { get; set; }

    }
}