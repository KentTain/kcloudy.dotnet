using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.Database.EFRepository;
using KC.Framework.Tenant;
using Microsoft.EntityFrameworkCore;

namespace KC.DataAccess.Account
{
    public class ComAccountUnitOfWorkContext : EFUnitOfWorkContextBase
    {
        public ComAccountUnitOfWorkContext(ComAccountContext context)
        {
            CFWinAccountContext = context;
        }

        //public ComAccountUnitOfWorkContext(Tenant tenant)
        //{
        //    CFWinAccountContext = new ComAccountDatabaseInitializer().Create(tenant);
        //}

        /// <summary>
        ///     获取或设置 当前使用的数据访问上下文对象
        /// </summary>
        public override DbContext Context
        {
            get { return CFWinAccountContext; }
        }

        /// <summary>
        ///     获取或设置 默认的CFWinSupplyChain项目数据访问上下文对象
        /// </summary>
        //[Import(typeof(DbContext))]
        protected ComAccountContext CFWinAccountContext { get; private set; }
    }
}
