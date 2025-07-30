using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using KC.Database.EFRepository;
using KC.Enums.CRM;
using KC.Framework.Tenant;
using KC.Model.Customer;
using Microsoft.EntityFrameworkCore;

namespace KC.DataAccess.Customer.Repository
{
    public class CustomerContactRepository : EFRepositoryBase<CustomerContact>, ICustomerContactRepository
    {
        public CustomerContactRepository(EFUnitOfWorkContextBase unitOfWork)
            : base(unitOfWork)
        {
        }

        public Tuple<int, IList<CustomerContact>> FindPagenatedListWithCount(int pageIndex, int pageSize,
            Expression<Func<CustomerContact, bool>> predicate)
        {
            var databaseItems = Entities.AsNoTracking().Include(m => m.CustomerInfo).Where(predicate);
            databaseItems = databaseItems.Where(predicate);
            int recordCount = databaseItems.Count();

            return new Tuple<int, IList<CustomerContact>>(recordCount,
                databaseItems.Skip((pageIndex - 1)*pageSize).Take(pageSize).ToList());
        }

        public IList<CustomerContact> GetCustomerContactsByCompanyType(CompanyType customerType)
        {
            var query = Entities.AsNoTracking().Include(m => m.CustomerInfo).Where(m => m.CustomerInfo.CompanyType == customerType);
            return query.ToList();
        }

        public IList<CustomerContact> GetCustomerCompanyByCustomerContact(int customerContactId)
        {
            var query = Entities.AsNoTracking().Include(m => m.CustomerInfo).Where(m => m.Id == customerContactId);
            return query.ToList();
        }
    }
}
