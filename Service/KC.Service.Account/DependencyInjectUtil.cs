using KC.DataAccess.Account;
using KC.DataAccess.Account.Repository;
using KC.Database.EFRepository;
using KC.Database.IRepository;
using KC.Framework.Base;
using KC.Model.Account;
using KC.Service.Account.Message;
using KC.Service.Account.WebApiService;
using KC.Service.Message;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace KC.Service.Account
{
    public static class DependencyInjectUtil
    {
        public static void InjectService(IServiceCollection services)
        {
            services
                .AddMemoryCache()
                //.AddEntityFrameworkSqlServer()
                .AddDbContext<ComAccountContext>(
                    options => 
                        options.UseSqlServer(GlobalConfig.GetDecryptDatabaseConnectionString()));

            services.AddIdentityCore<User>()
                .AddRoles<Role>()
                .AddUserManager<UserManager<User>>()
                .AddRoleManager<RoleManager<Role>>()
                .AddSignInManager<SignInManager<User>>()
                .AddEntityFrameworkStores<ComAccountContext>();

            services.AddTransient(typeof(EFUnitOfWorkContextBase), typeof(ComAccountUnitOfWorkContext));

            services.AddScoped(typeof(IDbRepository<>), typeof(CommonEFRepository<>));
            services.AddScoped(typeof(IDbTreeRepository<>), typeof(CommonEFTreeRepository<>));

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, AspNetRoleRepository>();
            services.AddScoped<IMenuNodeRepository, MenuNodeRepository>();
            services.AddScoped<IPermissionRepository, PermissionRepository>();
            services.AddScoped<IOrganizationRepository, OrganizationRepository>();
            services.AddScoped<IUserSettingRepository, UserSettingRepository>();
            
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<ISysManageService, SysManageService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IMenuService, MenuService>();
            services.AddScoped<IPermissionService, PermissionService>();
            services.AddScoped<ISettingService, SettingService>();
            
            services.AddScoped<IMessageApiService, MessageApiService>();

            //services.AddScoped<IMessageGenerator, UserTemplateGenerator>();

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
