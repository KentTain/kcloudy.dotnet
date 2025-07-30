using KC.Model.Portal;
using System;
using System.Threading.Tasks;

namespace KC.DataAccess.Portal.Repository
{
    public interface IWebSitePageRepository : Database.IRepository.IDbRepository<WebSitePage>
    {
        Task<WebSitePage> GetWebSiteColumnDetailInfoAsync(Guid id);
        Task<WebSitePage> GetWebSiteColumnDetailBySkinCodeAsync(string skinCode, string url);
    }
}