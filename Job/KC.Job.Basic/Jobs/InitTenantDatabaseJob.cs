using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using KC.Framework.Extension;
using KC.Framework.Util;
using KC.Job.Core;
using KC.Service.Admin;
using KC.Service.Util;
using KC.Service.WebApiService.Business;
using Microsoft.Extensions.DependencyInjection;

namespace KC.Job.Basic.Jobs
{
    public class InitTenantDatabaseJob : WorkerRoleJobBase
    {
        public const string THREAD_NAME = "InitTenantDatabaseJob";
        public const int DEFAULT_SLEEP_TIME = 24 * 24 * 60 * 60 * 1000;//24天执行一次
        public static DateTime BEGINRUN_TIME = new DateTime(2021, 1, 1, 08, 20, 00, DateTimeKind.Utc);

        private ITenantUserService _tenantService => ServiceProvider.GetService<ITenantUserService>();
        public InitTenantDatabaseJob(ThreadStatusUpdater threadStatusUpdater, string workerRoleId, IServiceProvider serviceProvider)
            : base(threadStatusUpdater, workerRoleId, THREAD_NAME, DEFAULT_SLEEP_TIME, BEGINRUN_TIME.AddHours(-8), serviceProvider)
        {
        }

        protected override bool? RunOneJob()
        {
            try
            {
                if (_tenantService == null)
                    return true;

                #region 重新更新租户数据库时，租户状态的设置(Removed: 代码放到更新租户应用时，设置应用状态为：ApplicationStatus.GeneratDataBase)
                ////根据文件：Precious\\InitDbJobStatus.txt中的设置如下：
                ////  0或是空值：需要更新所有租户下所有应用的状态为：ApplicationStatus.GeneratDataBase，然后再更新所有租户的数据库；
                ////  1：只更新租户下有应用的状态为：ApplicationStatus.GeneratDataBase的租户
                //var statusFilePath = System.AppDomain.CurrentDomain.BaseDirectory.EndsWith(@"\")
                //    ? System.AppDomain.CurrentDomain.BaseDirectory + @"Precious\InitDbJobStatus.txt"
                //    : System.AppDomain.CurrentDomain.BaseDirectory + @"\Precious\InitDbJobStatus.txt";
                //var status = Com.Common.FileHelper.FileOperate.ReadFile(statusFilePath, Encoding.ASCII);
                //if (string.IsNullOrWhiteSpace(status) || status.Equals("0", StringComparison.OrdinalIgnoreCase))
                //{
                //    var errorMsg = _tenantService.SetInitTenantsDbJobAppsStatus();
                //    if (!string.IsNullOrWhiteSpace(errorMsg))
                //    {
                //        EmailUtil.SendAdministratorMail(string.Format("[系统消息]初始化所有租户的应用状态失败消息"), errorMsg);
                //        return true;
                //    }

                //    Com.Common.FileHelper.FileOperate.WriteFile(statusFilePath, "1");
                //}
                //LogUtil.LogInfo(string.Format("----Begin to call job: {0} with file[{1}]'s status: {2}",
                //    THREAD_NAME, statusFilePath, status));
                #endregion

                var allTenants = _tenantService.FindAllNeedInitDbTenantUsers();

                //方法四：租户按不同的数据库服务器进行分配后，开启线程处理每台服务器下的租户
                var serverGroups = allTenants.GroupBy(m => m.Server);
                foreach (var serverGroup in serverGroups)
                {
                    var groupNum = serverGroup.Count() / 3 + serverGroup.Count() % 3;
                    foreach (var avgServerGroup in serverGroup.GetAverageGroupList(groupNum))
                    {
                        var t = new Thread(GeneratDataBase);
                        var tenantIds = avgServerGroup.Select(m => m.TenantId).ToList();
                        var tenantNames = avgServerGroup.Select(m => m.TenantName).ToList();
                        t.Start(new ThreadParams() { TenantIds = tenantIds, TenantNames = tenantNames });
                    }
                }

                ////方法三：租户按不同的数据库服务器下的数据库实例进行分配后，开启线程处理各个数据库实例下的租户
                //var serverGroups = allTenants.GroupBy(m => m.Server);
                //foreach (var serverGroup in serverGroups)
                //{
                //    var dbGroups = serverGroup.ToList().GroupBy(m => m.Database);
                //    foreach (var dbGroup in dbGroups)
                //    {
                //        var t = new Thread(GeneratDataBase);
                //        var tenantIds = dbGroup.Select(m => m.TenantId).ToList();
                //        var tenantNames = dbGroup.Select(m => m.TenantName).ToList();
                //        t.Start(new ThreadParams() { TenantIds = tenantIds, TenantNames = tenantNames, TenantService = _tenantService });
                //    }
                //}

                ////方法二：将每100个租户分配为一组，开启线程处理100个租户
                //var avGroupTenants =
                //    allTenants.OrderBy(m => m.Server)
                //        .ThenBy(m => m.Database)
                //        .GetAverageGroupList(100);
                //foreach (var groupTenants in avGroupTenants)
                //{
                //    var t = new Thread(GeneratDataBase);
                //    var tenantIds = groupTenants.Select(m => m.TenantId).ToList();
                //    var tenantNames = groupTenants.Select(m => m.TenantName).ToList();
                //    t.Start(new ThreadParams() { TenantIds = tenantIds, TenantNames = tenantNames, TenantService = _tenantService });
                //}

                ////方法一：一个线程处理所有的租户
                //LogUtil.LogInfo("Thread Id=" + Thread.CurrentThread.ManagedThreadId + "： Begin to call job: " + THREAD_NAME);
                //var errors = _tenantService.UpgradeAllTenantDatabase();
                //if (errors.Any())
                //{
                //    var message = "更新数据库出错，错误消息：" + string.Join(". " + Environment.NewLine, errors);
                //    LogUtil.LogError(message);
                //    EmailUtil.SendAdministratorMail("[系统消息]更新数据库出错", message);
                //}

                //LogUtil.LogDebug("----End to call job: " + THREAD_NAME);

                return true;
            }
            catch (Exception ex)
            {
                var errorMsg = string.Format(THREAD_NAME + "--Thread Id={0}: {1} throw exception: {2}, {3}.",
                    Thread.CurrentThread.ManagedThreadId, THREAD_NAME, ex.Message, ex.StackTrace);
                LogUtil.LogError(errorMsg);
                EmailUtil.SendAdministratorMail("[系统消息]更新Tenant相关的数据库出错", errorMsg);

                return false;
            }
        }

        private void GeneratDataBase(object threadParams)
        {
            var tenantParams = threadParams as ThreadParams;
            if (tenantParams != null)
            {
                var tenants = tenantParams.TenantNames.ToCommaSeparatedString();
                LogUtil.LogInfo(THREAD_NAME + "--Thread Id=" + Thread.CurrentThread.ManagedThreadId +
                    " begin to initialize the tenants: " + tenants);

                var watch = new Stopwatch();
                watch.Start();

                var tenantService = ServiceProvider.GetService<ITenantUserService>();
                var errors = tenantService.UpgradeTenantDatabaseByIds(tenantParams.TenantIds);
                if (errors.Any())
                {
                    //tenantService.UpgradeTenantDatabaseByIds方法中已经添加了日志记录：AddOpenAppErrorLog
                    var message = "更新数据库出错，错误消息：" + string.Join(". " + Environment.NewLine, errors);
                    LogUtil.LogError(message);
                    //EmailUtil.SendAdministratorMail("[系统消息]更新数据库出错", message);
                }
                watch.Stop();
                LogUtil.LogInfo(THREAD_NAME + "--Thread Id=" + Thread.CurrentThread.ManagedThreadId +
                    " end to initialize the tenants: " + tenants + " by time: " + watch.ElapsedMilliseconds);
            }
        }
    }

    public class ThreadParams
    {
        public List<int> TenantIds { get; set; } 
        public List<string> TenantNames { get; set; }
    }
}
