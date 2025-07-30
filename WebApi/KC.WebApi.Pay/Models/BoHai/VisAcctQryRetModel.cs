using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KC.WebApi.Pay.Models.BoHai
{
    public class VisAcctQryRetModel : BoHaiReturnModel
    {
        /// <summary>
        /// 虚拟客户号
        /// </summary>
        public string EleCifNo { get; set; }

        /// <summary>
        /// 平台号
        /// </summary>
        public string MerchantId { get; set; }

        /// <summary>
        /// 平台会员号
        /// </summary>
        public string MerUserId { get; set; }

        /// <summary>
        /// 虚拟客户姓名
        /// </summary>
        public string CustomerName { get; set; }

        /// <summary>
        /// 对公对私标识1:个人 2:企业 3:同业
        /// </summary>
        public string UserType { get; set; }

        /// <summary>
        /// 客户状态
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 绑定实体账户账号
        /// </summary>
        public string AcctNo { get; set; }

        /// <summary>
        /// 绑定实体账户户名
        /// </summary>
        public string AcctName { get; set; }

        /// <summary>
        /// 绑定实体账户开户行号
        /// </summary>
        public string AcctBankId { get; set; }

        /// <summary>
        /// 绑定实体账户账户类型 D:借记卡 C:贷记卡  B:折
        /// </summary>
        public string AcctType { get; set; }

        /// <summary>
        /// 绑定实体账户行内外标识 0:行外 1:行内
        /// </summary>
        public string AcctBHFlag { get; set; }

        /// <summary>
        /// 绑定实体账户款项代码
        /// </summary>
        public string AcctSubAcctNo { get; set; }

        /// <summary>
        /// 绑定实体账户核心客户号
        /// </summary>
        public string AcctCifNo { get; set; }

        /// <summary>
        /// 绑定实体账户为信用卡时的CVV
        /// </summary>
        public string CVV { get; set; }

        /// <summary>
        /// 绑定实体账户为信用卡时的卡有效日期
        /// </summary>
        public string ExpiryDate { get; set; }

        /// <summary>
        /// 证件类型（主要对私）
        /// </summary>
        public string CertType { get; set; }

        /// <summary>
        /// 证件号码（主要对私）
        /// </summary>
        public string CertNo { get; set; }

        /// <summary>
        /// 统一社会信用代码
        /// </summary>
        public string BusPermitId { get; set; }

        /// <summary>
        /// 绑定实体账户预留手机号
        /// </summary>
        public string MobilePhone { get; set; }

        /// <summary>
        /// 所属机构
        /// </summary>
        public string OpenBrc { get; set; }

        /// <summary>
        /// 绑定卡行名
        /// </summary>
        public string AcctBankName { get; set; }

        /// <summary>
        /// 交易信息
        /// </summary>
        public List<TransInfoModel> TransInfoList { get; set; }
    }
}