using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using KC.Framework.Extension;
using KC.Model.Account;
using Microsoft.EntityFrameworkCore;
using KC.Database.EFRepository;

namespace KC.DataAccess.Account.Repository
{
    public class UserSettingRepository : EFRepositoryBase<UserSetting>, IUserSettingRepository
    {
        public UserSettingRepository(EFUnitOfWorkContextBase unitOfWork)
            : base(unitOfWork)
        {
        }

        public UserSetting GetDetailUserSettingByCode(string code)
        {
            return Entities
                    .Include(m => m.PropertyAttributeList)
                    .Include(m => m.User)
                    .AsNoTracking()
                    .FirstOrDefault(m => m.Code.Equals(code));
        }
    }
}
