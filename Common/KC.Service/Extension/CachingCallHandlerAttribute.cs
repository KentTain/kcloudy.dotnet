using AspectCore.DynamicProxy;
using AspectCore.DynamicProxy.Parameters;
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
    public class CachingCallHandlerAttribute : AbstractInterceptorAttribute
    {
        public int ExpirationMinutes { get; set; }
        private int DefaultExpirationMinutes = TimeOutConstants.DefaultCacheTimeOut;

        public CachingCallHandlerAttribute()
        {
            this.ExpirationMinutes = DefaultExpirationMinutes;
        }
        public CachingCallHandlerAttribute(int expiratinMinutes)
        {
            this.ExpirationMinutes = expiratinMinutes;
        }

        public async override Task Invoke(AspectContext context, AspectDelegate next)
        {
            //判断是否是异步方法
            bool isAsync = context.IsAsync();
            
            //先判断方法是否有返回值，无就不进行缓存判断
            var methodReturnType = context.GetReturnParameter().Type;
            if (methodReturnType == typeof(void) || methodReturnType == typeof(Task) || methodReturnType == typeof(ValueTask))
            {
                await next(context);
                return;
            }
            var returnType = methodReturnType;
            if (isAsync)
            {
                //取得异步返回的类型
                returnType = returnType.GenericTypeArguments.FirstOrDefault();
            }

            var targetType = context.Proxy;
            var targetMethod = context.ServiceMethod;
            var inputs = context.Parameters;

            var tenantName = string.Empty;
            if (targetType is EFService.IEFService)
            {
                var target = targetType as EFService.IEFService;
                if (target.Tenant != null)
                    tenantName = target.Tenant.TenantName;
            }

            // 根据租户代码、调用方法及传入参数生成缓存键值
            var cacheKey = Util.CacheKeyUtil.CacheKeyGenerator(tenantName, targetMethod, inputs);
            var cachedResult = await CacheUtil.GetCacheAsync<object>(returnType, cacheKey);
            if (cachedResult != null)
            {
                if (isAsync)
                {
                    //判断是Task还是ValueTask
                    if (methodReturnType == typeof(Task<>).MakeGenericType(returnType))
                    {
                        //反射获取Task<>类型的返回值，相当于Task.FromResult(value)
                        context.ReturnValue = typeof(Task).GetMethod(nameof(Task.FromResult)).MakeGenericMethod(returnType).Invoke(null, new[] { cachedResult });
                    }
                    else if (methodReturnType == typeof(ValueTask<>).MakeGenericType(returnType))
                    {
                        //反射构建ValueTask<>类型的返回值，相当于new ValueTask(value)
                        context.ReturnValue = Activator.CreateInstance(typeof(ValueTask<>).MakeGenericType(returnType), cachedResult);
                    }
                }
                else
                {
                    context.ReturnValue = cachedResult;
                }

                LogUtil.LogDebug(string.Format("getCache by cacheKey【{0}】: {1}", cacheKey, returnType.FullName));
                return;
            }

            await next(context);
            object returnValue;
            if (isAsync)
            {
                returnValue = await context.UnwrapAsyncReturnValue();
                //反射获取异步结果的值，相当于(context.ReturnValue as Task<>).Result
                //returnValue = typeof(Task<>).MakeGenericType(returnType).GetProperty(nameof(Task<object>.Result)).GetValue(context.ReturnValue);
            }
            else
            {
                returnValue = context.ReturnValue;
            }

            if (returnValue != null)
            {
                LogUtil.LogDebug(string.Format("SetCache by cacheKey【{0}】", cacheKey));
                await CacheUtil.SetCacheAsync(cacheKey, returnValue, TimeSpan.FromMinutes(ExpirationMinutes));
            }
        }
    }
}
