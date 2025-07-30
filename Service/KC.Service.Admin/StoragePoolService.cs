using AutoMapper;
using KC.Database.IRepository;
using KC.Framework.Extension;
using KC.Model.Admin;
using KC.Service.EFService;
using KC.Service.WebApiService.Business;
using KC.Service.DTO.Admin;
using KC.Service.DTO;
using KC.Service.Component;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using KC.Storage.Util;

namespace KC.Service.Admin
{
    public class StoragePoolService : EFServiceBase, IStoragePoolService
    {
        private readonly IMapper _mapper;

        private readonly IDbRepository<StoragePool> _storagePoolRepository;

        public StoragePoolService(
            IMapper mapper,
            IDbRepository<StoragePool> StoragePoolRepository,
            System.Net.Http.IHttpClientFactory clientFactory,
            Microsoft.Extensions.Logging.ILogger<StoragePoolService> logger)
            : base(clientFactory, logger)
        {
            _mapper = mapper;
            _storagePoolRepository = StoragePoolRepository;
        }

        /// <summary>
        /// 获取存储链接列表 + List<StoragePoolDTO> GetAllStoragePools()
        /// </summary>
        /// <returns></returns>
        public List<StoragePoolDTO> FindAllStoragePools()
        {
            var data = _storagePoolRepository.FindAll().ToList();
            return _mapper.Map<List<StoragePoolDTO>>(data);
        }

        /// <summary>
        /// 条件搜索存储链接对象 + PaginatedBaseDTO<StoragePoolDTO> GetStoragePoolsByFilter(int pageIndex, int pageSize, string accessName)
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="accessName"></param>
        /// <returns></returns>
        public PaginatedBaseDTO<StoragePoolDTO> FindStoragePoolsByFilter(int pageIndex, int pageSize, string accessName, string order)
        {
            Expression<Func<StoragePool, bool>> predicate = m => true;
            if (!string.IsNullOrWhiteSpace(accessName))
            {
                predicate = predicate.And(m => m.AccessName.Contains(accessName));
            }

            var data = _storagePoolRepository.FindPagenatedListWithCount(
                pageIndex,
                pageSize,
                predicate,
                m => m.PasswordExpiredTime, order.Equals("asc"));

            var total = data.Item1;
            var rows = _mapper.Map<List<StoragePoolDTO>>(data.Item2);
            return new PaginatedBaseDTO<StoragePoolDTO>(pageIndex, pageSize, total, rows);
        }
        /// <summary>
        /// Id查找存储链接对象 + StoragePoolDTO GetStoragePoolById(int id)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public StoragePoolDTO GetStoragePoolById(int id)
        {
            var data = _storagePoolRepository.GetById(id);
            return _mapper.Map<StoragePoolDTO>(data);
        }
        /// <summary>
        /// 添加/新增存储链接 + bool SaveStoragePool(StoragePoolDTO model)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool SaveStoragePool(StoragePoolDTO model)
        {
            //var aKPH = EncryptPasswordUtil.EncryptPassword(model.AccessKeyPasswordHash);
            //model.AccessKeyPasswordHash = string.Empty;
            //model.AccessKeyPasswordHash = aKPH;

            var data = _mapper.Map<StoragePool>(model);
            if (data.StoragePoolId == 0)
            {
                return _storagePoolRepository.Add(data);
            }
            else
            {
                return _storagePoolRepository.Modify(data, true);
            }
        }
        /// <summary>
        /// 删除存储链接 + bool RemoveStoragePool(int id)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool RemoveStoragePool(int id)
        {
            return _storagePoolRepository.SoftRemoveById(id);
        }
        public string TestStorageConnection(StoragePoolDTO model, string filePath, string privateKey = null)
        {
            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                var storageType = model.CloudType;
                var connectionString = model.GetStorageConnectionString(privateKey);

                var fsLen = (int)stream.Length;
                var heByte = new byte[fsLen];
                stream.Read(heByte, 0, fsLen);

                return BlobUtil.TestStorageConnection(storageType, connectionString, heByte);
            }
        }
    }
}
