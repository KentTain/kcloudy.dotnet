using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KC.WebApi.Pay.Models.BoHai
{
    /// <summary>
    /// 账户信息绑定
    /// 商户上送需要变更的账户信息或者手机号信息，通过账户绑定信息变更交易平台对变更后的信息进行鉴权，
    /// 鉴权成功后可以对虚账户绑定实体账户、用户手机号等信息进行变更，并且将原绑定信息记录到历史数据中以供追踪客户操作痕迹。
    /// </summary>
    public class VisAcctBindingModel : BoHaiBaseModel
    {
        /// <summary>
        /// 平台会员号 --如果为商户则为商户号
        /// </summary>
        public string MerUserId { get; set; }

        /// <summary>
        /// 来源交易  XXBG:变更或绑定信息
        /// </summary>
        public string TransCodeIdx { get; set; }

        /// <summary>
        /// 平台会员账户类型，用于校验交易密码  (TransCodeIdx= XXBG必输) 01:基本户 02:保证金户 03:结算户
        /// </summary>
        public string VirlAcctType { get; set; }

        /// <summary>
        /// (TransCodeIdx= XXBG必输)来源交易类型  01:仅修改手机号 02:仅修改绑定账户信息 03:修改手机号及绑定账户
        /// </summary>
        public string TransCodeType { get; set; }

        /// <summary>
        /// 卡密码 如果绑定账户为渤海个人卡必输 非1(默认0)：验密   1：不验密
        /// </summary>
        public string CardPassword { get; set; }

        /// <summary>
        /// 卡密码校验标志  如果绑定账户为渤海个人卡必输 0：不验证 1：验证 默认验证
        /// </summary>
        public string PwdCheckFlag { get; set; }

        /// <summary>
        /// 绑定实体账户账号 (TransCodeIdx= XXBG&& TransCodeType=01 非必输)
        /// </summary>
        public string AcctNo { get; set; }

        /// <summary>
        /// 绑定实体账户户名 (TransCodeIdx= XXBG && TransCodeType=01 非必输)(行内账户 非必输)
        /// </summary>
        public string AcctName { get; set; }

        /// <summary>
        /// 绑定实体账户开户行号 (TransCodeIdx= XXBG && TransCodeType=01 非必输)行内账户非必输对私绑卡非必输
        /// </summary>
        public string AcctBankId { get; set; }

        /// <summary>
        /// 绑定实体账户开户行名 (TransCodeIdx= XXBG && TransCodeType=01 非必输)行内账户非必输对私绑卡非必输
        /// </summary>
        public string AcctBankName { get; set; }

        /// <summary>
        /// 绑定实体账户账户类型 (TransCodeIdx= XXBG && TransCodeType=01 非必输)D:借记卡C:贷记卡B:折
        /// </summary>
        public string AcctType { get; set; }

        /// <summary>
        /// 绑定实体账户行内外标识  (TransCodeIdx= XXBG && TransCodeType=01 非必输)0:行外1:行内
        /// </summary>
        public string AcctBHFlag { get; set; }

        /// <summary>
        /// 绑定实体账户为信用卡时的CVV   TransCodeIdx= XXBG && TransCodeType=01 非必输)
        /// </summary>
        public string CVV { get; set; }

        /// <summary>
        /// 绑定实体账户为信用卡时的卡有效日期 (TransCodeIdx= XXBG && TransCodeType=01 非必输)
        /// </summary>
        public string ExpiryDate { get; set; }

        /// <summary>
        /// 绑定实体账户预留手机号  (TransCodeIdx= XXBG && TransCodeType=02 非必输)
        /// </summary>
        public string MobilePhone { get; set; }

        /// <summary>
        /// 交易密码 (TransCodeIdx= XXBG必输) 注：Des加密密文
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 是否验证密码    非1(默认0)：验密   1：不验密
        /// </summary>
        public string Flag { get; set; }

        /// <summary>
        /// 是否修改用户ID  TransCodeType=1 或者3 时， 1：修改  空或者0：不修改
        /// </summary>
        public string IdUpFlag { get; set; }

        /// <summary>
        /// 短信验证码  由四要素鉴权接口发送短信验证码，用于对私开户;PwdCheckFlag字段为0时需上送
        /// </summary>
        public string VerifyNo { get; set; }

        /// <summary>
        /// 鉴权编号  由四要素鉴权（发送验证码）接口返回，绑定非渤海卡必输 ，用于对私开户
        /// </summary>
        public string OrderId { get; set; }

        /// <summary>
        /// 代扣通道
        /// </summary>
        public string PPayMerId { get; set; }
    }
}