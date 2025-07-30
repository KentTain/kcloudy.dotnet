using System;
using KC.Framework.Util;
using KC.Model.Job.Table;

namespace KC.Job.Core
{
    public partial class ConfigurationMonitor
    {
        public event EventHandler RoleConfigReceived;
        public event EventHandler RoleConfigUpdated;

        private readonly string _instanceName;
        private readonly string _hostName;
        private readonly string _workerRoleId;
        private System.Timers.Timer _configurationTimer;
        private IThreadService _threadService;

        public ThreadConfigInfo RoleConfig { get; private set; }

        public bool HasRoleConfig { get { return RoleConfig != null; } }

        public ConfigurationMonitor(string instanceName, string hostName, string workerRoleId, IThreadService threadService)
        {
            this._instanceName = instanceName;
            this._hostName = hostName;
            this._workerRoleId = workerRoleId;

            this._threadService = threadService;
        }

        public void Start()
        {
            this._configurationTimer = new System.Timers.Timer();
            this._configurationTimer.AutoReset = true;
            this._configurationTimer.Elapsed += new System.Timers.ElapsedEventHandler(_configurationTimer_Elapsed);
            this._configurationTimer.Interval = new TimeSpan(0, 2, 0).TotalMilliseconds;
            this._configurationTimer.Start();

            //Execute immediatily on starting
            this._configurationTimer_Elapsed(null, null);
        }

        private void _configurationTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                this.RefreshWorkerRoleConfig();
            }
            catch (Exception ex)
            {
                string fromHost = this._instanceName + '-' + this._workerRoleId;
                //ErrorEmailHelper.SendErrorEmail(fromHost, "WokerRole", "RefreshConfiguration", ex);
                LogUtil.LogException(ex);
            }
        }

        private void RefreshWorkerRoleConfig()
        {
            try
            {
                var workerRoleConfig = _threadService.GetWorkerRoleConfigToRun(_workerRoleId, _hostName);
                if (workerRoleConfig != null)
                {
                    if (this.RoleConfig == null)
                    {
                        this.RoleConfig = workerRoleConfig;

                        if (this.RoleConfigReceived != null)
                            this.RoleConfigReceived(this, EventArgs.Empty);
                    }
                    else if (this.RoleConfig.LastModifyTime < workerRoleConfig.LastModifyTime)
                    {
                        this.RoleConfig = workerRoleConfig;

                        if (this.RoleConfigUpdated != null)
                            this.RoleConfigUpdated(this, EventArgs.Empty);
                    }
                }
            }
            catch (Exception)
            {
                
            }
        }
    }
}
