using KC.Enums.Portal;
using KC.Model.Portal;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace KC.DataAccess.Portal.Repository
{
    public interface IRecommendCategoryRepository : Database.IRepository.IDbTreeRepository<RecommendCategory>
    {
        List<RecommendCategory> FindFatherCategory(int? id);

        Task<RecommendCategory> GetRecommendCategoryByIdAsync(int id);
        RecommendCategory GetRecommendCategoryWithChildrenById(int id);

        Task<bool> SaveRecommendCategoryAsync(RecommendCategory model);
    }
}
