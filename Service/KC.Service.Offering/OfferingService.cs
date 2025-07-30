using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using KC.Database.EFRepository;
using KC.Service.EFService;
using KC.DataAccess.Offering.Repository;
using KC.Model.Offering;
using KC.Database.IRepository;
using KC.Service.WebApiService.Business;
using KC.Framework.Tenant;
using AutoMapper;

namespace KC.Service.Offering
{
    public interface IOfferingService : IEFService
    {

    }

    public class OfferingService : EFServiceBase, IOfferingService
    {
        private readonly IMapper _mapper;

        /// <summary>
        /// ComAccountUnitOfWorkContext
        ///     use AccountUnitOfWorkContext.Commit() to save the context change(transaction).
        /// </summary>
        protected EFUnitOfWorkContextBase _unitOfWorkContext { get; private set; }

        private ICategoryRepository _categoryRepository;
        private IOfferingRepository _offeringRepository;

        private IDbRepository<OfferingProperty> _offeringPropertyRepository;

        public OfferingService(
            Tenant tenant,
            IMapper mapper,
            EFUnitOfWorkContextBase unitOfWorkContext,

            ICategoryRepository offeringtCategoryRepository,
            IOfferingRepository offeringRepository,
            IDbRepository<OfferingProperty> offeringPropertyRepository,

            System.Net.Http.IHttpClientFactory clientFactory,
            Microsoft.Extensions.Logging.ILogger<OfferingService> logger)
            : base(tenant, clientFactory, logger)
        {
            _mapper = mapper;
            _unitOfWorkContext = unitOfWorkContext;
            
            _offeringPropertyRepository = offeringPropertyRepository;

            _offeringRepository = offeringRepository;

            _categoryRepository = offeringtCategoryRepository;
        }

    }
}
