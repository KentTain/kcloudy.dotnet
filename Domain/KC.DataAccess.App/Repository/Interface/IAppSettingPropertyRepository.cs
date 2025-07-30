using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using KC.Model.App;
using Microsoft.EntityFrameworkCore;

namespace KC.DataAccess.App.Repository
{
    public interface IAppSettingPropertyRepository : Database.IRepository.IDbRepository<AppSettingProperty>
    {
        Task<AppSettingProperty> GetWithSettingByIdAsync(int id);
    }
}