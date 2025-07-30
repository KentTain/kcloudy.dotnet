using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.Database.EFRepository;
using KC.Framework.Tenant;
using Microsoft.EntityFrameworkCore;

namespace KC.DataAccess.Training
{
    public class ComTrainingUnitOfWorkContext : EFUnitOfWorkContextBase
    {
        public ComTrainingUnitOfWorkContext(ComTrainingContext context)
        {
            ComConfigContext = context;
        }

        //public ComConfigUnitOfWorkContext(Tenant tenant)
        //{
        //    ComConfigContext = new ComConfigDatabaseInitializer().Create(tenant);
        //}

        /// <summary>
        ///     获取或设置 当前使用的数据访问上下文对象
        /// </summary>
        public override DbContext Context
        {
            get { return ComConfigContext; }
        }

        /// <summary>
        ///     获取或设置 默认的CFWinSupplyChain项目数据访问上下文对象
        /// </summary>
        //[Import(typeof(DbContext))]
        protected ComTrainingContext ComConfigContext { get; private set; }
    }
}
