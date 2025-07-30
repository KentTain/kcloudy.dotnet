using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using KC.Framework.Extension;
using KC.DataAccess.Message.Repository;
using KC.Database.EFRepository;
using KC.Model.Message;
using KC.Service.EFService;
using KC.Service.DTO.Message;
using KC.Service.WebApiService.Business;
using KC.Database.IRepository;
using KC.Service.DTO;
using KC.Service;
using KC.Framework.Tenant;
using KC.Enums.Message;

namespace KC.Service.Message
{
    public interface IMessageService : IEFService
    {
        #region  消息分类
        List<MessageCategoryDTO> FindMessageCategoryList(string name);

        MessageCategoryDTO GetMessageCategoryById(int id);

        Task<bool> SaveMessageCategoryAsync(MessageCategoryDTO mess, string operatorId, string operatorName);
        
        bool SoftRemoveMessageCategory(int id, string operatorId, string operatorName);
        Task<bool> RemoveCategoryByIdAsync(int id);

        Task<bool> ExistCategoryNameAsync(int pid, string name);
        #endregion

        #region 消息模板类别
        List<MessageClassDTO> FindAllMessageClasses();
        List<MessageClassDTO> FindMessageByMessageCategoryId(int id);
        List<MessageClassDTO> FindMessageclassBytypename(int id, string name);
        PaginatedBaseDTO<MessageClassDTO> FindPaginatedMessageClassesByFilter(int pageIndex, int pageSize, int? categoryId, string name, BusinessType? type);
        
        MessageClassDTO GetMessageClassById(int id);
        MessageClassDTO GetMessageClassByCode(string code);
        
        bool SaveMessageClass(MessageClassDTO data, int id);
        bool SoftRemoveMessageClass(int id);
        Task<bool> ExistMessageClassAsync(string name);
        #endregion

        #region 消息模板
        List<MessageTemplateDTO> FindMessageTemplatesByClassId(int appId);
        PaginatedBaseDTO<MessageTemplateDTO> FindPaginatedMessageTemplatesByFilter(int pageIndex, int pageSize, string name);
        
        MessageTemplateDTO GetMessageTemplateById(int id);

        bool SaveMessageTemplate(MessageTemplateDTO data);
        bool SoftRemoveMessageTemplateById(int id);
        #endregion

        #region 消息模板日志

        PaginatedBaseDTO<MessageTemplateLogDTO> FindPaginatedMessageTemplateLogs(int pageIndex, int pageSize, string name);
        #endregion

        #region 用户消息
        List<MemberRemindMessageDTO> FindTop10UserMessages(string userId, MessageStatus? status);
        PaginatedBaseDTO<MemberRemindMessageDTO> FindPaginatedRemindMessagesByFilter(int pageIndex, int pageSize, string userId, string title, MessageStatus? status);
        
        MemberRemindMessageDTO GetRemindMessageById(int id);

        bool AddRemindMessages(List<MemberRemindMessageDTO> models);

        bool ReadRemindMessage(int receiverId);
        #endregion

    }

    public class MessageService : EFServiceBase, IMessageService
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

        private readonly IMessageCategoryRepository _messageCategoryRepository;
        private readonly IMessageClassRepository _messageClassRepository;
        private readonly IMemberRemindMessageRepository _memberRemindMessageRepository;

        private readonly IMessageTemplateRepository _messageTemplateRepository;
        private readonly IDbRepository<MessageTemplateLog> _messageLogsRepository;
        #endregion

        public MessageService(
            Tenant tenant,
            IMapper mapper,
            IConfigApiService configApiService,
            EFUnitOfWorkContextBase unitOfWorkContext,

            IMessageCategoryRepository messageCategoryRepository,
            IMessageClassRepository messageClassRepository,
            IMemberRemindMessageRepository memberRemindMessageRepository,
            IMessageTemplateRepository messageTemplateRepository,
            IDbRepository<MessageTemplateLog> messageLogsRepository,

            System.Net.Http.IHttpClientFactory clientFactory,
            Microsoft.Extensions.Logging.ILogger<MessageService> logger)
            : base(tenant, clientFactory, logger)
        {
            _mapper = mapper;
            _unitOfWorkContext = unitOfWorkContext;
            ConfigApiService = configApiService;

            _messageCategoryRepository = messageCategoryRepository;
            _messageClassRepository = messageClassRepository;
            _memberRemindMessageRepository = memberRemindMessageRepository;
            _messageLogsRepository = messageLogsRepository;
            _messageTemplateRepository = messageTemplateRepository;
        }
        #endregion

        #region  消息分类

        public List<MessageCategoryDTO> FindMessageCategoryList(string name)
        {
            Expression<Func<MessageCategory, bool>> predicate = m => !m.IsDeleted;
            if (!name.IsNullOrEmpty())
            {
                predicate = predicate.And(m => m.Name.Contains(name));
            }
            var data = _messageCategoryRepository.FindTreeNodesWithNestParentAndChildByFilter(predicate);
            return _mapper.Map<List<MessageCategoryDTO>>(data);
        }

        public MessageCategoryDTO GetMessageCategoryById(int id)
        {
            var model = _messageCategoryRepository.GetCategoryDetailById(id);
            return _mapper.Map<MessageCategoryDTO>(model);
        }

        public async Task<bool> SaveMessageCategoryAsync(MessageCategoryDTO model, string operatorId, string operatorName)
        {
            if (model == null)
                throw new ArgumentException(string.Format("传入对象为空"));
            
            var exist = await _messageCategoryRepository.ExistByTreeNameAsync(model.Id, model.ParentId, model.Text);
            if (exist)
                throw new ArgumentException("已存在名为【" + model.Text + "】的分类，请勿重复添加！");
            if (model.ParentId <= 0)
                model.ParentId = null;

            var data = _mapper.Map<MessageCategory>(model);
            var success = await _messageCategoryRepository.SaveCategoryAsync(data);
            if (success)
                model.Id = data.Id;
            return success;
        }
        public bool SoftRemoveMessageCategory(int id, string operatorId, string operatorName)
        {
            var model = _messageCategoryRepository.GetById(id);
            model.IsDeleted = true;
            model.Id = id;
            const string logMessage = "{0}在{1}进行了以下操作：{2}！";
            _messageCategoryRepository.SoftRemove(model, false);
            return _unitOfWorkContext.Commit() > 0;
        }

        public async Task<bool> RemoveCategoryByIdAsync(int id)
        {
            if (id <= 0)
                return true;

            return await _messageCategoryRepository.RemoveByIdAsync(id);
        }
        public async Task<bool> ExistCategoryNameAsync(int pid, string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name", "分类名称为空.");

            Expression<Func<MessageCategory, bool>> predicate = m => !m.IsDeleted && m.Name.Equals(name);
            if (pid == 0)
                predicate = predicate.And(c => c.ParentId == null);
            else
                predicate = predicate.And(c => c.ParentId == pid);

            return await _messageCategoryRepository.ExistByFilterAsync(predicate);
        }
        #endregion

        #region 消息模板类别
        public List<MessageClassDTO> FindAllMessageClasses()
        {
            var data = _messageClassRepository.FindAll();
            return _mapper.Map<List<MessageClassDTO>>(data);
        }
        public List<MessageClassDTO> FindMessageByMessageCategoryId(int id)
        {
            var model = _messageClassRepository.GetMessageClassesByMessageCategoryId(id);
            return _mapper.Map<List<MessageClassDTO>>(model);
        }
        public List<MessageClassDTO> FindMessageclassBytypename(int id, string name)
        {
            List<MessageClass> messageClasses = new List<MessageClass>();
            if (id != 0)
            {
                messageClasses = _messageClassRepository.FindAll().Where(m => m.Name == name).Where(m => m.Id != id).Where(m => !m.IsDeleted).ToList();
                return _mapper.Map<List<MessageClassDTO>>(messageClasses);
            }
            else
            {
                messageClasses = _messageClassRepository.FindAll().Where(m => m.Name == name).Where(m => !m.IsDeleted).ToList();
                return _mapper.Map<List<MessageClassDTO>>(messageClasses);
            }

        }
        
        public PaginatedBaseDTO<MessageClassDTO> FindPaginatedMessageClassesByFilter(int pageIndex, int pageSize, int? categoryId, string name, BusinessType? type)
        {
            Expression<Func<MessageClass, bool>> predicate = m => true && !m.IsDeleted;
            //未分类
            if (categoryId.HasValue && categoryId.Value < 0)
            {
                predicate = predicate.And(m => m.MessageCategoryId == null);
            }
            //已分类
            else if (categoryId.HasValue && categoryId.Value > 0)
            {
                predicate = predicate.And(m => m.MessageCategoryId.Equals(categoryId.Value));
            }

            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate = predicate.And(m => m.Name.Contains(name));
            }
            if (type.HasValue)
            {
                predicate = predicate.And(m => m.Type == type);
            }

            var data = _messageClassRepository.FindPagenatedListWithCount(pageIndex, pageSize, predicate, m => m.Index);

            var total = data.Item1;
            var rows = _mapper.Map<List<MessageClassDTO>>(data.Item2);
            return new PaginatedBaseDTO<MessageClassDTO>(pageIndex, pageSize, total, rows);
        }
        
        public MessageClassDTO GetMessageClassById(int id)
        {
            var data = _messageClassRepository.GetById(id);
            return _mapper.Map<MessageClassDTO>(data);
        }
        public MessageClassDTO GetMessageClassByCode(string code)
        {
            var data = _messageClassRepository.GetMessageClassDetailByCode(code);
            return _mapper.Map<MessageClassDTO>(data);
        }
        
        public bool SaveMessageClass(MessageClassDTO data, int id)
        {
            var model = _mapper.Map<MessageClass>(data);
            if (model.Id == 0)
            {
                model.Code = ConfigApiService.GetSeedCodeByName("MessageTemplate");
                return _messageClassRepository.Add(model);
            }
            else
            {
                return _messageClassRepository.Modify(model, new string[] { "Type", "Name", "MessageCategoryId", "ReplaceParametersString" });
            }
        }

        public bool SoftRemoveMessageClass(int id)
        {
            if (id <= 0)
                throw new ArgumentException("未找到Id=" + id + "的消息模板！");

            var data = _messageClassRepository.GetMessageClassDetailById(id);
            if (data.MessageTemplates.Any())
                throw new ArgumentException("消息模板【" + data.Name + "】下还存在相关模板数据，请删除子项后再进行删除！");

            data.IsDeleted = true;
            return _messageClassRepository.Modify(data, new string[] { "IsDeleted" });
        }

        public async Task<bool> ExistMessageClassAsync(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name", "分类名称为空.");

            Expression<Func<MessageClass, bool>> predicate = m => !m.IsDeleted && m.Name.Equals(name);
            return await _messageClassRepository.ExistByFilterAsync(predicate);
        }
        #endregion

        #region 消息模板
        public List<MessageTemplateDTO> FindMessageTemplatesByClassId(int classId)
        {
            var data = _messageTemplateRepository.FindMessageTemplatesByClassId(classId);
            return _mapper.Map<List<MessageTemplateDTO>>(data);
        }

        public PaginatedBaseDTO<MessageTemplateDTO> FindPaginatedMessageTemplatesByFilter(int pageIndex, int pageSize,
            string name)
        {
            Expression<Func<MessageTemplate, bool>> predicate = m => true;
            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate = predicate.And(m => m.Name.Contains(name));
            }
            var data = _messageTemplateRepository.FindPagenatedListWithCount(
                pageIndex,
                pageSize,
                predicate,
                m => m.CreatedDate);

            var total = data.Item1;
            var rows = _mapper.Map<List<MessageTemplateDTO>>(data.Item2);
            return new PaginatedBaseDTO<MessageTemplateDTO>(pageIndex, pageSize, total, rows);
        }

        public MessageTemplateDTO GetMessageTemplateById(int id)
        {
            var data = _messageTemplateRepository.GetMessageTemplateDetailById(id);
            return _mapper.Map<MessageTemplateDTO>(data);
        }

        public bool SaveMessageTemplate(MessageTemplateDTO data)
        {
            var model = _mapper.Map<MessageTemplate>(data);
            var logMsg = "{0}模板【{1}-{2}】成功";
            if (model.Id == 0)
            {
                model.Subject = model.Name;
                _messageTemplateRepository.Add(model, false);

                logMsg = string.Format(logMsg, "新增", model.TemplateType.ToDescription(), model.Name);
            }
            else
            {
                _messageTemplateRepository.Modify(model, new string[] { "Name", "Subject", "Content" }, false);

                logMsg = string.Format(logMsg, "修改", model.TemplateType.ToDescription(), model.Name);
            }

            //插入日志
            var log = new MessageTemplateLog()
            {
                OperatorId = data.ModifiedBy,
                Operator = data.ModifiedName,
                TemplateType = Enum.Parse<MessageTemplateType>(data.TemplateType.ToString()),
                TemplateId = data.Id,
                TemplateName = data.Name,
                TemplateSubject = data.Subject,
                TemplateContent = data.Content,
                Remark = logMsg,
            };
            _messageLogsRepository.Add(log, false);

            return _unitOfWorkContext.Commit() > 0;
        }
        public bool SoftRemoveMessageTemplateById(int id)
        {
            if (id <= 0)
                throw new ArgumentException("未找到Id=" + id + "的消息模板！");

            var data = _messageTemplateRepository.GetById(id);
            if (data == null)
                throw new ArgumentException("未找到Id=" + id + "的消息模板！");

            data.IsDeleted = true;
            _messageTemplateRepository.Modify(data, new string[] { "IsDeleted" }, false);
            
            //插入日志
            var log = new MessageTemplateLog()
            {
                OperatorId = data.ModifiedBy,
                Operator = data.ModifiedName,
                TemplateType = Enum.Parse<MessageTemplateType>(data.TemplateType.ToString()),
                TemplateId = data.Id,
                TemplateName = data.Name,
                TemplateSubject = data.Subject,
                TemplateContent = data.Content,
                Remark = string.Format("{0}模板【{1}-{2}】成功", "删除", data.TemplateType.ToDescription(), data.Name),
            };
            _messageLogsRepository.Add(log, false);
            return _unitOfWorkContext.Commit() > 0;
        }

        #endregion

        #region 日志

        public PaginatedBaseDTO<MessageTemplateLogDTO> FindPaginatedMessageTemplateLogs(int pageIndex, int pageSize, string name)
        {
            Expression<Func<MessageTemplateLog, bool>> predicate = m => true;
            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate = predicate.And(m => m.Operator.Contains(name));
            }

            var data = _messageLogsRepository.FindPagenatedListWithCount(
                pageIndex, pageSize, predicate, m => m.OperateDate, true);

            var total = data.Item1;
            var rows = _mapper.Map<List<MessageTemplateLogDTO>>(data.Item2);
            return new PaginatedBaseDTO<MessageTemplateLogDTO>(pageIndex, pageSize, total, rows);

        }
        #endregion

        #region 用户消息
        public List<MemberRemindMessageDTO> FindTop10UserMessages(string userId, MessageStatus? status)
        {
            Expression<Func<MemberRemindMessage, bool>> predicate = m => !m.IsDeleted && m.UserId == userId && m.Status != MessageStatus.Deleted;
            if (status.HasValue)
            {
                predicate = predicate.And(m => m.Status.Equals(status.Value));
            }
            var data = _memberRemindMessageRepository.FindPagenatedList(1, 10, predicate, m => m.CreatedDate, false);

            return _mapper.Map<List<MemberRemindMessageDTO>>(data);
        }

        public PaginatedBaseDTO<MemberRemindMessageDTO> FindPaginatedRemindMessagesByFilter(int pageIndex, int pageSize, string userId, string title, MessageStatus? status)
        {
            Expression<Func<MemberRemindMessage, bool>> predicate = m => !m.IsDeleted && m.UserId == userId && m.Status != MessageStatus.Deleted;
            if (!string.IsNullOrWhiteSpace(title))
            {
                predicate = predicate.And(m => m.MessageTitle.Contains(title));
            }
            if (status.HasValue)
            {
                predicate = predicate.And(m => m.Status.Equals(status.Value));
            }

            var data = _memberRemindMessageRepository.FindPagenatedListWithCount(pageIndex, pageSize, predicate, m => m.CreatedDate, false);

            var total = data.Item1;
            var rows = _mapper.Map<List<MemberRemindMessageDTO>>(data.Item2);
            return new PaginatedBaseDTO<MemberRemindMessageDTO>(pageIndex, pageSize, total, rows);
        }

        public MemberRemindMessageDTO GetRemindMessageById(int id)
        {
            var data = _memberRemindMessageRepository.GetById(id);
            return _mapper.Map<MemberRemindMessageDTO>(data);
        }

        public bool AddRemindMessages(List<MemberRemindMessageDTO> models)
        {
            var data = _mapper.Map< List<MemberRemindMessage>>(models);
            return _memberRemindMessageRepository.Add(data) > 0;
        }

        public bool ReadRemindMessage(int id)
        {
            var data = new MemberRemindMessage
            {
                Id = id,
                Status = MessageStatus.Read,
                ReadDate = DateTime.UtcNow
            };
            return _memberRemindMessageRepository.Modify(data, new string[] { "Status", "ReadDate" });
        }
        #endregion

    }
}
