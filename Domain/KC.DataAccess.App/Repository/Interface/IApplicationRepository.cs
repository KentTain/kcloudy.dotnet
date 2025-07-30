using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using KC.Model.App;

namespace KC.DataAccess.App.Repository
{
    public interface IApplicationRepository : Database.IRepository.IDbRepository<Application>
    {
        Task<List<Application>> GetAllApplicationsAsync(string name);
        Task<List<Application>> GetAllApplicationDetailsAsync();
        Task<List<Application>> GetApplicationsWithSettingsByIds(List<Guid> ids);
        Task<Application> GetApplicationWithSettingsById(Guid id);
        Task<Application> GetApplicationWithTemplateById(Guid id);
        Task<Application> GetApplicationDetailsById(Guid id);
    }
}