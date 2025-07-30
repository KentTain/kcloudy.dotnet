using System.Collections.Generic;
using KC.Service.DTO.Admin;
using KC.Service.DTO;

namespace KC.Service.Admin
{
    public interface IQueuePoolService
    {
        List<QueuePoolDTO> FindAllQueuePools();
        QueuePoolDTO GetQueuePoolById(int id);
        PaginatedBaseDTO<QueuePoolDTO> FindQueuePoolsByFilter(int pageIndex, int pageSize, string accessName, string order);
        bool RemoveQueuePool(int id);
        bool SaveQueuePool(QueuePoolDTO model);
        string TestQueueConnection(QueuePoolDTO model, string privateKey = null);
    }
}