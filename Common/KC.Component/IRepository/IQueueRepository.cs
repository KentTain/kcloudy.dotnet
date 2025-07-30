using System;
using System.Threading.Tasks;
using KC.Component.Base;

namespace KC.Component.IRepository
{
    public interface IQueueRepository<T> where T : QueueEntity
    {
        /// <summary>
        /// 处理所有消息队列列表
        /// </summary>
        /// <param name="callback">处理每个消息队列的方法</param>
        /// <param name="failCallback">队列发生错误后的错误处理方法</param>
        /// <returns></returns>
        bool ProcessQueueList(Func<T, QueueActionType> callback, Action<T> failCallback);
        Task<bool> ProcessQueueListAsync(Func<T, QueueActionType> callback, Action<T> failCallback);

        /// <summary>
        /// 处理单个消息队列
        /// </summary>
        /// <param name="callback">处理单个消息队列的方法</param>
        /// <param name="failCallback">队列发生错误后的错误处理方法</param>
        /// <returns></returns>
        bool ProcessQueue(Func<T, QueueActionType> callback, Action<T> failCallback);
        Task<bool> ProcessQueueAsync(Func<T, QueueActionType> callback, Action<T> failCallback);

        /// <summary>
        /// 获取消息队列中消息的个数
        /// </summary>
        /// <returns></returns>
        long GetMessageCount();
        /// <summary>
        /// 添加条消息到队列中
        /// </summary>
        /// <param name="entity"></param>
        void AddMessage(T entity);
        /// <summary>
        /// 修改某条消息
        /// </summary>
        /// <param name="entity"></param>
        void ModifyMessage(T entity);
        /// <summary>
        /// 移除所有的消息
        /// </summary>
        void RemoveAllMessage();
        /// <summary>
        /// 移除最上方的消息
        /// </summary>
        void RemoveTopMessage();
        /// <summary>
        /// 移除最上方的消息
        /// </summary>
        void RemoveMessage(T entity);
    }
}
