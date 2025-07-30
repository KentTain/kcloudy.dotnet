using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using KC.Model.Pay;

namespace KC.DataAccess.Pay.Repository
{
    public interface IReceivableRepository : Database.IRepository.IDbRepository<Receivable>
    {
        List<Receivable> GetPayableByFilter(Expression<Func<Receivable, bool>> where, string order = "asc");
        decimal GetReceivableAmount();
        List<Receivable> LoadLatelyReceivables(int count = 5);
    }
}