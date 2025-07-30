using KC.DataAccess.Customer;
using KC.DataAccess.Customer.Repository;
using KC.Database.EFRepository;
using KC.Database.IRepository;
using KC.Framework.Base;
using KC.Service.WebApiService.Business;
using KC.Service.Customer.WebApiService;
using KC.Service.Customer.WebApiService.Business;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace KC.Service.Customer
{
    public static class DependencyInjectUtil
    {
        public static void InjectService(IServiceCollection services)
        {
            services
                //.AddEntityFrameworkSqlServer()
                //.AddScoped<IMigrationsSqlGenerator, SqlServerSchemaAwareMigrationSqlGenerator>()
                //.AddScoped<IModelCacheKeyFactory, MultiTenantModelCacheKeyFactory>()
                .AddDbContext<ComCustomerContext>(options =>
                {
                    //解决跟踪同一个ID问题
                    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
                    options.UseSqlServer(GlobalConfig.GetDecryptDatabaseConnectionString());
                });

            services.AddTransient(typeof(EFUnitOfWorkContextBase), typeof(ComCustomerUnitOfWorkContext));

            services.AddScoped(typeof(IDbRepository<>), typeof(CommonEFRepository<>));
            services.AddScoped(typeof(IDbTreeRepository<>), typeof(CommonEFTreeRepository<>));

            services.AddScoped<ICustomerContactRepository, CustomerContactRepository>();
            services.AddScoped<ICustomerInfoRepository, CustomerInfoRepository>();
            services.AddScoped<ICustomerSeasRepository, CustomerSeasRepository>();
            services.AddScoped<ICustomerManagerRepository, CustomerManagerRepository>();
            services.AddScoped<ICustomerTracingLogRepository, CustomerTracingLogRepository>();

            services.AddSingleton<ITenantSimpleApiService, TenantSimpleApiService>();
            services.AddSingleton<IDictionaryApiService, DictionaryApiService>();
            services.AddSingleton<ICustomerApiService, CustomerApiService>();

            services.AddTransient<ICustomerService, CustomerService>();
            services.AddTransient<ICustomerSeaService, CustomerSeaService>();

            // 实现两个接口的类的注入方式： class Foo ：IFoo，IBar {}
            // 使用实例：public TodoController(IFoo foo, IBar) {} 
            //services.AddSingleton<Foo>();
            //services.AddSingleton<IFoo>(x => x.GetRequiredService<Foo>()); 
            //services.AddSingleton<IBar>(x => x.GetRequiredService<Foo>()); 

            // 实现一个接口多个实现的类的注入方式：
            //  class Foo1: IFoo {}   class Foo2: IFoo {}
            // 使用实例：public TodoController(Func<Type, IFoo> funcFactory) 
            //         { var foo1 = funcFactory(typeof(Foo1));
            //           var foo2 = funcFactory(typeof(Foo2)); }
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
