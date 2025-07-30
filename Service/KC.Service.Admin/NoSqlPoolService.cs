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
    public class NoSqlPoolService : EFServiceBase, INoSqlPoolService
    {
        private readonly IMapper _mapper;

        private readonly IDbRepository<NoSqlPool> _noSqlPoolRepository;

        public NoSqlPoolService(
            IMapper mapper,
            IDbRepository<NoSqlPool> NoSqlPoolRepository,
            System.Net.Http.IHttpClientFactory clientFactory,
            Microsoft.Extensions.Logging.ILogger<NoSqlPoolService> logger)
            : base(clientFactory, logger)
        {
            _mapper = mapper;
            _noSqlPoolRepository = NoSqlPoolRepository;
        }

        /// <summary>
        /// 获取队列链接列表 + List<QueuePoolDTO> GetAllQueuePools()
        /// </summary>
        /// <returns></returns>
        public List<NoSqlPoolDTO> FindAllNoSqlPools()
        {
            var data = _noSqlPoolRepository.FindAll().ToList();
            return _mapper.Map<List<NoSqlPoolDTO>>(data);
        }

        public PaginatedBaseDTO<NoSqlPoolDTO> FindNoSqlPoolsByFilter(int pageIndex, int pageSize, string accessName, string order)
        {
            Expression<Func<NoSqlPool, bool>> predicate = m => true;
            if (!string.IsNullOrWhiteSpace(accessName))
            {
                predicate = predicate.And(m => m.AccessName.Contains(accessName));
            }

            var data = _noSqlPoolRepository.FindPagenatedListWithCount(pageIndex,
                pageSize,
                predicate,
                m => m.PasswordExpiredTime, order.Equals("asc"));

            var total = data.Item1;
            var rows = _mapper.Map<List<NoSqlPoolDTO>>(data.Item2);
            return new PaginatedBaseDTO<NoSqlPoolDTO>(pageIndex, pageSize, total, rows);
        }

        /// <summary>
        ///  Id查找存储链接对象 + NoSqlPoolDTO GetNoSqlPoolById(int id)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public NoSqlPoolDTO GetNoSqlPoolById(int id)
        {
            var data = _noSqlPoolRepository.GetById(id);
            return _mapper.Map<NoSqlPoolDTO>(data);
        }

        /// <summary>
        /// 添加/新增队列链接 + bool SaveNoSqlPool(NoSqlPoolDTO model)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool SaveNoSqlPool(NoSqlPoolDTO model)
        {
            //var aKPH = EncryptPasswordUtil.EncryptPassword(model.AccessKeyPasswordHash);
            //model.AccessKeyPasswordHash = string.Empty;
            //model.AccessKeyPasswordHash = aKPH;
            var data = _mapper.Map<NoSqlPool>(model);
            if (data.NoSqlPoolId == 0)
            {
                return _noSqlPoolRepository.Add(data);
            }
            else
            {
                return _noSqlPoolRepository.Modify(data, true);
            }
        }
        /// <summary>
        /// 删除队列链接 + bool RemoveNoSqlPool(int id)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool RemoveNoSqlPool(int id)
        {
            return _noSqlPoolRepository.SoftRemoveById(id);
        }
        public string TestNoSqlConnection(NoSqlPoolDTO model, string privateKey = null)
        {
            //TODO: 实现不同NoSql下的连通性测试
            return string.Empty;
        }
    }
}
