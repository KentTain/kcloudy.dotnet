using System;
using System.Threading.Tasks;
using KC.Model.CodeGenerate;

namespace KC.DataAccess.CodeGenerate.Repository
{
    public interface IRelationDefinitionRepository : Database.IRepository.IDbRepository<RelationDefinition>
    {
        Task<RelationDefinition> GetBusinessWithSettingByIdAsync(int id);
    }
}