using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.Database.IRepository;
using KC.Framework.Base;

namespace KC.Database.EFRepository
{
    public class CommonEFRepository<T> : EFRepositoryBase<T> where T : EntityBase
    {
        public CommonEFRepository(EFUnitOfWorkContextBase unitOfWork)
            : base(unitOfWork)
        {
        }
    }

}
