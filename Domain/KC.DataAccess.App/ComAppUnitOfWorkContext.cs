using KC.Database.EFRepository;
using KC.Framework.Tenant;
using Microsoft.EntityFrameworkCore;

namespace KC.DataAccess.App
{
    public class ComAppUnitOfWorkContext : EFUnitOfWorkContextBase
    {
        public ComAppUnitOfWorkContext(ComAppContext context)
        {
            ComDbContext = context;
        }

        //public ComAppUnitOfWorkContext()
        //{
        //    ComDbContext = new ComAppDatabaseInitializer().Create(TenantConstant.DbaTenantApiAccessInfo);
        //}

        /// <summary>
        ///     获取或设置 当前使用的数据访问上下文对象
        /// </summary>
        public override DbContext Context
        {
            get { return ComDbContext; }
        }

        /// <summary>
        ///     获取或设置 默认的CFWinSupplyChain项目数据访问上下文对象
        /// </summary>
        //[Import(typeof(DbContext))]
        protected ComAppContext ComDbContext { get; private set; }
    }
}
