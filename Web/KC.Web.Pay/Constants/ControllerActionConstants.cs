using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using KC.Web.Constants;

namespace KC.Web.Pay.Constants
{
    public sealed class ControllerName : ControllerNameBase
    {
        public const string OnlinePayment = "OnlinePayment";
        public const string BankAccount = "BankAccount";
        public const string CapitalManager = "CapitalManager";
        public const string Payment = "Payment";
    }
    public sealed class ActionName : ActionNameBase
    {
        public sealed class OnlinePayment
        {
            public const string Payment = "Payment";
            public const string QueryPaymentMethod = "QueryPaymentMethod";
            public const string CreatePayment = "CreatePayment";
            public const string GetOnlinePaymentRecords = "GetOnlinePaymentRecords";
            public const string GetCBSAccounts = "GetCBSAccounts";
            public const string CBSPaymentMethod = "CBSPaymentMethod";
            public const string PayBill = "PayBill";
            public const string GetCashAndBillPaymentAmount = "GetCashAndBillPaymentAmount";
            public const string PayBillOfAccountStatement = "PayBillOfAccountStatement";

            public const string GetCashAndBillPaymentAmountByAccountStatementId =
                "GetCashAndBillPaymentAmountByAccountStatementId";

            public const string LoadChargeFlow = "LoadChargeFlow";
            public const string Charge = "Charge";
            public const string ChargeFlow = "ChargeFlow";

        }

        public sealed class BankAccount
        {
            public const string BankInfo = "BankInfo";
            public const string SetPhone = "SetPhone";
            public const string SetPassword = "SetPassword";
            public const string PasswordManage = "PasswordManage";
            public const string UpdatePhone = "UpdatePhone";
            public const string UpdatePassword = "UpdatePassword";
            public const string AccountInfo = "AccountInfo";

            public const string AuthenApplication = "AuthenApplication";
            public const string BankAuthenthentication = "BankAuthenthentication";

            public const string BindBankAccount = "BindBankAccount";
            public const string LoadBankInfo = "LoadBankInfo";
            public const string GetBankForm = "GetBankForm";
            public const string SaveBankForm = "SaveBankForm";
            public const string RemoveBank = "RemoveBank";
            public const string GetBankAccountType = "GetBankAccountType";
            public const string GetCardType = "GetCardType";
            public const string OpenBoHaiPayment = "OpenBoHaiPayment";
            public const string BindingBoHaiBank = "BindingBoHaiBank";
        }

        public sealed class CapitalManager
        {
            //资金管理首页
            public const string AssetSummary = "AssetSummary";
            public const string QueryAssetSummary = "QueryAssetSummary";
            public const string OpenPayment = "OpenPayment";
            //收入/支出视图
            public const string GetCashIncomeData = "GetCashIncomeData";
            public const string GetCreditIncomeData = "GetCreditIncomeData";
            public const string GetCashExpenditureData = "GetCashExpenditureData";
            public const string GetCreditExpenditureData = "GetCreditExpenditureData";
            //充值
            public const string Charge = "Charge";
            public const string ChargeFlow = "ChargeFlow";
            public const string LoadChargeFlow = "LoadChargeFlow";
            public const string FindChargingList = "FindChargingList";
            //提现
            public const string Withdrawals = "Withdrawals";
            public const string WithdrawalsFlow = "WithdrawalsFlow";
            public const string ApplyForWithdrawal = "ApplyForWithdrawal";
            public const string LoadWithdrawalsFlow = "LoadWithdrawalsFlow";

            public const string ServiceFeeFlow = "ServiceFeeFlow'";
            public const string LoadServiceFeeFlow = "LoadServiceFeeFlow";
            public const string GetNeedPayServiceFeeAmount = "GetNeedPayServiceFeeAmount";
            //交易流水
            public const string TransactionFlow = "TransactionFlow";
            public const string LoadTransactionFlows = "LoadTransactionFlows";
            public const string LoadCashReceipts = "LoadCashReceipts";
            public const string LoadCashExpenditure = "LoadCashExpenditure";
            public const string LoadGetBill = "LoadGetBill";
            public const string LoadUsedBill = "LoadUsedBill";
            //应收应付
            public const string QueryReceivablesAndPayable = "QueryReceivablesAndPayable";
            public const string LoadLogs = "LoadLogs";
            //应收
            public const string Receivables = "Receivables";
            public const string LoadReceivables = "LoadReceivables";
            public const string ExportReceivables = "ExportReceivables";
            public const string LoadPendingReceivables = "LoadPendingReceivables";
            public const string RemindBuyerPayment = "RemindBuyerPayment";
            public const string ConfirmReceivables = "ConfirmReceivables";
            //应付
            public const string Payables = "Payables";
            public const string LoadPayables = "LoadPayables";
            public const string ExportPayables = "ExportPayables";
            public const string LoadPendingPayables = "LoadPendingPayables";
            public const string CancelPayable = "CancelPayable";
            public const string GetVoucherInfo = "GetVoucherInfo";
            public const string GetVoucherContent = "GetVoucherContent";
            //资金总额
            public const string CapitalSummary = "CapitalSummary";
            //冻结资金
            public const string LoadFrozenRecords = "LoadFrozenRecords";

            public const string UpdateApplication = "UpdateApplication";

            public const string GenerateVerfiyPhoneCode = "GenerateVerfiyPhoneCode";
            public const string GenerateVerfiyPhoneCodes = "GenerateVerfiyPhoneCodes";
            public const string SubmitUpPhone = "SubmitUpPhone";
            public const string SubmitUpPassword = "SubmitUpPassword";
            public const string SubmitSetPhone = "SubmitSetPhone";
            public const string SubmitSetPassword = "SubmitSetPassword";
            public const string SubmitInTransactionSearch = "SubmitInTransactionSearch";
            
            public const string IsOpenPaymentPhone = "IsOpenPaymentPhone";

            public const string GetChargeCodeForm = "GetChargeCodeForm";
            public const string TransactionPassword = "TransactionPassword";
            public const string AddChargeRecord = "AddChargeRecord";
            public const string CencelChargeRecord = "CencelChargeRecord";
            public const string CreateChargeDataUrl = "CreateChargeDataUrl";
            public const string UpChargeRecord = "UpChargeRecord";

            public const string CashReceipts = "CashReceipts";
            public const string CashExpenditure = "CashExpenditure";
            public const string GetBill = "GetBill";
            public const string UsedBill = "UsedBill";
            public const string UsedCredit = "UsedCredit";
            
            public const string LoadUsedCredit = "LoadUsedCredit";
            public const string LoadAllCustomers = "LoadAllCustomers";
            public const string LoadAllVendors = "LoadAllVendors";
            public const string GetRecentAwaitRepayments = "GetRecentAwaitRepayments";
            public const string GetRecentCreditUsageApplys = "GetRecentCreditUsageApplys";
            public const string GetRepaymentRemindAndInterestDetailList = "GetRepaymentRemindAndInterestDetailList";
            public const string GetPaginatedCreditUsageDetailsByFilter = "GetPaginatedCreditUsageDetailsByFilter";
            public const string LoadPayableTop5 = "LoadPayableTop5";
            public const string LoadReceivableTop5 = "LoadReceivableTop5";

            public const string GetReceivedAndPaidCautionMoney = "GetReceivedAndPaidCautionMoney";

            public const string GetCertificateDetails = "GetCertificateDetails";

            public const string RefreshBoHaiAccount = "RefreshBoHaiAccount";

        }

        public sealed class Payment
        {
            public const string LoadCFCAInfo = "LoadCFCAInfo";
            //票据支付
            public const string AddOfflineUsageBillRecord = "AddOfflineUsageBillRecord";
            public const string LoadOfflineUsageBills = "LoadOfflineUsageBills";
            public const string CancelOfflineUsageBill = "CancelOfflineUsageBill";
            public const string ReturnOfflineUsageBill = "ReturnOfflineUsageBill";
            public const string ConfirmOfflineUsageBill = "ConfirmOfflineUsageBill";

            //线下支付
            public const string AddOfflinePaymentRecord = "AddOfflinePaymentRecord";
            public const string LoadOfflinePayments = "LoadOfflinePayments";
            public const string CancelOfflinePayment = "CancelOfflinePayment";
            public const string ReturnOfflinePayment = "ReturnOfflinePayment";
            public const string ConfirmOfflinePayment = "ConfirmOfflinePayment";

            public const string QueryPayResult = "QueryPayResult";
            public const string CancelPayment = "CancelPayment";
            public const string CreatePaymentData = "CreatePaymentData";

            public const string GetElectronicBillRecords = "GetElectronicBillRecords";

            public const string GetElectronicBillRecordsOfAccountStatement =
                "GetElectronicBillRecordsOfAccountStatement";

            public const string GetOrderStillNeedPayAmount = "GetOrderStillNeedPayAmount";
            public const string GetOrderASPayAmount = "GetOrderASPayAmount";

            
            public const string AddVoucherPayment = "AddVoucherPayment";
            public const string LoadVoucherPaymentRecord = "LoadVoucherPaymentRecord";
            public const string CancelVoucher = "CancelVoucher";
            public const string CreateVoucher = "CreateVoucher";
            public const string ConfirmVoucher = "ConfirmVoucher";
            public const string ReturnVoucher = "ReturnVoucher";
            public const string GetPointRule = "GetPointRule";
            public const string UsePoint = "UsePoint";
            public const string GetGuaranteeAmount = "GetGuaranteeAmount";
            public const string SubmitEntrustedPayment = "SubmitEntrustedPayment";
            public const string GetEntrustedPayment = "GetEntrustedPayment";

        }
    }
}