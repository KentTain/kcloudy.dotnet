using KC.Model.Workflow;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KC.DataAccess.Workflow.Repository
{
    public interface IWorkflowProTaskRepository : Database.IRepository.IDbRepository<WorkflowProTask>
    {
        WorkflowProTask GetWorkflowTaskDetailByCode(string code);

        List<WorkflowProTask> FindAllWfTaskDetailsByProcessCode(string code);

        Task<IList<WorkflowProTask>> FindAllWfTaskDetailsByProcessCodeAsync(string code);
    }
}