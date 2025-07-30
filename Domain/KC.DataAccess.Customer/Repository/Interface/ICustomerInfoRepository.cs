using System.Collections.Generic;
using KC.Model.Customer;

namespace KC.DataAccess.Customer.Repository
{
    public interface ICustomerInfoRepository : Database.IRepository.IDbRepository<CustomerInfo>
    {
        IList<CustomerInfo> GetCustomerInfosByIds(List<int> ids);
        IList<CustomerInfo> GetUnassignCustomerInfos();
        bool UpdateCustomerInfo(IList<CustomerInfo> customerInfos, List<CustomerManager> addCustomerManagers, string[] properties = null);
    }
}