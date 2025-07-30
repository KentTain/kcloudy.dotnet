using System;
using System.Collections.Generic;
using KC.Service.DTO.Dict;
using KC.Service.DTO;
using KC.Service.DTO.Pay;
using KC.Enums.Pay;
using KC.Service;

namespace KC.Service.Pay.WebApiService.Platform
{
    public interface IFinanceApiService
    {
        ServiceResult<bool> ApplyForRecharge(decimal amount, string orderid, bool? state, string errormessage = "");
        ServiceResult<Tuple<bool, string>> ApplyForWithdrawal(decimal amount, string billNo, bool? state, string errormessage = "", int paymentdate = 0);
        ServiceResult<Tuple<decimal, decimal>> GetAssetSummary();
        ServiceResult<PaginatedBaseDTO<ElectronicBillDTO>> GetBill(string customer, string num, DateTime? startDate, DateTime? endDate, int pageIndex = 1, int pageSize = 10);
        ServiceResult<PaginatedBaseDTO<PaymentRecordDTO>> GetCashExpenditure(string vendor, string num, DateTime? startDate, DateTime? endDate, int pageIndex = 1, int pageSize = 10);
        ServiceResult<PaginatedBaseDTO<PaymentRecordDTO>> GetCashReceipts(string customer, string num, DateTime? startDate, DateTime? endDate, int pageIndex = 1, int pageSize = 10);
        ServiceResult<ThirdPartyType> GetCashUsageDetailType(string orderId);
        ServiceResult<List<ElectronicBillRecordsDTO>> GetElectronicBillRecords(string orderId);
        ServiceResult<List<ElectronicBillRecordsDTO>> GetElectronicBillRecordsOfAccountStatement(Guid id);
        ServiceResult<decimal> GetFrozenByOrderId(string orderId);
        ServiceResult<decimal> GetNeedPayServiceFeeAmount();
        ServiceResult<PaginatedBaseDTO<OnlinePaymentRecordDTO>> GetOnlinePaymentRecordByPage(DateTime? beginTime, DateTime? endTime, string memberId = "", string peeMemberId = "", int pageIndex = 1, int pageSize = 10);
        ServiceResult<PaginatedBaseDTO<OnlinePaymentRecordDTO>> GetOnlinePaymentRecordsByFilter(DateTime? beginTime, DateTime? endTime, bool isExpended, string memberId = "", List<string> memberIds = null, int pageIndex = 1, int pageSize = 10);
        ServiceResult<PaginatedBaseDTO<OnlinePaymentRecordDTO>> GetOnlinePaymentRecordsTop10(string memberId, int pageIndex = 1, int pageSize = 10);
        ServiceResult<List<JsonDictionaryDTO>> GetOrderStillNeedPayAmount(List<string> orders);
        ServiceResult<PaymentBankAccountDTO> GetPlatformAccount(ThirdPartyType thirdPartyType);
        ServiceResult<PaginatedBaseDTO<PlatformServiceFeeDTO>> GetPlatformServiceFeeRecords(bool? isPay, DateTime? startDate, DateTime? endDate, int pageIndex = 1, int pageSize = 10);
        ServiceResult<PaginatedBaseDTO<ChargeFlowDTO>> GetRechargeRecords(DateTime? startDate, DateTime? endDate, RechargeStatus? status, int pageIndex = 1, int pageSize = 10);
        ServiceResult<decimal> GetTodayWithdrawAmount();
        ServiceResult<PaginatedBaseDTO<ElectronicBillDTO>> GetUsedBill(string vendor, string num, DateTime? startDate, DateTime? endDate, int pageIndex = 1, int pageSize = 10);
        ServiceResult<PaginatedBaseDTO<ChargeFlowDTO>> GetWithdrawalsRecords(DateTime? startDate, DateTime? endDate, WithdrawalsStatus? status, int pageIndex = 1, int pageSize = 10);
        ServiceResult<List<WithdrawRuleDTO>> GetWithdrawRule();
        ServiceResult<PaginatedBaseDTO<JsonDictionaryDTO>> LoadFrozenRecords(int pageIndex = 1, int pageSize = 10, string order = "asc");
        ServiceResult<PaginatedBaseDTO<CashUsageDetailDTO>> LoadTransactionFlows(CashType? type, TradingAccount? account, DateTime? startDate, DateTime? endDate, int pageIndex = 1, int pageSize = 10);
        ServiceResult<List<ElectronicBillRecordsDTO>> QueryPlatElectronicBill(DateTime startDate, DateTime endDate, string supplierId, string buyerId, List<string> alreadyReconcileds);
        ServiceResult<List<UnionBankNumberDTO>> QueryUnionBankNumber(int bankId, int cityCode);
        ServiceResult<bool> QueryWaitSigninBill();
        ServiceResult<bool> ReleaseFreezeCapital(decimal amount, string orderId);
    }
}