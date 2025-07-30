using KC.Common;
using KC.Framework.Tenant;
using KC.Service.DTO;
using KC.Service.DTO.Message;
using KC.Service.Enums.Message;
using KC.Service.WebApiService.Business;
using System;
using System.Collections.Generic;
using System.Text;

namespace KC.Service.Account.WebApiService
{
    public class MessageApiService : IdSrvOAuth2ClientRequestBase, IMessageApiService
    {
        private const string ServiceName = "KC.Service.Account.WebApiService";

        public MessageApiService(
            Tenant tenant,
            System.Net.Http.IHttpClientFactory httpClient,
            Microsoft.Extensions.Logging.ILogger<MessageApiService> logger)
            : base(tenant, httpClient, logger)
        {
        }

        #region 新闻公告
        /// <summary>
        /// 获取最新的新闻公告
        /// </summary>
        /// <returns></returns>
        public List<NewsBulletinDTO> LoadLatestNewsBulletins(NewsBulletinType? type)
        {
            ServiceResult<List<NewsBulletinDTO>> result = null;
            string t = type.HasValue ? ((int)type.Value).ToString() : string.Empty;
            WebSendGet<ServiceResult<List<NewsBulletinDTO>>>(
                ServiceName + ".LoadLatestNewsBulletins",
                MessageApiUrl + "NewsBulletinApi/LoadLatestNewsBulletins?type=" + t,
                ApplicationConstant.MsgScope,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<List<NewsBulletinDTO>>(ServiceResultType.Error, httpStatusCode, errorMessage);
                }, true);
            if (result.success && result.Result != null)
            {
                return result.Result;
            }

            return null;
        }

        /// <summary>
        /// 获取新闻公告的分页数据
        /// </summary>
        /// <param name="pageIndex">分页页码</param>
        /// <param name="pageSize">分页大小（不能超过100）</param>
        /// <param name="categoryId">所属分类</param>
        /// <param name="title">新闻公告的标题</param>
        /// <returns>新闻公告的分页数据</returns>
        public PaginatedBaseDTO<NewsBulletinDTO> LoadPaginatedNewsBulletins(int pageIndex, int pageSize, int? categoryId, string title)
        {
            ServiceResult<PaginatedBaseDTO<NewsBulletinDTO>> result = null;
            var c = categoryId.HasValue ? categoryId.Value.ToString() : string.Empty;
            WebSendGet<ServiceResult<PaginatedBaseDTO<NewsBulletinDTO>>>(
                ServiceName + ".LoadPaginatedNewsBulletins",
                MessageApiUrl + "NewsBulletinApi/LoadPaginatedNewsBulletins?pageIndex=" + pageIndex + "&pageSize=" + pageSize + "&categoryId=" + c + "&title=" + title,
                ApplicationConstant.MsgScope,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<PaginatedBaseDTO<NewsBulletinDTO>>(ServiceResultType.Error, httpStatusCode, errorMessage);
                }, true);
            if (result.success && result.Result != null)
            {
                return result.Result;
            }

            return null;
        }

        /// <summary>
        /// 获取新闻公告的详情
        /// </summary>
        /// <param name="id">新闻公告的Id号</param>
        /// <returns>新闻公告的详情</returns>
        public NewsBulletinDTO GetNewsBulletinById(int id)
        {
            ServiceResult<NewsBulletinDTO> result = null;
            WebSendGet<ServiceResult<NewsBulletinDTO>>(
                ServiceName + ".GetNewsBulletinById",
                MessageApiUrl + "NewsBulletinApi/GetNewsBulletinById?id=" + id,
                ApplicationConstant.MsgScope,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<NewsBulletinDTO>(ServiceResultType.Error, httpStatusCode, errorMessage);
                }, true);
            if (result.success && result.Result != null)
            {
                return result.Result;
            }

            return null;
        }
        #endregion

        #region 站内信消息
        /// <summary>
        /// 获取最新的用户消息
        /// </summary>
        /// <param name="userId">用户的UserId</param>
        /// <param name="status">消息的状态值：0-未读；1-已读；2-已删除</param>
        /// <returns></returns>
        public List<MemberRemindMessageDTO> LoadTop10UserMessages(string userid, MessageStatus? status)
        {
            var s = status.HasValue ? ((int)status.Value).ToString() : string.Empty;
            ServiceResult <List<MemberRemindMessageDTO>> result = null;
            WebSendGet<ServiceResult<List<MemberRemindMessageDTO>>>(
                ServiceName + ".LoadTop10UserMessages",
                MessageApiUrl + "MessageApi/LoadTop10UserMessages?userId=" + userid + "&status=" + s,
                ApplicationConstant.MsgScope,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<List<MemberRemindMessageDTO>>(ServiceResultType.Error, httpStatusCode, errorMessage);
                }, true);
            if (result.success && result.Result != null)
            {
                return result.Result;
            }

            return null;
        }

        /// <summary>
        /// 获取消息的分页数据
        /// </summary>
        /// <param name="pageIndex">分页页码</param>
        /// <param name="pageSize">分页大小（不能超过100）</param>
        /// <param name="userId">用户Id，必填项</param>
        /// <param name="title">消息的标题</param>
        /// <param name="status">消息的状态</param>
        /// <returns>消息的分页数据</returns>
        public PaginatedBaseDTO<MemberRemindMessageDTO> LoadPaginatedRemindMessages(int pageIndex, int pageSize, string userId, string title, MessageStatus? status)
        {
            ServiceResult<PaginatedBaseDTO<MemberRemindMessageDTO>> result = null;
            var s = status.HasValue ? ((int)status.Value).ToString() : string.Empty;
            WebSendGet<ServiceResult<PaginatedBaseDTO<MemberRemindMessageDTO>>>(
                ServiceName + ".LoadPaginatedRemindMessages",
                MessageApiUrl + "NewsBulletinApi/LoadPaginatedRemindMessages?pageIndex=" + pageIndex + "&pageSize=" + pageSize + "&userId=" + userId + "&title=" + title + "&status=" + s,
                ApplicationConstant.MsgScope,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<PaginatedBaseDTO<MemberRemindMessageDTO>>(ServiceResultType.Error, httpStatusCode, errorMessage);
                }, true);
            if (result.success && result.Result != null)
            {
                return result.Result;
            }

            return null;
        }

        /// <summary>
        /// 获取消息的详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public MemberRemindMessageDTO GetRemindMessageById(int id)
        {
            ServiceResult<MemberRemindMessageDTO> result = null;
            WebSendGet<ServiceResult<MemberRemindMessageDTO>>(
                ServiceName + ".GetRemindMessageById",
                MessageApiUrl + "NewsBulletinApi/GetRemindMessageById?id=" + id,
                ApplicationConstant.MsgScope,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<MemberRemindMessageDTO>(ServiceResultType.Error, httpStatusCode, errorMessage);
                }, true);
            if (result.success && result.Result != null)
            {
                return result.Result;
            }

            return null;
        }

        /// <summary>
        /// 消息已读
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool ReadRemindMessage(int id)
        {
            ServiceResult<bool> result = null;
            WebSendGet<ServiceResult<bool>>(
                ServiceName + ".ReadRemindMessage",
                MessageApiUrl + "MessageApi/ReadRemindMessage?id=" + id,
                ApplicationConstant.MsgScope,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<bool>(ServiceResultType.Error, httpStatusCode, errorMessage);
                }, true);

            if (result.success)
            {
                return result.Result;
            }

            return false;
        }
        #endregion

    }
}
