using KC.Database.EFRepository;
using KC.Database.Extension;
using KC.Enums.Doc;
using KC.Framework.Extension;
using KC.Model.Doc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace KC.DataAccess.Doc.Repository
{
    public class DocCategoryRepository : EFTreeRepositoryBase<DocCategory>, IDocCategoryRepository
    {
        public DocCategoryRepository(EFUnitOfWorkContextBase unitOfWork)
            : base(unitOfWork)
        {
        }

        public List<DocCategory> FindFatherCategory(int? id, LableType? type)
        {
            Expression<Func<DocCategory, bool>> predicate = m => !m.IsDeleted && m.Level == 1;
            if (type.HasValue)
            {
                predicate = predicate.And(m => m.Type == type.Value);
            }
            if (id.HasValue)
            {
                predicate = predicate.And(m => m.Id != id.Value);
            }
            return Entities
                .Where(predicate)
                .OrderByDescending(m => m.CreatedDate)
                .AsNoTracking()
                .ToList();
        }

        public async Task<DocCategory> GetDocCategoryByIdAsync(int id)
        {
            var result = await EFContext.Set<DocCategory>()
                .Where(m => !m.IsDeleted && m.Id == id)
                .AsNoTracking()
                .FirstOrDefaultAsync();
            //EFContext.Context.Detach(result);
            return result;
        }

        public DocCategory GetDocCategoryWithChildrenById(int id)
        {
            return Entities
                .Where(m => !m.IsDeleted && m.Id == id)
                .Include(m => m.ChildNodes)
                .Include(m => m.DocumentInfos)
                .AsNoTracking()
                .FirstOrDefault();
        }

        public async Task<bool> SaveDocCategoryAsync(DocCategory model)
        {
            //新增记录时，先保存数据，后更新TreeCode、Level、Leaf字段
            if (model.Id == 0)
            {
                await EFContext.Context.Set<DocCategory>().AddAsync(model);
                await EFContext.Context.SaveChangesAsync();
            }

            //更新TreeCode、Level、Leaf字段，下面方法未更新字段？？
            //return await UpdateExtendFieldsByFilterAsync(m => m.Id == model.Id);

            if (model.ParentId.HasValue)
            {
                var dbPModel = await EFContext.Context.Set<DocCategory>().FirstOrDefaultAsync(m => m.Id == model.ParentId);
                model.Level = dbPModel.Level + 1;
                model.TreeCode = dbPModel.TreeCode + DatabaseExtensions.TreeCodeSplitIdWithChar + model.Id;
            }
            else
            {
                model.Level = 1;
                model.TreeCode = model.Id.ToString();
            }
            if (EFContext.Context.Entry(model).State == EntityState.Detached)
            {
                EFContext.Context.Set<DocCategory>().Attach(model);
                EFContext.Context.Entry(model).State = EntityState.Modified;
            }

            return await EFContext.Context.SaveChangesAsync() >= 0;
        }
    }
}
