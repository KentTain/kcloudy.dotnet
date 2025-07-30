using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace KC.WebApi.Pay.Models
{
    /// <summary>
    /// 中金支付返回的资金model
    /// </summary>
    [Serializable, DataContract(IsReference = true)]
    public class ReturnAmtDTO
    {
        /// <summary>
        /// 资金余额(单位:分)
        /// </summary>
        [DataMember]
        public decimal BalanceAmt { get; set; }

        /// <summary>
        /// 可用资金(单位:分)
        /// </summary>
        [DataMember]
        public decimal UseAmt { get; set; }

        /// <summary>
        /// 冻结资金(单位:分)
        /// </summary>
        [DataMember]
        public decimal FreezeAmt { get; set; }

        /// <summary>
        /// T1 正常出金（A00）时的额度(单位:分)
        /// </summary>
        [DataMember]
        public decimal T1CtAmtA00 { get; set; }


        /// <summary>
        /// T1 解冻出金（B01）时的额度(单位:分) 暂时不使用
        /// </summary>
        [DataMember]
        public decimal T1CtAmtB01 { get; set; }


        /// <summary>
        /// T0 正常出金（A00）时的额度(单位:分)
        /// </summary>
        [DataMember]
        public decimal T0CtAmtA00 { get; set; }


        /// <summary>
        /// T0 解冻出金（B01）时的额度(单位:分) 暂时不使用
        /// </summary>
        [DataMember]
        public decimal T0CtAmtB01 { get; set; }

        /// <summary>
        /// 可提现金额
        /// </summary>
        [DataMember]
        public decimal TxAviAmt { get; set; }
    }
}