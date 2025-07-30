using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using ILogger = KC.Framework.Util.Log.ILogger;
using LogLevel = KC.Framework.Util.Log.LogLevel;

namespace KC.Common.LogHelper
{
    public class NlogLoggingService : ILogger
    {
        private static readonly Logger logger;

        static NlogLoggingService()
        {
            logger = LogManager.LoadConfiguration("nlog.config").GetCurrentClassLogger();
            //logger = LogManager.GetCurrentClassLogger();
        }

        public void Log(string msg, string detail, LogLevel level, long timeInMillis)
        {
            switch (level)
            {
                case LogLevel.FATAL:
                case LogLevel.ERROR:
                    logger.Error(msg + Environment.NewLine + detail);
                    break;

                case LogLevel.WARNING:
                    logger.Warn(msg + Environment.NewLine + detail);
                    break;

                case LogLevel.DEBUG:
                    logger.Debug(msg + Environment.NewLine + detail);
                    break;
                case LogLevel.INFO:
                case LogLevel.EXCLAMATION:
                    logger.Info(msg + Environment.NewLine + detail);
                    break;
            }
        }
    }
}
