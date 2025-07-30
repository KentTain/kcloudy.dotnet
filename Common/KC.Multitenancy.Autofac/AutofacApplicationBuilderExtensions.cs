using SaasKit.Multitenancy;
using SaasKit.Multitenancy.Autofac.Internal;

namespace Microsoft.AspNetCore.Builder
{
    public static class AutofacApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseTenantContainers<TTenant>(
            this IApplicationBuilder app)
        {
            Ensure.Argument.NotNull(app, nameof(app));
            return app.UseMiddleware<MultitenantContainerMiddleware<TTenant>>();
        }
    }
}
