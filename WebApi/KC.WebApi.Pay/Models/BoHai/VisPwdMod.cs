using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KC.WebApi.Pay.Models.BoHai
{
    /// <summary>
    /// 支付密码修改
    /// 通过交易密码修改交易可以修改账户的交易密码，需要校验原交易密码无误才能完成交易
    /// </summary>
    public class VisPwdMod : BoHaiBaseModel
    {
        /// <summary>
        /// 平台会员号
        /// </summary>
        public string MerUserId { get; set; }

        /// <summary>
        /// 平台会员账户类型 01:基本户 03:结算户
        /// </summary>
        public string VirlAcctType { get; set; }

        /// <summary>
        /// 原交易密码 注：Des加密密文
        /// </summary>
        public string OldPassword { get; set; }

        /// <summary>
        /// 新交易密码 注：Des加密密文
        /// </summary>
        public string NewPassword { get; set; }
    }
}