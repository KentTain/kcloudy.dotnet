using KC.DataAccess.Offering;
using KC.DataAccess.Offering.Repository;
using KC.Database.EFRepository;
using KC.Database.IRepository;
using KC.Framework.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace KC.Service.Offering
{
    public static class DependencyInjectUtil
    {
        public static void InjectService(IServiceCollection services)
        {
            services
                //.AddEntityFrameworkSqlServer()
                //.AddScoped<IMigrationsSqlGenerator, SqlServerSchemaAwareMigrationSqlGenerator>()
                //.AddScoped<IModelCacheKeyFactory, MultiTenantModelCacheKeyFactory>()
                .AddDbContext<ComOfferingContext>(options =>
                    {
                        //解决跟踪同一个ID问题
                        options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
                        options.UseSqlServer(GlobalConfig.GetDecryptDatabaseConnectionString());
                    });

            services.AddTransient(typeof(EFUnitOfWorkContextBase), typeof(ComOfferingUnitOfWorkContext));

            services.AddScoped(typeof(IDbRepository<>), typeof(CommonEFRepository<>));
            services.AddScoped(typeof(IDbTreeRepository<>), typeof(CommonEFTreeRepository<>));

            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IOfferingRepository, OfferingRepository>();

            services.AddTransient<IOfferingService, OfferingService>();
            services.AddTransient<IPropertyProviderService, PropertyProviderService>(); 
        }
    }
}
