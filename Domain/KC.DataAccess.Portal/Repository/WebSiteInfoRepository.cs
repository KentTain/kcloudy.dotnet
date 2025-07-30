using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using KC.Framework.Extension;
using KC.Database.EFRepository;
using KC.Model.Portal;
using Microsoft.EntityFrameworkCore;

namespace KC.DataAccess.Portal.Repository
{
    public class WebSiteInfoRepository : EFRepositoryBase<WebSiteInfo>, IWebSiteInfoRepository
    {
        public WebSiteInfoRepository(EFUnitOfWorkContextBase unitOfWork)
           : base(unitOfWork)
        {
        }

        public override Tuple<int, IList<WebSiteInfo>> FindPagenatedListWithCount<K>(int pageIndex, int pageSize, Expression<Func<WebSiteInfo, bool>> predicate, Expression<Func<WebSiteInfo, K>> keySelector,
            bool @ascending = true)
        {
            var databaseItems = EFContext.Set<WebSiteInfo>()
                .Where(predicate)
                .AsNoTracking();
            int recordCount = databaseItems.Count();

            return @ascending
                ? new Tuple<int, IList<WebSiteInfo>>(recordCount, databaseItems.OrderBy(keySelector).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList())
                : new Tuple<int, IList<WebSiteInfo>>(recordCount, databaseItems.OrderByDescending(keySelector).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList());
        }

        
    }
}
