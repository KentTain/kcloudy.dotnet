using KC.Database.EFRepository;
using KC.Framework.Tenant;
using Microsoft.EntityFrameworkCore;

namespace KC.DataAccess.Job
{
    public class ComJobUnitOfWorkContext : EFUnitOfWorkContextBase
    {
        public ComJobUnitOfWorkContext(ComJobContext context)
        {
            ComDbContext = context;
        }

        //public ComDictUnitOfWorkContext()
        //{
        //    ComDbContext = new ComDictDatabaseInitializer().Create(TenantConstant.DbaTenantApiAccessInfo);
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
        protected ComJobContext ComDbContext { get; private set; }
    }
}
