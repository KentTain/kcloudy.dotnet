using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Service.Pay.Constants
{
    /// <summary>
    /// 中金交互的方法名
    /// </summary>
    public class CPCNMethodConstants
    {
        private const string ControllerName = "CPCNPayApi";

        public const string OpenAccount = ControllerName + "/OpenAccount";

        public const string BankAuthenticationAppliction = ControllerName + "/BankAuthenticationAppliction";

        public const string BankAuthentication = ControllerName + "/BankAuthentication";

        public const string BindBankAccount = ControllerName + "/BindBankAccount";

        public const string SearchAccountBalance = ControllerName + "/SearchAccountBalance";

        public const string PaymentBankInfo = ControllerName + "/PaymentBankInfo";

        public const string FreezeAmt = ControllerName + "/FreezeAmt";

        public const string OrderPay = ControllerName + "/OrderPay";

        public const string Notice = ControllerName + "/Notice";

        public const string OrderPayment = ControllerName + "/OrderPayment";

        public const string InTransaction = ControllerName + "/InTransaction";

        public const string InTransactionSearch = ControllerName + "/InTransactionSearch";

        public const string OutTransaction = ControllerName + "/OutTransaction";

        public const string SearchWithdrawAmt = ControllerName + "/SearchWithdrawAmt";

        public const string Charge = ControllerName + "/Charge";

        public const string QRInTransaction = ControllerName + "/QRInTransaction";

        public const string SubmitCharge = ControllerName + "/SubmitCharge";
    }

    public class FinancingMethodConstants
    {
        public const string AreaName = "CreditManage";
        public const string ControllerName = "RepayManage";
        /// <summary>
        /// 主动还款
        /// </summary>
        public const string ActionName = "Repayment";

        public const string ActionConfirName = "RepaymentConfirmation";


    }


    public class VoucherMethodConstants
    {
        public const string AreaName = "CreditManage";
        public const string ControllerName = "RepayManage";
        public const string ActionName = "CertificatePreview";
        public const string ActionNameForShop = "CertificatePreviewForShop";


    }
}
