using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KC.Service.DTO.Message;
using KC.Enums.Message;
using KC.Framework.Base;
using KC.Framework.Tenant;
using KC.Service.Message;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using KC.Service.Util;
using KC.Framework.Extension;

namespace KC.Web.Message.Controllers
{
    public class MessageController : MessageBaseController
    {
        private IMessageService _messageService => ServiceProvider.GetService<IMessageService>();

        public MessageController(
            Tenant tenant,
            IServiceProvider serviceProvider,
            ILogger<MessageController> logger)
            : base(tenant, serviceProvider, logger)
        {
        }

        /// <summary>
        /// 三级菜单：消息管理/消息模板管理/消息模板列表
        /// </summary>
        [Web.Extension.MenuFilter("消息模板管理", "消息模板列表", "/Message/Index",
            Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType,
            SmallIcon = "fa fa-list-alt", AuthorityId = "ACED2BB0-0FE8-44B8-9C6B-3E98952370A6",
            DefaultRoleId = RoleConstants.ExecutorManagerRoleId, Order = 1, IsExtPage = false, Level = 3)]
        [Web.Extension.PermissionFilter("消息模板列表", "消息模板列表", "/Message/Index",
            "ACED2BB0-0FE8-44B8-9C6B-3E98952370A6", DefaultRoleId = RoleConstants.ExecutorManagerRoleId,
            Order = 1, IsPage = true, ResultType = ResultType.ActionResult)]
        public ActionResult Index()
        {
            MakeMessageTypeList();
            return View();
        }
        private void MakeMessageTypeList()
        {
            ViewBag.MessageTypeList = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnumWithAll<BusinessType>();
        }

        #region 消息分类
        /// <summary>
        /// 获取分类的树结构数据
        /// </summary>
        /// <param name="name">分类名称</param>
        /// <param name="excludeId">需要排除的分类Id</param>
        /// <param name="selectedId">需要设置为选中的分类Id</param>
        /// <param name="hasAll">是否要添加：所有及未分类的类别</param>
        /// <param name="hasRoot">是否要添加顶级分类</param>
        /// <param name="maxLevel">树结构节点的深度：Level</param>
        /// <returns></returns>
        public JsonResult LoadCategoryTree(string name, int excludeId, int selectedId, bool hasAll = false, bool hasRoot = true, int maxLevel = 3)
        {
            var result = new List<MessageCategoryDTO>();
            if (hasAll)
            {
                result.Add(new MessageCategoryDTO()
                {
                    Id = 0,
                    Text = "所有消息",
                    Description = "所有消息",
                    Children = null,
                    Level = 1
                });
                result.Add(new MessageCategoryDTO()
                {
                    Id = -1,
                    Text = "未分类消息",
                    Description = "未分配类型的消息",
                    Children = null,
                    Level = 1
                });
            }
            var data = _messageService.FindMessageCategoryList(name);
            if (data != null && data.Any())
                result.AddRange(data);

            if (hasRoot)
            {
                var rootMenu = new MessageCategoryDTO() { Text = "顶级类型", Children = result, Level = 0 };
                var org = TreeNodeUtil.GetNeedLevelTreeNodeDTO(rootMenu, maxLevel, new List<int>() { excludeId }, new List<int>() { selectedId });
                return Json(new List<MessageCategoryDTO> { org });
            }

            var orgList = TreeNodeUtil.LoadNeedLevelTreeNodeDTO(result, maxLevel, new List<int>() { excludeId }, new List<int>() { selectedId });
            return Json(orgList);
        }

        public ActionResult GetMessageCategoryForm(int id = 0, int parentId = 0)
        {
            MessageCategoryDTO model;
            if (id > 0)
            {
                model = _messageService.GetMessageCategoryById(id);
                model.IsEditMode = true;
            }
            else
            {
                model = new MessageCategoryDTO() { ParentId = parentId > 0 ? parentId : 0 };
            }

            return PartialView("_messageCategoryForm", model);
        }

        [Web.Extension.PermissionFilter("消息模板列表", "保存消息分类", "/Message/SaveMessageCategory",
            "82D91217-49F9-490D-8FAE-088F77C2DB63", DefaultRoleId = RoleConstants.ExecutorManagerRoleId,
            Order = 2, IsPage = false, ResultType = ResultType.JsonResult)]
        public async Task<JsonResult> SaveMessageCategory(MessageCategoryDTO model)
        {
            return await GetServiceJsonResultAsync(async () =>
            {
                if (model == null)
                    throw new ArgumentException(string.Format("传入对象为空"));

                //对象属性验证
                var errMsg = GetAllModelErrorMessages();
                if (!errMsg.IsNullOrEmpty())
                    throw new ArgumentException(errMsg);

                if (!model.IsEditMode)
                {
                    model.CreatedBy = CurrentUserId;
                    model.CreatedName = CurrentUserDisplayName;
                    model.CreatedDate = DateTime.UtcNow;
                }
                model.ModifiedBy = CurrentUserId;
                model.ModifiedName = CurrentUserDisplayName;
                model.ModifiedDate = DateTime.UtcNow;
                model.Level = model.ParentId > 0 ? 2 : 1;

                return await _messageService.SaveMessageCategoryAsync(model, CurrentUserId, CurrentUserDisplayName);
            });

        }

        [Web.Extension.PermissionFilter("消息模板列表", "删除消息分类", "/Message/RemoveMessageCategory",
            "0DFD5258-A12C-4424-8C90-61DEB778EB8A", DefaultRoleId = RoleConstants.ExecutorManagerRoleId,
            Order = 3, IsPage = false, ResultType = ResultType.JsonResult)]
        public JsonResult RemoveMessageCategory(int id)
        {
            return GetServiceJsonResult(() =>
            {
                if (id == 0)
                {
                    throw new ArgumentException("请选中消息类型Id");
                }

                var MessageCategorymodel = _messageService.GetMessageCategoryById(id);
                if (MessageCategorymodel == null)
                {
                    throw new ArgumentException(string.Format("未找到类型ID:{0}", id));
                }
                if (MessageCategorymodel.Children.Any(m => !m.IsDeleted))
                {
                    throw new ArgumentException(string.Format("类型:【{0}】存在子类型，无法删除，请先删除子类型！", MessageCategorymodel.Text));
                }
                var messagecategorylist = _messageService.FindMessageByMessageCategoryId(id);
                foreach (var item in messagecategorylist)
                {
                    if (messagecategorylist.Count != 0)
                    {
                        throw new ArgumentException(string.Format("消息分类【{0}】有引用该类型,不能删除", item.Name));
                    }
                }

                return _messageService.SoftRemoveMessageCategory(id, CurrentUserId, CurrentUserDisplayName);
            });
        }

        [AllowAnonymous]
        public async Task<JsonResult> ExistCategoryName(int id, string name, string orginalName, bool isEditMode)
        {
            if (isEditMode && name.Equals(orginalName, StringComparison.OrdinalIgnoreCase))
                return Json(false);

            return Json(await _messageService.ExistCategoryNameAsync(id, name));
        }
        #endregion

        #region 消息模板类别

        public JsonResult LoadMessageClassList(int page = 1, int rows = 10, int? categoryId = 0, string name = "", BusinessType? type = null)
        {
            var result = _messageService.FindPaginatedMessageClassesByFilter(page, rows, categoryId, name, type);
            return Json(result);
        }

        public PartialViewResult GetMessageClassForm(int id, int? pid)
        {
            MessageClassDTO model;
            if (id > 0)
            {
                model = _messageService.GetMessageClassById(id);
                model.IsEditMode = true;
            }
            else
            {
                model = new MessageClassDTO() { MessageCategoryId = pid.HasValue && pid.Value > 0 ? pid : null };
            }

            ViewBag.MessageTypes = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum<BusinessType>((int)model.Type);
            return PartialView("_messageClassForm", model);
        }

        [Web.Extension.PermissionFilter("消息模板列表", "保存消息模板类别", "/Message/SaveMessageClassForm",
            "F7936F06-DF2D-4AE7-9B10-046F8B8BC573", DefaultRoleId = RoleConstants.ExecutorManagerRoleId,
            Order = 4, IsPage = false, ResultType = ResultType.JsonResult)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SaveMessageClassForm(MessageClassDTO model, int id)
        {
            return GetServiceJsonResult(() =>
            {
                if (model == null)
                    throw new ArgumentException(string.Format("传入对象为空"));

                model.ApplicationId = CurrentApplicationId;
                model.ApplicationName = CurrentApplicationName;
                //对象属性验证
                var errMsg = GetAllModelErrorMessages();
                if (!errMsg.IsNullOrEmpty())
                    throw new ArgumentException(errMsg);

                if (!model.IsEditMode)
                {
                    model.CreatedBy = CurrentUserId;
                    model.CreatedName = CurrentUserDisplayName;
                    model.CreatedDate = DateTime.UtcNow;
                }
                model.ModifiedBy = CurrentUserId;
                model.ModifiedName = CurrentUserDisplayName;
                model.ModifiedDate = DateTime.UtcNow;

                var messageclasslist = _messageService.FindMessageclassBytypename(model.Id, model.Name);
                if (messageclasslist.Count != 0)
                {
                    throw new ArgumentException(string.Format("【{0}】分类名称已存在，请重新输入", model.Name));
                }
                return _messageService.SaveMessageClass(model, id);
            });
        }

        [Web.Extension.PermissionFilter("消息模板列表", "删除消息模板类别", "/Message/RemoveMessageClass",
            "C681B352-292F-4B01-A779-C383FB92D12F", DefaultRoleId = RoleConstants.ExecutorManagerRoleId,
            Order = 5, IsPage = false, ResultType = ResultType.JsonResult)]
        public JsonResult RemoveMessageClass(int id)
        {
            return GetServiceJsonResult(() =>
            {
                return _messageService.SoftRemoveMessageClass(id); 
            });
        }

        [AllowAnonymous]
        public async Task<JsonResult> ExistMessageClassName(string name, string orginalName, bool isEditMode)
        {
            if (isEditMode && name.Equals(orginalName, StringComparison.OrdinalIgnoreCase))
                return Json(false);

            return Json(await _messageService.ExistMessageClassAsync(name));
        }
        #endregion

        #region 消息模板

        public JsonResult LoadMessageTemplateList(int appId)
        {
            var result = _messageService.FindMessageTemplatesByClassId(appId);
            return Json(result);
        }

        [Web.Extension.PermissionFilter("消息模板列表", "删除消息模板", "/Message/RemoveMessageTemplate",
            "20CDD4FA-7080-4C1A-BC0C-37615703D973", DefaultRoleId = RoleConstants.ExecutorManagerRoleId,
            Order = 6, IsPage = false, ResultType = ResultType.JsonResult)]
        public JsonResult RemoveMessageTemplate(int id)
        {
            return GetServiceJsonResult(() =>
            {
                return _messageService.SoftRemoveMessageTemplateById(id);
            });
        }

        public PartialViewResult GetMessageTemplateForm(int id, int mcId, string parmString)
        {
            var model = new MessageTemplateDTO();
            model.TemplateType = Service.Enums.Message.MessageTemplateType.SmsMessage;
            if (id > 0)
            {
                model = _messageService.GetMessageTemplateById(id);
                model.IsEditMode = true;
            }
            else
            {
                var existTemplates = _messageService.FindMessageTemplatesByClassId(mcId);
                var hasSmsTemplate = existTemplates.Any(m => m.TemplateType == Service.Enums.Message.MessageTemplateType.SmsMessage);
                if (hasSmsTemplate)
                {
                    model = existTemplates.FirstOrDefault(m => m.TemplateType == Service.Enums.Message.MessageTemplateType.SmsMessage);
                    model.IsEditMode = true;
                }
            }
            model.MessageClassId = mcId;
            model.ReplaceParametersString = parmString;

            ViewBag.MessageTemplateTypeList = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum<MessageTemplateType>((int)model.TemplateType);
            return PartialView("_messageTemplateForm", model);
        }

        [Web.Extension.PermissionFilter("消息模板列表", "保存短信模板", "/Message/SaveSmsTemplate",
            "2D4E6BBA-CB63-4168-BB2B-DF62D2BC8804", DefaultRoleId = RoleConstants.ExecutorManagerRoleId,
            Order = 7, IsPage = false, ResultType = ResultType.JsonResult)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SaveSmsTemplate(MessageTemplateDTO model)
        {
            return GetServiceJsonResult(() =>
            {
                if (model == null)
                    throw new ArgumentException(string.Format("传入对象为空"));

                //对象属性验证
                var errMsg = GetAllModelErrorMessages();
                if (!errMsg.IsNullOrEmpty())
                    throw new ArgumentException(errMsg);

                if (!model.IsEditMode)
                {
                    model.CreatedBy = CurrentUserId;
                    model.CreatedName = CurrentUserDisplayName;
                    model.CreatedDate = DateTime.UtcNow;
                }
                model.ModifiedBy = CurrentUserId;
                model.ModifiedName = CurrentUserDisplayName;
                model.ModifiedDate = DateTime.UtcNow;

                return _messageService.SaveMessageTemplate(model);
            });
        }


        /// <summary>
        /// 三级菜单：消息管理/消息模板管理/通用模板
        /// </summary>
        [Web.Extension.MenuFilter("消息模板管理", "通用模板", "/Message/GetCommonTemplateForm",
            Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType,
            SmallIcon = "fa fa-list-alt", AuthorityId = "8962A908-27AD-40BF-A52B-C537C95E182C",
            DefaultRoleId = RoleConstants.ExecutorManagerRoleId, Order = 2, IsExtPage = true, Level = 3)]
        [Web.Extension.PermissionFilter("通用模板", "通用模板", "/Message/GetCommonTemplateForm",
            "8962A908-27AD-40BF-A52B-C537C95E182C", DefaultRoleId = RoleConstants.ExecutorManagerRoleId,
            Order = 2, IsPage = true, ResultType = ResultType.ActionResult)]
        public ActionResult GetCommonTemplateForm(int id, int mcId, string parmString)
        {
            var model = new MessageTemplateDTO();
            model.IsEditMode = true;
            ViewBag.HasCommonTemplate = false;
            if (id > 0)
            {
                model = _messageService.GetMessageTemplateById(id);
            }
            else
            {
                var existTemplates = _messageService.FindMessageTemplatesByClassId(mcId);
                var hasInsideTemplate = existTemplates.Any(m => m.TemplateType == Service.Enums.Message.MessageTemplateType.InsideMessage);
                var hasEmailTemplate = existTemplates.Any(m => m.TemplateType == Service.Enums.Message.MessageTemplateType.EmailMessage);
                ViewBag.HasCommonTemplate = hasInsideTemplate || hasEmailTemplate;
                model.IsEditMode = false; 
                if (hasInsideTemplate)
                {
                    model.TemplateType = Service.Enums.Message.MessageTemplateType.EmailMessage;
                } 
                else if (hasEmailTemplate)
                {
                    model.TemplateType = Service.Enums.Message.MessageTemplateType.InsideMessage;
                }
                else
                {
                    model.TemplateType = Service.Enums.Message.MessageTemplateType.InsideMessage;
                }
            }
            model.MessageClassId = mcId;
            model.ReplaceParametersString = parmString;

            ViewBag.MessageTemplateTypeList = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum<MessageTemplateType>((int)model.TemplateType, new List<MessageTemplateType>() { MessageTemplateType.SmsMessage });
            return View("CommonTemplateForm", model);
        }

        [Web.Extension.PermissionFilter("通用模板", "保存通用模板", "/Message/SaveCommonTemplate",
            "A50EB553-D99D-46E8-9FEE-0113F6ED1608", DefaultRoleId = RoleConstants.ExecutorManagerRoleId,
            Order = 2, IsPage = false, ResultType = ResultType.JsonResult)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SaveCommonTemplate(MessageTemplateDTO model)
        {
            return GetServiceJsonResult(() =>
            {
                if (model == null)
                    throw new ArgumentException(string.Format("传入对象为空"));

                //对象属性验证
                var errMsg = GetAllModelErrorMessages();
                if (!errMsg.IsNullOrEmpty())
                    throw new ArgumentException(errMsg);

                if (!model.IsEditMode)
                {
                    model.CreatedBy = CurrentUserId;
                    model.CreatedName = CurrentUserDisplayName;
                    model.CreatedDate = DateTime.UtcNow;
                }
                model.ModifiedBy = CurrentUserId;
                model.ModifiedName = CurrentUserDisplayName;
                model.ModifiedDate = DateTime.UtcNow;

                return _messageService.SaveMessageTemplate(model);
            });
        }
        #endregion

        #region 二级菜单（公共页面）：我的消息列表、消息详情
        /// <summary>
        /// 二级菜单：消息管理/我的消息列表
        /// </summary>
        /// <returns></returns>
        [Web.Extension.MenuFilter("消息管理", "我的消息列表", "/Message/MyMessageList",
            Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType,
            SmallIcon = "fa fa-list-alt", AuthorityId = "6F6711A4-089D-4FD3-9FA5-739838331337",
            DefaultRoleId = RoleConstants.DefaultRoleId, Order = 3, IsExtPage = true, Level = 2)]
        [Web.Extension.PermissionFilter("我的消息列表", "我的消息列表", "/Message/MyMessageList",
            "6F6711A4-089D-4FD3-9FA5-739838331337", DefaultRoleId = RoleConstants.DefaultRoleId,
            Order = 3, IsPage = true, ResultType = ResultType.ActionResult)]
        public IActionResult MyMessageList(MessageStatus? status)
        {
            ViewBag.selectStatus = status.HasValue ? ((int)status.Value).ToString() : string.Empty;
            ViewBag.StatusList = status.HasValue
                ? KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum(new List<int>() { (int)status.Value }, new List<MessageStatus>() { MessageStatus.Deleted })
                : KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum(null, new List<MessageStatus>() { MessageStatus.Deleted });

            return View();
        }

        public JsonResult LoadMyMessages(int page, int rows, string title, MessageStatus? status)
        {
            var result = _messageService.FindPaginatedRemindMessagesByFilter(page, rows, CurrentUserId, title, status);
            return Json(result);
        }

        /// <summary>
        /// 二级菜单：消息管理/消息详情
        /// </summary>
        /// <returns></returns>
        [Web.Extension.MenuFilter("消息管理", "消息详情", "/Message/MessageDetail",
            Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType,
            SmallIcon = "fa fa-list-alt", AuthorityId = "F15D8E2F-FF40-4517-98C5-33949907457B",
            DefaultRoleId = RoleConstants.DefaultRoleId, Order = 4, IsExtPage = true, Level = 2)]
        [Web.Extension.PermissionFilter("消息详情", "消息详情", "/Message/MessageDetail",
            "F15D8E2F-FF40-4517-98C5-33949907457B", DefaultRoleId = RoleConstants.DefaultRoleId,
            Order = 4, IsPage = true, ResultType = ResultType.ActionResult)]
        public IActionResult MessageDetail(int id)
        {
            var model = _messageService.GetRemindMessageById(id);
            ViewBag.StatusList = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum<MessageStatus>((int)model.Status);
            if (model != null)
            {
                var isRead = _messageService.ReadRemindMessage(id);
            }

            return View("MessageDetail", model);
        }

        #endregion

        #region 消息模板日志
        /// <summary>
        /// 三级菜单：消息管理/消息模板管理/消息模板日志
        /// </summary>
        /// <returns></returns>
        [Web.Extension.MenuFilter("消息模板管理", "消息模板日志", "/Message/MessageTemplateLog",
            Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType,
            SmallIcon = "fa fa-list-alt", AuthorityId = "B18F03FC-922B-4450-898F-B0DF23007225",
            DefaultRoleId = RoleConstants.ExecutorManagerRoleId, Order = 3, IsExtPage = false, Level = 3)]
        [Web.Extension.PermissionFilter("消息模板日志", "消息模板日志", "/Message/MessageTemplateLog",
            "B18F03FC-922B-4450-898F-B0DF23007225", DefaultRoleId = RoleConstants.ExecutorManagerRoleId,
            Order = 3, IsPage = true, ResultType = ResultType.ActionResult)]
        public IActionResult MessageTemplateLog()
        {
            return View();
        }

        public JsonResult LoadMessageTemplateLogList(int page = 1, int rows = 10, string name = null)
        {
            var result = _messageService.FindPaginatedMessageTemplateLogs(page, rows, name);

            return Json(result);
        }
        #endregion

    }
}