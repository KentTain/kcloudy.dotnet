using System.Collections.Generic;
using KC.Service.DTO.Admin;
using KC.Service.DTO;

namespace KC.Service.Admin
{
    public interface INoSqlPoolService
    {
        List<NoSqlPoolDTO> FindAllNoSqlPools();
        NoSqlPoolDTO GetNoSqlPoolById(int id);
        PaginatedBaseDTO<NoSqlPoolDTO> FindNoSqlPoolsByFilter(int pageIndex, int pageSize, string accessName, string order);
        bool RemoveNoSqlPool(int id);
        bool SaveNoSqlPool(NoSqlPoolDTO model);
        string TestNoSqlConnection(NoSqlPoolDTO model, string privateKey = null);
    }
}