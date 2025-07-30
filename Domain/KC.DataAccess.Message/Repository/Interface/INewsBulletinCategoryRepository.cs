using KC.Model.Message;
using System.Threading.Tasks;

namespace KC.DataAccess.Message.Repository
{
    public interface INewsBulletinCategoryRepository : Database.IRepository.IDbTreeRepository<NewsBulletinCategory>
    {
        NewsBulletinCategory GetCategoryWithChildrenById(int id);
        Task<bool> SaveCategoryAsync(NewsBulletinCategory model);
    }
}