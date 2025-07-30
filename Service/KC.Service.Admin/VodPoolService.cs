using AutoMapper;
using KC.Database.IRepository;
using KC.Framework.Extension;
using KC.Model.Admin;
using KC.Service.EFService;
using KC.Service.WebApiService.Business;
using KC.Service.DTO.Admin;
using KC.Service.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace KC.Service.Admin
{
    public interface IVodPoolService
    {
        List<VodPoolDTO> FindAllVodPools();
        VodPoolDTO GetVodPoolById(int id);
        PaginatedBaseDTO<VodPoolDTO> FindVodPoolsByFilter(int pageIndex, int pageSize, string accessName, string order);
        bool RemoveVodPool(int id);
        bool SaveVodPool(VodPoolDTO model);
        string TestVodConnection(VodPoolDTO model, string privateKey = null);
    }

    public class VodPoolService : EFServiceBase, IVodPoolService
    {
        private readonly IMapper _mapper;

        private readonly IDbRepository<VodPool> _VodPoolRepository;

        public VodPoolService(
            IMapper mapper,
            IDbRepository<VodPool> VodPoolRepository,
            System.Net.Http.IHttpClientFactory clientFactory,
            Microsoft.Extensions.Logging.ILogger<VodPoolService> logger)
            : base(clientFactory, logger)
        {
            _mapper = mapper;
            _VodPoolRepository = VodPoolRepository;
        }

        /// <summary>
        /// 获取队列链接列表 + List<QueuePoolDTO> GetAllQueuePools()
        /// </summary>
        /// <returns></returns>
        public List<VodPoolDTO> FindAllVodPools()
        {
            var data = _VodPoolRepository.FindAll().ToList();
            return _mapper.Map<List<VodPoolDTO>>(data);
        }

        public PaginatedBaseDTO<VodPoolDTO> FindVodPoolsByFilter(int pageIndex, int pageSize, string accessName, string order)
        {
            Expression<Func<VodPool, bool>> predicate = m => true;
            if (!string.IsNullOrWhiteSpace(accessName))
            {
                predicate = predicate.And(m => m.AccessName.Contains(accessName));
            }

            var data = _VodPoolRepository.FindPagenatedListWithCount(pageIndex,
                pageSize,
                predicate,
                m => m.PasswordExpiredTime, order.Equals("asc"));

            var total = data.Item1;
            var rows = _mapper.Map<List<VodPoolDTO>>(data.Item2);
            return new PaginatedBaseDTO<VodPoolDTO>(pageIndex, pageSize, total, rows);
        }

        /// <summary>
        ///  Id查找存储链接对象 + VodPoolDTO GetVodPoolById(int id)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public VodPoolDTO GetVodPoolById(int id)
        {
            var data = _VodPoolRepository.GetById(id);
            return _mapper.Map<VodPoolDTO>(data);
        }

        /// <summary>
        /// 添加/新增队列链接 + bool SaveVodPool(VodPoolDTO model)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool SaveVodPool(VodPoolDTO model)
        {
            //var aKPH = EncryptPasswordUtil.EncryptPassword(model.AccessKeyPasswordHash);
            //model.AccessKeyPasswordHash = string.Empty;
            //model.AccessKeyPasswordHash = aKPH;
            var data = _mapper.Map<VodPool>(model);
            if (data.VodPoolId == 0)
            {
                return _VodPoolRepository.Add(data);
            }
            else
            {
                return _VodPoolRepository.Modify(data, true);
            }
        }
        /// <summary>
        /// 删除队列链接 + bool RemoveVodPool(int id)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool RemoveVodPool(int id)
        {
            return _VodPoolRepository.SoftRemoveById(id);
        }
        public string TestVodConnection(VodPoolDTO model, string privateKey = null)
        {
            //TODO: 实现不同Vod下的连通性测试
            return string.Empty;
        }
    }
}
