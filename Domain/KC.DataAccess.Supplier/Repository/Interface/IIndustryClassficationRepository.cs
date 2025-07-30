using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using KC.Model.Supplier;

namespace KC.DataAccess.Supplier.Repository
{
    public interface IIndustryClassficationRepository : Database.IRepository.IDbTreeRepository<IndustryClassfication>
    {
        List<IndustryClassfication> GetChildIndustryClassificationList(int parentId);
        IndustryClassfication GetIndustryClassificationByFilter(Expression<Func<IndustryClassfication, bool>> predicate);
        IndustryClassfication GetIndustryClassificationById(int Id);
        List<IndustryClassfication> GetIndustryClassificationList(int level);
        IList<IndustryClassfication> GetIndustryClassificationsByIds(List<int> industryIds);
        IList<IndustryClassfication> GetRootIndeustryClassificatiosByFilter<K>(Expression<Func<IndustryClassfication, bool>> predicate, Expression<Func<IndustryClassfication, K>> keySelector, bool ascending = true);
        IList<IndustryClassfication> GetRootIndustryClassification();
    }
}