using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.Framework.Base;
using KC.Component.Base;

namespace KC.Component.IRepository
{

    public interface ITopicRepository<T, V> 
        where T : TopicEntity<V> 
        where V : EntityBase
    {
        void CreateTopic(T entity);
    }

    public interface ISubscriptionRepository<V> 
        where V : EntityBase
    {
        bool ProcessTopic(List<string> subscriptions, Func<V, bool> callback, Action<string> failCallback);
    }
}
