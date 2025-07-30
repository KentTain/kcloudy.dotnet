using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using KC.Model.CodeGenerate;

namespace KC.DataAccess.CodeGenerate.Repository
{
    public interface IModelDefFieldRepository : Database.IRepository.IDbRepository<ModelDefField>
    {
    }
}