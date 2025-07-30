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
    public class RecommendCustomerRepository : EFRepositoryBase<RecommendCustomer>, IRecommendCustomerRepository
    {
        public RecommendCustomerRepository(EFUnitOfWorkContextBase unitOfWork) 
            : base(unitOfWork)
        {
        }

        
        public List<RecommendCustomer> FindRedCustomersByIds(List<int> ids)
        {
            return Entities.Where(m => !m.IsDeleted && ids.Contains(m.RecommendId))
                    .AsNoTracking()
                    .ToList();
        }

        public async Task<List<RecommendCustomer>> FindTop10RecommendInfosByTypeAsync(CompanyType? type)
        {
            Expression<Func<RecommendCustomer, bool>> predicate = m => !m.IsDeleted
                    && m.Status == RecommendStatus.Approved;
            if (type.HasValue)
            {
                predicate = predicate.And(m => type.Value == m.CompanyType);
            }
            var databaseItems = EFContext.Set<RecommendCustomer>().AsNoTracking()
                .Where(predicate)
                .OrderBy(m => m.IsTop)
                .ThenBy(m => m.ModifiedDate)
                .Take(10);
            return await databaseItems.ToListAsync();
        }

        public Tuple<int, List<RecommendCustomer>> FindPagenatedByDataPermission(int pageIndex, int pageSize, Expression<Func<RecommendCustomer, bool>> predicate, string sortProperty, bool ascending = true)
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
            return new Tuple<int, List<RecommendCustomer>>(recordCount, data);
        }

        public RecommendCustomer FindRedCustomerById(int id)
        {
            return Entities
                .Where(m => m.RecommendId == id)
                .Include(m => m.Category)
                .AsNoTracking()
                .FirstOrDefault();
        }

    }
}
