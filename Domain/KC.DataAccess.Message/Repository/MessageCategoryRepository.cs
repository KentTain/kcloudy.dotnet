using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using KC.Database.EFRepository;
using KC.Database.Extension;
using KC.Model.Message;
using Microsoft.EntityFrameworkCore;

namespace KC.DataAccess.Message.Repository
{
    public class MessageCategoryRepository : EFTreeRepositoryBase<MessageCategory>, IMessageCategoryRepository
    {
        public MessageCategoryRepository(EFUnitOfWorkContextBase unitOfWork)
            : base(unitOfWork)
        {
        }

        public MessageCategory GetCategoryDetailById(int id)
        {
            return Entities
                .Include(m => m.ChildNodes)
                    .ThenInclude(m => m.ChildNodes)
                .AsNoTracking()
                .FirstOrDefault(m => m.Id == id);
        }

        public async Task<bool> SaveCategoryAsync(MessageCategory model)
        {
            //新增记录时，先保存数据，后更新TreeCode、Level、Leaf字段
            if (model.Id == 0)
            {
                await EFContext.Context.Set<MessageCategory>().AddAsync(model);
                await EFContext.Context.SaveChangesAsync();
            }

            //更新TreeCode、Level、Leaf字段，下面方法未更新字段？？
            //return await UpdateExtendFieldsByFilterAsync(m => m.Id == model.Id);

            if (model.ParentId.HasValue)
            {
                var dbPModel = await EFContext.Context.Set<MessageCategory>().FirstOrDefaultAsync(m => m.Id == model.ParentId);
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
                EFContext.Context.Set<MessageCategory>().Attach(model);
                EFContext.Context.Entry(model).State = EntityState.Modified;
            }

            return await EFContext.Context.SaveChangesAsync() >= 0;
        }
    }
}
