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
    public class WorkflowProTaskRepository : EFRepositoryBase<WorkflowProTask>, IWorkflowProTaskRepository
    {
        public WorkflowProTaskRepository(EFUnitOfWorkContextBase unitOfWork) 
            : base(unitOfWork)
        {
        }

        public WorkflowProTask GetWorkflowTaskDetailByCode(string code)
        {
            return Entities
                .Include(m => m.Process)
                .Include(m => m.TaskExecutes)
                .Include(m => m.Rules)
                .AsNoTracking()
                .FirstOrDefault(m => code.Equals(m.Code));
        }

        public List<WorkflowProTask> FindAllWfTaskDetailsByProcessCode(string code)
        {
            return Entities
                .Include(m => m.Process)
                    .ThenInclude(p => p.Context)
                .Include(m => m.Rules)
                .AsNoTracking()
                .Where(m => m.Process.Code.Equals(code))
                .ToList();
        }
        public async Task<IList<WorkflowProTask>> FindAllWfTaskDetailsByProcessCodeAsync(string code)
        {
            return await EFContext
                .Set<WorkflowProTask>()
                .Where(m => m.Process.Code.Equals(code))
                .Include(m => m.Process)
                .Include(m => m.TaskExecutes)
                .Include(m => m.Rules)
                .AsNoTracking()
                .ToListAsync();
        }

        public async override Task<IList<WorkflowProTask>> FindAllAsync(Expression<Func<WorkflowProTask, bool>> predicate)
        {
            return await EFContext.Set<WorkflowProTask>()
                .Include(m => m.Process)
                .Where(predicate)
                .AsNoTracking()
                .ToListAsync();
        }

        public override Tuple<int, IList<WorkflowProTask>> FindPagenatedListWithCount<K>(
            int pageIndex, int pageSize, Expression<Func<WorkflowProTask, bool>> predicate,
            Expression<Func<WorkflowProTask, K>> keySelector, bool ascending = true)
        {
            var databaseItems = EFContext
                .Set<WorkflowProTask>()
                .Include(m => m.Process)
                .Where(predicate)
                .AsNoTracking();

            int recordCount = databaseItems.Count();
            if (recordCount == 0)
                return new Tuple<int, IList<WorkflowProTask>>(0, new List<WorkflowProTask>());

            return @ascending
                ? new Tuple<int, IList<WorkflowProTask>>(recordCount, databaseItems.OrderBy(keySelector).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList())
                : new Tuple<int, IList<WorkflowProTask>>(recordCount, databaseItems.OrderByDescending(keySelector).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList());
        }
    }
}
