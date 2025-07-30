using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.Database.EFRepository;
using KC.Enums.CRM;
using KC.Framework.Tenant;
using KC.Model.Customer;

namespace KC.DataAccess.Customer.Repository
{
    public class CustomerTracingLogRepository : EFRepositoryBase<CustomerTracingLog>, ICustomerTracingLogRepository
    {
        public CustomerTracingLogRepository(EFUnitOfWorkContextBase unitOfWork)
            : base(unitOfWork)
        {
        }

        public CustomerTracingLog GetTracingLogBySessionId(string sessionId)
        {
            return
                Entities.AsNoTracking()
                    .FirstOrDefault(m => m.ReferenceId == sessionId);
        }

        public CompanyType GetCustomerTypeByProcessLogId(int processLogId)
        {
            var data = from o in EFContext.Set<CustomerInfo>()
                join p in EFContext.Set<CustomerTracingLog>()
                    on o.CustomerId equals p.CustomerId
                    where p.ProcessLogId==processLogId
                select o.CompanyType;
            return data.FirstOrDefault();
        }
    }
}
