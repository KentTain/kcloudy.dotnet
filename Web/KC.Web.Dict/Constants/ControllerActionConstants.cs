using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using KC.Web.Constants;

namespace KC.Web.Dict.Constants
{
    public sealed class ControllerName : ControllerNameBase
    {
        public const string Dictionary = "Dictionary";

    }

    public sealed class ActionName : ActionNameBase
    {
        public sealed class Dictionary
        {
            public const string SaveDictType = "SaveDictType";
            public const string RemoveDictType = "RemoveDictType";
            public const string LoadDictTypeList = "LoadDictTypeList";
            public const string GetDictTypeForm = "GetDictTypeForm";

            public const string LoadDictValueList = "LoadDictValueList";
            public const string GetDictValueForm = "GetDictValueForm";
            public const string SaveDictValue = "SaveDictValue";
            public const string RemoveDictValue = "RemoveDictValue";
        }
    }
}