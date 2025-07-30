using System;
using System.Data;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.Database.EFRepository;
using KC.Database.Extension;
using KC.Model.Customer;
using Microsoft.EntityFrameworkCore;

namespace KC.DataAccess.Customer.Repository
{
    public class CustomerInfoRepository : EFRepositoryBase<CustomerInfo>, ICustomerInfoRepository
    {
        public CustomerInfoRepository(EFUnitOfWorkContextBase unitOfWork)
            : base(unitOfWork)
        {
        }

        public IList<CustomerInfo> GetCustomerInfosByIds(List<int> ids)
        {
            return
                Entities.Include(m => m.CustomerContacts)
                    .Include(m => m.CustomerExtInfos)
                    .Include(m => m.CustomerManagers)
                    .AsNoTracking()
                    .Where(m => ids.Contains(m.CustomerId))
                    .ToList();
        }

        public IList<CustomerInfo> GetUnassignCustomerInfos()
        {
            var customers = Entities.Include(o => o.CustomerManagers);
            customers.Load();
            return customers.Where(o => o.CustomerManagers.All(p => o.CustomerId != p.CustomerId)).ToList();
        }

        //public DataTable GetUsersForToExcel(string sqlstr, SqlParameter[] sqlparams)
        //{
        //    return EFContext.Context.Database.SqlQueryForDataTatable(sqlstr, sqlparams);
        //}

        public bool UpdateCustomerInfo(IList<CustomerInfo> customerInfos, List<CustomerManager> addCustomerManagers,
            string[] properties = null)
        {
            foreach (var customerInfo in customerInfos)
            {
                if (addCustomerManagers.Any())
                {
                    var customerManagers =
                        customerInfo.CustomerManagers.Where(o => addCustomerManagers.Any(p => p.Id == o.Id));
                    EFContext.Set<CustomerManager>().RemoveRange(customerManagers);
                }
                addCustomerManagers.ForEach(o =>
                {
                    o.CustomerId = customerInfo.CustomerId;
                    EFContext.Set<CustomerManager>().Add(o);
                });
                base.ModifyAsync(customerInfo, properties, false);
            }
            return EFContext.Commit() > 0;
        }
    }
}
