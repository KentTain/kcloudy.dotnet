
using KC.DataAccess.Pay;
using KC.DataAccess.Pay.Repository;
using KC.Database.EFRepository;
using KC.Database.IRepository;
using KC.Framework.Base;
using KC.Service.Pay.WebApiService.Platform;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace KC.Service.Pay
{
    public static class DependencyInjectUtil
    {
        public static void InjectService(IServiceCollection services)
        {
            services.AddEntityFrameworkSqlServer()
                .AddDbContext<ComPayContext>(options =>
                    {
                        //解决跟踪同一个ID问题
                        options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
                        options.UseSqlServer(GlobalConfig.GetDecryptDatabaseConnectionString());
                    });

            services.AddTransient(typeof(EFUnitOfWorkContextBase), typeof(ComPayUnitOfWorkContext));

            services.AddScoped(typeof(IDbRepository<>), typeof(CommonEFRepository<>));
            services.AddTransient(typeof(IDbTreeRepository<>), typeof(CommonEFTreeRepository<>));

            //services.AddSingleton<PaymentFactoryUtil>();

            services.AddTransient<IBankAccountRepository, BankAccountRepository>();
            services.AddTransient<IPaymentBankAccountRepository, PaymentBankAccountRepository>();
            services.AddTransient<IPayableRepository, PayableRepository>();
            services.AddTransient<IReceivableRepository, ReceivableRepository>();

            services.AddTransient<IPayableService, PayableService>();
            services.AddTransient<IPaymentRecordService, PaymentRecordService>();
            services.AddTransient<IReceivableService, ReceivableService>();
            services.AddTransient<IPaymentService, PaymentService>();
            services.AddTransient<IOfflinePaymentService, OfflinePaymentService>();
            services.AddTransient<IOfflineUsageBillService, OfflineUsageBillService>();

            services.AddTransient<IPaymentApiService, PaymentApiService>();
            services.AddTransient<IFinanceApiService, FinanceApiService>();
        }
    }
}
