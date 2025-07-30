using KC.Database.EFRepository;
using KC.Framework.Base;
using KC.Model.Config;
using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Data;

namespace KC.DataAccess.Config.Repository
{
    public class SysSequenceRepository : EFRepositoryBase<SysSequence>, ISysSequenceRepository
    {
        public SysSequenceRepository(EFUnitOfWorkContextBase unitOfWork)
            : base(unitOfWork)
        {
        }

        public SeedEntity GetSeedByType(string seedType, int step = 1)
        {
            var inParameter1 = new SqlParameter("@seqname", seedType);
            var inParameter2 = new SqlParameter("@length", 5);
            var inParameter3 = new SqlParameter("@currdate", DateTime.UtcNow.AddHours(8).ToString("yyyy-MM-dd"));
            var inParameter4 = new SqlParameter("@step", step);
            var outParameter = new SqlParameter("@code", step) { Direction = ParameterDirection.Output };

            var context = UnitOfWork.Context as ComConfigContext;
            var tenantName = context.TenantName;
            var list = context.SeedEntities.FromSqlRaw(
                    $"EXECUTE [" + tenantName + "].[Utility_GetRegularDateVal] @seqname,@length,@currdate,@step,@code ",
                    inParameter1, inParameter2, inParameter3, inParameter4, outParameter).ToList();

            return list.FirstOrDefault();
        }
    }
}
