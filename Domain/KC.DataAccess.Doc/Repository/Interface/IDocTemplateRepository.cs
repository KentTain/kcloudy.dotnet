using KC.Model.Doc;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace KC.DataAccess.Doc.Repository
{
    public interface IDocTemplateRepository : Database.IRepository.IDbRepository<DocTemplate>
    {
        DocTemplate FindDocTemplateById(int id);

        List<DocTemplate> FindDocTemplatesByIds(List<int> ids);

        Tuple<int, List<DocTemplate>> FindPagenatedByDataPermission(int pageIndex, int pageSize,
            Expression<Func<DocTemplate, bool>> predicate, string sortProperty, bool ascending = true);
    }
}