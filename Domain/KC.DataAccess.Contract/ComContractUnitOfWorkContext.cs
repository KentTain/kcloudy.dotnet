using KC.Database.EFRepository;
using KC.Framework.Tenant;
using Microsoft.EntityFrameworkCore;

namespace KC.DataAccess.Contract
{
    public class ComContractUnitOfWorkContext : EFUnitOfWorkContextBase
    {
        public ComContractUnitOfWorkContext(ComContractContext context)
        {
            ComDbContext = context;
        }

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
        protected ComContractContext ComDbContext { get; private set; }
    }
}
