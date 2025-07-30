using System;
using System.Linq;
using KC.Framework.Util;
using KC.Framework.Tenant;
using KC.Model.Component.Table;
using KC.Component.NoSqlRepository;
using KC.Job.IRepository;
using KC.Model.Job.Table;

namespace KC.Component.Repository.Table
{
    public class DatabaseVersionInfoDataContext : CommonTableServiceRepository<DatabaseVerInfoTable>, IDatabaseVersionInfoRepository
    {
        public DatabaseVersionInfoDataContext()
            : base()
        {

        }
        public DatabaseVersionInfoDataContext(Tenant tenant)
            : base(tenant)
        {

        }

        public int GetCurrentVersion(string databaseName)
        {
            int result = 0;
            var databaseVersion = base.FindAll(m => m.DatabaseName == databaseName).FirstOrDefault();
            if (databaseVersion != null)
            {
                result = databaseVersion.VersionNumber;
            }
            else
            {
                base.Add(new DatabaseVerInfoTable()
                {
                    DatabaseName = databaseName,
                    VersionNumber = 0,
                    Status = DatabaseStatus.Draft,
                    LastModifiedDate = DateTime.UtcNow,
                });
            }

            return result;
        }

        public void UpdateDatabaseStatus(string databaseName, DatabaseStatus status)
        {
            var databaseVersion = base.FindAll(m => m.DatabaseName == databaseName).FirstOrDefault();
            if (databaseVersion != null)
            {
                databaseVersion.Status = status;
                databaseVersion.LastModifiedDate = DateTime.UtcNow;
            }

            base.Modify(databaseVersion);
        }

        public void UpdateDatabaseVersion(string databaseName, int versionNumber)
        {
            var databaseVersion = base.FindAll(m => m.DatabaseName == databaseName).FirstOrDefault();
            if (databaseVersion != null)
            {
                databaseVersion.VersionNumber = versionNumber;
                databaseVersion.Status = DatabaseStatus.Finished;
                databaseVersion.LastModifiedDate = DateTime.UtcNow;
            }
            else
            {
                base.Add(new DatabaseVerInfoTable()
                {
                    DatabaseName = databaseName,
                    VersionNumber = 0,
                    Status = DatabaseStatus.Draft,
                    LastModifiedDate = DateTime.UtcNow,
                });
            }

            base.Modify(databaseVersion);
        }

        public bool IsUpgrading(string databaseName)
        {
            try
            {
                var databaseVersion = base.FindAll(m => m.DatabaseName == databaseName).FirstOrDefault();
                return databaseVersion != null && databaseVersion.Status == DatabaseStatus.InProcess;
            }
            catch (Exception ex)
            {
                LogUtil.LogException(ex);
                if (ex.Message.Contains("Invalid column name 'UpdateStatus'"))
                    return true; //be compatible with the versions without Upgrade Lock
                else
                    throw;
            }
        }

    }
}
