using KC.Framework.Base;
using KC.Model.Training;
using System.Collections.Generic;

namespace KC.DataAccess.Training.Repository
{
    public interface ITeacherRepository : Database.IRepository.IDbRepository<Teacher>
    {
        IList<Teacher> GetTeachersByName(string name);
    }
}