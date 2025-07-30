using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using KC.Database.EFRepository;
using KC.Model.CodeGenerate;
using Microsoft.EntityFrameworkCore;

namespace KC.DataAccess.CodeGenerate.Repository
{
    public class ModelDefinitionRepository : EFRepositoryBase<ModelDefinition>, IModelDefinitionRepository
    {
        public ModelDefinitionRepository(EFUnitOfWorkContextBase unitOfWork)
            : base(unitOfWork)
        {
        }

        public Task<List<ModelDefinition>> GetAllModelDefinitionsAsync()
        {
            return Entities
                .OrderBy(m => m.Index)
                .AsNoTracking()
                .ToListAsync();
        }

        public Task<List<ModelDefinition>> GetAllModelDefinitionDetailsAsync()
        {
            return Entities
                .Include(m => m.PropertyAttributeList)
                .OrderBy(m => m.Index)
                .AsNoTracking()
                .ToListAsync();
        }

        public List<ModelDefinition> GetModelDefinitionsWithModulesByIds(List<int> ids)
        {
            return Entities
                .Include(m => m.PropertyAttributeList)
                .AsNoTracking()
                .Where(m => ids.Contains(m.PropertyId))
                .OrderBy(m => m.Index)
                .ToList();
        }

        public List<ModelDefinition> GetModelDefinitionsWithBusinessesByIds(List<int> ids)
        {
            return Entities
                .Include(m => m.PropertyAttributeList)
                .AsNoTracking()
                .Where(m => ids.Contains(m.PropertyId))
                .OrderBy(m => m.Index)
                .ToList();
        }

        public ModelDefinition GetModelDefinitionWithModulesById(int id)
        {
            return Entities
                .Include(m => m.PropertyAttributeList)
                .AsNoTracking()
                .FirstOrDefault(m => m.PropertyId == id);
        }

        public ModelDefinition GetModelDefinitionWithBusinessesById(int id)
        {
            return Entities
                .Include(m => m.PropertyAttributeList)
                .AsNoTracking()
                .FirstOrDefault(m => m.PropertyId == id);
        }

        public ModelDefinition GetModelDefinitionDetailsById(int id)
        {
            return Entities
                .Include(m => m.PropertyAttributeList)
                .AsNoTracking()
                .FirstOrDefault(m => m.PropertyId == id);
        }
    }
}
