using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Service.Pay.Constants
{
    /// <summary>
    /// 中金支付接口名称常量
    /// </summary>
    public class CPCNInterfaceConstants
    {
        /// <summary>
        /// 开销户
        /// </summary>
        public const string OpenAccount = "T1001";

        /// <summary>
        /// 开户结果查询
        /// </summary>
        public const string OpenAccountSearch = "T1002";

        /// <summary>
        /// 结算账户维护
        /// </summary>
        public const string SettlementAccount = "T1004";

        /// <summary>
        /// 账户余额查询
        /// </summary>
        public const string FundAccountBalance = "T1005";

        /// <summary>
        /// 查询可代付出金额度[T1018]
        /// </summary>
        public const string WithdrawAmt = "T1018";

        /// <summary>
        /// 支付行信息模糊查询[T1017]
        /// </summary>
        public const string PaymentBankInfo = "T1017";

        /// <summary>
        /// 企业-企业账户认证(打款认证)-申请[T1131]
        /// </summary>

        public const string BankAuthenticationApplication = "T1131";

        /// <summary>
        /// 企业-企业账户认证(打款认证)-验证[T1132]
        /// </summary>
        public const string BankAuthentication = "T1132";

        /// <summary>
        /// 冻结/解冻[T3001]
        /// </summary>
        public const string FreezeAmt = "T3001";

        /// <summary>
        /// 订单支付[T3004]
        /// </summary>
        public const string OrderPay = "T3004";

        /// <summary>
        /// 网银入金
        /// </summary>
        public const string InTransaction = "T2006";

        /// <summary>
        /// 网银入金查询
        /// </summary>
        public const string InTransactionSearch = "T2012";

        /// <summary>
        /// 出金
        /// </summary>
        public const string OutTransaction = "T2004";

        /// <summary>
        /// 扫码入金
        /// </summary>
        public const string QRPayIn = "T4031";

        /// <summary>
        /// 扫码入金查询
        /// </summary>
        public const string QRPayInSearch = "T4033";
    }
}
