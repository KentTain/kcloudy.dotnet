using System.Collections.Generic;
using KC.Service.DTO.Admin;
using KC.Service.DTO;

namespace KC.Service.Admin
{
    public interface IStoragePoolService
    {
        List<StoragePoolDTO> FindAllStoragePools();
        StoragePoolDTO GetStoragePoolById(int id);
        PaginatedBaseDTO<StoragePoolDTO> FindStoragePoolsByFilter(int pageIndex, int pageSize, string accessName, string order);
        bool RemoveStoragePool(int id);
        bool SaveStoragePool(StoragePoolDTO model);
        string TestStorageConnection(StoragePoolDTO model, string filePath, string privateKey = null);
    }
}