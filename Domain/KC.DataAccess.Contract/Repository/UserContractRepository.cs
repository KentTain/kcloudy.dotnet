using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using KC.Database.EFRepository;
using KC.Model.Contract;
using Microsoft.EntityFrameworkCore;

namespace KC.DataAccess.Contract.Repository
{
    public class UserContractRepository : EFRepositoryBase<UserContract>, IUserContractRepository
    {
        public UserContractRepository(EFUnitOfWorkContextBase unitOfWork)
            : base(unitOfWork)
        {
        }


        public UserContract GetContract(Expression<Func<UserContract, bool>> predicate)
        {
            return EFContext.Set<UserContract>().Include(m => m.ContractGroup).FirstOrDefault(predicate);

        }
    }
}
