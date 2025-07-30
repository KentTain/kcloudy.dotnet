using KC.Framework.Util;
using KC.Framework.Tenant;
using KC.Model.Job.Table;
using KC.Model.Component.Table;
using KC.Component.NoSqlRepository;
using KC.Job.IRepository;
using System.Collections.Generic;
using System.Linq.Expressions;
using System;
using KC.Framework;
using AutoMapper;
using KC.Framework.Base;
using KC.Framework.Extension;
using System.Linq;

namespace KC.Component.Repository.Table
{
    public class QueueErrorMessageDataContext : CommonTableServiceRepository<QueueErrorMessageTable>, IQueueErrorMessageRepository
    {
        private readonly IMapper _mapper;

        public QueueErrorMessageDataContext(IMapper mapper)
            : base()
        {
            _mapper = mapper;
        }
        public QueueErrorMessageDataContext(IMapper mapper, Tenant tenant)
            : base(tenant)
        {
            _mapper = mapper;
        }

        public bool Add(QueueErrorMessage entity)
        {
            var data = _mapper.Map<QueueErrorMessageTable>(entity);
            return base.Add(data);
        }

        public PaginatedBase<QueueErrorMessage> FindPagenatedListWithCount(int pageIndex, int pageSize, string queueName, string sort, string order)
        {
            Expression<Func<QueueErrorMessageTable, bool>> predicate = m => true;
            if (queueName != null && !queueName.Trim().Equals(string.Empty) && !queueName.Equals("全部"))
            {
                predicate = predicate.And(m => m.QueueName.ToLower() == queueName.ToLower());
            }
            var data = base.FindPagenatedListWithCount(pageIndex, pageSize, predicate, sort, order.Equals("asc"));
            var total = data.Item1;
            var rows = data.Item2.Select(m => m as QueueErrorMessage).ToList();
      
            return new PaginatedBase<QueueErrorMessage>(pageIndex, pageSize, total, rows);
        }

        public bool RemoveByRowKey(string rowKey)
        {
            var model = base.FindByRowKey(rowKey);
            return base.Remove(model);
        }

        public new List<QueueErrorMessage> FindAll()
        {
            var data = base.FindAll();
            return data.Select(m => m as QueueErrorMessage).ToList();
        }

        public new QueueErrorMessage FindByRowKey(string rowKey)
        {
            return base.FindByRowKey(rowKey);
        }
    }
}
