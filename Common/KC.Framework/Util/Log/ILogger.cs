using System;
using System.Collections.Generic;
using System.Text;

namespace KC.Framework.Util.Log
{
    public enum LogLevel
    {
        NONE = 0,
        DEBUG = 1,
        INFO = 2,
        EXCLAMATION = 3,
        WARNING = 4,
        ERROR = 5,
        FATAL = 6
    }

    public interface ILogger
    {
        void Log(string msg, string detail, LogLevel level, long timeInMillis);
    }
}
