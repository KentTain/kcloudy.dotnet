using AutoMapper;
using KC.Database.EFRepository;
using KC.Database.IRepository;
using KC.Service.DTO.Contract;
using KC.Framework.Tenant;
using KC.Model.Contract;
using KC.Service.Contract.WebApiService.Business;
using KC.Service.Contract.WebApiService.ThridParty;
using KC.Service.EFService;
using KC.Service.WebApiService.Business;
using System.Threading.Tasks;


namespace KC.Service.Contract
{
    public interface IElectronicSignService : IEFService
    {
        Task<ElectronicOrganizationDTO> GetElectronicOrganizationAsync();
        Task<bool> RemoveElectronicOrganization();

        Task<ElectronicPersonDTO> GetElectronicPersonAsync(string userId);
        Task<bool> RemoveElectronicPerson(string userId);
    }

    public class ElectronicSignService : EFServiceBase, IElectronicSignService
    {
        private readonly IMapper _mapper;
        /// <summary>
        /// ComAccountUnitOfWorkContext
        ///     use AccountUnitOfWorkContext.Commit() to save the context change(transaction).
        /// </summary>
        protected EFUnitOfWorkContextBase _unitOfWorkContext { get; private set; }

        private readonly IElectronicSignApiService CurrencySignApiService;
        private readonly IContractApiService CurrencyApiService;

        private readonly IDbRepository<ElectronicPerson> ElectronicPersonRepository;
        private readonly IDbRepository<ElectronicOrganization> ElectronicOrganizationRepository;

        public ElectronicSignService(
            Tenant tenant,
            IMapper mapper,
            EFUnitOfWorkContextBase unitOfWorkContext,

            IElectronicSignApiService currencySignApiService,
            IContractApiService currencyApiService,

            IDbRepository<ElectronicPerson> electronicPersonRepository,
            IDbRepository<ElectronicOrganization> electronicOrganizationRepository,

            System.Net.Http.IHttpClientFactory clientFactory,
            Microsoft.Extensions.Logging.ILogger<ContractService> logger)
            : base(tenant, clientFactory, logger)
        {
            _mapper = mapper;
            _unitOfWorkContext = unitOfWorkContext;

            CurrencySignApiService = currencySignApiService;
            CurrencyApiService = currencyApiService;

            ElectronicPersonRepository = electronicPersonRepository;
            ElectronicOrganizationRepository = electronicOrganizationRepository;
        }

        

        public async Task<ElectronicOrganizationDTO> GetElectronicOrganizationAsync()
        {
            var data = await ElectronicOrganizationRepository.GetByFilterAsync(m => true);
            return _mapper.Map<ElectronicOrganizationDTO>(data);
        }

        public async Task<bool> RemoveElectronicOrganization()
        {
            return await ElectronicOrganizationRepository.RemoveAllAsync();
        }


        public async Task<ElectronicPersonDTO> GetElectronicPersonAsync(string userId)
        {
            var data = await ElectronicPersonRepository.GetByIdAsync(userId);
            return _mapper.Map<ElectronicPersonDTO>(data);
        }

        public async Task<bool> RemoveElectronicPerson(string userId)
        {
            return await ElectronicPersonRepository.RemoveByIdAsync(userId);
        }
    }
}
