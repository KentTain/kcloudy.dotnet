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
    public class DocTemplateRepository : EFRepositoryBase<DocTemplate>, IDocTemplateRepository
    {
        public DocTemplateRepository(EFUnitOfWorkContextBase unitOfWork)
            : base(unitOfWork)
        {
        }

        public DocTemplate FindDocTemplateById(int id)
        {
            return Entities
                .Where(m => m.Id == id)
                .AsNoTracking()
                .FirstOrDefault();
        }

        public List<DocTemplate> FindDocTemplatesByIds(List<int> ids)
        {
            return
                Entities.Where(m => !m.IsDeleted && ids.Contains(m.Id))
                    .AsNoTracking()
                    .ToList();
        }

        public Tuple<int, List<DocTemplate>> FindPagenatedByDataPermission(int pageIndex, int pageSize, Expression<Func<DocTemplate, bool>> predicate, string sortProperty, bool ascending = true)
        {
            var a = Entities.Where(predicate).AsNoTracking();
            var recordCount = a.Count();
            var data = a.SingleOrderBy(sortProperty, ascending).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

            return new Tuple<int, List<DocTemplate>>(recordCount, data);
        }
    }
}
