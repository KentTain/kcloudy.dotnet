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
using KC.Service.DTO.Offering;
using KC.DataAccess.Offering.Repository;
using KC.Model.Offering;
using KC.DataAccess.Offering;
using KC.Service.DTO.Admin;
using KC.Database.IRepository;
using KC.Service.WebApiService.Business;
using KC.Framework.Tenant;

namespace KC.Service.Offering
{
    public interface IPropertyProviderService : IEFService
    {
    }

    public class PropertyProviderService : EFServiceBase, IPropertyProviderService
    {
        private readonly IMapper _mapper;

        private IOfferingRepository _mobileLocationRepository;

        public PropertyProviderService(
            Tenant tenant,
            IMapper mapper,
            IOfferingRepository mobileLocationRepository,
            System.Net.Http.IHttpClientFactory clientFactory,
            Microsoft.Extensions.Logging.ILogger<PropertyProviderService> logger)
            : base(tenant,  clientFactory, logger)
        {
            _mapper = mapper;
            _mobileLocationRepository = mobileLocationRepository;
        }

    }
}
