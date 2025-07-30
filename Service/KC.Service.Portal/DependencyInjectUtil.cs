using KC.DataAccess.Portal;
using KC.DataAccess.Portal.Repository;
using KC.Database.EFRepository;
using KC.Database.IRepository;
using KC.Framework.Base;
using KC.Service.Portal.WebApiService;
using KC.Service.Portal.WebApiService.Business;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace KC.Service.Portal
{
    public static class DependencyInjectUtil
    {
        public static void InjectService(IServiceCollection services)
        {
            services
                //.AddEntityFrameworkSqlServer()
                //.AddScoped<IMigrationsSqlGenerator, SqlServerSchemaAwareMigrationSqlGenerator>()
                //.AddScoped<IModelCacheKeyFactory, MultiTenantModelCacheKeyFactory>()
                .AddDbContext<ComPortalContext>(options =>
                    {
                        //解决跟踪同一个ID问题
                        options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
                        options.UseSqlServer(GlobalConfig.GetDecryptDatabaseConnectionString());
                    });

            services.AddTransient(typeof(EFUnitOfWorkContextBase), typeof(ComPortalUnitOfWorkContext));

            services.AddScoped(typeof(IDbRepository<>), typeof(CommonEFRepository<>));
            services.AddScoped(typeof(IDbTreeRepository<>), typeof(CommonEFTreeRepository<>));

            services.AddScoped<IWebSiteInfoRepository, WebSiteInfoRepository>();
            services.AddScoped<IWebSitePageRepository, WebSitePageRepository>();
            services.AddScoped<IWebSiteColumnRepository, WebSiteColumnRepository>();

            services.AddScoped<ICompanyInfoRepository, CompanyInfoRepository>();
            services.AddScoped<ICompanyAuthenticationRepository, CompanyAuthenticationRepository>();

            services.AddScoped<IRecommendCategoryRepository, RecommendCategoryRepository>();
            services.AddScoped<IRecommendCustomerRepository, RecommendCustomerRepository>();
            services.AddScoped<IRecommendOfferingRepository, RecommendOfferingRepository>(); 
            services.AddScoped<IRecommendRequirementRepository, RecommendRequirementRepository>();

            services.AddSingleton<ITenantSimpleApiService, TenantSimpleApiService>();
            services.AddSingleton<INewsBulletinApiService, NewsBulletinApiService>();

            services.AddScoped<ICompanyInfoService, CompanyInfoService>();
            services.AddScoped<IWebSiteService, WebSiteService>();
            services.AddScoped<IRecommendService, RecommendService>();
            
            services.AddScoped<IFrontWebInfoService, FrontWebInfoService>();
        }
    }
}
