using SaasKit.Multitenancy;
using SaasKit.Multitenancy.Autofac;
using System;

namespace Autofac
{
    public static class MultitenancyContainerExtensions
    {
        public static void ConfigureTenants<TTenant>(this ContainerBuilder container, Action<ContainerBuilder> configure)
        {
            Ensure.Argument.NotNull(container, nameof(container));
            //Ensure.Argument.NotNull(configure, nameof(configure));

            container.RegisterType<AutofacTenantContainerBuilder<TTenant>>()
                    .As<ITenantContainerBuilder<TTenant>>();
        }

        public static void ConfigureTenants<TTenant>(this ContainerBuilder container, Action<TTenant, ContainerBuilder> configure)
        {
            Ensure.Argument.NotNull(container, nameof(container));
            //Ensure.Argument.NotNull(configure, nameof(configure));

            container.RegisterType<AutofacTenantContainerBuilder<TTenant>>()
                    .As<ITenantContainerBuilder<TTenant>>();
        }
    }
}
