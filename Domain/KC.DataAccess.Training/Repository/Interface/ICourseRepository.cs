using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using KC.Framework.Base;
using KC.Model.Training;

namespace KC.DataAccess.Account.Repository
{
    public interface ICourseRepository : Database.IRepository.IDbRepository<Course>
    {
        bool AddCallCenterConfig();
        bool AddEmailConfig();
        bool AddSmsConfig();

        List<Course> GetConfigsWithAttributesByFilter(Expression<Func<Course, bool>> predicate);

        Course GetConfigWithAttributesById(int id);
        Course GetConfigWithAttributesByName(string configName);
        
    }
}