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
    public class WorkflowVerDefinitionRepository : EFRepositoryBase<WorkflowVerDefinition>, IWorkflowVerDefinitionRepository
    {
        public WorkflowVerDefinitionRepository(EFUnitOfWorkContextBase unitOfWork) 
            : base(unitOfWork)
        {
        }

        public WorkflowVerDefinition GetWfVerDefinitionDetailById(Guid id)
        {
            return Entities.AsNoTracking()
                .Include(m => m.WorkflowVerNodes)
                    .ThenInclude(n => n.Rules)
                .Include(m => m.WorkflowVerFields)
                    .ThenInclude(f => f.ParentNode)
                .FirstOrDefault(m => id.Equals(m.Id));
        }

        public WorkflowVerDefinition GetWfVerDefinitionDetailByCode(string code)
        {
            return Entities.AsNoTracking()
                .Include(m => m.WorkflowVerNodes)
                    .ThenInclude(n => n.Rules)
                .Include(m => m.WorkflowVerFields)
                    .ThenInclude(f => f.ParentNode)
                .FirstOrDefault(m => code.Equals(m.Code));
        }

        public async Task<WorkflowVerDefinition> GetWfVerDefinitionDetailByIdAsync(Guid id)
        {
            return await Entities.AsNoTracking()
                .Include(m => m.WorkflowVerNodes)
                    .ThenInclude(n => n.Rules)
                .Include(m => m.WorkflowVerFields)
                    .ThenInclude(f => f.ParentNode)
                .FirstOrDefaultAsync(m => id.Equals(m.Id));
        }
        public async Task<WorkflowVerDefinition> GetWfVerDefinitionDetailByCodeAsync(string code)
        {
            return await Entities.AsNoTracking()
                .Include(m => m.WorkflowVerNodes)
                    .ThenInclude(n => n.Rules)
                .Include(m => m.WorkflowVerFields)
                    .ThenInclude(f => f.ParentNode)
                .FirstOrDefaultAsync(m => code.Equals(m.Code));
        }


        //paging 不要分页参数 特定方法使用
        public Tuple<int, IList<WorkflowVerDefinition>> FindPagenatedVerDefinitionsByFilter(int pageIndex, int pageSize, System.Linq.Expressions.Expression<Func<WorkflowVerDefinition, bool>> predicate, string sortProperty, bool ascending = true, bool paging = true)
        {
            var q = Entities
                .Where(predicate)
                .Include(m => m.WorkflowCategory)
                .AsNoTracking();

            int recordCount = q.Count();
            if (!paging)
            {
                var results = q.SingleOrderBy(sortProperty, ascending).ToList();
                return new Tuple<int, IList<WorkflowVerDefinition>>(recordCount, results);
            }
            var result = q
                .SingleOrderBy(sortProperty, ascending)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            return new Tuple<int, IList<WorkflowVerDefinition>>(recordCount, result);
        }
    }
}
