using System;
using System.Collections.Generic;
using System.Linq;
using KC.Framework.Tenant;
using KC.Framework.Extension;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace KC.UnitTest
{
    public abstract class TestBase<T> : IDisposable, Xunit.IClassFixture<T> where T : CommonFixture
    {
        /// <summary>
        /// Local Storage（FTP）、SupplyChainFinance（供应链金融）
        /// </summary>
        protected Tenant DbaTenant;
        /// <summary>
        /// Local Storage（Local Disk）、CommercialFactoring（商业保理）
        /// </summary>
        protected Tenant TestTenant;
        /// <summary>
        /// Azure Storage、StoreCredit（店铺赊销）
        /// </summary>
        protected Tenant BuyTenant;
        /// <summary>
        /// Aliyun OSS、StoreCredit（店铺赊销）
        /// </summary>
        protected Tenant SaleTenant;

        protected IServiceCollection Services;

        protected ILoggerFactory LoggerFactory;

        public TestBase(T data)
        {
            DbaTenant = data.DbaTenant;
            TestTenant = data.TestTenant;
            BuyTenant = data.BuyTenant;
            SaleTenant = data.SaleTenant;

            Services = data.Services;

            LoggerFactory = data.LoggerFactory;

            SetUp();
        }

        /// <summary>
        /// 每个测试用例方法执行前，都会执行一次
        /// </summary>
        protected abstract void SetUp();
        /// <summary>
        /// 每个测试用例方法执行后，都会执行一次
        /// </summary>
        protected abstract void TearDown();

        protected virtual void InjectTenant(Tenant tenant)
        {
            Services.AddTransient(serviceProvider =>
            {
                switch (tenant.TenantName)
                {
                    case TenantConstant.DbaTenantName:
                        return DbaTenant;
                    case TenantConstant.SaleTenantName:
                        return SaleTenant;
                    case TenantConstant.BuyTenantName:
                        return BuyTenant;
                    default:
                        return TestTenant;
                }
            });
        }
        protected string ToAssertableString(IDictionary<string, List<int>> dictionary)
        {
            var pairStrings = dictionary.OrderBy(p => p.Key)
                                        .Select(p => p.Key + ": " + string.Join(", ", p.Value));
            return string.Join("; ", pairStrings);
        }
        protected string ToAssertableString(IDictionary<string, List<string>> dictionary)
        {
            var pairStrings = dictionary.OrderBy(p => p.Key)
                                        .Select(p => p.Key + ": " + string.Join(", ", p.Value));
            return string.Join("; ", pairStrings);
        }

        public void Dispose()
        {
            TearDown();
        }
    }

    public class CommonTestBase : TestBase<CommonFixture>
    {
        public CommonTestBase(CommonFixture data)
            : base(data)
        {
        }

        protected override void SetUp()
        {

        }

        protected override void TearDown()
        {

        }
    }
}
