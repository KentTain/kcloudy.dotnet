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
    public class PayableRepository : EFRepositoryBase<Payable>, IPayableRepository
    {
        public PayableRepository(EFUnitOfWorkContextBase unitOfWork)
            : base(unitOfWork)
        {
        }

        public List<Payable> GetPayableByFilter(Expression<Func<Payable, bool>> where, string order = "asc")
        {
            var result = Entities.Where(where).AsNoTracking();
            if (order == "asc")
                return result.OrderBy(m => m.StartDate).ToList();
            return result.OrderByDescending(m => m.StartDate).ToList();
        }

        public List<Payable> LoadLatelyPayables(int count = 5)
        {
            return Entities.Where(m => m.PayableAmount - m.AlreadyPayAmount > 0).Take(count).OrderBy(m => m.StartDate).ThenBy(m => m.PayableAmount).AsNoTracking().ToList();
        }

        public decimal GetPayableAmount()
        {
            var result = Entities.Sum(m => (decimal?)m.PayableAmount - (decimal?)m.AlreadyPayAmount);
            return result ?? 0M;
        }

    }
}
