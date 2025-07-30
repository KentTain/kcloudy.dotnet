using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using KC.Model.Message;

namespace KC.DataAccess.Message.Repository
{
    public interface IMessageCategoryRepository : Database.IRepository.IDbTreeRepository<MessageCategory>
    {
        MessageCategory GetCategoryDetailById(int id);

        Task<bool> SaveCategoryAsync(MessageCategory model);
    }
}