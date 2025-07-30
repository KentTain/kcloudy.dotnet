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
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace KC.Service.Admin
{
    public class QueuePoolService : EFServiceBase, IQueuePoolService
    {
        private readonly IMapper _mapper;

        private readonly IDbRepository<QueuePool> _queuePoolRepository;

        public QueuePoolService(
            IMapper mapper,
            IDbRepository<QueuePool> QueuePoolRepository,
            System.Net.Http.IHttpClientFactory clientFactory,
            Microsoft.Extensions.Logging.ILogger<QueuePoolService> logger)
            : base(clientFactory, logger)
        {
            _mapper = mapper;
            _queuePoolRepository = QueuePoolRepository;
        }

        /// <summary>
        /// 获取队列链接列表 + List<QueuePoolDTO> GetAllQueuePools()
        /// </summary>
        /// <returns></returns>
        public List<QueuePoolDTO> FindAllQueuePools()
        {
            var data = _queuePoolRepository.FindAll().ToList();
            return _mapper.Map<List<QueuePoolDTO>>(data);
        }

        public PaginatedBaseDTO<QueuePoolDTO> FindQueuePoolsByFilter(int pageIndex, int pageSize, string accessName, string order)
        {
            Expression<Func<QueuePool, bool>> predicate = m => true;
            if (!string.IsNullOrWhiteSpace(accessName))
            {
                predicate = predicate.And(m => m.AccessName.Contains(accessName));
            }

            var data = _queuePoolRepository.FindPagenatedListWithCount(pageIndex,
                pageSize,
                predicate,
                m => m.PasswordExpiredTime, order.Equals("asc"));


            var total = data.Item1;
            var rows = _mapper.Map<List<QueuePoolDTO>>(data.Item2);
            return new PaginatedBaseDTO<QueuePoolDTO>(pageIndex, pageSize, total, rows);
        }

        /// <summary>
        ///  Id查找存储链接对象 + QueuePoolDTO GetQueuePoolById(int id)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public QueuePoolDTO GetQueuePoolById(int id)
        {
            var data = _queuePoolRepository.GetById(id);
            return _mapper.Map<QueuePoolDTO>(data);
        }

        /// <summary>
        /// 添加/新增队列链接 + bool SaveQueuePool(QueuePoolDTO model)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool SaveQueuePool(QueuePoolDTO model)
        {
            //var aKPH = EncryptPasswordUtil.EncryptPassword(model.AccessKeyPasswordHash);
            //model.AccessKeyPasswordHash = string.Empty;
            //model.AccessKeyPasswordHash = aKPH;

            var data = _mapper.Map<QueuePool>(model);
            if (data.QueuePoolId == 0)
            {
                return _queuePoolRepository.Add(data);
            }
            else
            {
                return _queuePoolRepository.Modify(data, true);
            }
        }
        /// <summary>
        /// 删除队列链接 + bool RemoveQueuePool(int id)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool RemoveQueuePool(int id)
        {
            return _queuePoolRepository.SoftRemoveById(id);
        }
        public string TestQueueConnection(QueuePoolDTO model, string privateKey = null)
        {
            var connectionString = model.GetQueueConnectionString(privateKey);
            return new StorageQueueService().TestQueueConnection(model.QueueType, connectionString);
        }
    }
}
