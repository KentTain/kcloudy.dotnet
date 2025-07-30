using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using KC.Enums.Portal;
using KC.Model.Portal;

namespace KC.DataAccess.Portal.Repository
{
    public interface IRecommendOfferingRepository : Database.IRepository.IDbRepository<RecommendOffering>
    {
        List<RecommendOffering> FindRedOfferingsByIds(List<int> ids);
        Task<List<RecommendOffering>> FindTop10RecommendInfosByTypeAsync(string typeCode);
        Tuple<int, List<RecommendOffering>> FindPagenatedByDataPermission(int pageIndex, int pageSize,
            Expression<Func<RecommendOffering, bool>> predicate, string sortProperty, bool ascending = true);

        RecommendOffering FindRedOfferingById(int id);

    }
}