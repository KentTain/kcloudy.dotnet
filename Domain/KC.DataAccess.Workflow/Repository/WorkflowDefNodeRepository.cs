using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using KC.Database.EFRepository;
using KC.Model.Workflow;
using Microsoft.EntityFrameworkCore;

namespace KC.DataAccess.Workflow.Repository
{
    public class WorkflowDefNodeRepository : EFRepositoryBase<WorkflowDefNode>, IWorkflowDefNodeRepository
    {
        public WorkflowDefNodeRepository(EFUnitOfWorkContextBase unitOfWork) 
            : base(unitOfWork)
        {
        }

        public WorkflowDefNode GetWorkflowDefNodeByCode(string code)
        {
            return Entities
                .Include(m => m.WorkflowDefinition)
                .Include(m => m.Rules)
                .AsNoTracking()
                .FirstOrDefault(m => code.Equals(m.Code));
        }

        public async Task<IList<WorkflowDefNode>> FindAllDetailNodesAsync(Expression<Func<WorkflowDefNode, bool>> predicate)
        {
            return await Entities
                .Include(m => m.WorkflowDefinition)
                .Include(m => m.Rules)
                .Where(predicate)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
