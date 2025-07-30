using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.Component.IRepository;
using KC.Framework.Tenant;

namespace KC.Component.Repository
{
    public abstract class StorageServiceBase : IStorageServiceBase
    {
        public Tenant Tenant { get; set; }

        protected StorageServiceBase()
        {
        }

        protected StorageServiceBase(Tenant tenant)
        {
            Tenant = tenant;
        }

    }
}
