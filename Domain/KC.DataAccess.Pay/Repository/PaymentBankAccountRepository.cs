using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using KC.Database.EFRepository;
using KC.Enums.Pay;
using KC.Framework.Extension;
using KC.Model.Pay;
using Microsoft.EntityFrameworkCore;

namespace KC.DataAccess.Pay.Repository
{
    public class PaymentBankAccountRepository : EFRepositoryBase<PaymentBankAccount>, IPaymentBankAccountRepository
    {
        public PaymentBankAccountRepository(EFUnitOfWorkContextBase unitOfWork)
            : base(unitOfWork)
        {
        }

        /// <summary>
        /// 根据租户ID 查找中金支付账号信息
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        public PaymentBankAccount GetBankAccountByMemberId(string memberId, ThirdPartyType type = ThirdPartyType.CPCNConfigSign)
        {
            return
                EFContext.Set<PaymentBankAccount>().AsNoTracking()
                    .FirstOrDefault(m => m.MemberId == memberId && m.State != 2 && m.PaymentType == type && !m.IsDeleted);
        }

        public List<Tuple<string, decimal>> QueryAvailableBalance(List<string> memberIds)
        {
            return Entities.Where(m => memberIds.Contains(m.MemberId))
                .AsEnumerable()
                .Select(m => new Tuple<string, decimal>(m.MemberId, m.CFWinTotalAmount - m.CFWinFreezeAmount)).ToList();
        }

        public List<PaymentBankAccount> GetPaymentAccounts(string memberId)
        {
            return
                EFContext.Set<PaymentBankAccount>().AsNoTracking()
                    .Where(m => m.MemberId == memberId && m.State != 2 && !m.IsDeleted).ToList();
        }
    }
}
