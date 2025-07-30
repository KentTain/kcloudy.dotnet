using KC.Model.Dict;

namespace KC.DataAccess.Dict.Repository
{
    public interface IMobileLocationRepository : Database.IRepository.IDbRepository<MobileLocation>
    {
        MobileLocation GetMobileLocation(string mobilePhone);
    }
}