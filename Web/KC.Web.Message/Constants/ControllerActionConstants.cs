using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using KC.Web.Constants;

namespace KC.Web.Message.Constants
{
    public sealed class ControllerName : ControllerNameBase
    {
        public const string NewsBulletin = "NewsBulletin";
        public const string Message = "Message";
    }

    public sealed class ActionName : ActionNameBase
    {
        public sealed class Message
        {
            public const string LoadCategoryTree = "LoadCategoryTree";
            public const string GetMessageCategoryForm = "GetMessageCategoryForm";
            public const string SaveMessageCategory = "SaveMessageCategory";
            public const string RemoveMessageCategory = "RemoveMessageCategory";
            public const string ExistCategoryName = "ExistCategoryName";

            public const string LoadMessageClassList = "LoadMessageClassList";
            public const string GetMessageClassForm = "GetMessageClassForm";
            public const string SaveMessageClassForm = "SaveMessageClassForm";
            public const string RemoveMessageClass = "RemoveMessageClass";
            public const string ExistMessageClassName = "ExistMessageClassName";

            public const string LoadMessageTemplateList = "LoadMessageTemplateList";
            public const string GetMessageTemplateForm = "GetMessageTemplateForm";
            public const string RemoveMessageTemplate = "RemoveMessageTemplate";

            public const string SaveSmsTemplate = "SaveSmsTemplate";
            public const string GetCommonTemplateForm = "GetCommonTemplateForm";
            public const string SaveCommonTemplate = "SaveCommonTemplate";

            public const string LoadMessageTemplateLogList = "LoadMessageTemplateLogList";

            public const string MyMessageList = "MyMessageList";
            public const string LoadMyMessages = "LoadMyMessages";
            public const string MessageDetail = "MessageDetail";

        }

        public sealed class NewsBulletin
        {
            public const string LoadCategoryTree = "LoadCategoryTree";
            public const string GetCategoryForm = "GetCategoryForm";
            public const string SaveCategory = "SaveCategory";
            public const string RemoveCategory = "RemoveCategory";
            public const string ExistCategoryName = "ExistCategoryName";

            public const string LoadNewsBulletins = "LoadNewsBulletins";
            public const string GetNewsBulletinForm = "GetNewsBulletinForm";
            public const string SaveNewsBulletin = "SaveNewsBulletin";
            public const string DeleteNewsBulletin = "DeleteNewsBulletin";

            public const string NewsBulletinList = "NewsBulletinList";
            public const string NewsBulletinDetail = "NewsBulletinDetail";

            public const string NewsBulletinLog = "NewsBulletinLog";
            public const string LoadNewsBulletinList = "LoadNewsBulletinList";

        }
    }
}