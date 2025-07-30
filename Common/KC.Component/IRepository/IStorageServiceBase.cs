using KC.Framework.Tenant;
using System;
using System.Collections.Generic;
using System.Text;

namespace KC.Component.IRepository
{
    public interface IStorageServiceBase
    {
        Tenant Tenant { get; set; }

    }
}
