using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KC.WebApi.Pay.Models.BoHai
{
    /// <summary>
    /// 提现
    /// 通过提现交易可实时提取互联网合作平台账户余额账户至绑定账户，实际额度和可用额度实时减少。
    /// </summary>
    public class VisTransferOutModel : BoHaiBaseModel
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
        /// 金额
        /// </summary>
        public string TransAmount { get; set; }

        /// <summary>
        /// 币种  人民币：CNY
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// 交易密码Flag为非1时必输  注：Des加密密文
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 非1(默认0)：验密   1：不验密(Flag或VerifyFlag必须有一个要验证)
        /// </summary>
        public string Flag { get; set; }

        /// <summary>
        /// 绑定卡行号  行外卡提现上送
        /// </summary>
        public string AcctBankId { get; set; }

        /// <summary>
        /// 绑定卡行名 行外卡提现上送
        /// </summary>
        public string AcctBankName { get; set; }

        /// <summary>
        /// 短信验证码
        /// </summary>
        public string VerifyNo { get; set; }

        /// <summary>
        /// 是否验证短息 0：不验证 1：验证(Flag或VerifyFlag必须有一个要验证)
        /// </summary>
        public string VerifyFlag { get; set; }

        /// <summary>
        /// 提现业务模式 0：T+0模式提现，有1w份额限额； 1：T+1模式提现，无份额限额
        /// </summary>
        public string BizType { get; set; }
    }
}