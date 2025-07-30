using KC.Service.DTO.Admin;
using KC.Service.DTO;

namespace KC.Service.Admin
{
    public interface IServiceBusPoolService
    {
        bool DeleteServiceBusPool(int id);
        PaginatedBaseDTO<ServiceBusPoolDTO> FindPaginatedServiceBusPoolList(int pageIndex, int pageSize, string accessName);
        ServiceBusPoolDTO GetServiceBusPoolbyId(int id);
        bool SaveServiceBusPool(ServiceBusPoolDTO sbpool);
        string TestServiceBusConnection(ServiceBusPoolDTO model, string privateKey = null);
    }
}