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
    public class ReceivableRepository : EFRepositoryBase<Receivable>, IReceivableRepository
    {
        public ReceivableRepository(EFUnitOfWorkContextBase unitOfWork)
            : base(unitOfWork)
        {
        }

        public List<Receivable> GetPayableByFilter(Expression<Func<Receivable, bool>> where, string order = "asc")
        {
            var result = Entities.Where(where).AsNoTracking();
            if (order == "asc")
                return result.OrderBy(m => m.StartDate).ToList();
            return result.OrderByDescending(m => m.StartDate).ToList();
        }

        public List<Receivable> LoadLatelyReceivables(int count = 5)
        {
            return Entities.Where(m => m.ReceivableAmount - m.AlreadyPayAmount > 0).Take(count).OrderBy(m => m.StartDate).ThenBy(m => m.ReceivableAmount).AsNoTracking().ToList();
        }

        public decimal GetReceivableAmount()
        {
            var result = Entities.Sum(m => (decimal?)m.ReceivableAmount - (decimal?)m.AlreadyPayAmount);
            return result ?? 0M;
        }

    }
}
