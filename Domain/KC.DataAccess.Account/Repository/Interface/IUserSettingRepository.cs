using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using KC.Model.Account;

namespace KC.DataAccess.Account.Repository
{
    public interface IUserSettingRepository : Database.IRepository.IDbRepository<UserSetting>
    {
        UserSetting GetDetailUserSettingByCode(string code);
        
    }
}