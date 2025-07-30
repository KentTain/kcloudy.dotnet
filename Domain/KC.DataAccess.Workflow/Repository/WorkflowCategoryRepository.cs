using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using KC.Database.EFRepository;
using KC.DataAccess.Workflow;
using KC.Model.Workflow;
using Microsoft.EntityFrameworkCore;

namespace KC.DataAccess.Workflow.Repository
{
    public class WorkflowCategoryRepository : EFTreeRepositoryBase<WorkflowCategory>, IWorkflowCategoryRepository
    {
        public WorkflowCategoryRepository(EFUnitOfWorkContextBase unitOfWork)
            : base(unitOfWork)
        {
        }

        public IList<WorkflowCategory> GetRootWorkflowCategory()
        {
            //base.EFContext.Context.Configuration.LazyLoadingEnabled = true;
            var result = Entities
                .Include(m => m.ChildNodes.Select(c => c.ChildNodes.Select(p => p.ChildNodes)))
                .Where(m => m.ParentId == null && !m.IsDeleted)
                .AsNoTracking()
                .OrderBy(m => m.Index)
                .ToList();
            //base.EFContext.Context.Configuration.LazyLoadingEnabled = false;

            return result;
        }

        public IList<WorkflowCategory> GetRootWorkflowCategoriesByFilter<K>(Expression<Func<WorkflowCategory, bool>> predicate, Expression<Func<WorkflowCategory, K>> keySelector, bool ascending = true)
        {
            var result = @ascending
                ? Entities
                    .Include(m => m.ChildNodes)
                    .Where(predicate)
                    .AsNoTracking()
                    .OrderBy(keySelector)
                    .ToList()
                : Entities
                    .Include(m => m.ChildNodes)
                    .Where(predicate)
                    .AsNoTracking()
                    .OrderByDescending(keySelector)
                    .ToList();
            return result;
        }

        public IList<WorkflowCategory> GetWorkflowCategoriesByIds(List<int> industryIds)
        {
            var result = Entities
                .Include(m => m.ChildNodes.Select(c => c.ChildNodes))
                .Where(m => industryIds.Contains(m.Id) && !m.IsDeleted)
                .AsNoTracking()
                .OrderBy(m => m.Index)
                .ToList();
            return result;
        }

        public WorkflowCategory GetWorkflowCategoryById(int Id)
        {
            var result = Entities
                .Include(m => m.ChildNodes.Select(c => c.ChildNodes))
                .AsNoTracking()
                .FirstOrDefault(m => m.Id == Id);
            return result;
        }

        public WorkflowCategory GetWorkflowCategoryByFilter(Expression<Func<WorkflowCategory, bool>> predicate)
        {
            var result = Entities
                .Include(m => m.ChildNodes)
                .Include(m => m.ParentNode)
                .AsNoTracking()
                .FirstOrDefault(predicate);
            return result;
        }

        public List<WorkflowCategory> GetWorkflowCategoryList(int level)
        {
            var result = Entities
                .Include(m => m.ChildNodes)
                .Where(m => m.Level == level)
                .AsNoTracking()
                .ToList();
            return result;
        }

        public List<WorkflowCategory> GetChildWorkflowCategoryList(int parentId)
        {
            var result = Entities
                .Include(m => m.ChildNodes)
                .Where(m => m.ParentId == parentId)
                .AsNoTracking()
                .ToList();
            return result;

        }

        //public IndustryClassfication FindById(int id)
        //{
        //    var result = Entities
        //        //.Include(m => m.IndustryStandard)
        //        .FirstOrDefault(i => i.Id == id && i.IsValid);
        //    return result;
        //}
    }
}
