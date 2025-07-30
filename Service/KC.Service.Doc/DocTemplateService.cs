using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using KC.Service.EFService;
using KC.Service.WebApiService.Business;
using KC.DataAccess.Doc.Repository;
using KC.Service.DTO.Doc;
using KC.Service.DTO;
using KC.Framework.Extension;
using KC.Model.Doc;
using KC.Framework.Tenant;
using KC.Framework.Base;
using KC.Database.IRepository;
using KC.Enums.Doc;

namespace KC.Service.Doc
{
    public interface IDocTemplateService : IEFService
    {
        PaginatedBaseDTO<DocTemplateDTO> LoadPaginatedDocTemplatesByCondition(int pageIndex, int pageSize, WorkflowBusStatus? type, string name, List<string> orgIds, List<string> rIds, string userId, string userName);

        DocTemplateDTO GetDocTemplateById(int id);

        string RemoveDocTemplatesByIds(string ids);

        bool SaveDocTemplate(DocTemplateDTO model);

        PaginatedBaseDTO<DocTemplateLogDTO> FindPaginatedDocTemplateLogs(int pageIndex, int pageSize, string name);
    }

    public class DocTemplateService : EFServiceBase, IDocTemplateService
    {
        private readonly IMapper _mapper;

        private readonly IDocTemplateRepository _docTemplateRepository;
        private readonly IDbRepository<DocTemplateLog> _docTemplateLogRepository;

        public DocTemplateService(
            Tenant tenant,
            IMapper mapper,
            IDocTemplateRepository docTemplateRepository,
            IDbRepository<DocTemplateLog> documentLogRepository,
            System.Net.Http.IHttpClientFactory clientFactory,
            Microsoft.Extensions.Logging.ILogger<DocTemplateService> logger)
            : base(tenant, clientFactory, logger)
        {
            _mapper = mapper;
            _docTemplateRepository = docTemplateRepository;
            _docTemplateLogRepository = documentLogRepository;
        }

        public PaginatedBaseDTO<DocTemplateDTO> LoadPaginatedDocTemplatesByCondition(int pageIndex, int pageSize, WorkflowBusStatus? status, string name, List<string> orgCodes, List<string> rIds, string userId, string userName)
        {
            Expression<Func<DocTemplate, bool>> predicate = m => !m.IsDeleted;
            if (status.HasValue)
            {
                predicate = predicate.And(x => x.Status == status);
            }

            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate = predicate.And(m => m.Name.Trim().Contains(name.Trim()));
            }
            //if (!string.IsNullOrWhiteSpace(userName))
            //{
            //    predicate = predicate.And(m => (m.CreatedBy == userName));
            //}

            Expression<Func<DocTemplate, bool>> dataPredicate = Util.DataPermitUtil.GetDatePermitPredicate<DocTemplate>(userId, rIds, orgCodes);

            predicate = predicate.And(dataPredicate);

            var result = _docTemplateRepository.FindPagenatedByDataPermission(pageIndex, pageSize, predicate, "ModifiedDate", false);
            var total = result.Item1;
            var rows = _mapper.Map<List<DocTemplateDTO>>(result.Item2);
            return new PaginatedBaseDTO<DocTemplateDTO>(pageIndex, pageSize, total, rows);
        }

        public DocTemplateDTO GetDocTemplateById(int id)
        {
            var result = _docTemplateRepository.FindDocTemplateById(id);
            return _mapper.Map<DocTemplateDTO>(result);
        }

        
        public string RemoveDocTemplatesByIds(string ids)
        {
            if (string.IsNullOrWhiteSpace(ids))
                return "传入的文档模板Ids为空";

            List<int> idlist = ids.ArrayFromCommaDelimitedIntegers().ToList();
            var infolist = _docTemplateRepository.FindDocTemplatesByIds(idlist);
            if (!infolist.Any())
                return string.Format("未找到文档模板Ids为：{0} 的数据", ids);

            var logs = new List<DocTemplateLog>();
            infolist.ForEach(k =>
            {
                k.IsDeleted = true;
                logs.Add(new DocTemplateLog()
                {
                    DocTemplateId = k.Id,
                    DocOperType = DocOperType.Delete,
                    OperatorId = k.ModifiedBy,
                    Operator = k.ModifiedName,
                    OperateDate = k.ModifiedDate,
                    Remark = string.Format("将文档模板【{0}】进行删除。", k.Name),
                });
            });
            var data = _docTemplateRepository.Modify(infolist, new[] { "IsDeleted" }) > 0;
            if (data)
            {
                _docTemplateLogRepository.Add(logs);
                return "删除成功";
            }
                
            return "删除失败";
        }

        public bool SaveDocTemplate(DocTemplateDTO model)
        {
            var entity = _mapper.Map<DocTemplate>(model);
            var success = model.Id == 0 
                ? _docTemplateRepository.Add(entity) 
                : _docTemplateRepository.Modify(entity);
            if (success)
            {
                var msg = model.Id == 0
                    ? "创建了新的文档模板"
                    : "修改了文档模板的内容";
                _docTemplateLogRepository.Add(new DocTemplateLog()
                {
                    DocTemplateId = entity.Id,
                    DocOperType = model.Id == 0 ? DocOperType.Create : DocOperType.Update,
                    OperatorId = model.ModifiedBy,
                    Operator = model.ModifiedName,
                    OperateDate = model.ModifiedDate,
                    Remark = msg,
                });
            }

            return success;
        }

        #region 日志
        public PaginatedBaseDTO<DocTemplateLogDTO> FindPaginatedDocTemplateLogs(int pageIndex, int pageSize, string name)
        {
            Expression<Func<DocTemplateLog, bool>> predicate = m => true;
            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate = predicate.And(m => m.Operator.Contains(name));
            }

            var data = _docTemplateLogRepository.FindPagenatedListWithCount(
                pageIndex, pageSize, predicate, m => m.OperateDate, true);

            var total = data.Item1;
            var rows = _mapper.Map<List<DocTemplateLogDTO>>(data.Item2);
            return new PaginatedBaseDTO<DocTemplateLogDTO>(pageIndex, pageSize, total, rows);

        }
        #endregion
    }
}
