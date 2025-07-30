
using KC.Job.Core;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace KC.Service.Component
{
    public static class DependencyInjectUtil
    {
        public static void InjectService(IServiceCollection services)
        {
            services.AddTransient<IThreadService, ThreadNoSqlService>(); 
            services.AddScoped<IStorageQueueService, StorageQueueService>();

            services.AddScoped<ITopicService, TopicService>();
            services.AddScoped<ISubscriptionService, SubscriptionService>();

        }
    }
}
