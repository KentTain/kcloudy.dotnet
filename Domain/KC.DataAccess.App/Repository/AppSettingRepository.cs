using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using KC.Database.EFRepository;
using KC.Model.App;
using Microsoft.EntityFrameworkCore;

namespace KC.DataAccess.App.Repository
{
    public class AppSettingRepository : EFRepositoryBase<AppSetting>, IAppSettingRepository
    {
        public AppSettingRepository(EFUnitOfWorkContextBase unitOfWork)
            : base(unitOfWork)
        {
        }

        public override async Task<Tuple<int, IList<AppSetting>>> FindPagenatedListWithCountAsync<K>(
            int pageIndex, int pageSize, Expression<Func<AppSetting, bool>> predicate,
            Expression<Func<AppSetting, K>> keySelector, bool ascending = true)
        {
            var databaseItems = EFContext.Set<AppSetting>()
                .Include(m => m.PropertyAttributeList)
                .Where(predicate).AsNoTracking();
            int recordCount = databaseItems.Count();

            return @ascending
                ? new Tuple<int, IList<AppSetting>>(recordCount, await databaseItems.OrderBy(keySelector).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync())
                : new Tuple<int, IList<AppSetting>>(recordCount, await databaseItems.OrderByDescending(keySelector).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync());
        }

        public async Task<IList<AppSetting>> FindAppSettingsWithAttributesByAppIdAsync(Guid appId)
        {
            return await Entities.AsNoTracking()
                .Include(m => m.PropertyAttributeList)
                .Where(c => c.ApplicationId == appId && !c.IsDeleted)
                .ToListAsync();
        }

        public async Task<AppSetting> GetAppSettingWithAttributesByIdAsync(int id)
        {
            return await Entities.AsNoTracking()
                .Include(m => m.PropertyAttributeList)
                .FirstOrDefaultAsync(c => c.PropertyId == id && !c.IsDeleted);
        }

        
    }
}
