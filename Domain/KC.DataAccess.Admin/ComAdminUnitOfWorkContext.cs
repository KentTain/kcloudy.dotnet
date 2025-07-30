using KC.Database.EFRepository;
using KC.Framework.Tenant;
using Microsoft.EntityFrameworkCore;

namespace KC.DataAccess.Admin
{
    public class ComAdminUnitOfWorkContext : EFUnitOfWorkContextBase
    {
        public ComAdminUnitOfWorkContext(ComAdminContext context)
        {
            CFWinAdminContext = context;
        }

        //public ComAdminUnitOfWorkContext()
        //{
        //    CFWinAdminContext = new ComAdminDatabaseInitializer().Create(TenantConstant.DbaTenantApiAccessInfo);
        //}

        /// <summary>
        ///     获取或设置 当前使用的数据访问上下文对象
        /// </summary>
        public override DbContext Context
        {
            get { return CFWinAdminContext; }
        }

        /// <summary>
        ///     获取或设置 默认的CFWinSupplyChain项目数据访问上下文对象
        /// </summary>
        //[Import(typeof(DbContext))]
        protected ComAdminContext CFWinAdminContext { get; private set; }
    }
}
