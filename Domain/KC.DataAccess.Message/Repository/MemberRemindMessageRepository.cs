using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using KC.Framework.Extension;
using KC.Database.EFRepository;
using KC.Enums.Message;
using KC.Model.Message;
using Microsoft.EntityFrameworkCore;

namespace KC.DataAccess.Message.Repository
{
    public class MemberRemindMessageRepository : EFRepositoryBase<MemberRemindMessage>, IMemberRemindMessageRepository
    {
        public MemberRemindMessageRepository(EFUnitOfWorkContextBase unitOfWork)
            : base(unitOfWork)
        {
        }

        
    }
}
