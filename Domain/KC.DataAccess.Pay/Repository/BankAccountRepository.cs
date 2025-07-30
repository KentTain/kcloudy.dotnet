using KC.Database.EFRepository;
using KC.Framework.Extension;
using KC.Model.Pay;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace KC.DataAccess.Pay.Repository
{
    public class BankAccountRepository : EFRepositoryBase<BankAccount>, IBankAccountRepository
    {
        public BankAccountRepository(EFUnitOfWorkContextBase unitOfWork)
            : base(unitOfWork)
        {
        }

        /// <summary>
        /// 根据租户ID 查找银行账号信息
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        public List<BankAccount> GetBankAccountByMemberId(string memberId, string searchInfo)
        {
            return
                EFContext.Set<BankAccount>()
                    .Where(m => m.MemberId == memberId && !m.IsDeleted
                    && m.BankName.Contains(searchInfo)).ToList();
        }
    }
}
