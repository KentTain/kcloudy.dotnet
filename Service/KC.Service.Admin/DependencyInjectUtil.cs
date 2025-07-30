using KC.DataAccess.Admin;
using KC.DataAccess.Admin.Repository;
using KC.Database.EFRepository;
using KC.Database.IRepository;
using KC.Framework.Base;
using KC.Service.Admin.WebApiService;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace KC.Service.Admin
{
    public static class DependencyInjectUtil
    {
        public static void InjectService(IServiceCollection services)
        {
            services
                //.AddEntityFrameworkSqlServer()
                .AddDbContext<ComAdminContext>(options =>
                {
                    //解决跟踪同一个ID问题
                    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
                    options.UseSqlServer(GlobalConfig.GetDecryptDatabaseConnectionString());
                });

            services.AddSingleton<IJobApiService, JobApiService>();

            services.AddTransient(typeof(EFUnitOfWorkContextBase), typeof(ComAdminUnitOfWorkContext));

            services.AddScoped(typeof(IDbRepository<>), typeof(CommonEFRepository<>));
            services.AddScoped(typeof(IDbTreeRepository<>), typeof(CommonEFTreeRepository<>));

            services.AddScoped<ITenantUserRepository, TenantUserRepository>();
            services.AddScoped<ITenantUserApplicationRepository, TenantUserApplicationRepository>();
            services.AddScoped<ITenantUserOpenAppErrorLogRepository, TenantUserOpenAppErrorLogRepository>();
            services.AddScoped<ITenantUserOperationLogRepository, TenantUserOperationLogRepository>();

            services.AddScoped<IDatabasePoolService, DatabasePoolService>();
            services.AddScoped<IStoragePoolService, StoragePoolService>();
            services.AddScoped<INoSqlPoolService, NoSqlPoolService>();
            services.AddScoped<IQueuePoolService, QueuePoolService>();
            services.AddScoped<IVodPoolService, VodPoolService>();
            services.AddScoped<ICodePoolService, CodePoolService>();
            services.AddScoped<IServiceBusPoolService, ServiceBusPoolService>();

            services.AddScoped<ITenantUserService, TenantUserService>();

            // 实现两个接口的类的注入方式： class Foo ：IFoo，IBar {}
            //services.AddSingleton<Foo>();
            //services.AddSingleton<IFoo>(x => x.GetRequiredService<Foo>()); // Forward requests to Foo
            //services.AddSingleton<IBar>(x => x.GetRequiredService<Foo>()); // Forward requests to Foo

            //实现一个接口多个实现的类的注入方式：
            //class Foo1: IFoo {}   class Foo2: IFoo {}
            //services.AddTransient<Foo1>();
            //services.AddTransient<Foo2>();
            //services.AddTransient(serviceProvider =>
            //{
            //    Func<Type, IFoo> accesor = key =>
            //    {
            //        if (key == typeof(Foo1))
            //            return serviceProvider.GetService<Foo1>();
            //        else if (key == typeof(Foo2))
            //            return serviceProvider.GetService<Foo2>();
            //        else
            //            throw new ArgumentException($"不支持的DI Key: {key}");
            //    };
            //    return accesor;
            //});
        }
    }
}
