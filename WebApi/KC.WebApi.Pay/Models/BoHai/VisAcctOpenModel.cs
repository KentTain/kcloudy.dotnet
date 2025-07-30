using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KC.WebApi.Pay.Models.BoHai
{
    /// <summary>
    /// 开通账户
    /// 通过账户开通交易可以给每个交易平台用户开通不同类型的互联网合作平台账户，并为互联网合作平台账户绑定相应的实体账户。
    /// 在开通互联网合作平台账户的同时需要设置相应的交易密码，如果交易平台用户选择使用动态口令的方式进行安全认证，
    /// 在开通账户的同时为该客户绑定的手机号开通动态口令验证功能，该账户支持余额冻结解冻和不同账户之间的转账,提现，充值等功能。
    /// </summary>
    public class VisAcctOpenModel : BoHaiBaseModel
    {
        /// <summary>
        /// 平台会员号 32
        /// </summary>
        public string MerUserId { get; set; }

        /// <summary>
        /// 平台会员账户类型 01:基本户 03:结算户
        /// </summary>
        public string VirlAcctType { get; set; }

        /// <summary>
        /// 虚拟客户对公对私标识  1:个人 2:企业 3:同业（同一虚客户，首次开通虚账户时必输）
        /// </summary>
        public string UserType { get; set; }

        /// <summary>
        /// 虚拟客户名称 （同一虚客户，首次开通虚账户时必输）
        /// </summary>
        public string EleCustomerName { get; set; }

        /// <summary>
        /// 开通虚账户户名 （同一虚客户，首次开通虚账户时必输）
        /// </summary>
        public string EleAcctName { get; set; }

        /// <summary>
        /// 手机号码 （同一虚客户，首次开通虚账户时必输）
        /// </summary>
        public string MobilePhone { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string EleMail { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        //public string EleAddress { get; set; }

        /// <summary>
        /// 性别 0：女 1：男
        /// </summary>
        //public string EleSex { get; set; }

        /// <summary>
        /// 国籍
        /// </summary>
        //public string EleNationality { get; set; }

        /// <summary>
        /// 邮编
        /// </summary>
        //public string ElePostcode { get; set; }

        /// <summary>
        /// 设置虚账户交易密码 Des加密密文
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 证件类型01 身份证
        /// </summary>
        public string CertType { get; set; }

        /// <summary>
        /// 证件号码
        /// </summary>
        public string CertNo { get; set; }

        /// <summary>
        /// 统一社会信用代码 （对公必输UserType=2）
        /// </summary>
        public string BusPermitId { get; set; }

        /// <summary>
        /// 法人姓名
        /// </summary>
        public string BossName { get; set; }

        /// <summary>
        /// 卡密码 如果PwdCheckFlag为不0时必输
        /// </summary>
        public string CardPassword { get; set; }

        /// <summary>
        /// 如果绑定账户为渤海个人卡必输 0：不验证 1：验证 默认验证
        /// </summary>
        public string PwdCheckFlag { get; set; }

        /// <summary>
        /// 绑定实体账户账号 
        /// </summary>
        public string AcctNo { get; set; }

        /// <summary>
        /// 绑定实体账户户名 (行内账户 非必输)
        /// </summary>
        public string AcctName { get; set; }

        /// <summary>
        /// 绑定实体账户开户行号 行内账户非必输
        /// </summary>
        public string AcctBankId { get; set; }

        /// <summary>
        /// 绑定实体账户开户行名 行内账户非必输
        /// </summary>
        public string AcctBankName { get; set; }

        /// <summary>
        /// 绑定实体账户账户类型 D:借记卡 
        /// </summary>
        public string AcctType { get; set; }

        /// <summary>
        /// 绑定实体账户行内外标识  0:行外1:行内如果不上送该字段值，则根据卡bin信息自动处理
        /// </summary>
        public string AcctBHFlag { get; set; }

        /// <summary>
        /// 绑定实体账户为信用卡时的CVV
        /// </summary>
        public string CVV { get; set; }

        /// <summary>
        /// 绑定实体账户为信用卡时的卡有效日期  0117 月+年
        /// </summary>
        public string ExpiryDate { get; set; }

        /// <summary>
        /// 所属机构
        /// </summary>
        public string OpenBrc { get; set; }

        /// <summary>
        /// 推荐人代码
        /// </summary>
        public string ReferNo { get; set; }

        /// <summary>
        /// 是否开通添金宝 0：不开 1;开 为空的话默认不开通
        /// </summary>
        public string FundFlag { get; set; }

        /// <summary>
        /// 产品代码 BC001
        /// </summary> 
        public string ProdCode { get; set; }

        /// <summary>
        /// 鉴权编号 由四要素鉴权（发送验证码）接口返回，绑定非渤海卡必输，用于对私开户
        /// </summary>
        public string OrderId { get; set; }

        /// <summary>
        /// 短信验证码 由四要素鉴权接口发送短信验证码，用于对私开户;PwdCheckFlag字段为0时需上送
        /// </summary>
        public string VerifyNo { get; set; }

        /// <summary>
        /// 代扣通道
        /// </summary>
        public string PPayMerId { get; set; }


    }
}