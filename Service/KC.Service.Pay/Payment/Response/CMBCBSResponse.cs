using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KC.Service.Pay.Response
{
    public class CBSAccount
    {
        public string CompanyName { get; set; }

        public string BankNumber { get; set; }

        public string BankAddress { get; set; }

        public string BankType { get; set; }

        public string OpenBankName { get; set; }
    }

    public class CMBCBSResponse
    {
        [DataMember]
        public string erpPaymentId { get; set; }

        [DataMember]
        public CBSState state { get; set; }

        [DataMember]
        public string cbs_comment { get; set; }

        [DataMember]
        public string sign { get; set; }
    }

    /// <summary>
    /// 通过企业名查询银行账号信息返回结果
    /// </summary>
    public class CBSQueryBankAccountInfoResponse
    {
        public string code { get; set; }

        public string description { get; set; }


        public List<CBSBankAccountInfo> result { get; set; }

    }
    /// <summary>
    /// 请求委托支付返回结果
    /// </summary>
    public class CBSPayResponse
    {
        public string code { get; set; }

        public string description { get; set; }

    }
    /// <summary>
    /// CBS银行账户信息
    /// </summary>
    public class CBSBankAccountInfo
    {
        /// <summary>
        /// 银行账号
        /// </summary>
        public string bankAccount { get; set; }

        /// <summary>
        /// 银行类型
        /// </summary>
        public string bankType { get; set; }

        /// <summary>
        /// 账号名称
        /// </summary>
        public string accountName { get; set; }

        /// <summary>
        /// 开户行
        /// </summary>
        public string openBankName { get; set; }

        /// <summary>
        /// 开户行所在的省
        /// </summary>
        public string openAccountProvince { get; set; }

        /// <summary>
        /// 开户行所在的市
        /// </summary>
        public string openAccountCity { get; set; }

    }

    public enum CBSState
    {
        /// <summary>
        /// 等待CBS系统读取数据
        /// </summary>
        [Description("等待CBS系统读取数据")]
        Available,

        /// <summary>
        /// CBS系统已读取数据
        /// </summary>
        [Description("CBS系统已读取数据")]
        Delegated,

        /// <summary>
        /// CBS系统已经受理
        /// </summary>
        [Description("CBS系统已经受理")]
        Accepted,

        /// <summary>
        /// 付款成功
        /// </summary>
        [Description("付款成功")]
        Success,

        /// <summary>
        /// 所有非成功的状态
        /// </summary>
        [Description("所有非成功的状态")]
        Failed
    }

}