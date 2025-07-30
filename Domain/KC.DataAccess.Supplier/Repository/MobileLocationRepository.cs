using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.Database.EFRepository;
using KC.Model.Supplier;
using Microsoft.EntityFrameworkCore;

namespace KC.DataAccess.Supplier.Repository
{
    public class MobileLocationRepository : EFRepositoryBase<MobileLocation>, IMobileLocationRepository
    {
        public MobileLocationRepository(EFUnitOfWorkContextBase unitOfWork) 
            : base(unitOfWork)
        {
        }

        public MobileLocation GetMobileLocation(string mobilePhone)
        {
            return Entities
                .AsNoTracking()
                .FirstOrDefault(m => mobilePhone.StartsWith(m.Mobile));
        }
    }
}
