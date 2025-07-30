using KC.Model.Workflow;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KC.DataAccess.Workflow.Repository
{
    public interface IWorkflowDefinitionRepository : Database.IRepository.IDbRepository<WorkflowDefinition>
    {
        Tuple<int, IList<WorkflowDefinition>> FindPagenatedDefinitionsByFilter(int pageIndex, int pageSize, System.Linq.Expressions.Expression<Func<WorkflowDefinition, bool>> predicate, string sortProperty, bool ascending = true);

        WorkflowDefinition GetWfDefinitionDetailById(Guid id);
        WorkflowDefinition GetWfDefinitionDetailByCode(string code);

        Task<WorkflowDefinition> GetWfDefinitionDetailByIdAsync(Guid id);
        Task<WorkflowDefinition> GetWfDefinitionWithNodesByIdAsync(Guid id);
        Task<WorkflowDefinition> GetWfDefinitionDetailByCodeAsync(string code);
    }
}