using KC.Framework.Util;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace KC.Database.EFRepository
{
    public class MultiTenantModelCacheKeyFactory : ModelCacheKeyFactory
    {
        protected string _schemaName;

        public MultiTenantModelCacheKeyFactory(ModelCacheKeyFactoryDependencies dependencies)
            : base(dependencies)
        {
        }
        public override object Create(DbContext context)
        {
            var dataContext = (context as MultiTenantDataContext)?.TenantName;

            //LogUtil.LogDebug("------MultiTenantModelCacheKeyFactory: " + _schemaName);
            return new MultiTenantModelCacheKey(context);
        }
    }

    public class MultiTenantModelCacheKey : ModelCacheKey
    {
        private readonly string _schemaName;
        public MultiTenantModelCacheKey(DbContext context)
            : base(context)
        {
            _schemaName = (context as MultiTenantDataContext)?.TenantName; ;
        }

        protected override bool Equals(ModelCacheKey other)
        => base.Equals(other)
            && (other as MultiTenantModelCacheKey)?._schemaName == _schemaName;

        public override int GetHashCode()
        {
            var hashCode = base.GetHashCode() * 397;
            if (_schemaName != null)
            {
                hashCode ^= _schemaName.GetHashCode();
            }

            //LogUtil.LogDebug("------MultiTenantModelCacheKey: " + _schemaName);
            return hashCode;
        }
    }
}
