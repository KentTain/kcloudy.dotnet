using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using KC.Web.Constants;

namespace KC.Web.Training.Constants
{
    public sealed class ControllerName : ControllerNameBase
    {
        public const string ConfigManager = "ConfigManager";

        public const string SysSequence = "SysSequence";
    }

    public sealed class ActionName : ActionNameBase
    {
        public sealed class ConfigManager
        {
            public const string GetConfigTypeList = "GetConfigTypeList";

            public const string LoadConfigList = "LoadConfigList";
            public const string GetConfigForm = "GetConfigForm";
            public const string SaveConfig = "SaveConfig";
            public const string RemoveConfig = "RemoveConfig";

            public const string LoadPropertyList = "LoadPropertyList";
            public const string GetPropertyForm = "GetPropertyForm";
            public const string SaveConfigAttribute = "SaveConfigAttribute";
            public const string RemoveConfigAttribute = "RemoveConfigAttribute";
        }

        public sealed class SysSequence
        {

            public const string LoadSysSequenceList = "LoadSysSequenceList";
            public const string GetSysSequenceForm = "GetSysSequenceForm";
            public const string SaveSysSequence = "SaveSysSequence";
            public const string RemoveSysSequence = "RemoveSysSequence";
        }
    }
}