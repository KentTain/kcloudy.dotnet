using KC.Framework.Util;
using System;
using System.Threading;

namespace KC.Job.Core
{
    public abstract class JobBase : IJob
    {
        private Thread _jobThread;
        private DateTime _lastThreadStartTime = DateTime.UtcNow;

        private ThreadStatusUpdater _threadStatusUpdater { get; set; }
        private string _workerRoleId { get; set; }
        private int _threadSleepMilliseconds { get; set; }
        private int _threadDelayRunningMilliseconds { get; set; }

        protected bool IsRunFirstly { get; set; }
        protected string ThreadName { get; set; }
        public TimeSpan ThreadRunningDuration { get { return DateTime.UtcNow.Subtract(_lastThreadStartTime); } }
        public bool IsThreadRunning { get { return _jobThread != null && _jobThread.IsAlive; } }

        protected JobBase(ThreadStatusUpdater threadStatusUpdater, string workerRoleId, string threadName, int threadSleepMilliseconds)
            : this(threadStatusUpdater, workerRoleId, threadName, threadSleepMilliseconds, null)
        {

        }

        protected JobBase(ThreadStatusUpdater threadStatusUpdater, string workerRoleId, string threadName, int threadSleepMilliseconds, DateTime? beginRunDateTime)
        {
            if (string.IsNullOrEmpty(threadName) || threadSleepMilliseconds <= 0)
            {
                throw new ArgumentNullException("threadStatusUpdater", "threadStatusUpdater or threadName cannot be null.");
            }

            this.IsRunFirstly = false;
            this.ThreadName = threadName;
            this._threadStatusUpdater = threadStatusUpdater;
            this._workerRoleId = workerRoleId;
            this._threadSleepMilliseconds = threadSleepMilliseconds;
            if (beginRunDateTime.HasValue)
            {
                var now = DateTime.UtcNow;
                var startDateTime = beginRunDateTime.Value;
                if (beginRunDateTime.Value < now)
                {
                    var beginDate = now.Date.AddDays(1);
                    var beginYear = beginDate.Year;
                    var beginMonth = beginDate.Month;
                    var beginDay = beginDate.Day;
                    var beginHour = beginRunDateTime.Value.Hour;
                    var beginMinute = beginRunDateTime.Value.Minute;
                    var beginSecond = beginRunDateTime.Value.Second;
                    startDateTime = new DateTime(beginYear, beginMonth, beginDay, beginHour, beginMinute, beginSecond, DateTimeKind.Utc);
                }

                this._threadDelayRunningMilliseconds = (int)(startDateTime.Subtract(now).TotalMilliseconds);
                var msg = string.Format(this.ThreadName + "----begin:【{0}】--now：【{1}】--Delay Running Seconds: 【{2}】", beginRunDateTime.Value.ToString("yyyy-MM-dd HH:mm:ss"), DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), SecondToTime(this._threadDelayRunningMilliseconds / 1000));
                LogUtil.LogDebug(msg);
            }
        }

        protected string SecondToTime(long second)
        {
            long days = second / 86400;            //转换天数
            second = second % 86400;            //剩余秒数
            long hours = second / 3600;            //转换小时
            second = second % 3600;                //剩余秒数
            long minutes = second / 60;            //转换分钟
            second = second % 60;                //剩余秒数
            if (days > 0)
            {
                return days + "天" + hours + "小时" + minutes + "分" + second + "秒";
            }
            else
            {
                return hours + "小时" + minutes + "分" + second + "秒";
            }
        }

        public virtual void OnStart()
        {
        }

        public void RunOneJobUseThread()
        {
            if (this.IsThreadRunning)
                throw new InvalidOperationException("Previous Job not finish yet, cannot start new one");

            this._jobThread = new Thread(new ThreadStart(RunOneJobThread));
            this._jobThread.Name = ThreadName;
            this._jobThread.Start();
        }

        public virtual void AbortJobThread()
        {
            if (this.IsThreadRunning)
            {
                this._jobThread.Abort();
            }
        }

        public virtual void AfterThreadAbort(ThreadAbortException ex)
        {

        }

        public virtual void OnStop()
        {
        }

        // do not throw exception in this method.
        protected abstract bool? RunOneJob();

        private void RunOneJobThread()
        {
            try
            {
                _lastThreadStartTime = DateTime.UtcNow;

                bool? success = RunOneJob();
                this.IsRunFirstly = true;
                if (success != null && _threadStatusUpdater != null)
                {
                    if (success.Value)
                        _threadStatusUpdater.AccumulateSuccessCount(ThreadName);
                    else
                        _threadStatusUpdater.AccumulateFailureCount(ThreadName);
                }
            }
            catch (ThreadAbortException ex)
            {
                LogUtil.LogException(ex);

                if (_threadStatusUpdater != null)
                    _threadStatusUpdater.AccumulateFailureCount(ThreadName);

                AfterThreadAbort(ex);
            }

            if (this._threadDelayRunningMilliseconds > 0)
            {
                //var msg = string.Format(this.ThreadName + "----sleep Seconds【{0}】to run", SecondToTime(this._threadDelayRunningMilliseconds / 1000));
                //LogUtil.LogDebug(msg);

                Thread.Sleep(this._threadDelayRunningMilliseconds);
                this._threadDelayRunningMilliseconds = 0;
            }
            else
            {
                //var msg = string.Format(this.ThreadName + "----sleep Seconds【{0}】to run", SecondToTime(this._threadSleepMilliseconds / 1000));
                //LogUtil.LogDebug(msg);

                Thread.Sleep(_threadSleepMilliseconds);
            }
        }


    }
}
