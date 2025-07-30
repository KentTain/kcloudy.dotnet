using System.Collections.Generic;
using KC.Service.DTO.Account;
using KC.Service.DTO.Message;

namespace KC.Service.WebApiService.Business
{
    public interface IMessageApiService
    {
        MessageClassDTO GetMessageClassByCode(string code);
        MessageClassDTO GetMessageClassByName(string name);

        bool AddMessageLog(MessageLogDTO data);
        bool AddRemindMessages(List<MemberRemindMessageDTO> data);
    }
}