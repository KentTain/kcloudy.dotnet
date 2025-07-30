using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using KC.Framework.Base;

namespace KC.DataAccess.Account.Repository
{
    public interface IConfigEntityRepository : Database.IRepository.IDbRepository<ConfigEntity>
    {
        bool AddCallCenterConfig();
        bool AddEmailConfig();
        bool AddSmsConfig();

        List<ConfigEntity> GetConfigsWithAttributesByFilter(Expression<Func<ConfigEntity, bool>> predicate);

        ConfigEntity GetConfigWithAttributesById(int id);
        ConfigEntity GetConfigWithAttributesByName(string configName);
        
    }
}