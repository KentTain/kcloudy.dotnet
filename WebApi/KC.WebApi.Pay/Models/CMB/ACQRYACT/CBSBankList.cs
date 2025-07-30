using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace KC.WebApi.Pay.Models.CMB
{
    public class CBSBankList
    {
        /// <summary>
        /// 银行账号
        /// </summary>
        [DataMember]
        public string AccountNumber { get; set; }

        /// <summary>
        /// 付款账号名
        /// </summary>
        [DataMember]
        public string AccountName { get; set; }


        /// <summary>
        /// 银行类型
        /// </summary>
        public string BankType { get; set; }

        /// <summary>
        /// 开户行
        /// </summary>
        public string OpenBank { get; set; }

        /// <summary>
        /// 付方客户号
        /// </summary>
        [DataMember]
        public string CLTNBR { get; set; }
    }
}