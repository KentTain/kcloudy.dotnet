using KC.Enums.CRM;
using KC.Model.Customer;

namespace KC.DataAccess.Customer.Repository
{
    public interface ICustomerTracingLogRepository : Database.IRepository.IDbRepository<CustomerTracingLog>
    {
        CompanyType GetCustomerTypeByProcessLogId(int processLogId);
        CustomerTracingLog GetTracingLogBySessionId(string sessionId);
    }
}