using Microsoft.AspNetCore.Http;
using Autofac;
using System;
using System.Threading.Tasks;
using Autofac.Extensions.DependencyInjection;

namespace SaasKit.Multitenancy.Autofac.Internal
{
    internal class MultitenantContainerMiddleware<TTenant>
    {
        private readonly RequestDelegate next;

        public MultitenantContainerMiddleware(RequestDelegate next)
        {
            Ensure.Argument.NotNull(next, nameof(next));
            this.next = next;
        }

        public async Task Invoke(HttpContext context, Lazy<ITenantContainerBuilder<TTenant>> builder)
        {
            Ensure.Argument.NotNull(context, nameof(context));

            var tenantContext = context.GetTenantContext<TTenant>();

            if (tenantContext != null)
            {
                var tenantContainer = await GetTenantContainerAsync(tenantContext, builder);

                using (var scope = tenantContainer.BeginLifetimeScope(tenantContext.Tenant))
                {
                    // This service provider will have access to global singletons
                    // and registrations but the "singletons" for things registered
                    // in the service collection will be "rooted" under this
                    // child scope, unavailable to the rest of the application.
                    //
                    // Obviously it's not super helpful being in this using block,
                    // so likely you'll create the scope at app startup, keep it
                    // around during the app lifetime, and dispose of it manually
                    // yourself during app shutdown.
                    context.RequestServices = new AutofacServiceProvider(scope);
                    await next.Invoke(context);
                };

                //For IoC stracture: StructureMap 
                //using (var requestContainer = tenantContainer.GetNestedContainer())
                //{
                //    // Replace the request IServiceProvider created by IServiceScopeFactory
                //    context.RequestServices = requestContainer.GetInstance<IServiceProvider>();
                //    await next.Invoke(context);
                //}
            }
        }

        private async Task<IContainer> GetTenantContainerAsync(
            TenantContext<TTenant> tenantContext, 
            Lazy<ITenantContainerBuilder<TTenant>> builder)
        {
            var tenantContainer = tenantContext.GetTenantContainer();

            if (tenantContainer == null)
            {
                tenantContainer = await builder.Value.BuildAsync(tenantContext.Tenant);
                tenantContext.SetTenantContainer(tenantContainer);
            }

            return tenantContainer;
        }
    }
}
