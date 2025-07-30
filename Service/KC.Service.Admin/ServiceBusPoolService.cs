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
using KC.Service.Component;

namespace KC.Service.Admin
{
    public class ServiceBusPoolService : EFServiceBase, IServiceBusPoolService
    {
        private readonly IMapper _mapper;

        private readonly IDbRepository<ServiceBusPool> _serviceBusPoolRespository;

        public ServiceBusPoolService(
            IMapper mapper,
            IDbRepository<ServiceBusPool> ServiceBusPoolRepository,
            System.Net.Http.IHttpClientFactory clientFactory,
            Microsoft.Extensions.Logging.ILogger<ServiceBusPoolService> logger)
            : base(clientFactory, logger)
        {
            _mapper = mapper;
            _serviceBusPoolRespository = ServiceBusPoolRepository;
        }

        #region serviceBusPool
        public ServiceBusPoolDTO GetServiceBusPoolbyId(int id)
        {
            var model = _serviceBusPoolRespository.GetById(id);
            return _mapper.Map<ServiceBusPoolDTO>(model);
        }

        public PaginatedBaseDTO<ServiceBusPoolDTO> FindPaginatedServiceBusPoolList(int pageIndex, int pageSize, string accessName)
        {
            Expression<Func<ServiceBusPool, bool>> pageservicebuspool = m => m.IsDeleted == false;
            if (!string.IsNullOrWhiteSpace(accessName))
            {
                pageservicebuspool = pageservicebuspool.And(m => m.AccessName.Contains(accessName));

            }
            var data = _serviceBusPoolRespository.FindPagenatedListWithCount(pageIndex, pageSize, pageservicebuspool,
         m => m.CreatedDate);
            var total = data.Item1;
            var rows = _mapper.Map<List<ServiceBusPoolDTO>>(data.Item2);
            return new PaginatedBaseDTO<ServiceBusPoolDTO>(pageIndex, pageSize, total, rows);
        }
        public bool DeleteServiceBusPool(int id)
        {
            return _serviceBusPoolRespository.RemoveById(id);
        }
        public bool SaveServiceBusPool(ServiceBusPoolDTO sbpool)
        {

            var data = _mapper.Map<ServiceBusPool>(sbpool);
            if (data.ServiceBusPoolId == 0)
            {
                return _serviceBusPoolRespository.Add(data);
            }
            else
            {
                return _serviceBusPoolRespository.Modify(data, true);
            }
        }

        #endregion

        public string TestServiceBusConnection(ServiceBusPoolDTO model, string privateKey = null)
        {
            var connectionString = model.GetServiceBusConnectionString(privateKey);
            return new TopicService().TestServiceBusConnection(model.ServiceBusType, connectionString);
        }
    }
}
