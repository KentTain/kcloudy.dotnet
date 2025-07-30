using System.Collections.Generic;
using KC.Model.Pay;

namespace KC.DataAccess.Pay.Repository
{
    public interface IBankAccountRepository : Database.IRepository.IDbRepository<BankAccount>
    {
        List<BankAccount> GetBankAccountByMemberId(string memberId, string searchInfo);
    }
}