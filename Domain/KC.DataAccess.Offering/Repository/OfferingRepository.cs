using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.Database.EFRepository;
using KC.Model.Offering;
using Microsoft.EntityFrameworkCore;

namespace KC.DataAccess.Offering.Repository
{
    public class OfferingRepository : EFRepositoryBase<Model.Offering.Offering>, IOfferingRepository
    {
        public OfferingRepository(EFUnitOfWorkContextBase unitOfWork) 
            : base(unitOfWork)
        {
        }
    }
}
