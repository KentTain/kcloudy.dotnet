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
    public class WebSitePageRepository : EFRepositoryBase<WebSitePage>, IWebSitePageRepository
    {
        public WebSitePageRepository(EFUnitOfWorkContextBase unitOfWork)
           : base(unitOfWork)
        {
        }

        public async Task<WebSitePage> GetWebSiteColumnDetailInfoAsync(Guid id)
        {
            var data = await EFContext.Set<WebSitePage>()
                .Include(m => m.WebSiteColumns)
                    .ThenInclude(m => m.WebSiteItems)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);
            if (data == null)
                return new WebSitePage();
            return data;
        }

        public async Task<WebSitePage> GetWebSiteColumnDetailBySkinCodeAsync(string skinCode, string url)
        {
            var data = await EFContext.Set<WebSitePage>()
                .Include(m => m.WebSiteColumns)
                    .ThenInclude(m => m.WebSiteItems)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.SkinCode.Equals(skinCode) && m.Link.Contains(url));

            return data;

        }
    }
}
