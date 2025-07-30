using KC.Model.Workflow;
using System;
using System.Threading.Tasks;

namespace KC.DataAccess.Workflow.Repository
{
    public interface IWorkflowProcessRepository : Database.IRepository.IDbRepository<WorkflowProcess>
    {
        WorkflowProcess GetWorkflowProcessDetailById(Guid id);
        WorkflowProcess GetWorkflowProcessDetailByCode(string code);

        Task<WorkflowProcess> GetWorkflowProcessDetailByCodeAsync(string code);
    }
}