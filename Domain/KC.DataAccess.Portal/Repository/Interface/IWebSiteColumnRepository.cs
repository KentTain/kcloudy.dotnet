using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using KC.Enums.Portal;
using KC.Model.Portal;

namespace KC.DataAccess.Portal.Repository
{
    public interface IWebSiteColumnRepository : Database.IRepository.IDbRepository<WebSiteColumn>
    {
        List<WebSiteColumn> GetAllAdvertisingList(Expression<Func<WebSiteColumn, bool>> predicate);
    }
}