using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using KC.Enums.Portal;
using KC.Model.Portal;

namespace KC.DataAccess.Portal.Repository
{
    public interface IRecommendCustomerRepository : Database.IRepository.IDbRepository<RecommendCustomer>
    {
        RecommendCustomer FindRedCustomerById(int id);

        List<RecommendCustomer> FindRedCustomersByIds(List<int> ids);
        Task<List<RecommendCustomer>> FindTop10RecommendInfosByTypeAsync(CompanyType? type);

        Tuple<int, List<RecommendCustomer>> FindPagenatedByDataPermission(int pageIndex, int pageSize,
            Expression<Func<RecommendCustomer, bool>> predicate, string sortProperty, bool ascending = true);
        
    }
}