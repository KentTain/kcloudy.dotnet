
using KC.Service.Message;
using KC.Service.WebApiService;
using KC.Service.WebApiService.Business;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace KC.Service.Util
{
    public static class DependencyInjectUtil
    {
        public static void InjectService(IServiceCollection services)
        {
            services.AddSingleton<ITenantUserApiService, TenantUserApiService>();
            services.AddSingleton<IApplicationApiService, ApplicationApiService>();
            services.AddSingleton<IGlobalConfigApiService, GlobalConfigApiService>();
            services.AddSingleton<IOAuth2ClientCommonService, OAuth2ClientCommonService>();

            services.AddScoped<IAccountApiService, AccountApiService>();
            services.AddScoped<IMessageApiService, MessageApiService>();
            services.AddScoped<IConfigApiService, ConfigApiService>();
            services.AddScoped<IDictionaryApiService, DictionaryApiService>();
            services.AddScoped<IWorkflowApiService, WorkflowApiService>();
            services.AddScoped<ITestComApiService, TestComApiService>();

            #region 通用消息处理：MessageUtil
            // 使用实例：public TodoController(Func<Type, IMessageGenerator> funcFactory) 
            //         { var foo1 = funcFactory(typeof(CommonMessageGenerator));
            //           var foo2 = funcFactory(typeof(UserTemplateGenerator)); }
            services.AddScoped<IMessageGenerator, CommonMessageGenerator>();
            //services.AddSingleton<UserTemplateGenerator>();
            //services.AddSingleton<ProjectTemplateGenerator>();
            //services.AddTransient(serviceProvider =>
            //{
            //    Func<Type, IMessageGenerator> accesor = key =>
            //    {
            //        if (key == typeof(CommonMessageGenerator))
            //            return serviceProvider.GetService<CommonMessageGenerator>();
            //        else if (key == typeof(UserTemplateGenerator))
            //            return serviceProvider.GetService<UserTemplateGenerator>();
            //        else if (key == typeof(ProjectTemplateGenerator))
            //            return serviceProvider.GetService<ProjectTemplateGenerator>();
            //        else
            //            throw new ArgumentException($"不支持的DI Key: {key}");
            //    };
            //    return accesor;
            //});

            services.AddScoped(typeof(MessageUtil));
            #endregion

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
