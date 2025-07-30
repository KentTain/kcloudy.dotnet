using System.Threading.Tasks;
using KC.Model.Portal;

namespace KC.DataAccess.Portal.Repository
{
    public interface ICompanyAuthenticationRepository : Database.IRepository.IDbRepository<CompanyAuthentication>
    {
        Task<CompanyAuthentication> GetComAuthenticationAsync();
    }
}