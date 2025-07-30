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
    public class MessageClassRepository : EFRepositoryBase<MessageClass>, IMessageClassRepository
    {
        public MessageClassRepository(EFUnitOfWorkContextBase unitOfWork)
            : base(unitOfWork)
        {
        }

        public MessageClass GetMessageClassDetailById(int id)
        {
            return Entities.Include(m => m.MessageTemplates)
                .AsNoTracking()
                .FirstOrDefault(m => m.Id == id);
        }

        public MessageClass GetMessageClassDetailByCode(string code)
        {
            return Entities.Include(m => m.MessageTemplates)
                .AsNoTracking()
                .FirstOrDefault(m => m.Code == code);
        }

        public  List<MessageClass>  GetMessageClassesByMessageCategoryId(int messagecayrId) {
            return Entities
                .Where(m => !m.IsDeleted && m.MessageCategoryId == messagecayrId)
                .AsNoTracking()
                .ToList();
        }

        public override Tuple<int, IList<MessageClass>> FindPagenatedListWithCount<K>(
            int pageIndex, int pageSize, Expression<Func<MessageClass, bool>> predicate,
            Expression<Func<MessageClass, K>> keySelector, bool ascending = true)
        {
            var databaseItems = EFContext.Set<MessageClass>()
                .Include(m => m.MessageTemplates)
                .Where(predicate)
                .AsNoTracking();
            int recordCount = databaseItems.Count();
            if (recordCount == 0)
                return new Tuple<int, IList<MessageClass>>(0, new List<MessageClass>());

            return @ascending
                ? new Tuple<int, IList<MessageClass>>(recordCount, databaseItems.OrderBy(keySelector).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList())
                : new Tuple<int, IList<MessageClass>>(recordCount, databaseItems.OrderByDescending(keySelector).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList());
        }
    }
}
