using System;
using System.Collections.Generic;
using System.Threading;
using KC.Framework.Util;

namespace KC.Job.Core
{
    public class ThreadStatusUpdater : IJob
    {
        private const int STATUS_UPDATE_INTERVAL = 60000; // 1 minute
        private const string THREAD_NAME = "ThreadStatusUpdater";

        private Thread _jobThread;
        private DateTime _lastThreadStartTime = DateTime.UtcNow;

        private string _workerRoleId;
        private Dictionary<string, int> _successDictionary;
        private Dictionary<string, int> _failDictionary;
        private List<string> _threadNames;
        private Dictionary<string, int> _lastUpdateSuccessCount;
        private Dictionary<string, int> _lastUpdateFailCount;
        private bool _initialized = false;

        private static object _lock = new object();
        private const int MAX_COUNT = 2147483600;

        private IThreadService _threadStatusDataStore;

        public ThreadStatusUpdater(string workerRoleId, IThreadService threadStatusds)
        {
            _successDictionary = new Dictionary<string, int>();
            _failDictionary = new Dictionary<string, int>();
            _threadNames = new List<string>();
            _lastUpdateSuccessCount = new Dictionary<string, int>();
            _lastUpdateFailCount = new Dictionary<string, int>();
            _workerRoleId = workerRoleId;
            _threadStatusDataStore = threadStatusds;
        }

        public void RegisterThread(string threadName)
        {
            lock (_lock)
            {
                if (!_threadNames.Contains(threadName))
                {
                    _threadNames.Add(threadName);
                }
                if (!_successDictionary.ContainsKey(threadName))
                {
                    _successDictionary.Add(threadName, 0);
                }
                if (!_failDictionary.ContainsKey(threadName))
                {
                    _failDictionary.Add(threadName, 0);
                }
            }
        }

        public void UnregisterThread(string threadName)
        {
            lock (_lock)
            {
                if (_threadNames.Contains(threadName))
                {
                    _threadNames.Remove(threadName);
                }
                if (_successDictionary.ContainsKey(threadName))
                {
                    _successDictionary.Remove(threadName);
                }
                if (_failDictionary.ContainsKey(threadName))
                {
                    _failDictionary.Remove(threadName);
                }
            }
        }

        public void AccumulateSuccessCount(string threadName)
        {
            lock (_lock)
            {
                if (_successDictionary.ContainsKey(threadName))
                {
                    _successDictionary[threadName] = _successDictionary[threadName] + 1;

                    if (_successDictionary[threadName] > MAX_COUNT) // Avoid DB overflow
                    {
                        _successDictionary[threadName] = 0;
                    }
                }
            }
        }

        public void AccumulateFailureCount(string threadName)
        {
            lock (_lock)
            {
                if (_failDictionary.ContainsKey(threadName))
                {
                    _failDictionary[threadName] = _failDictionary[threadName] + 1;

                    if (_failDictionary[threadName] > MAX_COUNT) // Avoid DB overflow
                    {
                        _failDictionary[threadName] = 0;
                    }
                }
            }
        }

        private int GetCurrentSuccessCount(string threadName)
        {
            lock (_lock)
            {
                return _successDictionary.ContainsKey(threadName) ? _successDictionary[threadName] : 0;
            }
        }

        private int GetCurrentFailCount(string threadName)
        {
            lock (_lock)
            {
                return _failDictionary.ContainsKey(threadName) ? _failDictionary[threadName] : 0;
            }
        }

        private int GetLastSuccessCount(string threadName)
        {
            lock (_lock)
            {
                return _lastUpdateSuccessCount.ContainsKey(threadName) ? _lastUpdateSuccessCount[threadName] : 0;
            }
        }

        private int GetLastFailCount(string threadName)
        {
            lock (_lock)
            {
                return _lastUpdateFailCount.ContainsKey(threadName) ? _lastUpdateFailCount[threadName] : 0;
            }
        }

        public void UpdateLastSuccessCount(string threadName, int count)
        {
            lock (_lock)
            {
                if (!_lastUpdateSuccessCount.ContainsKey(threadName))
                {
                    _lastUpdateSuccessCount.Add(threadName, count);
                }
                else
                {
                    _lastUpdateSuccessCount[threadName] = count;
                }
            }
        }

        public void UpdateLastFailureCount(string threadName, int count)
        {
            lock (_lock)
            {
                if (!_lastUpdateFailCount.ContainsKey(threadName))
                {
                    _lastUpdateFailCount.Add(threadName, count);
                }
                else
                {
                    _lastUpdateFailCount[threadName] = count;
                }
            }
        }

        private void UpdateStatus()
        {
            try
            {
                if (!_initialized)
                {
                    foreach (var threadName in _threadNames)
                    {
                        _threadStatusDataStore.InitializeWorkerRoleThreadStatus(_workerRoleId, threadName);
                    }

                    _initialized = true;
                }

                _threadStatusDataStore.UpdateWorkerRoleDate(_workerRoleId);
                foreach (var threadName in _threadNames)
                {
                    int currSuccessCount = GetCurrentSuccessCount(threadName);
                    int currFailCount = GetCurrentFailCount(threadName);
                    int lastSuccessCount = GetLastSuccessCount(threadName);
                    int lastFailCount = GetLastFailCount(threadName);

                    if (currSuccessCount != lastSuccessCount || currFailCount != lastFailCount)
                    {
                        _threadStatusDataStore.UpdateWorkerRoleThreadStatus(
                            _workerRoleId, threadName, currSuccessCount, currFailCount);

                        UpdateLastSuccessCount(threadName, currSuccessCount);
                        UpdateLastFailureCount(threadName, currFailCount);
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtil.LogWarning("Unable to update thread status:" + ex.Message);
                return;
            }
        }

        public void OnStart()
        {
        }

        public TimeSpan ThreadRunningDuration
        {
            get
            {
                DateTime dtNow = DateTime.UtcNow;
                TimeSpan ts = dtNow.Subtract(_lastThreadStartTime);
                return ts;
            }
        }

        public bool IsThreadRunning
        {
            get
            {
                return _jobThread != null && _jobThread.IsAlive;
            }
        }

        public void RunOneJobUseThread()
        {
            if (IsThreadRunning)
            {
                throw new InvalidOperationException("Previous Job not finish yet, cannot start new one");
            }

            _jobThread = new Thread(new ThreadStart(RunOneJobThread));
            _jobThread.Name = "ThreadStatusUpdater";
            _jobThread.Start();
        }

        private void RunOneJobThread()
        {
            try
            {
                _lastThreadStartTime = DateTime.UtcNow;

                UpdateStatus();
            }
            catch (ThreadAbortException ex)
            {
                AfterThreadAbort(ex);
            }

            Thread.Sleep(STATUS_UPDATE_INTERVAL);
        }

        public void AbortJobThread()
        {
            if (IsThreadRunning)
            {
                _jobThread.Abort();
            }
        }

        public void AfterThreadAbort(ThreadAbortException ex)
        {
            LogUtil.LogFatal(ex.Message);
            //ErrorEmailHelper.SendErrorEmail("ThreadStatusUpdater", "Internal", "StatusUpdateThread", ex);
        }

        public void OnStop()
        {
        }

        public void SetConversionAuthToken()
        {
        }
    }
}
