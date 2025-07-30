using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KC.WebApi.Pay.Models.BoHai
{
    /// <summary>
    /// 转账
    /// 系统校验收款方和付款方账户信息和状态，
    /// 校验通过后可以实现平台的客户的互联网合作平台账户间进行转账的业务交易结果可以通过交易查询查看。
    /// </summary>
    public class VisTransferModel : BoHaiBaseModel
    {
        /// <summary>
        /// 付款平台会员号
        /// </summary>
        public string PayMerUserId { get; set; }

        /// <summary>
        /// 付款平台会员账户类型 01:基本户 02:保证金户 03:结算户
        /// </summary>
        public string PayVirlAcctType { get; set; }

        /// <summary>
        /// 收款平台会员号
        /// </summary>
        public string RcvMerUserId { get; set; }

        /// <summary>
        /// 收款平台会员账户类型
        /// </summary>
        public string RcvVirlAcctType { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public string TransAmount { get; set; }

        /// <summary>
        /// 交易密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 是否验证密码 非1(默认0)：验密   1：不验密(Flag或VerifyFlag必须有一个要验证)
        /// </summary>
        public string Flag { get; set; }

        /// <summary>
        /// 币种 人民币：CNY
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// 短信验证码 
        /// </summary>
        public string VerifyNo { get; set; }

        /// <summary>
        /// 是否验证短息 0：不验证 1：验证(Flag或VerifyFlag必须有一个要验证)
        /// </summary>
        public string VerifyFlag { get; set; }

        /// <summary>
        /// 附言
        /// </summary>
        public string Remark1 { get; set; }
    }
}