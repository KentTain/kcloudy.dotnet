using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.Framework.Util;
using KC.Service.Events;
using KC.Service.DTO.Account;

namespace KC.Service.Account.EventHandlers
{
    public class UserModifiedHandler: IHandler
    {
        private UserDTO _user;

        public UserModifiedHandler(UserDTO user)
        {
            _user = user;
        }

        public bool Handle()
        {
            //TODO: send email to args.Customer

            LogUtil.LogDebug("------------UserCreatedHandler---------");

            return true;
        }
    }
}
