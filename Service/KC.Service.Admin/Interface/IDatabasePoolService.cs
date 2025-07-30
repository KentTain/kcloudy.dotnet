using System.Collections.Generic;
using KC.Service.DTO.Admin;
using KC.Service.DTO;

namespace KC.Service.Admin
{
    public interface IDatabasePoolService
    {
        List<DatabasePoolDTO> FindAllDatabasePools();
        DatabasePoolDTO GetDatabasePoolById(int id);
        PaginatedBaseDTO<DatabasePoolDTO> FindDatabasePoolsByFilter(int pageIndex, int pageSize, string server, string database, string userName);
        bool RemoveDatabasePool(int id);
        bool SaveDatabasePool(DatabasePoolDTO model);
        string TestDatabaseConnection(DatabasePoolDTO model, string privateKey = null);
    }
}