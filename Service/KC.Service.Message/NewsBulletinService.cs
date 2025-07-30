using AutoMapper;
using KC.DataAccess.Message.Repository;
using KC.Database.EFRepository;
using KC.Database.IRepository;
using KC.Service.Enums.Message;
using KC.Framework.Extension;
using KC.Framework.Tenant;
using KC.Model.Message;
using KC.Service.DTO;
using KC.Service.DTO.Message;
using KC.Service.EFService;
using KC.Service.WebApiService.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace KC.Service.Message
{
    public interface INewsBulletinService : IEFService
    {
        #region  新闻公告类别
        List<NewsBulletinCategoryDTO> FindCategoryTreesByName(string name, NewsBulletinType? type);

        NewsBulletinCategoryDTO GetNewsBulletinCategoryById(int id);

        Task<bool> SaveNewsBulletinCategoryAsync(NewsBulletinCategoryDTO model);

        Task<bool> RemoveNewsBulletinCategoryByIdAsync(int id);
        bool SoftRemoveNewsBulletinCategoryById(int id);
        Task<bool> ExistCategoryNameAsync(int pid, string name);
        #endregion

        #region 新闻公告
        List<NewsBulletinDTO> FindTop10NewsBulletins(NewsBulletinType? type);

        PaginatedBaseDTO<NewsBulletinDTO> LoadPaginatedNewsBulletinsByFilter(int pageIndex, int pageSize, int? categoryId, string name, NewsBulletinType? type);

        NewsBulletinDTO GetNewsBulletinById(int id);

        bool RemoveNewsBulletinById(int id);

        bool SaveNewsBulletin(NewsBulletinDTO model);
        #endregion

        #region 日志

        PaginatedBaseDTO<NewsBulletinLogDTO> FindPaginatedNewsBulletinLog(int pageIndex, int pageSize, string userName);

        #endregion
    }

    public class NewsBulletinService : EFServiceBase, INewsBulletinService
    {
        private readonly IMapper _mapper;

        #region Repository & Construcation
        /// <summary>
        /// ComAccountUnitOfWorkContext
        ///     use AccountUnitOfWorkContext.Commit() to save the context change(transaction).
        /// </summary>
        protected EFUnitOfWorkContextBase _unitOfWorkContext { get; private set; }

        #region Repository
        private readonly IConfigApiService ConfigApiService;

        private readonly INewsBulletinCategoryRepository _newsBulletinCategoryRepository;
        private readonly IDbRepository<NewsBulletin> _newsBulletinRepository;
        private readonly IDbRepository<NewsBulletinLog> _newsBulletinLogRepository;
        #endregion

        public NewsBulletinService(
            Tenant tenant,
            IMapper mapper,
            IConfigApiService configApiService,
            EFUnitOfWorkContextBase unitOfWorkContext,

            INewsBulletinCategoryRepository newsBulletinCategoryRepository,
            IDbRepository<NewsBulletin> newsBulletinLogsRepository,
            IDbRepository<NewsBulletinLog> newsBulletinLogRepository,

            System.Net.Http.IHttpClientFactory clientFactory,
            Microsoft.Extensions.Logging.ILogger<NewsBulletinService> logger)
            : base(tenant, clientFactory, logger)
        {
            _mapper = mapper;
            _unitOfWorkContext = unitOfWorkContext;
            ConfigApiService = configApiService;

            _newsBulletinCategoryRepository = newsBulletinCategoryRepository;
            _newsBulletinRepository = newsBulletinLogsRepository;
            _newsBulletinLogRepository = newsBulletinLogRepository;
        }
        #endregion

        #region  新闻公告类别
        public List<NewsBulletinCategoryDTO> FindCategoryTreesByName(string name, NewsBulletinType? type)
        {
            Expression<Func<NewsBulletinCategory, bool>> predicate = m => !m.IsDeleted;
            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate = predicate.And(m => m.Name.Contains(name));
            }
            if (type.HasValue)
            {
                var t = Enum.Parse<KC.Enums.Message.NewsBulletinType>(type.Value.ToString());
                predicate = predicate.And(m => m.Type == t);
            }

            var data = _newsBulletinCategoryRepository.FindTreeNodesWithNestParentAndChildByFilter(predicate);
            return _mapper.Map<List<NewsBulletinCategoryDTO>>(data);
        }

        public NewsBulletinCategoryDTO GetNewsBulletinCategoryById(int id)
        {
            var data = _newsBulletinCategoryRepository.GetTreeNodeWithNestChildById(id);
            return _mapper.Map<NewsBulletinCategoryDTO>(data);
        }

        public async Task<bool> SaveNewsBulletinCategoryAsync(NewsBulletinCategoryDTO model)
        {
            if (model == null)
                throw new ArgumentException(string.Format("传入对象为空"));
            
            var exist = await _newsBulletinCategoryRepository.ExistByTreeNameAsync(model.Id, model.ParentId, model.Text);
            if (exist)
                throw new ArgumentException("已存在名为【" + model.Text + "】的分类，请勿重复添加！");
            if (model.ParentId <= 0)
                model.ParentId = null;

            var data = _mapper.Map<NewsBulletinCategory>(model);
            var result = await _newsBulletinCategoryRepository.SaveCategoryAsync(data);
            if (result)
                model.Id = data.Id;
            return result;
        }

        public async Task<bool> RemoveNewsBulletinCategoryByIdAsync(int id)
        {
            if (id <= 0)
                return true;

            return await _newsBulletinCategoryRepository.RemoveByIdAsync(id);
        }

        public bool SoftRemoveNewsBulletinCategoryById(int id)
        {
            if (id <= 0)
                throw new ArgumentException("未找到Id【" + id + "】的新闻公告分类记录！");

            var result = _newsBulletinCategoryRepository.GetCategoryWithChildrenById(id);
            if (result == null)
                throw new ArgumentException("未找到Id【" + id + "】的新闻公告分类记录！");

            if (result.ChildNodes.Any(x => !x.IsDeleted))
                throw new ArgumentException("新闻公告分类【" + result.Name + "】下有子级新闻公告分类，请先将其子级新闻公告分类移到到其他新闻公告分类下再进行删除！");

            var data = _newsBulletinRepository.FindAll(m => !m.IsDeleted && m.CategoryId == id);
            if (data != null && data.Any())
                throw new ArgumentException("新闻公告分类【" + result.Name + "】下有新闻公告，请先将新闻公告移动到其他新闻公告分类下再进行删除！");

            result.IsDeleted = true;
            return _newsBulletinCategoryRepository.Modify(result, new[] { "IsDeleted" });
        }

        public async Task<bool> ExistCategoryNameAsync(int pid, string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name", "分类名称为空.");

            Expression<Func<NewsBulletinCategory, bool>> predicate = m => !m.IsDeleted && m.Name.Equals(name);
            if (pid == 0)
                predicate = predicate.And(c => c.ParentId == null);
            else
                predicate = predicate.And(c => c.ParentId == pid);

            return await _newsBulletinCategoryRepository.ExistByFilterAsync(predicate);
        }
        #endregion

        #region 新闻公告
        public List<NewsBulletinDTO> FindTop10NewsBulletins(NewsBulletinType? type)
        {
            Expression<Func<NewsBulletin, bool>> predicate = m => !m.IsDeleted && m.IsShow;
            if (type.HasValue)
            {
                var t = Enum.Parse<KC.Enums.Message.NewsBulletinType>(type.Value.ToString());
                predicate = predicate.And(m => m.Type == t);
            }
            var data = _newsBulletinRepository.FindPagenatedList(1, 10, predicate, m => m.CreatedDate, false);

            return _mapper.Map<List<NewsBulletinDTO>>(data);

        }

        public PaginatedBaseDTO<NewsBulletinDTO> LoadPaginatedNewsBulletinsByFilter(int pageIndex, int pageSize, int? categoryId, string name, NewsBulletinType? type)
        {
            Expression<Func<NewsBulletin, bool>> predicate = m => !m.IsDeleted;
            //未分类
            if (categoryId.HasValue && categoryId.Value < 0)
            {
                predicate = predicate.And(m => m.CategoryId == null);
            }
            //已分类
            else if (categoryId.HasValue && categoryId.Value > 0)
            {
                predicate = predicate.And(m => m.CategoryId.Equals(categoryId.Value));
            }

            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate = predicate.And(m => m.Title.Contains(name.Trim()));
            }
            if (type.HasValue)
            {
                var t = Enum.Parse<KC.Enums.Message.NewsBulletinType>(type.Value.ToString());
                predicate = predicate.And(m => m.Type == t);
            }

            var result = _newsBulletinRepository.FindPagenatedListWithCount(pageIndex, pageSize, predicate, m => m.CreatedDate, false);
            var total = result.Item1;
            var rows = _mapper.Map<List<NewsBulletinDTO>>(result.Item2);
            return new PaginatedBaseDTO<NewsBulletinDTO>(pageIndex, pageSize, total, rows);
        }

        public NewsBulletinDTO GetNewsBulletinById(int id)
        {
            var result = _newsBulletinRepository.GetById(id);
            return _mapper.Map<NewsBulletinDTO>(result);
        }

        public bool RemoveNewsBulletinById(int id)
        {
            if (id <= 0) return false;

            var data = _newsBulletinRepository.GetById(id);
            data.IsDeleted = true;
            return _newsBulletinRepository.Modify(data, new string[] { "IsDeleted" });
        }

        public bool SaveNewsBulletin(NewsBulletinDTO data)
        {
            var model = _mapper.Map<NewsBulletin>(data);
            if (model.CategoryId.HasValue)
            {
                var category = _newsBulletinCategoryRepository.GetById(model.CategoryId);
                if (category == null)
                    throw new ArgumentException("未找到Id=" + model.CategoryId + "的新闻公告分类！");
            }

            var logMsg = "{0}新闻公告【{1}-{2}】成功";
            if (model.Id == 0)
            {
                _newsBulletinRepository.Add(model, false);

                logMsg = string.Format(logMsg, "新增", model.Type.ToDescription(), model.Title);
            }
            else
            {
                _newsBulletinRepository.Modify(model, new string[] { "CategoryId", "Title", "Keywords", "Description", "ImageBlob", "FileBlob", "Link", "Content", "IsShow", "Status" }, false);

                logMsg = string.Format(logMsg, "修改", model.Type.ToDescription(), model.Title);
            }

            //插入日志
            var t = Enum.Parse<KC.Enums.Message.NewsBulletinType>(data.Type.ToString());
            var log = new NewsBulletinLog()
            {
                OperatorId = data.ModifiedBy,
                Operator = data.ModifiedName,
                NewsBulletinType = t,
                NewsBulletinId = data.Id,
                NewsBulletinTitle = data.Title,
                NewsBulletinAuthor = data.Author,
                NewsBulletinStatus = data.Status,
                NewsBulletinContent = data.Content,
                Remark = logMsg,
            };
            _newsBulletinLogRepository.Add(log, false);

            return _unitOfWorkContext.Commit() > 0;
        }

        #endregion

        #region 日志

        public PaginatedBaseDTO<NewsBulletinLogDTO> FindPaginatedNewsBulletinLog(int pageIndex, int pageSize, string userName)
        {
            Expression<Func<NewsBulletinLog, bool>> predicate = m => true;
            if (!string.IsNullOrWhiteSpace(userName))
            {
                predicate = predicate.And(m => m.Operator.Contains(userName));
            }

            var data = _newsBulletinLogRepository.FindPagenatedListWithCount(
                pageIndex, pageSize, predicate, m => m.OperateDate, true);

            var total = data.Item1;
            var rows = _mapper.Map<List<NewsBulletinLogDTO>>(data.Item2);
            return new PaginatedBaseDTO<NewsBulletinLogDTO>(pageIndex, pageSize, total, rows);

        }
        #endregion
    }
}
