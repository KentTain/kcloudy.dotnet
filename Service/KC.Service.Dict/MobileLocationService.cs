using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using KC.Framework.Extension;
using KC.Service;
using KC.Service.Util;
using KC.Service.Constants;
using KC.Service.EFService;
using KC.Service.DTO.Dict;
using KC.DataAccess.Dict.Repository;
using KC.Service.WebApiService.Business;
using KC.Framework.Tenant;

namespace KC.Service.Dict
{
    public interface IMobileLocationService : IEFService
    {
        MobileLocationDTO GetMobileLocation(string mobilePhone);
    }

    public class MobileLocationService : EFServiceBase, IMobileLocationService
    {
        private readonly IMapper _mapper;

        private IMobileLocationRepository _mobileLocationRepository;

        public MobileLocationService(
            Tenant tenant,
            IMapper mapper,
            IAccountApiService accountApiService,
            IMobileLocationRepository mobileLocationRepository,
            System.Net.Http.IHttpClientFactory clientFactory,
            Microsoft.Extensions.Logging.ILogger<MobileLocationService> logger)
            : base(tenant, clientFactory, logger)
        {
            _mapper = mapper;
            _mobileLocationRepository = mobileLocationRepository;
        }

        public MobileLocationDTO GetMobileLocation(string mobilePhone)
        {
            var data = _mobileLocationRepository.GetMobileLocation(mobilePhone);
            return _mapper.Map<MobileLocationDTO>(data);
        }
    }
}
