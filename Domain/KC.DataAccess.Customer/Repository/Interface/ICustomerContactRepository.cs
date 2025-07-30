using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using KC.Enums.CRM;
using KC.Model.Customer;

namespace KC.DataAccess.Customer.Repository
{
    public interface ICustomerContactRepository : Database.IRepository.IDbRepository<CustomerContact>
    {
        Tuple<int, IList<CustomerContact>> FindPagenatedListWithCount(int pageIndex, int pageSize, Expression<Func<CustomerContact, bool>> predicate);
        IList<CustomerContact> GetCustomerCompanyByCustomerContact(int customerContactId);
        IList<CustomerContact> GetCustomerContactsByCompanyType(CompanyType customerType);
    }
}