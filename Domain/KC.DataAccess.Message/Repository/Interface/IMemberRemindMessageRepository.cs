using System;
using System.Collections.Generic;
using KC.Enums.Message;
using KC.Model.Message;

namespace KC.DataAccess.Message.Repository
{
    public interface IMemberRemindMessageRepository : Database.IRepository.IDbRepository<MemberRemindMessage>
    {

    }
}