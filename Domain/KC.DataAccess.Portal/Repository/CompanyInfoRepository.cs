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
    public class CompanyInfoRepository : EFRepositoryBase<CompanyInfo>, ICompanyInfoRepository
    {
        public CompanyInfoRepository(EFUnitOfWorkContextBase unitOfWork)
           : base(unitOfWork)
        {
        }

        public async Task<CompanyInfo> GetCompanyDetailInfoAsync()
        {
            var data = await EFContext.Set<CompanyInfo>()
                .Include(m => m.CompanyAccounts)
                .Include(m => m.CompanyContacts)
                .Include(m => m.CompanyAddresses)
                .Where(m => true)
                .AsNoTracking()
                .FirstOrDefaultAsync();
            if (data == null)
                return new CompanyInfo();
            return data;
        }


    }
}
