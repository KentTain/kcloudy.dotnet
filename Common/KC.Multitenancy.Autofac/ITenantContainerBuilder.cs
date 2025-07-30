using Autofac;
using System.Threading.Tasks;

namespace SaasKit.Multitenancy.Autofac
{
    public interface ITenantContainerBuilder<TTenant>
    {
        Task<IContainer> BuildAsync(TTenant tenant);
    }
}
