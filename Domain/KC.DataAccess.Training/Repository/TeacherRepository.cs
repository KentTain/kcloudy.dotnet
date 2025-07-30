using KC.Database.EFRepository;
using KC.Framework.Base;
using KC.Model.Training;
using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace KC.DataAccess.Training.Repository
{
    public class TeacherRepository : EFRepositoryBase<Teacher>, ITeacherRepository
    {
        public TeacherRepository(EFUnitOfWorkContextBase unitOfWork)
            : base(unitOfWork)
        {
        }

        public IList<Teacher> GetTeachersByName(string name)
        {
            return Entities
                .Where(m => name.Contains(m.Name) && !m.IsDeleted)
                .AsNoTracking()
                .OrderBy(m => m.CreatedDate)
                .ToList();
        }
    }
}
