using KC.Model.Workflow;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace KC.DataAccess.Workflow.Repository
{
    public interface IWorkflowDefNodeRepository : Database.IRepository.IDbRepository<WorkflowDefNode>
    {
        WorkflowDefNode GetWorkflowDefNodeByCode(string code);

        Task<IList<WorkflowDefNode>> FindAllDetailNodesAsync(Expression<Func<WorkflowDefNode, bool>> predicate);
    }
}