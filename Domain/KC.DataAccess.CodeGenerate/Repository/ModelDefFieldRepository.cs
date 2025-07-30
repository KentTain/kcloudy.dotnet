using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using KC.Database.EFRepository;
using KC.Model.CodeGenerate;
using Microsoft.EntityFrameworkCore;

namespace KC.DataAccess.CodeGenerate.Repository
{
    public class ModelDefFieldRepository : EFRepositoryBase<ModelDefField>, IModelDefFieldRepository
    {
        public ModelDefFieldRepository(EFUnitOfWorkContextBase unitOfWork)
            : base(unitOfWork)
        {
        }

        public override async Task<Tuple<int, IList<ModelDefField>>> FindPagenatedListWithCountAsync<K>(
            int pageIndex, int pageSize, Expression<Func<ModelDefField, bool>> predicate,
            Expression<Func<ModelDefField, K>> keySelector, bool ascending = true)
        {
            var databaseItems = EFContext.Set<ModelDefField>()
                .Include(m => m.ModelDefinition)
                .Where(predicate).AsNoTracking();
            int recordCount = databaseItems.Count();

            return @ascending
                ? new Tuple<int, IList<ModelDefField>>(recordCount, await databaseItems.OrderBy(keySelector).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync())
                : new Tuple<int, IList<ModelDefField>>(recordCount, await databaseItems.OrderByDescending(keySelector).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync());
        }
    }
}
