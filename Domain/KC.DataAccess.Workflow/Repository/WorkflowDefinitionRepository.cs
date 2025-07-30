using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.Database.EFRepository;
using KC.Framework.Extension;
using KC.Model.Workflow;
using Microsoft.EntityFrameworkCore;

namespace KC.DataAccess.Workflow.Repository
{
    public class WorkflowDefinitionRepository : EFRepositoryBase<WorkflowDefinition>, IWorkflowDefinitionRepository
    {
        public WorkflowDefinitionRepository(EFUnitOfWorkContextBase unitOfWork) 
            : base(unitOfWork)
        {
        }

        public WorkflowDefinition GetWfDefinitionDetailById(Guid id)
        {
            return Entities.AsNoTracking()
                .Include(m => m.WorkflowNodes)
                    .ThenInclude(n => n.Rules)
                .Include(m => m.WorkflowFields)
                .FirstOrDefault(m => id.Equals(m.Id));
        }

        public WorkflowDefinition GetWfDefinitionDetailByCode(string code)
        {
            return Entities.AsNoTracking()
                .Include(m => m.WorkflowNodes)
                    .ThenInclude(n => n.Rules)
                .Include(m => m.WorkflowFields)
                .FirstOrDefault(m => code.Equals(m.Code));
        }

        public async Task<WorkflowDefinition> GetWfDefinitionDetailByIdAsync(Guid id)
        {
            return await Entities.AsNoTracking()
                .Include(m => m.WorkflowNodes)
                    .ThenInclude(n => n.Rules)
                .Include(m => m.WorkflowFields)
                .FirstOrDefaultAsync(m => id.Equals(m.Id));
        }
        public async Task<WorkflowDefinition> GetWfDefinitionWithNodesByIdAsync(Guid id)
        {
            return await Entities.AsNoTracking()
                .Include(m => m.WorkflowNodes)
                    .ThenInclude(n => n.Rules)
                .Include(m => m.WorkflowFields)
                .FirstOrDefaultAsync(m => id.Equals(m.Id));
        }
        public async Task<WorkflowDefinition> GetWfDefinitionDetailByCodeAsync(string code)
        {
            return await Entities.AsNoTracking()
                .Include(m => m.WorkflowNodes)
                    .ThenInclude(n => n.Rules)
                .Include(m => m.WorkflowFields)
                .FirstOrDefaultAsync(m => code.Equals(m.Code));
        }


        //paging 不要分页参数 特定方法使用
        public Tuple<int, IList<WorkflowDefinition>> FindPagenatedDefinitionsByFilter(int pageIndex, int pageSize, System.Linq.Expressions.Expression<Func<WorkflowDefinition, bool>> predicate, string sortProperty, bool ascending = true)
        {
            var q = Entities
                .Where(predicate)
                .Include(m => m.WorkflowCategory)
                .AsNoTracking();

            int recordCount = q.Count();
            var result = q
                .SingleOrderBy(sortProperty, ascending)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            return new Tuple<int, IList<WorkflowDefinition>>(recordCount, result);
        }
    }
}
