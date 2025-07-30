using System;
using System.Linq;
using KC.Framework.Tenant;
using KC.Component.FileRespository;
using KC.Model.Job.Table;
using KC.Model.Component.Table;
using KC.Job.IRepository;

namespace KC.Component.Repository.File
{
    public class DefaultThreadConfigRepository : CommonFileRepository<ThreadConfigInfoTable>, IThreadConfigRepository
    {
        public DefaultThreadConfigRepository()
            : base("ThreadConfigInfo")
        {
        }

        public DefaultThreadConfigRepository(Tenant tenant)
            : base(tenant)
        {
        }

        public ThreadConfigInfo GetWorkerRoleConfigToRun(string workerRoleId, string hostName)
        {
            DateTime utcNow = DateTime.UtcNow;
            var workerRoleConfiguration = new ThreadConfigInfoTable();
            workerRoleConfiguration.WorkerRoleId = workerRoleId;
            workerRoleConfiguration.HostName = hostName;
            workerRoleConfiguration.EnableLiveBackup = false;
            workerRoleConfiguration.EnableEmailService = true;
            workerRoleConfiguration.EnableNotificationService = true;
            workerRoleConfiguration.CreateTime = utcNow;
            workerRoleConfiguration.ModifiedDate = utcNow;
            workerRoleConfiguration.LastAccessTime = utcNow;
            workerRoleConfiguration.LastModifyTime = utcNow;

            try
            {
                StringComparison noCase = StringComparison.OrdinalIgnoreCase;

                var workerRoleConfigs = FindAll();
                foreach (ThreadConfigInfoTable workerRoleConfig in workerRoleConfigs)
                {
                    //Lost resposne more than 15 minutes
                    if ((utcNow - workerRoleConfig.LastAccessTime).TotalSeconds > 15 * 60)
                    {
                        workerRoleConfig.EnableLiveBackup = false;
                        workerRoleConfig.EnableEmailService = false;
                        workerRoleConfig.EnableNotificationService = false;
                        this.Modify(workerRoleConfig);
                    }
                }

                DeleteUselessWorkerRoleConfig();

                workerRoleConfiguration = workerRoleConfigs.FirstOrDefault(x => x.HostName.Equals(hostName, noCase));
                if (workerRoleConfiguration == null)
                {
                    var aliveRoles = workerRoleConfigs.Where(x => x.IsEnabled && (utcNow - x.LastAccessTime).TotalSeconds <= 10 * 60);

                    workerRoleConfiguration = new ThreadConfigInfoTable();
                    workerRoleConfiguration.WorkerRoleId = workerRoleId;
                    workerRoleConfiguration.HostName = hostName;
                    workerRoleConfiguration.CreateTime = utcNow;
                    workerRoleConfiguration.ModifiedDate = utcNow;
                    workerRoleConfiguration.LastAccessTime = utcNow;
                    workerRoleConfiguration.LastModifyTime = utcNow;
                    workerRoleConfiguration.EnableLiveBackup = !aliveRoles.Any(x => x.EnableLiveBackup);
                    workerRoleConfiguration.EnableEmailService = !aliveRoles.Any(x => x.EnableEmailService);
                    workerRoleConfiguration.EnableNotificationService = !aliveRoles.Any(x => x.EnableNotificationService);
                    this.Add(workerRoleConfiguration);
                }
                else
                {
                    UpdateWorkerRoleLastAccessTime(hostName);
                }

                return workerRoleConfiguration;
            }
            catch
            {
                try
                {
                    DeleteAllWorkerRoleConfig();
                }
                catch (Exception)
                {
                    return new ThreadConfigInfoTable();
                }
            }

            return new ThreadConfigInfoTable();
        }

        public void DeleteWorkerRoleConfigInfo(string rowKey)
        {
            ThreadConfigInfoTable info = this.FindAll(t => t.RowKey == rowKey).FirstOrDefault();
            if (info != null)
            {
                this.Remove(info);
            }
        }

        public void DeleteUselessWorkerRoleConfig()
        {
            try
            {
                var workerRoleConfigs = FindAll();
                DateTime dt = DateTime.UtcNow.Subtract(new TimeSpan(0, 30, 0));
                var toBeDeleteItems = workerRoleConfigs.Where(v => v.KeepThisConfig == false && v.LastAccessTime < dt).ToList();
                if (toBeDeleteItems.Count() > 0)
                {
                    foreach (ThreadConfigInfoTable info in toBeDeleteItems)
                    {
                        this.Remove(info);
                    }
                }
            }
            catch
            {
                // do nothing on error, because there maybe has confict on concurrency.
            }
        }

        public void DeleteAllWorkerRoleConfig()
        {
            try
            {
                var workerRoleConfigs = FindAll();
                if (workerRoleConfigs.Count() > 0)
                {
                    foreach (ThreadConfigInfoTable info in workerRoleConfigs)
                    {
                        this.Remove(info);
                    }
                }
            }
            catch
            {
                // do nothing on error, because there maybe has confict on concurrency.
            }
        }

        protected void UpdateWorkerRoleLastAccessTime(string workerRoleHostName)
        {
            var config = GetWorkerRoleConfigInfoByHostName(workerRoleHostName);
            if (config != null)
            {
                config.LastAccessTime = DateTime.UtcNow;
                try
                {
                    this.Modify(config);
                }
                catch
                {
                    config = GetWorkerRoleConfigInfoByHostName(workerRoleHostName);

                    if (config != null)
                    {
                        config.LastAccessTime = DateTime.UtcNow;
                        try
                        {
                            this.Modify(config);
                        }
                        catch
                        {
                        }
                    }
                }
            }
        }

        private ThreadConfigInfoTable GetWorkerRoleConfigInfoByHostName(string workerRoleHostName)
        {
            try
            {
                return FindAll().Where(v => v.HostName == workerRoleHostName).FirstOrDefault();
            }
            catch
            {
                return null;
            }
        }
    }
}
