using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Model.Message.Constants
{
    public sealed class Tables
    {
        private const string Prx = "msg_";

        public const string MessageClass = Prx + "MessageClass";
        public const string MessageCategory = Prx + "MessageCategory";
        public const string MessageTemplate = Prx + "MessageTemplate";
        public const string MessageTemplateLog = Prx + "MessageTemplateLog";
        public const string MemberRemindMessage = Prx + "MemberRemindMessage";
        public const string MemberMessageReceiver = Prx + "MemberMessageReceiver";

        public const string NewsBulletinCategory = Prx + "NewsBulletinCategory";
        public const string NewsBulletin = Prx + "NewsBulletin";
        public const string NewsBulletinLog = Prx + "NewsBulletinLog";

    }
}
