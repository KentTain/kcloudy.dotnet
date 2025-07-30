using System;
using System.Collections.Generic;
using System.Threading;
using KC.Framework.Util;
using KC.Job.Core;
using Microsoft.Extensions.DependencyInjection;

namespace KC.Job.Basic.Jobs
{
    public class SampleJob : WorkerRoleJobBase
    {
        public const string THREAD_NAME = "KC.Job.Basic.Jobs.SampleJob";
        public const int DEFAULT_SLEEP_TIME = 24 * 60 * 60 * 1000;//每天间隔时间
        public static DateTime BEGINRUN_TIME = new DateTime(2021, 2, 1, 08, 20, 00, DateTimeKind.Utc);

        public SampleJob(ThreadStatusUpdater threadStatusUpdater, string workerRoleId, IServiceProvider serviceProvider)
            : base(threadStatusUpdater, workerRoleId, THREAD_NAME, DEFAULT_SLEEP_TIME, BEGINRUN_TIME.AddHours(-8), serviceProvider)
        {
        }

        protected override bool? RunOneJob()
        {
            //启动不执行，到开始时间后才执行
            if (!this.IsRunFirstly)
                return true;

            var msg = string.Format("Thread Id=" + Thread.CurrentThread.ManagedThreadId + 
                "----begin execute job:【{0}】--Delay Running Seconds: 【{1}】", 
                DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), 
                SecondToTime(DEFAULT_SLEEP_TIME / 1000));
            LogUtil.LogInfo(msg);
            bool success = true;
            LogUtil.LogInfo("End to call job: " + THREAD_NAME + ".................");
            
            return success;
        }

        public void SetConversionAuthToken()
        {
        }
    }
}
