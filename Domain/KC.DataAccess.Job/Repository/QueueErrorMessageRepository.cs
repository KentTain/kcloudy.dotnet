using System;
using System.Linq;
using KC.Framework.Util;
using KC.Framework.Tenant;
using KC.Model.Job.Table;
using KC.Job.IRepository;
using KC.Database.EFRepository;
using System.Collections.Generic;
using KC.Framework.Base;

namespace KC.DataAccess.Job.Repository
{
    public class QueueErrorMessageRepository : EFRepositoryBase<QueueErrorMessage>, IQueueErrorMessageRepository
    {
        public QueueErrorMessageRepository(EFUnitOfWorkContextBase unitOfWork)
            : base(unitOfWork)
        {
        }

        public bool Add(QueueErrorMessage model)
        {
            return base.Add(model);
        }

        public override List<QueueErrorMessage> FindAll()
        {
            return base.FindAll().ToList();
        }

        public bool RemoveByRowKey(string rowKey)
        {
            return base.RemoveById(rowKey);
        }

        public PaginatedBase<QueueErrorMessage> FindPagenatedListWithCount(int pageIndex, int pageSize, string queueName, string sort, string order)
        {
            var data = base.FindPagenatedListWithCount<QueueErrorMessage>(pageIndex, pageSize, m => m.QueueName == queueName, sort, order.Equals("asc"));
            var total = data.Item1;
            var rows = data.Item2.Select(m => m as QueueErrorMessage).ToList();

            return new PaginatedBase<QueueErrorMessage>(pageIndex, pageSize, total, rows);
        }

        public QueueErrorMessage FindByRowKey(string rowKey)
        {
            return base.GetById(rowKey);
        }
    }
}
