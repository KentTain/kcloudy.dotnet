using System;
using System.Threading;

namespace KC.Job.Core
{
    public interface IJob
    {
        TimeSpan ThreadRunningDuration { get; }
        bool IsThreadRunning { get; }

        void OnStart();
        void RunOneJobUseThread();
        void AbortJobThread();
        void AfterThreadAbort(ThreadAbortException ex);
        void OnStop();
    }
}
