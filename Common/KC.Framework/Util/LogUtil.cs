using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KC.Framework.Util.Log;

namespace KC.Framework.Util
{
    /*
      
     AzureLogger : ILogger

     PluginLogger : ILogger
     Field: Logger (NLog)

        ctor()
        {
            // Read Config
        }

    WebLogger : ILogger
    
    SilverlightLogger : ILogger

    Usage: LogUtil.Logger.LogDebug()
     * 
     * TODO: Need to automatically resolve UserInfo;
     * TODO: Need to capture parameters?
     * 
     */
    public static class LogUtil
    {
        #region Log Event

        public delegate void LogEvent(object sender, LogEventArgs e);

        public static event LogEvent LogEventHandlers;

        public class LogEventArgs : EventArgs
        {
            public string Message { get; set; }

            public LogLevel Level { get; set; }

            public long TimeInMillis { get; set; }
        }

        public static void RaiseLogEvent(string msg, LogLevel level, long timeInMillis)
        {
            if (LogEventHandlers != null)
            {
                LogEventHandlers(null, new LogEventArgs { Message = msg, Level = level, TimeInMillis = timeInMillis });
            }
        }

        #endregion

        private static ILogger loggerStatic;

        public static ILogger Logger
        {
            get
            {
                return loggerStatic;
            }

            set
            {
                if (loggerStatic == value) return;

                if (loggerStatic == null)
                {
                    loggerStatic = value;
                    return;
                }

                throw new Exception("Logger must only be set once.");
            }
        }

        public static void LogDebug(string msg, string detail = null, long timeInMillis = 0, long expectedMaxTimeInMillis = 0)
        {
            if (Logger != null)
            {
                if (expectedMaxTimeInMillis > 0 && timeInMillis > 0 && expectedMaxTimeInMillis < timeInMillis)
                {
                    Logger.Log(Framework.Base.GlobalConfig.CurrentApplication?.AppName + "|" + msg, detail, LogLevel.WARNING, timeInMillis);
                }
                else
                {
                    Logger.Log(Framework.Base.GlobalConfig.CurrentApplication?.AppName + "|" + msg, detail, LogLevel.DEBUG, timeInMillis);
                    RaiseLogEvent(msg, LogLevel.DEBUG, timeInMillis);
                }
            }
        }

        public static void LogInfo(string msg, string detail = null, long timeInMillis = 0, long expectedMaxTimeInMillis = 0)
        {
            if (Logger != null)
            {
                if (expectedMaxTimeInMillis > 0 && timeInMillis > 0 && expectedMaxTimeInMillis < timeInMillis)
                {
                    Logger.Log(Framework.Base.GlobalConfig.CurrentApplication?.AppName + "|" + msg, detail, LogLevel.WARNING, timeInMillis);
                }
                else
                {
                    Logger.Log(Framework.Base.GlobalConfig.CurrentApplication?.AppName + "|" + msg, detail, LogLevel.INFO, timeInMillis);

                    RaiseLogEvent(msg, LogLevel.INFO, timeInMillis);
                }
            }
        }

        public static void LogException(Exception e)
        {
            if (Logger != null && e != null)
            {
                if (null != e.InnerException)
                {
                    Logger.Log(Framework.Base.GlobalConfig.CurrentApplication?.AppName + "|" + e.InnerException.Message, e.InnerException.StackTrace, LogLevel.ERROR, 0);
                }
                else
                {
                    Logger.Log(Framework.Base.GlobalConfig.CurrentApplication?.AppName + "|" + e.Message, e.StackTrace, LogLevel.ERROR, 0);
                }
                RaiseLogEvent(e.Message, LogLevel.INFO, 0);
            }
        }

        public static void LogException(string msg,Exception e)
        {
            Logger.Log(msg,string.Empty, LogLevel.ERROR,0);
            if (Logger != null && e != null)
            {
                if (null != e.InnerException)
                {
                    Logger.Log(Framework.Base.GlobalConfig.CurrentApplication?.AppName + "|" + e.InnerException.Message, e.InnerException.StackTrace, LogLevel.ERROR, 0);
                }
                else
                {
                    Logger.Log(Framework.Base.GlobalConfig.CurrentApplication?.AppName + "|" + e.Message, e.StackTrace, LogLevel.ERROR, 0);
                }
                RaiseLogEvent(e.Message, LogLevel.INFO, 0);
            }
        }

        public static void LogError(string msg, string detail = null, long timeInMillis = 0)
        {
            if (Logger != null)
            {
                Logger.Log(Framework.Base.GlobalConfig.CurrentApplication?.AppName + "|" + msg, detail, LogLevel.ERROR, timeInMillis);

                RaiseLogEvent(msg, LogLevel.ERROR, timeInMillis);
            }
        }

        public static void LogFatal(string msg, string detail = null, long timeInMillis = 0)
        {
            if (Logger != null)
            {
                Logger.Log(Framework.Base.GlobalConfig.CurrentApplication?.AppName + "|" + msg, detail, LogLevel.FATAL, timeInMillis);

                RaiseLogEvent(msg, LogLevel.FATAL, timeInMillis);
            }
        }

        public static void LogWarning(string msg, string detail = null, long timeInMillis = 0)
        {
            if (Logger != null)
            {
                Logger.Log(Framework.Base.GlobalConfig.CurrentApplication?.AppName + "|" + msg, detail, LogLevel.WARNING, timeInMillis);

                RaiseLogEvent(msg, LogLevel.WARNING, timeInMillis);
            }
        }

        public static void LogExclamation(string msg, string detail = null, long timeInMillis = 0)
        {
            if (Logger != null)
            {
                Logger.Log(Framework.Base.GlobalConfig.CurrentApplication?.AppName + "|" + msg, detail, LogLevel.EXCLAMATION, timeInMillis);

                RaiseLogEvent(msg, LogLevel.EXCLAMATION, timeInMillis);
            }
        }

        /// <summary>
        /// only use for some method that should not log everytime, otherwise it will generate too much log.
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="detail"></param>
        /// <param name="timeInMillis"></param>
        /// <param name="expectedMaxTimeInMillis"></param>
        public static void LogWarningIfExceedExpectedTime(string msg, string detail, long timeInMillis, long expectedMaxTimeInMillis)
        {
            if (Logger != null)
            {
                if (expectedMaxTimeInMillis > 0 && timeInMillis > 0 && expectedMaxTimeInMillis < timeInMillis)
                {
                    Logger.Log(Framework.Base.GlobalConfig.CurrentApplication?.AppName + "|" + msg, detail, LogLevel.WARNING, timeInMillis);
                }
            }
        }
    }
}