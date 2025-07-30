using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using KC.Model.Customer;

namespace KC.DataAccess.Customer.Repository
{
    public interface ICustomerManagerRepository : Database.IRepository.IDbRepository<CustomerManager>
    {
        List<CustomerManager> GetCustomerManagersById(Expression<Func<CustomerManager, bool>> predicate);
    }
}