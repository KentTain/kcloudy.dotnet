using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KC.Framework.Tenant;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using KC.Service.Message;
using KC.Service;
using KC.Service.DTO.Message;
using KC.Service.DTO;

namespace KC.Web.Message.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class MessageApiController : Web.Controllers.WebApiBaseController
    {
        protected IMessageService _messageService => ServiceProvider.GetService<IMessageService>();

        public MessageApiController(
            Tenant tenant,
            IServiceProvider serviceProvider,
            ILogger<MessageApiController> logger)
            : base(tenant, serviceProvider, logger)
        {
        }

        /// <summary>
        /// 获取最新的消息
        /// </summary>
        /// <param name="userId">用户的UserId</param>
        /// <param name="status">消息的状态值：0-未读；1-已读；2-已删除</param>
        /// <returns></returns>
        [HttpGet]
        [Route("LoadTop10UserMessages")]
        public ServiceResult<List<MemberRemindMessageDTO>> LoadTop10UserMessages(string userId, KC.Enums.Message.MessageStatus? status)
        {
            return GetServiceResult(() =>
            {
                return _messageService.FindTop10UserMessages(userId, status);
            });
        }

        /// <summary>
        /// 获取消息的分页数据
        /// </summary>
        /// <param name="pageIndex">分页页码</param>
        /// <param name="pageSize">分页大小（不能超过100）</param>
        /// <param name="userId">用户Id，必填项</param>
        /// <param name="title">消息的标题</param>
        /// <param name="status">消息的状态值：0-未读；1-已读；2-已删除</param>
        /// <returns>消息的分页数据</returns>
        [HttpGet]
        [Route("LoadPaginatedRemindMessages")]
        public ServiceResult<PaginatedBaseDTO<MemberRemindMessageDTO>> LoadPaginatedRemindMessages(int pageIndex, int pageSize, string userId, string title, KC.Enums.Message.MessageStatus? status)
        {
            return GetServiceResult(() =>
            {
                return _messageService.FindPaginatedRemindMessagesByFilter(pageIndex, pageSize, userId, title, status);
            });
        }

        /// <summary>
        /// 获取消息的详情
        /// </summary>
        /// <param name="id">消息的Id号</param>
        /// <returns>消息的详情</returns>
        [HttpGet]
        [Route("GetRemindMessageById")]
        public ServiceResult<MemberRemindMessageDTO> GetRemindMessageById(int id)
        {
            return GetServiceResult(() =>
            {
                return _messageService.GetRemindMessageById(id);
            });
        }


        /// <summary>
        /// 保存用户站内信消息
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("AddRemindMessages")]
        public ServiceResult<bool> AddRemindMessages([FromBody] List<MemberRemindMessageDTO> data)
        {
            return GetServiceResult(() =>
            {
                return _messageService.AddRemindMessages(data);
            });
        }

        /// <summary>
        /// 内信消息已读
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("ReadRemindMessage")]
        public ServiceResult<bool> ReadRemindMessage(int id)
        {
            return GetServiceResult(() =>
            {
                return _messageService.ReadRemindMessage(id);
            });
        }

    }
}
