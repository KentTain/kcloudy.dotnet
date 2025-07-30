using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using KC.Database.EFRepository;
using KC.Framework.Extension;
using KC.Model.Doc;
using Microsoft.EntityFrameworkCore;

namespace KC.DataAccess.Doc.Repository
{
    public class DocumentInfoRepository : EFRepositoryBase<DocumentInfo>, IDocumentInfoRepository
    {
        public DocumentInfoRepository(EFUnitOfWorkContextBase unitOfWork) 
            : base(unitOfWork)
        {
        }

        public DocumentInfo FindDocumentByCategoryId(int id)
        {
            return
                Entities
                .Where(m => !m.IsDeleted && m.DocCategory.Id == id)
                .Include(m => m.DocCategory)
                .AsNoTracking()
                .FirstOrDefault();
        }
        public DocumentInfo FindDocumntById(int id)
        {
            return Entities
                .Where(m => m.Id == id)
                .Include(m => m.DocCategory)
                .AsNoTracking()
                .FirstOrDefault();
        }

        public List<DocumentInfo> FindDocumentsByIds(List<int> ids)
        {
            return Entities.Where(m => !m.IsDeleted && ids.Contains(m.Id))
                    .AsNoTracking()
                    .ToList();
        }

        public Tuple<int, List<DocumentInfo>> FindPagenatedByDataPermission(int pageIndex, int pageSize, Expression<Func<DocumentInfo, bool>> predicate, string sortProperty, bool ascending = true)
        {
            var query = Entities
                .Where(predicate)
                .Include(o => o.DocCategory)
                .AsNoTracking();
            var recordCount = query.Count();
            var data = query
                .SingleOrderBy(sortProperty, ascending)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            return new Tuple<int, List<DocumentInfo>>(recordCount, data);
        }
    }
}
