using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using KC.Model.Pay;

namespace KC.DataAccess.Pay.Repository
{
    public interface IPayableRepository : Database.IRepository.IDbRepository<Payable>
    {
        decimal GetPayableAmount();
        List<Payable> GetPayableByFilter(Expression<Func<Payable, bool>> where, string order = "asc");
        List<Payable> LoadLatelyPayables(int count = 5);
    }
}