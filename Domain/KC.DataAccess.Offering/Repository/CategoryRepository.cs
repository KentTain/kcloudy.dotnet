using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using KC.Database.EFRepository;
using KC.DataAccess.Offering;
using KC.Model.Offering;
using Microsoft.EntityFrameworkCore;

namespace KC.DataAccess.Offering.Repository
{
    public class CategoryRepository : EFTreeRepositoryBase<Category>, ICategoryRepository
    {
        public CategoryRepository(EFUnitOfWorkContextBase unitOfWork)
            : base(unitOfWork)
        {
        }

    }
}
