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
    public class WorkflowProTaskExecuteRepository : EFRepositoryBase<WorkflowProTaskExecute>, IWorkflowProTaskExecuteRepository
    {
        public WorkflowProTaskExecuteRepository(EFUnitOfWorkContextBase unitOfWork) 
            : base(unitOfWork)
        {
        }

        public WorkflowProTaskExecute GetWorkflowTaskExecuteById(int id)
        {
            return Entities
                .Include(m => m.Task)
                    .ThenInclude(p => p.Process)
                .AsNoTracking()
                .FirstOrDefault(m => m.Id.Equals(id));
        }

        public List<WorkflowProTaskExecute> FindWorkflowTaskExecutesByTaskId(int taskId)
        {
            return Entities
                .Include(m => m.Task)
                    .ThenInclude(p => p.Process)
                .AsNoTracking()
                .Where(m => m.TaskId.Equals(taskId))
                .ToList();
        }

        public override Tuple<int, IList<WorkflowProTaskExecute>> FindPagenatedListWithCount<K>(
            int pageIndex, int pageSize, Expression<Func<WorkflowProTaskExecute, bool>> predicate,
            Expression<Func<WorkflowProTaskExecute, K>> keySelector, bool ascending = true)
        {
            var databaseItems = EFContext
                .Set<WorkflowProTaskExecute>()
                .Include(m => m.Task)
                    .ThenInclude(p => p.Process)
                .Where(predicate)
                .AsNoTracking();

            int recordCount = databaseItems.Count();
            if (recordCount == 0)
                return new Tuple<int, IList<WorkflowProTaskExecute>>(0, new List<WorkflowProTaskExecute>());

            return @ascending
                ? new Tuple<int, IList<WorkflowProTaskExecute>>(recordCount, databaseItems.OrderBy(keySelector).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList())
                : new Tuple<int, IList<WorkflowProTaskExecute>>(recordCount, databaseItems.OrderByDescending(keySelector).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList());
        }
    }
}
