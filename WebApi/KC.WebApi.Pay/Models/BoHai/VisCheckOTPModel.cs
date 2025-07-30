using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KC.WebApi.Pay.Models.BoHai
{
    /// <summary>
    /// 校验验证码 可以对短信验证码进行验证，在进行充值、提现、转账等交易时调用。
    /// </summary>
    public class VisCheckOTPModel : BoHaiBaseModel
    {
        /// <summary>
        /// 手机号
        /// </summary>
        public string MobilePhone { get; set; }

        /// <summary>
        /// 动态密码
        /// </summary>
        public string VerifyNo { get; set; }
    }
}