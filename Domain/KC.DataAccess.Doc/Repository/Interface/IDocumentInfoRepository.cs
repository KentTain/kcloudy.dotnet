using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using KC.Enums.Doc;
using KC.Model.Doc;

namespace KC.DataAccess.Doc.Repository
{
    public interface IDocumentInfoRepository : Database.IRepository.IDbRepository<DocumentInfo>
    {
        DocumentInfo FindDocumentByCategoryId(int id);
        DocumentInfo FindDocumntById(int id);

        List<DocumentInfo> FindDocumentsByIds(List<int> ids);

        Tuple<int, List<DocumentInfo>> FindPagenatedByDataPermission(int pageIndex, int pageSize,
            Expression<Func<DocumentInfo, bool>> predicate, string sortProperty, bool ascending = true);
        
    }
}