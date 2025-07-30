using Autofac;
using System;
using System.Threading.Tasks;

namespace SaasKit.Multitenancy.Autofac
{
    public class AutofacTenantContainerBuilder<TTenant> : ITenantContainerBuilder<TTenant>
    {
        public AutofacTenantContainerBuilder(ContainerBuilder container, Action<TTenant> config)
        {
            Ensure.Argument.NotNull(container, nameof(container));

            Container = container;
        }

        protected ContainerBuilder Container { get; }

        public virtual Task<IContainer> BuildAsync(TTenant tenant)
        {
            Ensure.Argument.NotNull(tenant, nameof(tenant));

            var tenantContainer = Container.Build();
            //tenantContainer.Configure(config => Configure(tenant, config));

            return Task.FromResult(tenantContainer);
        }
    }
}
