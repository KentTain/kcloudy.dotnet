using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.Database.IRepository;
using KC.Framework.Base;

namespace KC.Database.EFRepository
{
    public class CommonEFTreeRepository<T> : EFTreeRepositoryBase<T> where T : TreeNode<T>
    {
        public CommonEFTreeRepository(EFUnitOfWorkContextBase unitOfWork)
            : base(unitOfWork)
        {
        }
    }
}
