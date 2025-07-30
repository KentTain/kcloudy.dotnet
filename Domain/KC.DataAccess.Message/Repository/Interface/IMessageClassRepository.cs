using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using KC.Model.Message;

namespace KC.DataAccess.Message.Repository
{
    public interface IMessageClassRepository : Database.IRepository.IDbRepository<MessageClass>
    {
        MessageClass GetMessageClassDetailById(int id);
        MessageClass GetMessageClassDetailByCode(string code);
        List<MessageClass> GetMessageClassesByMessageCategoryId(int messagecayrId);
    }
}