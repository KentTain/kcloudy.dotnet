using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using KC.Model.Workflow;

namespace KC.DataAccess.Workflow.Repository
{
    public interface IWorkflowCategoryRepository : Database.IRepository.IDbTreeRepository<WorkflowCategory>
    {
        List<WorkflowCategory> GetChildWorkflowCategoryList(int parentId);
        WorkflowCategory GetWorkflowCategoryByFilter(Expression<Func<WorkflowCategory, bool>> predicate);
        WorkflowCategory GetWorkflowCategoryById(int Id);
        List<WorkflowCategory> GetWorkflowCategoryList(int level);
        IList<WorkflowCategory> GetWorkflowCategoriesByIds(List<int> industryIds);
        IList<WorkflowCategory> GetRootWorkflowCategoriesByFilter<K>(Expression<Func<WorkflowCategory, bool>> predicate, Expression<Func<WorkflowCategory, K>> keySelector, bool ascending = true);
        IList<WorkflowCategory> GetRootWorkflowCategory();
    }
}