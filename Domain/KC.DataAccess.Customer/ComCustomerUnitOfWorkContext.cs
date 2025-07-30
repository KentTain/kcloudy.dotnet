using KC.Database.EFRepository;
using KC.Framework.Tenant;
using Microsoft.EntityFrameworkCore;

namespace KC.DataAccess.Customer
{
    public class ComCustomerUnitOfWorkContext : EFUnitOfWorkContextBase
    {
        public ComCustomerUnitOfWorkContext(ComCustomerContext context)
        {
            ComCustomerContext = context;
        }

        //public ComCustomerUnitOfWorkContext(Tenant tenant)
        //{
        //    ComCustomerContext = new ComCustomerDatabaseInitializer().Create(tenant);
        //}

        /// <summary>
        ///     获取或设置 当前使用的数据访问上下文对象
        /// </summary>
        public override DbContext Context
        {
            get { return ComCustomerContext; }
        }

        /// <summary>
        ///     获取或设置 默认的CFWinSupplyChain项目数据访问上下文对象
        /// </summary>
        //[Import(typeof(DbContext))]
        protected ComCustomerContext ComCustomerContext { get; private set; }
    }
}
