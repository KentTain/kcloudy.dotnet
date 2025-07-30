using KC.Model.Supplier;

namespace KC.DataAccess.Supplier.Repository
{
    public interface IMobileLocationRepository : Database.IRepository.IDbRepository<MobileLocation>
    {
        MobileLocation GetMobileLocation(string mobilePhone);
    }
}