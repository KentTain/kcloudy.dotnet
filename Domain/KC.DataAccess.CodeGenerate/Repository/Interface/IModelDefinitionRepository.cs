using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using KC.Model.CodeGenerate;

namespace KC.DataAccess.CodeGenerate.Repository
{
    public interface IModelDefinitionRepository : Database.IRepository.IDbRepository<ModelDefinition>
    {
        Task<List<ModelDefinition>> GetAllModelDefinitionsAsync();
        Task<List<ModelDefinition>> GetAllModelDefinitionDetailsAsync();
        List<ModelDefinition> GetModelDefinitionsWithModulesByIds(List<int> ids);
        List<ModelDefinition> GetModelDefinitionsWithBusinessesByIds(List<int> ids);
        ModelDefinition GetModelDefinitionWithModulesById(int id);
        ModelDefinition GetModelDefinitionWithBusinessesById(int id);
        ModelDefinition GetModelDefinitionDetailsById(int id);
    }
}