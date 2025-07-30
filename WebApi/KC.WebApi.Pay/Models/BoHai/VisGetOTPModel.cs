using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KC.WebApi.Pay.Models.BoHai
{
    /// <summary>
    /// 获取验证码  
    /// 通过获取验证码交易，银行可以向操作的客户发送短信验证码，并且在进行充值、提现、转账等交易时进行验证。
    /// </summary>
    public class VisGetOTPModel:BoHaiBaseModel
    {
        /// <summary>
        /// 平台会员号
        /// </summary>
        public string MerUserId { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public string MobilePhone { get; set; }

        /// <summary>
        /// 短信内容 必输
        /// </summary>
        public string Msg { get; set; }
    }
}