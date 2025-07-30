using KC.Model.Job.Table;

namespace KC.Job.IRepository
{
    public interface IDatabaseVersionInfoRepository
    {
        int GetCurrentVersion(string databaseName);
        bool IsUpgrading(string databaseName);
        void UpdateDatabaseStatus(string databaseName, DatabaseStatus status);
        void UpdateDatabaseVersion(string databaseName, int versionNumber);
    }
}