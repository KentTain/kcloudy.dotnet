using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;

using KC.Database.EFRepository;
using KC.Enums.Portal;
using KC.Model.Portal;
using KC.Framework.Extension;
namespace KC.DataAccess.Portal.Repository
{
    public class WebSiteColumnRepository : EFRepositoryBase<WebSiteColumn>, IWebSiteColumnRepository
    {
        public WebSiteColumnRepository(EFUnitOfWorkContextBase unitOfWork)
            : base(unitOfWork)
        {
        }
   
        public override Tuple<int, IList<WebSiteColumn>> FindPagenatedListWithCount<K>(int pageIndex, int pageSize, Expression<Func<WebSiteColumn, bool>> predicate, Expression<Func<WebSiteColumn, K>> keySelector,
            bool @ascending = true)
        {
            var databaseItems = EFContext.Set<WebSiteColumn>()
                .Where(predicate)
                .Include(m => m.WebSiteItems)
                .AsNoTracking();
            int recordCount = databaseItems.Count();

            return @ascending
                ? new Tuple<int, IList<WebSiteColumn>>(recordCount, databaseItems.OrderBy(keySelector).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList())
                : new Tuple<int, IList<WebSiteColumn>>(recordCount, databaseItems.OrderByDescending(keySelector).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList());
        }


        public List<WebSiteColumn> GetAllAdvertisingList(Expression<Func<WebSiteColumn, bool>> predicate)
        {
             return Entities.Where(predicate).AsNoTracking().ToList();
        }
    }
}
