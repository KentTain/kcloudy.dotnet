using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using KC.Database.EFRepository;
using KC.Enums.Portal;
using KC.Framework.Extension;
using KC.Model.Portal;
using Microsoft.EntityFrameworkCore;

namespace KC.DataAccess.Portal.Repository
{
    public class RecommendOfferingRepository : EFRepositoryBase<RecommendOffering>, IRecommendOfferingRepository
    {
        public RecommendOfferingRepository(EFUnitOfWorkContextBase unitOfWork) 
            : base(unitOfWork)
        {
        }

        public async Task<List<RecommendOffering>> FindTop10RecommendInfosByTypeAsync(string typeCode)
        {
            Expression<Func<RecommendOffering, bool>> predicate = m => !m.IsDeleted
                    && m.Status == RecommendStatus.Approved ;
            if (!string.IsNullOrEmpty(typeCode))
            {
                predicate = predicate.And(m => typeCode.Equals(m.OfferingTypeCode));
            }
            var databaseItems = EFContext.Set<RecommendOffering>().AsNoTracking()
                .Where(predicate)
                .OrderBy(m => m.IsTop)
                .ThenBy(m => m.ModifiedDate)
                .Take(10);
            return await databaseItems.ToListAsync();
        }

        

        public List<RecommendOffering> FindRedOfferingsByIds(List<int> ids)
        {
            return Entities.Where(m => !m.IsDeleted && ids.Contains(m.RecommendId))
                    .AsNoTracking()
                    .ToList();
        }

        public Tuple<int, List<RecommendOffering>> FindPagenatedByDataPermission(int pageIndex, int pageSize, Expression<Func<RecommendOffering, bool>> predicate, string sortProperty, bool ascending = true)
        {
            var query = Entities
                .Where(predicate)
                .Include(o => o.Category)
                .AsNoTracking();
            var recordCount = query.Count();
            var data = query
                .SingleOrderBy(sortProperty, ascending)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            return new Tuple<int, List<RecommendOffering>>(recordCount, data);
        }

        public RecommendOffering FindRedOfferingById(int id)
        {
            return Entities
                .Where(m => m.RecommendId == id)
                .Include(m => m.Category)
                .AsNoTracking()
                .FirstOrDefault();
        }
    }
}
