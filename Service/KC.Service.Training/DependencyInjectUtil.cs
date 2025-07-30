using KC.DataAccess.Account.Repository;
using KC.DataAccess.Training;
using KC.DataAccess.Training.Repository;
using KC.Database.EFRepository;
using KC.Database.IRepository;
using KC.Framework.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace KC.Service.Training
{
    public static class DependencyInjectUtil
    {
        public static void InjectService(IServiceCollection services)
        {
            services
                //.AddEntityFrameworkSqlServer()
                //.AddScoped<IMigrationsSqlGenerator, SqlServerSchemaAwareMigrationSqlGenerator>()
                //.AddScoped<IModelCacheKeyFactory, MultiTenantModelCacheKeyFactory>()
                //.AddScoped<IMigrationsSqlGenerator, SqlServerSchemaAwareMigrationSqlGenerator>()
                //.AddScoped<IModelCacheKeyFactory, MultiTenantModelCacheKeyFactory>()
                .AddDbContext<ComTrainingContext>(options =>
                    {
                        //解决跟踪同一个ID问题
                        options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
                        options.UseSqlServer(GlobalConfig.GetDecryptDatabaseConnectionString());
                    }, ServiceLifetime.Scoped, ServiceLifetime.Scoped);

            services.AddScoped(typeof(EFUnitOfWorkContextBase), typeof(ComTrainingUnitOfWorkContext));

            services.AddScoped(typeof(IDbRepository<>), typeof(CommonEFRepository<>));
            services.AddTransient(typeof(IDbTreeRepository<>), typeof(CommonEFTreeRepository<>));

            services.AddScoped<ICourseRepository, CourseRepository>();
            services.AddScoped<ITeacherRepository, TeacherRepository>();

            services.AddTransient<ICourseService, CourseService>();
            services.AddTransient<ITeacherService, TeacherService>();
        }
    }
}
