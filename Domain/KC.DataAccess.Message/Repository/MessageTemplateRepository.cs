using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using KC.Database.EFRepository;
using KC.Framework.Extension;
using KC.Model.Message;
using Microsoft.EntityFrameworkCore;

namespace KC.DataAccess.Message.Repository
{
    public class MessageTemplateRepository : EFRepositoryBase<MessageTemplate>, IMessageTemplateRepository
    {
        public MessageTemplateRepository(EFUnitOfWorkContextBase unitOfWork)
            : base(unitOfWork)
        {
        }

        public List<MessageTemplate> FindMessageTemplatesByClassId(int classId)
        {
            return Entities
                .Where(m => !m.IsDeleted && m.MessageClassId == classId)
                .AsNoTracking()
                .ToList();
        }

        public MessageTemplate GetMessageTemplateDetailById(int id)
        {
            return Entities.Include(m => m.MessageClass)
                .AsNoTracking()
                .FirstOrDefault(m => m.Id == id);
        }
    }
}
