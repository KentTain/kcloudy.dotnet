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
    public class AppSettingPropertyRepository : EFRepositoryBase<AppSettingProperty>, IAppSettingPropertyRepository
    {
        public AppSettingPropertyRepository(EFUnitOfWorkContextBase unitOfWork)
            : base(unitOfWork)
        {
        }

        public async Task<AppSettingProperty> GetWithSettingByIdAsync(int id)
        {
            return await Entities.AsNoTracking()
                .Include(m => m.AppSetting)
                .FirstOrDefaultAsync(c => c.PropertyAttributeId == id && !c.IsDeleted);
        }
    }
}
