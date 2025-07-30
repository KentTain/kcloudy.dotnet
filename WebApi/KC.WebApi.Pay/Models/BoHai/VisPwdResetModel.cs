using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KC.WebApi.Pay.Models.BoHai
{
    /// <summary>
    /// 支付密码重置
    /// 遗忘支付密码的情况下，进行四要素鉴权通过后可以通过此接口进行交易密码重置。
    /// </summary>
    public class VisPwdResetModel : BoHaiBaseModel
    {
        /// <summary>
        /// 平台会员号
        /// </summary>
        public string MerUserId { get; set; }

        /// <summary>
        /// 平台会员账户类型  01:基本户 03:结算户
        /// </summary>
        public string VirlAcctType { get; set; }

        /// <summary>
        /// 新交易密码  注：Des加密密文
        /// </summary>
        public string NewPassword { get; set; }

        /// <summary>
        /// 卡密码 如果绑定账户为渤海个人卡必输  PwdCheckFlag字段为1时验证卡密码
        /// </summary>
        public string CardPassword { get; set; }

        /// <summary>
        /// 卡密码校验标志  如果绑定账户为渤海个人卡必输 0：不验证 1：验证 默认验证
        /// </summary>
        public string PwdCheckFlag { get; set; }

        /// <summary>
        /// 绑定实体账户账号 
        /// </summary>
        public string AcctNo { get; set; }

        /// <summary>
        /// 绑定实体账户户名 
        /// </summary>
        public string AcctName { get; set; }

        /// <summary>
        /// 绑定实体账户行内外标识  0:行外 1:行内如果不上送该字段值，则根据卡bin信息自动处理
        /// </summary>
        public string AcctBHFlag { get; set; }

        /// <summary>
        /// 绑定实体账户为信用卡时的CVV
        /// </summary>
        public string CVV { get; set; }

        /// <summary>
        /// 绑定实体账户为信用卡时的卡有效日期 
        /// </summary>
        public string ExpiryDate { get; set; }

        /// <summary>
        /// 绑定实体账户预留手机号
        /// </summary>
        public string MobilePhone { get; set; }

        /// <summary>
        /// 证件类型 01 身份证
        /// </summary>
        public string CertType { get; set; }

        /// <summary>
        /// 证件号码
        /// </summary>
        public string CertNo { get; set; }

        /// <summary>
        /// 虚拟客户对公对私标识 1:个人
        /// </summary>
        public string UserType { get; set; }

        /// <summary>
        /// 短信验证码  由四要素鉴权接口发送短信验证码，用于对私开户;PwdCheckFlag字段为0时需上送
        /// </summary>
        public string VerifyNo { get; set; }

        /// <summary>
        /// 鉴权编号 由四要素鉴权接口返回，绑定行外卡（非渤海卡）必输 用于对私开户
        /// </summary>
        public string OrderId { get; set; }
    }
}