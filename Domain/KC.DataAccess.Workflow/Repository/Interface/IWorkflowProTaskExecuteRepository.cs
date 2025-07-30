using KC.Model.Workflow;
using System.Collections.Generic;

namespace KC.DataAccess.Workflow.Repository
{
    public interface IWorkflowProTaskExecuteRepository : Database.IRepository.IDbRepository<WorkflowProTaskExecute>
    {
        WorkflowProTaskExecute GetWorkflowTaskExecuteById(int taskId);

        List<WorkflowProTaskExecute> FindWorkflowTaskExecutesByTaskId(int taskId);
    }
}