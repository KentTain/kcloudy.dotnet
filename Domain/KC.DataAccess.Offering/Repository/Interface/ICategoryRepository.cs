using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using KC.Model.Offering;

namespace KC.DataAccess.Offering.Repository
{
    public interface ICategoryRepository : Database.IRepository.IDbTreeRepository<Category>
    {
    }
}