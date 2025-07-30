using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using KC.Model.Portal;

namespace KC.DataAccess.Portal.Repository
{
    public interface IWebSiteInfoRepository : Database.IRepository.IDbRepository<WebSiteInfo>
    {
        Tuple<int, IList<WebSiteInfo>> FindPagenatedListWithCount<K>(int pageIndex, int pageSize, Expression<Func<WebSiteInfo, bool>> predicate, Expression<Func<WebSiteInfo, K>> keySelector, bool ascending = true);
    }
}