using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Model.Pay.Constants
{
    public sealed class Tables
    {
        private const string Prx = "pay_";

        public const string BankAccount = Prx + "BankAccount";
        public const string PaymentBankAccount = Prx + "PaymentBankAccount";

        public const string OnlinePaymentRecord = Prx + "OnlinePaymentRecord";
        public const string PaymentTradeRecord = Prx + "PaymentTradeRecord";

        public const string PaymentInfo = Prx + "PaymentInfo";

        public const string Payable = Prx + "Payable";
        public const string Receivable = Prx + "Receivable";

        public const string PaymentOperationLog = Prx + "PaymentOperationLog";
        public const string PaymentRecord = Prx + "PaymentRecord";
        public const string PayableAndReceivableRecord = Prx + "PayableAndReceivableRecord";
        public const string OfflinePayment = Prx + "OfflinePayment";
        public const string OfflineUsageBill = Prx + "OfflineUsageBill";
        public const string PaymentAttachment = Prx + "PaymentAttachment";
        public const string VoucherPaymentRecord = Prx + "VoucherPaymentRecord";
        public const string EntrustedPaymentRecord = Prx + "EntrustedPaymentRecord";
        public const string CashUsageDetail = Prx + "CashUsageDetail";

        public const string CautionMoneyLog = Prx + "CautionMoneyLog";
    }
}
