using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using KC.Framework.Extension;
using KC.Framework.Tenant;
using KC.DataAccess.Account.Repository;
using KC.Model.Account;
using KC.Service.EFService;
using KC.Service.DTO.Account;
using KC.Database.IRepository;
using Microsoft.Extensions.Logging;
using KC.Database.EFRepository;

namespace KC.Service.Account
{
    public sealed class SettingService : EFServiceBase, ISettingService
    {
        private readonly IMapper _mapper;

        #region Db Repository
        private EFUnitOfWorkContextBase _unitOfContext;
        
        private IDbRepository<SystemSetting> _systemSettingRespository;
        private IDbRepository<SystemSettingProperty> _systemSettingPropertyRepository;

        private IUserSettingRepository _userSettingRespository;
        private IDbRepository<UserSettingProperty> _userSettingPropertyRepository;

        #endregion

        public SettingService(
            Tenant tenant,
            IDbRepository<SystemSetting> systemSettingRespository,
            IDbRepository<SystemSettingProperty> systemSettingPropertyRepository,
            IUserSettingRepository userSettingRespository,
            IDbRepository<UserSettingProperty> userSettingPropertyRepository,

            IMapper mapper,
            EFUnitOfWorkContextBase unitOfContext,
            System.Net.Http.IHttpClientFactory clientFactory,
            Microsoft.Extensions.Logging.ILogger<RoleService> logger)
            : base(tenant, clientFactory, logger)
        {
            _systemSettingRespository = systemSettingRespository;
            _systemSettingPropertyRepository = systemSettingPropertyRepository;

            _userSettingRespository = userSettingRespository;
            _userSettingPropertyRepository = userSettingPropertyRepository;

            _mapper = mapper;
            _unitOfContext = unitOfContext;
        }

        public UserSettingDTO GetUserSettingDetailByCode(string code)
        {
            var data = _userSettingRespository.GetDetailUserSettingByCode(code);
            return _mapper.Map<UserSettingDTO>(data);
        }

    }

}
