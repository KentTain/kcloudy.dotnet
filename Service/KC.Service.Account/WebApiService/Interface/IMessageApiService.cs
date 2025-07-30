using KC.Service.DTO;
using KC.Service.DTO.Message;
using KC.Service.Enums.Message;
using System.Collections.Generic;

namespace KC.Service.Account.WebApiService
{
    public interface IMessageApiService
    {
        /// <summary>
        /// 获取最新的新闻公告
        /// </summary>
        /// <returns></returns>
        List<NewsBulletinDTO> LoadLatestNewsBulletins(NewsBulletinType? type);
        /// <summary>
        /// 获取新闻公告的分页数据
        /// </summary>
        /// <param name="pageIndex">分页页码</param>
        /// <param name="pageSize">分页大小（不能超过100）</param>
        /// <param name="categoryId">所属分类</param>
        /// <param name="title">新闻公告的标题</param>
        /// <returns>新闻公告的分页数据</returns>
        PaginatedBaseDTO<NewsBulletinDTO> LoadPaginatedNewsBulletins(int pageIndex, int pageSize, int? categoryId, string title);
        /// <summary>
        /// 获取新闻公告的详情
        /// </summary>
        /// <param name="id">新闻公告的Id号</param>
        /// <returns>新闻公告的详情</returns>
        NewsBulletinDTO GetNewsBulletinById(int id);


        /// <summary>
        /// 获取最新的用户消息
        /// </summary>
        /// <param name="userId">用户的UserId</param>
        /// <param name="status">消息的状态值：0-未读；1-已读；2-已删除</param>
        /// <returns></returns>
        List<MemberRemindMessageDTO> LoadTop10UserMessages(string userid, MessageStatus? status);
        /// <summary>
        /// 获取消息的分页数据
        /// </summary>
        /// <param name="pageIndex">分页页码</param>
        /// <param name="pageSize">分页大小（不能超过100）</param>
        /// <param name="userId">用户Id，必填项</param>
        /// <param name="title">消息的标题</param>
        /// <param name="status">消息的状态</param>
        /// <returns>消息的分页数据</returns>
        PaginatedBaseDTO<MemberRemindMessageDTO> LoadPaginatedRemindMessages(int pageIndex, int pageSize, string userId, string title, MessageStatus? status);
        /// <summary>
        /// 获取消息的详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        MemberRemindMessageDTO GetRemindMessageById(int id);
        /// <summary>
        /// 消息已读
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool ReadRemindMessage(int id);
    }
}