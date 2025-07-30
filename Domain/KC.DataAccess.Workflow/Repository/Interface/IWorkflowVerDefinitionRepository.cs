using KC.Model.Workflow;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KC.DataAccess.Workflow.Repository
{
    public interface IWorkflowVerDefinitionRepository : Database.IRepository.IDbRepository<WorkflowVerDefinition>
    {
        Tuple<int, IList<WorkflowVerDefinition>> FindPagenatedVerDefinitionsByFilter(int pageIndex, int pageSize, System.Linq.Expressions.Expression<Func<WorkflowVerDefinition, bool>> predicate, string sortProperty, bool ascending = true, bool paging = true);

        WorkflowVerDefinition GetWfVerDefinitionDetailById(Guid id);
        WorkflowVerDefinition GetWfVerDefinitionDetailByCode(string code);

        Task<WorkflowVerDefinition> GetWfVerDefinitionDetailByIdAsync(Guid id);
        Task<WorkflowVerDefinition> GetWfVerDefinitionDetailByCodeAsync(string code);
    }
}