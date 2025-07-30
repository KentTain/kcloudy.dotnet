using System.Collections.Generic;
using KC.Service.DTO.Config;
using KC.Service.DTO.Dict;
using KC.Service.DTO.Pay;
using KC.Enums.Pay;
using KC.Framework.Base;
using KC.Model.Pay;
using KC.Service;

namespace KC.Service.Pay.WebApiService.Platform
{
    public interface IPaymentApiService
    {
        ServiceResult<bool> BindBankAccount(BindBankAccountDTO entity);
        ServiceResult<bool> CheckDefalutPayOpen(string memberId);
        ServiceResult<BankAccountDTO> GetBankAccountById(int bankAccountId);
        ServiceResult<List<BankAccountDTO>> GetBankAccountByMemberId(string memberId, string searchInfo);
        ServiceResult<BankAccountDTO> GetBindBankAccount(string memberId);
        ServiceResult<ConfigEntityDTO> GetConfigEntityBySign(int sign, ConfigType type);

        ServiceResult<List<SupplyBankDTO>> GetSupplyBanksByBusiType(string busiType);
        ServiceResult<List<StdProvinceDTO>> GetStrandardProvinceList();
        ServiceResult<List<StdCityDTO>> GetStrandardCityList(string provinceCode);
        ServiceResult<List<BF_ZD_PAYBANKDTO>> GetPayBankList(string cityCode, string bankCode, string branchBankCode, string searchName);
        ServiceResult<UnitAuthenticationDTO> GetUnitAuthenticationByMemberName(string memberName);

        ServiceResult<List<ConfigEntity>> GetConfigsByType(ConfigType type);
        ServiceResult<List<BankAccountDTO>> GetMemberBankByBankNumber(string bankNumber);
        ServiceResult<OnlinePaymentRecordDTO> GetOnlinePaymentRecordByPaymentId(string paymentOrderId);
        ServiceResult<List<PaymentBankAccountDTO>> GetPaymentAccounts(string memberId);
        ServiceResult<PaymentBankAccountDTO> GetPaymentBankAccountByMemberId(string memberId, ThirdPartyType type = ThirdPartyType.CPCNConfigSign);
        ServiceResult<PaymentOperationLogDTO> GetPaymentInfo(string tenantName, ThirdPartyType thirdPartyType);
        ServiceResult<PaymentBankAccountDTO> GetPlatformAccount(ThirdPartyType thirdPartyType);
        ServiceResult<OnlinePaymentRecord> GetPlatPaymentRecord(OnlinePaymentRecord entity);

        ServiceResult<PaymentTradeRecordDTO> GetTradeRecordsByLastSuccessRecord(string memberId, PaymentOperationType operationType, int referenceId);
        ServiceResult<bool> IsOnlinePaymentSuccess(string paymentOrderId);
        ServiceResult<bool> IsOpenPayment(string tenantName);
        ServiceResult<bool> IsOpenPaymentPhone(string tenantName);
        ServiceResult<bool> RemoveBankById(int id);
        ServiceResult<bool> SaveBankAccount(BankAccountDTO bankAccount);
        ServiceResult<bool> SaveBankAuthenticationAppliction(BankAuthenticationApplicationDTO entity);
        ServiceResult<bool> SaveOpenAccount(OpenAccountDTO entity);
        ServiceResult<bool> SavePaymentPasswordById(int paymentInfoId, string password);
        ServiceResult<bool> SavePaymentPhoneById(int paymentInfoId, string phone);
        ServiceResult<bool> SavePaymentTradeRecords(PaymentTradeRecordDTO entity);
        ServiceResult<bool> UpdatePaymentRecord(OnlinePaymentRecord entity);
    }
}