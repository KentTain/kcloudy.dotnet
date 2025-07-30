using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using KC.Enums.Portal;
using KC.Model.Portal;

namespace KC.DataAccess.Portal.Repository
{
    public interface IRecommendRequirementRepository : Database.IRepository.IDbRepository<RecommendRequirement>
    {
        
        List<RecommendRequirement> FindRedRequirementsByIds(List<int> ids);
        Task<List<RecommendRequirement>> FindTop10RecommendInfosByTypeAsync(RequirementType? type);
        Tuple<int, List<RecommendRequirement>> FindPagenatedByDataPermission(int pageIndex, int pageSize,
            Expression<Func<RecommendRequirement, bool>> predicate, string sortProperty, bool ascending = true);
        RecommendRequirement FindRedRequirementById(int id);

    }
}