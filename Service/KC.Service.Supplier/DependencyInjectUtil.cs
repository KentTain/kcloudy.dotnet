using KC.DataAccess.Supplier;
using KC.DataAccess.Supplier.Repository;
using KC.Database.EFRepository;
using KC.Database.IRepository;
using KC.Framework.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace KC.Service.Supplier
{
    public static class DependencyInjectUtil
    {
        public static void InjectService(IServiceCollection services)
        {
            services
                //.AddEntityFrameworkSqlServer()
                //.AddScoped<IMigrationsSqlGenerator, SqlServerSchemaAwareMigrationSqlGenerator>()
                //.AddScoped<IModelCacheKeyFactory, MultiTenantModelCacheKeyFactory>()
                .AddDbContext<ComSupplierContext>(options => 
                    {
                        //解决跟踪同一个ID问题
                        options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
                        options.UseSqlServer(GlobalConfig.GetDecryptDatabaseConnectionString());
                    });

            services.AddTransient(typeof(EFUnitOfWorkContextBase), typeof(ComSupplierUnitOfWorkContext));

            services.AddScoped(typeof(IDbRepository<>), typeof(CommonEFRepository<>));
            services.AddTransient(typeof(IDbTreeRepository<>), typeof(CommonEFTreeRepository<>));

            services.AddScoped<IIndustryClassficationRepository, IndustryClassficationRepository>();
            services.AddScoped<IMobileLocationRepository, MobileLocationRepository>();

            services.AddTransient<IDictionaryService, DictionaryService>();
        }
    }
}
