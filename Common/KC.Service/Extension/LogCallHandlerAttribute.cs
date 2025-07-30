using AspectCore.DynamicProxy;
using KC.Common.LogHelper;
using KC.Framework.Util;
using KC.Service.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace KC.Service.Extension
{
    /// <summary>
    /// 缓存处理：默认30天过期
    /// </summary>
    public class LogCallHandlerAttribute : AbstractInterceptorAttribute
    {
        public override Task Invoke(AspectContext context, AspectDelegate next)
        {
            string title = string.Empty;
            var targetMethod = context.ProxyMethod;
            if (targetMethod != null)
            {
                title = string.Format("Method({0}) throw exception: ", targetMethod.Name);
            }

            var realReturn = next(context);
            if (realReturn.Exception != null)
            {
                if (LogUtil.Logger == null)
                    LogUtil.Logger = new NlogLoggingService();

                var msg = string.Format("Message: {0}, " + Environment.NewLine + "Stack Trace: {1}",
                    realReturn.Exception.Message, realReturn.Exception.StackTrace);
                LogUtil.LogError(title, msg);
            }

            return realReturn;
        }
    }
}
