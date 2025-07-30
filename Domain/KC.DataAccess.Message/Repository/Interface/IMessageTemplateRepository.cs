using KC.Model.Message;
using System.Collections.Generic;

namespace KC.DataAccess.Message.Repository
{
    public interface IMessageTemplateRepository : Database.IRepository.IDbRepository<MessageTemplate>
    {

        List<MessageTemplate> FindMessageTemplatesByClassId(int classId);

        MessageTemplate GetMessageTemplateDetailById(int id);
    }
}