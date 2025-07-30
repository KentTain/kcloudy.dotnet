using System;
using System.Collections.Generic;
using KC.Enums.Pay;
using KC.Model.Pay;

namespace KC.DataAccess.Pay.Repository
{
    public interface IPaymentBankAccountRepository : Database.IRepository.IDbRepository<PaymentBankAccount>
    {
        PaymentBankAccount GetBankAccountByMemberId(string memberId, ThirdPartyType type = ThirdPartyType.CPCNConfigSign);
        List<PaymentBankAccount> GetPaymentAccounts(string memberId);
        List<Tuple<string, decimal>> QueryAvailableBalance(List<string> memberIds);
    }
}