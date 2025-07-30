using System.Threading.Tasks;
using KC.Model.Portal;

namespace KC.DataAccess.Portal.Repository
{
    public interface ICompanyInfoRepository : Database.IRepository.IDbRepository<CompanyInfo>
    {
        Task<CompanyInfo> GetCompanyDetailInfoAsync();
    }
}