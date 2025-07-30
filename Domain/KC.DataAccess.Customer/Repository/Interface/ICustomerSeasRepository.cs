using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using KC.Model.Customer;

namespace KC.DataAccess.Customer.Repository
{
    public interface ICustomerSeasRepository : Database.IRepository.IDbRepository<CustomerSeas>
    {
        Tuple<int, IList<CustomerSeas>> GetCustomerSeasInfo(Expression<Func<CustomerSeas, bool>> predicate, bool requiredPaging, int? pageIndex, int? pageSize);
    }
}