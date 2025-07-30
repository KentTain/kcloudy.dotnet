using System;
using System.Collections.Generic;
using System.Text;

namespace KC.Service.Constants
{
    /// <summary>
    /// 单号前缀
    /// </summary>
    public class SequenceNumberConstants
    {
        /// <summary>
        /// 物流编号
        /// </summary>
        public const string Logistics = "Logistics";
        /// <summary>
        /// 对账单编号
        /// </summary>
        public const string AccountStatementNo = "BNO";
        /// <summary>
        /// 对账单生成的合同编号
        /// </summary>
        public const string AccountStatementContractNo = "DZDHT-";
        /// <summary>
        /// 采购销售的交易合同
        /// </summary>
        public const string ContractNo = "JYHT-";
        /// <summary>
        /// 协议合同
        /// </summary>
        public const string XYHT = "XYHT-";
        /// <summary>
        /// 通用合同
        /// </summary>
        public const string TYHT = "TYHT-";

        /// <summary>
        /// 应付单号
        /// </summary>
        public const string PaymentNo = "PN";

        /// <summary>
        /// 付款编号
        /// </summary>
        public const string PaymentApply = "Pay";

        /// <summary>
        /// 提现编号
        /// </summary>
        public const string TX = "TX";

        /// <summary>
        /// 充值编号
        /// </summary>
        public const string CZ = "CZ";

        /// <summary>
        /// 白条编号
        /// </summary>
        public const string SC = "SC";
    }
}
