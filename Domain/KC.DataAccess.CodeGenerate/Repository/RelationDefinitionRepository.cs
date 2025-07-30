using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.Database.EFRepository;
using KC.Model.CodeGenerate;
using Microsoft.EntityFrameworkCore;

namespace KC.DataAccess.CodeGenerate.Repository
{
    public class RelationDefinitionRepository : EFRepositoryBase<RelationDefinition>, IRelationDefinitionRepository
    {
        public RelationDefinitionRepository(EFUnitOfWorkContextBase unitOfWork) 
            : base(unitOfWork)
        {
        }

        public Task<RelationDefinition> GetBusinessWithSettingByIdAsync(int id)
        {
            return Entities
                .AsNoTracking()
                .FirstOrDefaultAsync(m => id == m.Id);
        }

    }
}
