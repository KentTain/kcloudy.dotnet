using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using KC.Service.EFService;
using KC.Framework.Base;
using KC.DataAccess.Config.Repository;
using KC.Framework.Tenant;
using KC.Service.DTO;
using KC.Framework.Extension;
using KC.Database.IRepository;
using KC.Model.Config;
using KC.Service.DTO.Config;
using AutoMapper;
using KC.Service.WebApiService.Business;

namespace KC.Service.Config
{
    public interface ISeedService : IEFService
    {
        PaginatedBaseDTO<SysSequenceDTO> FindPaginatedSysSequencesByName(int pageIndex, int pageSize, string name);

        SysSequenceDTO GetSysSequenceByName(string name);

        bool SaveSysSequence(SysSequenceDTO model);
        bool RemoveSysSequenceById(string name);

        SeedEntity GetSeedEntityByName(string name, int step = 1);

        string GetSeedCodeByName(string name);
    }

    public class SeedService : EFServiceBase, ISeedService
    {
        private readonly IMapper _mapper;

        private ISysSequenceRepository _seedEntityRepository;

        public SeedService(Tenant tenant,
            IMapper mapper,
            ISysSequenceRepository seedEntityRepository,
            System.Net.Http.IHttpClientFactory clientFactory,
            Microsoft.Extensions.Logging.ILogger<SeedService> logger)
            : base(tenant, clientFactory, logger)
        {
            _mapper = mapper;
            _seedEntityRepository = seedEntityRepository;
        }

        public PaginatedBaseDTO<SysSequenceDTO> FindPaginatedSysSequencesByName(int pageIndex, int pageSize, string name)
        {
            Expression<Func<SysSequence, bool>> predicate = m => true;
            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate = predicate.And(m => m.SequenceName.Contains(name));
            }

            var data = _seedEntityRepository.FindPagenatedListWithCount(pageIndex, pageSize, predicate, m => m.SequenceName, true);

            var total = data.Item1;
            var rows = _mapper.Map<List<SysSequenceDTO>>(data.Item2);
            return new PaginatedBaseDTO<SysSequenceDTO>(pageIndex, pageSize, total, rows);
        }

        public SysSequenceDTO GetSysSequenceByName(string name)
        {
            var data = _seedEntityRepository.GetById(name);
            return _mapper.Map<SysSequenceDTO>(data);
        }

        public bool SaveSysSequence(SysSequenceDTO model)
        {
            var data = _mapper.Map<SysSequence>(model);
            var sequence = _seedEntityRepository.GetById(model.SequenceName);
            if (sequence == null)
            {
                return _seedEntityRepository.Add(data);
            }
            else
            {
                return _seedEntityRepository.Modify(data, true);
            }
        }
        public bool RemoveSysSequenceById(string name)
        {
            var data = _seedEntityRepository.GetById(name);
            return _seedEntityRepository.Modify(data, true);
        }

        public SeedEntity GetSeedEntityByName(string seedType, int step = 1)
        {
            return _seedEntityRepository.GetSeedByType(seedType, step);
        }

        public string GetSeedCodeByName(string seedType)
        {
            var entity = _seedEntityRepository.GetSeedByType(seedType);
            return entity.SeedValue;
        }
    }
}
