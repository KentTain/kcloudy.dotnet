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
    public interface ICodePoolService
    {
        List<CodeRepositoryPoolDTO> FindAllCodeRepositoryPools();
        CodeRepositoryPoolDTO GetCodeRepositoryPoolById(int id);
        PaginatedBaseDTO<CodeRepositoryPoolDTO> FindCodeRepositoryPoolsByFilter(int pageIndex, int pageSize, string accessName, string order);
        bool RemoveCodeRepositoryPool(int id);
        bool SaveCodeRepositoryPool(CodeRepositoryPoolDTO model);
        string TestCodeRepositoryPoolConnection(CodeRepositoryPoolDTO model, string privateKey = null);
    }

    public class CodePoolService : EFServiceBase, ICodePoolService
    {
        private readonly IMapper _mapper;

        private readonly IDbRepository<CodeRepositoryPool> _CodeRepositoryPoolRepository;

        public CodePoolService(
            IMapper mapper,
            IDbRepository<CodeRepositoryPool> CodeRepositoryPoolRepository,
            System.Net.Http.IHttpClientFactory clientFactory,
            Microsoft.Extensions.Logging.ILogger<CodePoolService> logger)
            : base(clientFactory, logger)
        {
            _mapper = mapper;
            _CodeRepositoryPoolRepository = CodeRepositoryPoolRepository;
        }

        /// <summary>
        /// 获取队列链接列表 + List<QueuePoolDTO> GetAllQueuePools()
        /// </summary>
        /// <returns></returns>
        public List<CodeRepositoryPoolDTO> FindAllCodeRepositoryPools()
        {
            var data = _CodeRepositoryPoolRepository.FindAll().ToList();
            return _mapper.Map<List<CodeRepositoryPoolDTO>>(data);
        }

        public PaginatedBaseDTO<CodeRepositoryPoolDTO> FindCodeRepositoryPoolsByFilter(int pageIndex, int pageSize, string accessName, string order)
        {
            Expression<Func<CodeRepositoryPool, bool>> predicate = m => true;
            if (!string.IsNullOrWhiteSpace(accessName))
            {
                predicate = predicate.And(m => m.AccessName.Contains(accessName));
            }

            var data = _CodeRepositoryPoolRepository.FindPagenatedListWithCount(pageIndex,
                pageSize,
                predicate,
                m => m.PasswordExpiredTime, order.Equals("asc"));

            var total = data.Item1;
            var rows = _mapper.Map<List<CodeRepositoryPoolDTO>>(data.Item2);
            return new PaginatedBaseDTO<CodeRepositoryPoolDTO>(pageIndex, pageSize, total, rows);
        }

        /// <summary>
        ///  Id查找存储链接对象 + CodeRepositoryPoolDTO GetCodeRepositoryPoolById(int id)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public CodeRepositoryPoolDTO GetCodeRepositoryPoolById(int id)
        {
            var data = _CodeRepositoryPoolRepository.GetById(id);
            return _mapper.Map<CodeRepositoryPoolDTO>(data);
        }

        /// <summary>
        /// 添加/新增队列链接 + bool SaveCodeRepositoryPool(CodeRepositoryPoolDTO model)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool SaveCodeRepositoryPool(CodeRepositoryPoolDTO model)
        {
            //var aKPH = EncryptPasswordUtil.EncryptPassword(model.AccessKeyPasswordHash);
            //model.AccessKeyPasswordHash = string.Empty;
            //model.AccessKeyPasswordHash = aKPH;
            var data = _mapper.Map<CodeRepositoryPool>(model);
            if (data.CodePoolId == 0)
            {
                return _CodeRepositoryPoolRepository.Add(data);
            }
            else
            {
                return _CodeRepositoryPoolRepository.Modify(data, true);
            }
        }
        /// <summary>
        /// 删除队列链接 + bool RemoveCodeRepositoryPool(int id)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool RemoveCodeRepositoryPool(int id)
        {
            return _CodeRepositoryPoolRepository.SoftRemoveById(id);
        }
        public string TestCodeRepositoryPoolConnection(CodeRepositoryPoolDTO model, string privateKey = null)
        {
            //TODO: 实现不同CodeRepositoryPool下的连通性测试
            return string.Empty;
        }
    }
}
