using KC.Framework;
using KC.Framework.Base;
using KC.Model.Job.Table;
using System.Collections.Generic;

namespace KC.Job.IRepository
{
    public interface IQueueErrorMessageRepository
    {
        bool Add(QueueErrorMessage model);

        List<QueueErrorMessage> FindAll();

        bool RemoveByRowKey(string rowKey);
        PaginatedBase<QueueErrorMessage> FindPagenatedListWithCount(int pageIndex, int pageSize, string queueName, string sort, string order);

        QueueErrorMessage FindByRowKey(string rowKey);

        bool RemoveAll();
    }
}