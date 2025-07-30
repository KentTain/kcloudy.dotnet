using KC.Database.EFRepository;
using KC.Database.Extension;
using KC.Enums.Message;
using KC.Framework.Extension;
using KC.Model.Message;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace KC.DataAccess.Message.Repository
{
    public class NewsBulletinCategoryRepository : EFTreeRepositoryBase<NewsBulletinCategory>, INewsBulletinCategoryRepository
    {
        public NewsBulletinCategoryRepository(EFUnitOfWorkContextBase unitOfWork)
            : base(unitOfWork)
        {
        }

        public NewsBulletinCategory GetCategoryWithChildrenById(int id)
        {
            return Entities
                .Where(m => !m.IsDeleted && m.Id == id)
                .Include(m => m.ChildNodes)
                .Include(m => m.NewsBulletins)
                .AsNoTracking()
                .FirstOrDefault();
        }

        public async Task<bool> SaveCategoryAsync(NewsBulletinCategory model)
        {
            //新增记录时，先保存数据，后更新TreeCode、Level、Leaf字段
            if (model.Id == 0)
            {
                await EFContext.Context.Set<NewsBulletinCategory>().AddAsync(model);
                await EFContext.Context.SaveChangesAsync();
            }

            //更新TreeCode、Level、Leaf字段，下面方法未更新字段？？
            //return await UpdateExtendFieldsByFilterAsync(m => m.Id == model.Id);

            if (model.ParentId.HasValue)
            {
                var dbPModel = await EFContext.Context.Set<NewsBulletinCategory>().FirstOrDefaultAsync(m => m.Id == model.ParentId);
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
                EFContext.Context.Set<NewsBulletinCategory>().Attach(model);
                EFContext.Context.Entry(model).State = EntityState.Modified;
            }

            return await EFContext.Context.SaveChangesAsync() >= 0;
        }
    }
}
