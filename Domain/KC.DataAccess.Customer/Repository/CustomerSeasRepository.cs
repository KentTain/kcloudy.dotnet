using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using KC.Framework.Extension;
using KC.Database.EFRepository;
using KC.Framework.Tenant;
using KC.Model.Customer;
using Microsoft.EntityFrameworkCore;

namespace KC.DataAccess.Customer.Repository
{
    public class CustomerSeasRepository : EFRepositoryBase<CustomerSeas>, ICustomerSeasRepository
    {
        public CustomerSeasRepository(EFUnitOfWorkContextBase unitOfWork)
            : base(unitOfWork)
        {

        }

        /// <summary>
        /// 获取公海客户数据
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="requiredPaging"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="?"></param>
        /// <returns></returns>
        public Tuple<int, IList<CustomerSeas>> GetCustomerSeasInfo(
            Expression<Func<CustomerSeas, bool>> predicate, bool requiredPaging, int? pageIndex, int? pageSize
            )
        {
            var query = Entities.AsNoTracking().Include(m => m.CustomerInfo).Where(predicate);

            int recordCount = 0;
            if (requiredPaging && pageIndex.HasValue && pageSize.HasValue)
            {
                recordCount = query.Count();
                query = query.Skip((pageIndex.Value - 1)*pageSize.Value).Take(pageSize.Value);
            }
            return new Tuple<int, IList<CustomerSeas>>(recordCount, query.ToList());
        }

        //public IList<CustomerSeas> GetCustomerSeasesById(string operatorId, List<int> customerIds,List<int?> organizationsIds )
        //{
        //    Expression<Func<CustomerSeas, bool>> predicate = o => true;
        //    if (!string.IsNullOrEmpty(operatorId))
        //    {
        //        predicate = predicate.And(o => o.OperatorId == operatorId);
        //    }
        //    if (customerIds!=null&& customerIds.Any())
        //    {
        //        predicate = predicate.And(o => customerIds.Contains(o.CustomerId));
        //    }
        //    if (organizationsIds != null && organizationsIds.Any())
        //    {
        //        predicate = predicate.And(o => organizationsIds.Contains(o.OrganizationId));
        //    }
        //    var result = Entities.AsNoTracking().Where(predicate).ToList();
        //    return result;
        //}
    }
}
