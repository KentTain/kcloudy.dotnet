using AspectCore.DynamicProxy;
using KC.Framework.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace KC.Service.Extension
{
    /// <summary>
    /// 删除缓存处理：默认30天过期
    /// </summary>
    public class CachingDeleteHandlerAttribute : AbstractInterceptorAttribute
    {
        private readonly Type[] _types;
        private readonly string[] _methods;

        /// <summary>
        /// 需传入相同数量的type跟methodName </br>
        ///     同样位置的Type跟Method会组合成一个缓存key，进行删除
        /// </summary>
        /// <param name="type">传入要删除缓存的类</param>
        /// <param name="methodName">传入要删除缓存的方法名称</param>
        public CachingDeleteHandlerAttribute(Type cachedType, string cachedMethodName)
        {
            _types = new Type[] { cachedType };
            _methods = new string[] { cachedMethodName };
        }

        /// <summary>
        /// 需传入相同数量的type跟methodName </br>
        ///     同样位置的Type跟Method会组合成一个缓存key，进行删除
        /// </summary>
        /// <param name="type">传入要删除缓存的类</param>
        /// <param name="methodName">传入要删除缓存的方法名称</param>
        public CachingDeleteHandlerAttribute(Type[] cachedTypes, string[] cachedMethodNames)
        {
            if (cachedTypes.Length != cachedMethodNames.Length)
            {
                throw new ArgumentException("types must be consistent with methods");
            }

            _types = cachedTypes;
            _methods = cachedMethodNames;
        }

        public async override Task Invoke(AspectContext context, AspectDelegate next)
        {
            await next(context);

            var targetType = context.Proxy;
            var tenantName = string.Empty;
            if (targetType is EFService.IEFService)
            {
                var target = targetType as EFService.IEFService;
                if (target.Tenant != null)
                    tenantName = target.Tenant.TenantName;
            }

            var inputs = context.Parameters;
            for (int i = 0; i < _types.Length; i++)
            {
                var type = _types[i];
                var method = _methods[i];
                var targetMethod = type.GetMethod(method);
                // 根据租户代码、调用方法及传入参数生成缓存键值
                var cacheKey = Util.CacheKeyUtil.CacheKeyGenerator(tenantName, targetMethod, inputs);
                LogUtil.LogDebug(string.Format("RemoveCache by cacheKey【{0}】", cacheKey));
                await CacheUtil.RemoveCacheAsync(cacheKey);
            }
        }
    }
}
