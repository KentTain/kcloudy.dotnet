using KC.Framework.Base;
using KC.Model.Config;

namespace KC.DataAccess.Config.Repository
{
    public interface ISysSequenceRepository : Database.IRepository.IDbRepository<SysSequence>
    {
        SeedEntity GetSeedByType(string seedType, int step = 1);
    }
}