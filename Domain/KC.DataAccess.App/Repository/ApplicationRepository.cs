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
    public class ApplicationRepository : EFRepositoryBase<Application>, IApplicationRepository
    {
        public ApplicationRepository(EFUnitOfWorkContextBase unitOfWork)
            : base(unitOfWork)
        {
        }

        public Task<List<Application>> GetAllApplicationsAsync(string name)
        {
            return Entities
                .Where(m => m.ApplicationName.Contains(name))
                .OrderBy(m => m.Index)
                .AsNoTracking()
                .ToListAsync();
        }

        public Task<List<Application>> GetAllApplicationDetailsAsync()
        {
            return Entities
                .Include(m => m.AppTemplate)
                .Include(m => m.AppGits)
                .Include(m => m.AppSettings.OrderBy(p => p.Index))
                .ThenInclude(m => m.PropertyAttributeList.OrderBy(p => p.Index))
                .OrderBy(m => m.Index)
                .AsNoTracking()
                .ToListAsync();
        }

        public Task<List<Application>> GetApplicationsWithSettingsByIds(List<Guid> ids)
        {
            return Entities
                .Include(m => m.AppTemplate)
                .Include(m => m.AppSettings)
                .ThenInclude(m => m.PropertyAttributeList.OrderBy(p => p.Index))
                .AsNoTracking()
                .Where(m => ids.Contains(m.ApplicationId))
                .OrderBy(m => m.Index)
                .ToListAsync();
        }

        public Task<Application> GetApplicationWithSettingsById(Guid id)
        {
            return Entities
                .Include(m => m.AppSettings)
                .ThenInclude(m => m.PropertyAttributeList.OrderBy(p => p.Index))
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ApplicationId == id);
        }

        public Task<Application> GetApplicationWithTemplateById(Guid id)
        {
            return Entities
                .Include(m => m.AppTemplate)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ApplicationId == id);
        }

        public Task<Application> GetApplicationDetailsById(Guid id)
        {
            return Entities
                .Include(m => m.AppTemplate)
                .Include(m => m.AppGits)
                .Include(m => m.AppSettings.OrderBy(p => p.Index))
                .ThenInclude(m => m.PropertyAttributeList.OrderBy(p => p.Index))
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ApplicationId == id);
        }
    }
}
