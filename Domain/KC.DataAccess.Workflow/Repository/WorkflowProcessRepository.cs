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
    public class WorkflowProcessRepository : EFRepositoryBase<WorkflowProcess>, IWorkflowProcessRepository
    {
        public WorkflowProcessRepository(EFUnitOfWorkContextBase unitOfWork) 
            : base(unitOfWork)
        {
        }

        public WorkflowProcess GetWorkflowProcessDetailById(Guid id)
        {
            return Entities
                .Include(m => m.Tasks)
                    .ThenInclude(t => t.Rules)
                .Include(m => m.Tasks)
                    .ThenInclude(t => t.TaskExecutes)
                .Include(m => m.Context)
                .AsNoTracking()
                .FirstOrDefault(m => id.Equals(m.Id));
        }

        public WorkflowProcess GetWorkflowProcessDetailByCode(string code)
        {
            return Entities
                .Include(m => m.Tasks)
                    .ThenInclude(t => t.Rules)
                .Include(m => m.Tasks)
                    .ThenInclude(t => t.TaskExecutes)
                .Include(m => m.Context)
                .AsNoTracking()
                .FirstOrDefault(m => code.Equals(m.Code));
        }

        public async Task<WorkflowProcess> GetWorkflowProcessDetailByCodeAsync(string code)
        {
            return await Entities
                .Include(m => m.Tasks)
                    .ThenInclude(t => t.Rules)
                .Include(m => m.Tasks)
                    .ThenInclude(t => t.TaskExecutes)
                .Include(m => m.Context)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => code.Equals(m.Code));
        }
    }
}
