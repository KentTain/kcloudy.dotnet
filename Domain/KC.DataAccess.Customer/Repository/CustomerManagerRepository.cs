using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using KC.Framework.Extension;
using KC.Database.EFRepository;
using KC.Framework.Tenant;
using KC.Model.Customer;

namespace KC.DataAccess.Customer.Repository
{
  
    public class CustomerManagerRepository : EFRepositoryBase<CustomerManager>, ICustomerManagerRepository
    {
        public CustomerManagerRepository(EFUnitOfWorkContextBase unitOfWork)
            : base(unitOfWork)
        {
        }

        /// <summary>
        /// 根据客户Id和客户经理Id查找客户经理信息
        /// </summary>
        /// <param name="customerManagerId">客户经理Id</param>
        /// <param name="customerIds"></param>
        /// <param name="organizationIds"></param>
        /// <param name="isInSeas">是否被移入了公海</param>
        /// <returns></returns>
        //public List<CustomerManager>
        //    GetCustomerManagersById(string customerManagerId, List<int> customerIds, List<int?> organizationIds = null,bool? isInSeas=null)
        //{
        //    Expression<Func<CustomerManager, bool>> predicate = o => true;
        //    if (!string.IsNullOrEmpty(customerManagerId))
        //    {
        //        predicate = predicate.And(o => o.CustomerManagerId == customerManagerId);
        //    }
        //    if (customerIds!=null&&customerIds.Any())
        //    {
        //        predicate = predicate.And(o => customerIds.Contains(o.CustomerId.Value));
        //    }
        //    if (organizationIds != null && organizationIds.Any())
        //    {
        //        predicate = predicate.And(o => organizationIds.Contains(o.OrganizationId));
        //    }
        //    if (isInSeas.HasValue)
        //    {
        //        predicate = predicate.And(o =>o.IsInSeas==isInSeas);
        //    }
        //    var result = Entities.AsNoTracking().Where(predicate).ToList();
        //    return result;
        //}

        public List<CustomerManager> GetCustomerManagersById(Expression<Func<CustomerManager, bool>> predicate)
        {
            var result = Entities.AsNoTracking().Where(predicate).ToList();
            return result;
        }
    }
}
