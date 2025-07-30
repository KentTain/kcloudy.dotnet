using System;
using System.Linq.Expressions;
using KC.Model.Contract;

namespace KC.DataAccess.Contract.Repository
{
    public interface IUserContractRepository : Database.IRepository.IDbRepository<UserContract>
    {
        UserContract GetContract(Expression<Func<UserContract, bool>> predicate);
    }
}