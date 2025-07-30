using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using KC.Database.EFRepository;
using KC.Service.EFService;
using KC.Model.Doc;
using KC.DataAccess.Doc.Repository;
using KC.Enums.Doc;
using KC.Service.DTO;
using KC.Framework.Extension;
using KC.Service.DTO.Doc;
using KC.Framework.Tenant;
using KC.Service.WebApiService.Business;
using KC.Database.IRepository;

namespace KC.Service.Doc
{
    public interface IDocumentInfoService : IEFService
    {
        #region 文件分类
        List<DocCategoryDTO> LoadDocCategoryTree(string userId, string name);
        List<DocCategoryDTO> LoadFatherCategories(int? id, LableType? type);
        
        DocCategoryDTO GetDocCategoryById(int id);

        Task<bool> SaveDocCategoryAsync(DocCategoryDTO model);

        Task<bool> RemoveDocCategoryByIdAsync(int id);
        string SoftRemoveDocCategoryById(int id);
        Task<bool> ExistCategoryNameAsync(int pid, string name);
        #endregion

        #region 文件

        /// <summary>
        /// 根据条件筛选文件
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="categoryId">文件分类Id</param>
        /// <param name="name">文件名称</param>
        /// <param name="userId">权限设置：当前用户Id</param>
        /// <param name="orgIds">权限设置：部门集合</param>
        /// <param name="rIds">权限设置：角色集合</param>
        /// <returns></returns>

        PaginatedBaseDTO<DocumentInfoDTO> LoadPaginatedDocumentsByFilter(int pageIndex, int pageSize, int? categoryId, string name, string userId, List<string> orgIds, List<string> rIds);

        DocumentInfoDTO GetDocumentById(int id);

        string RemoveDocumentsByIds(string ids);
        string MoveDocumentsToCategory(string oldIds, string categoryId);

        bool SaveDocument(DocumentInfoDTO model);

        /// <summary>
        /// 查找文件分类返回Id 若不存在则插入新数据再返回
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type">0 我的文件 1 客户文件 2 报告</param>
        /// <returns></returns>
        ReturnMessageDTO GetCompanyIdByName(string name, LableType type);

        #endregion

        PaginatedBaseDTO<DocumentLogDTO> FindPaginatedDocumentLogs(int pageIndex, int pageSize, string name);
    }

    public class DocumentInfoService : EFServiceBase, IDocumentInfoService
    {
        private readonly IMapper _mapper;

        /// <summary>
        /// ComAccountUnitOfWorkContext
        ///     use AccountUnitOfWorkContext.Commit() to save the context change(transaction).
        /// </summary>
        protected EFUnitOfWorkContextBase _unitOfWorkContext { get; private set; }

        private readonly IConfigApiService ConfigApiService;
        private readonly IDocumentInfoRepository _documentInfoRepository;
        private readonly IDocCategoryRepository _docCategoryRepository;
        private readonly IDbRepository<DocumentLog> _documentLogRepository;

        public DocumentInfoService(
            Tenant tenant,
            IMapper mapper,
            EFUnitOfWorkContextBase unitOfWorkContext,
            IConfigApiService configApiService,

            IDocumentInfoRepository docInfoRepository,
            IDocCategoryRepository docCategoryRepository,
            IDbRepository<DocumentLog> documentLogRepository,
            System.Net.Http.IHttpClientFactory clientFactory,
            Microsoft.Extensions.Logging.ILogger<DocumentInfoService> logger)
            : base(tenant, clientFactory, logger)
        {
            _mapper = mapper;
            _unitOfWorkContext = unitOfWorkContext;
            ConfigApiService = configApiService;

            _documentInfoRepository = docInfoRepository;
            _docCategoryRepository = docCategoryRepository;
            _documentLogRepository = documentLogRepository;
        }

        #region 文件分类
        public List<DocCategoryDTO> LoadDocCategoryTree(string userId, string name)
        {
            Expression<Func<DocCategory, bool>> predicate = m => !m.IsDeleted;
            if(!userId.IsNullOrEmpty())
            {
                predicate = predicate.And(m => m.CreatedBy.Equals(userId));
            }
            if (!name.IsNullOrEmpty())
            {
                predicate = predicate.And(m => m.Name.Contains(name));
            }

            var data = _docCategoryRepository.FindTreeNodesWithNestParentAndChildByFilter(predicate);
            return _mapper.Map<List<DocCategoryDTO>>(data);
        }

        public List<DocCategoryDTO> LoadFatherCategories(int? id, LableType? type)
        {
            var result = _docCategoryRepository.FindFatherCategory(id, type);
            return _mapper.Map<List<DocCategoryDTO>>(result);
        }

        public DocCategoryDTO GetDocCategoryById(int id)
        {
            //var data = _docCategoryRepository.GetDocCategoryWithChildrenById(id);
            var data = _docCategoryRepository.GetTreeNodeWithNestChildById(id);
            return _mapper.Map<DocCategoryDTO>(data);
        }

        
        public async Task<bool> SaveDocCategoryAsync(DocCategoryDTO model)
        {
            if (model == null)
                throw new ArgumentException(string.Format("传入对象为空"));

            var exist = await _docCategoryRepository.ExistByTreeNameAsync(model.Id, model.ParentId, model.Text);
            if (exist)
                throw new ArgumentException("已存在名为【" + model.Text + "】的分类，请勿重复添加！");
            if (model.ParentId <= 0)
                model.ParentId = null;

            var data = _mapper.Map<DocCategory>(model);
            var success = await _docCategoryRepository.SaveDocCategoryAsync(data);
            if (success)
                model.Id = data.Id;
            return success;
        }

        public async Task<bool> RemoveDocCategoryByIdAsync(int id)
        {
            if (id <= 0)
                return true;

            return await _docCategoryRepository.RemoveByIdAsync(id);
        }

        public string SoftRemoveDocCategoryById(int id)
        {
            if (id <= 0)
                return "未找到Id【" + id + "】的文件分类记录！";

            var result = _docCategoryRepository.GetDocCategoryWithChildrenById(id);
            if (result == null)
                return "未找到Id【" + id + "】的文件分类记录！";

            if (result.ChildNodes.Any(x => x.IsDeleted == false))
                return "文件分类【" + result.Name + "】下有子级文件分类，请先将其子级文件分类移到到其他文件分类下再进行删除！";

            if (result.DocumentInfos.Any())
            {
                return "文件分类【" + result.Name + "】下有文件，请先将文件移动到其他文件分类下再进行删除！";
            }

            result.IsDeleted = true;
            var res = _docCategoryRepository.Modify(result, new[] { "IsDeleted" });
            if (!res)
                return "删除文件分类失败，请重试！";

            return string.Empty;
        }

        public async Task<bool> ExistCategoryNameAsync(int pid, string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name", "分类名称为空.");

            Expression<Func<DocCategory, bool>> predicate = m => !m.IsDeleted && m.Name.Equals(name);
            if (pid == 0)
                predicate = predicate.And(c => c.ParentId == null);
            else
                predicate = predicate.And(c => c.ParentId == pid);

            return await _docCategoryRepository.ExistByFilterAsync(predicate);
        }
        #endregion

        #region 文件

        /// <summary>
        /// 根据条件筛选文件
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="categoryId">文件分类Id</param>
        /// <param name="name">文件名称</param>
        /// <param name="userId">权限设置：当前用户Id</param>
        /// <param name="orgIds">权限设置：部门集合</param>
        /// <param name="rIds">权限设置：角色集合</param>
        /// <returns></returns>
        public PaginatedBaseDTO<DocumentInfoDTO> LoadPaginatedDocumentsByFilter(int pageIndex, int pageSize, int? categoryId, string name, string userId, List<string> orgIds, List<string> rIds)
        {
            Expression<Func<DocumentInfo, bool>> predicate = m => !m.IsDeleted;
            //未分类
            if (categoryId.HasValue && categoryId.Value < 0)
            {
                predicate = predicate.And(m => m.DocCategoryId == null);
            }
            //已分类
            else if (categoryId.HasValue && categoryId.Value > 0)
            {
                predicate = predicate.And(m => m.DocCategoryId.Equals(categoryId.Value));
            }

            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate = predicate.And(m => m.Name.Contains(name.Trim()));
            }

            if (userId != null || rIds != null || orgIds != null)
            {
                Expression<Func<DocumentInfo, bool>> dataPredicate = Util.DataPermitUtil.GetDatePermitPredicate<DocumentInfo>(userId, rIds, orgIds);

                predicate = predicate.And(dataPredicate);
            }

            var result = _documentInfoRepository.FindPagenatedByDataPermission(pageIndex, pageSize, predicate, "ModifiedDate", false);
            var total = result.Item1;
            var rows = _mapper.Map<List<DocumentInfoDTO>>(result.Item2);
            return new PaginatedBaseDTO<DocumentInfoDTO>(pageIndex, pageSize, total, rows);
        }

        public DocumentInfoDTO GetDocumentById(int id)
        {
            var result = _documentInfoRepository.FindDocumntById(id);
            return _mapper.Map<DocumentInfoDTO>(result);
        }

        public string MoveDocumentsToCategory(string oldIds, string cateId)
        {
            if (string.IsNullOrWhiteSpace(oldIds))
            {
                return "请重新选择文件数据进行移动";
            }
            if (string.IsNullOrWhiteSpace(cateId))
            {
                return "请重新选择文件分类数据";
            }
            int newId = 0;
            var parserSuccess = int.TryParse(cateId, out newId);
            if (!parserSuccess)
            {
                return "请重新选择文件分类数据";
            }

            List<int> ids = oldIds.ArrayFromCommaDelimitedIntegers().ToList();
            var data = _documentInfoRepository.FindAll(m => ids.Contains(m.Id)).ToList();
            if (data == null)
            {
                return "数据有误";
            }

            var logs = new List<DocumentLog>();
            data.ForEach(k =>
            { 
                k.DocCategoryId = newId;
                logs.Add(new DocumentLog()
                {
                    DocumentId = k.Id,
                    DocOperType = DocOperType.Update,
                    OperatorId = k.ModifiedBy,
                    Operator = k.ModifiedName,
                    OperateDate = k.ModifiedDate,
                    Remark = string.Format("将文档【{0}】由分类：{1}，移动至分类：{2}。", k.Name, k.DocCategoryId, cateId),
                });
            });
            var result = _documentInfoRepository.Modify(data, new[] { "DocCategoryId" }) > 0;
            if (result)
            {
                _documentLogRepository.Add(logs);
                return "移动成功";
            }

            return "移动失败";
        }
        public string RemoveDocumentsByIds(string ids)
        {
            if (string.IsNullOrWhiteSpace(ids))
                return "传入的文档Ids为空";
            List<int> idlist = ids.ArrayFromCommaDelimitedIntegers().ToList();
            var infolist = _documentInfoRepository.FindDocumentsByIds(idlist);
            if (!infolist.Any())
                return string.Format("未找到文档Ids为：{0} 的数据", ids);
            
            var logs = new List<DocumentLog>();
            infolist.ForEach(k =>
            {
                k.IsDeleted = true;
                logs.Add(new DocumentLog()
                {
                    DocumentId = k.Id,
                    DocOperType = DocOperType.Delete,
                    OperatorId = k.ModifiedBy,
                    Operator = k.ModifiedName,
                    OperateDate = k.ModifiedDate,
                    Remark = string.Format("将文档【{0}】进行删除。", k.Name),
                });
            });
            var data = _documentInfoRepository.Modify(infolist, new[] { "IsDeleted" }) > 0;
            if (data)
            {
                _documentLogRepository.Add(logs);
                return "删除成功";
            }

            return "删除失败";

        }

        public bool SaveDocument(DocumentInfoDTO model)
        {
            var entity = _mapper.Map<DocumentInfo>(model);
            if (string.IsNullOrEmpty(entity.DocCode))
            {
                entity.DocCode = ConfigApiService.GetSeedCodeByName("DocumentCode");
            }

            var success = false;
            var msg = model.Id == 0
                    ? model.IsArchive 
                        ? "文件归档数据" 
                        : "创建了新的文件"
                    : "修改了文件的内容";
            if (model.Id == 0)
            {
                if (model.IsArchive)
                {
                    var blobId = model.Attachment?.BlobId;
                    var data = _documentInfoRepository.GetByFilter(m => m.AttachmentBlob.Contains(blobId));
                    if (data != null)
                    {
                        entity.Id = data.Id;
                        success = _documentInfoRepository.Modify(entity);
                    }
                    else
                    {
                        success = _documentInfoRepository.Add(entity);
                    }
                }
                else
                {
                    success = _documentInfoRepository.Add(entity);
                }
            }
            else
            {
                success = _documentInfoRepository.Modify(entity);
            }

            if (success)
            {
                _documentLogRepository.Add(new DocumentLog()
                {
                    DocumentId = entity.Id,
                    DocOperType = model.Id == 0 
                                    ? model.IsArchive 
                                        ? DocOperType.Archive 
                                        : DocOperType.Create 
                                    : DocOperType.Update,
                    OperatorId = model.ModifiedBy,
                    Operator = model.ModifiedName,
                    OperateDate = model.ModifiedDate,
                    Remark = msg,
                });
            }

            return success;
        }

        /// <summary>
        /// 查找文件分类返回Id 若不存在则插入新数据再返回
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type">0 我的文件 1 客户文件 2 报告</param>
        /// <returns></returns>
        public ReturnMessageDTO GetCompanyIdByName(string name, LableType type)
        {
            var msg = new ReturnMessageDTO()
            {
                Success = false,
            };
            if (!string.IsNullOrWhiteSpace(name))
            {
                var model = _docCategoryRepository.GetByFilter(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase) && x.Type == type);
                if (model == null)
                {
                    DocCategoryDTO category = new DocCategoryDTO
                    {
                        Text = name,
                        Type = type,
                    };
                    return SaveCategoryReturnId(category);
                }
                else
                {
                    msg.Id = model.Id.ToString();
                    msg.Success = true;
                    return msg;
                }
            }
            return msg;
        }

        /// <summary>
        /// 插入文件分类并返回Id
        /// </summary>
        /// <param name="model"></param>
        /// <param name="type">0 我的文件 1 客户文件</param>
        /// <returns></returns>
        private ReturnMessageDTO SaveCategoryReturnId(DocCategoryDTO model)
        {
            var msg = new ReturnMessageDTO() { Success = false };
            Expression<Func<DocCategory, bool>> predicate = m =>
            !m.IsDeleted && m.Level == model.Level &&
            m.Name.Equals(model.Text, StringComparison.OrdinalIgnoreCase) &&
            m.Type == model.Type;
            if (model.Id > 0)
            {
                predicate = predicate.And(m => m.Id != model.Id);
            }
            var resultany = _docCategoryRepository.GetByFilter(predicate);
            if (resultany != null)
            {
                msg.ErrorMessage = "已存在名为【" + model.Text + "】的文件分类，请勿重复添加！";
                return msg;
            }
            model.Text = model.Text;
            
            if (model.ParentId.HasValue && model.ParentId.Value > 0)
            {
                model.Level = 2;
                model.Leaf = true;
            }
            else
            {
                model.Level = 1;
                model.Leaf = false;
            }
            var result = _mapper.Map<DocCategory>(model);
            msg.Success = _docCategoryRepository.Add(result);
            if (msg.Success)
            {
                msg.Id = result.Id.ToString();
                return msg;
            }
            return msg;
        }

        #endregion

        #region 日志
        public PaginatedBaseDTO<DocumentLogDTO> FindPaginatedDocumentLogs(int pageIndex, int pageSize, string name)
        {
            Expression<Func<DocumentLog, bool>> predicate = m => true;
            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate = predicate.And(m => m.Operator.Contains(name));
            }

            var data = _documentLogRepository.FindPagenatedListWithCount(
                pageIndex, pageSize, predicate, m => m.OperateDate, true);

            var total = data.Item1;
            var rows = _mapper.Map<List<DocumentLogDTO>>(data.Item2);
            return new PaginatedBaseDTO<DocumentLogDTO>(pageIndex, pageSize, total, rows);

        }
        #endregion

    }
}
