using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using KC.Model.App;
using Microsoft.EntityFrameworkCore;

namespace KC.DataAccess.App.Repository
{
    public interface IAppSettingRepository : Database.IRepository.IDbRepository<AppSetting>
    {
        Task<AppSetting> GetAppSettingWithAttributesByIdAsync(int id);

        Task<IList<AppSetting>> FindAppSettingsWithAttributesByAppIdAsync(Guid appId);
    }
}