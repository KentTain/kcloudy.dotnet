using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using KC.Framework.Extension;
using KC.Database.EFRepository;
using KC.Model.Portal;
using Microsoft.EntityFrameworkCore;

namespace KC.DataAccess.Portal.Repository
{
    public class CompanyAuthenticationRepository : EFRepositoryBase<CompanyAuthentication>, ICompanyAuthenticationRepository
    {
        public CompanyAuthenticationRepository(EFUnitOfWorkContextBase unitOfWork)
           : base(unitOfWork)
        {
        }

        public async Task<CompanyAuthentication> GetComAuthenticationAsync()
        {
            var data = await EFContext.Set<CompanyAuthentication>()
                .Include(m => m.CompanyInfo)
                .Where(m => !m.IsDeleted)
                .AsNoTracking()
                .FirstOrDefaultAsync();
            return data;
        }


    }
}
