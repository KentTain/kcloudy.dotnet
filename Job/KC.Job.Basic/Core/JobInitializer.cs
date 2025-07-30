using KC.Component.IRepository;
using KC.Framework.Util;
using KC.Job.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace KC.Job.Basic
{
    public class JobInitializer : AbstractJobInitializer
    {
        private string _databaseLocation;

        private string _deploymentId;
        private string _currentRoleInstanceId;
        private string _roleIdSuffix;
        private bool _installPrerequisits;
        private IServiceProvider _serviceProvider;

        public JobInitializer(IServiceProvider serviceProvider)
            : base()
        {
            this._deploymentId = Guid.NewGuid().ToString();
            this._currentRoleInstanceId = Guid.NewGuid().ToString();

            this._roleIdSuffix = this._currentRoleInstanceId.Substring(this._currentRoleInstanceId.Length - 1);
            this.WorkerRoleId = this.HostName + "_" + this._roleIdSuffix;

            _serviceProvider = serviceProvider;
        }

        static JobInitializer()
        {
            // Set the maximum number of concurrent connections 
            if (ServicePointManager.DefaultConnectionLimit < 24)
                ServicePointManager.DefaultConnectionLimit = 24;

            ////Diagnostics configuration requires diagnostics.wadcfgx; configuration in code is no longer supported
            //// http://msdn.microsoft.com/en-us/library/azure/dn873976.aspx#BKMK_breaking
            //if (!NupitchRuntime.IsDevelopmentEnvironment)
            //{
            //    DiagnosticHelper.RegisterMonitor();
            //}
        }

        protected override bool CanRun()
        {
            //if (this._deploymentId.StartsWith("deployment"))
            //{
            //    LogUtil.LogError("Please do not run worker role in development environment against NuPitch production environment");
            //    return false;
            //}

            return true;
        }

        protected override void Start()
        {
            this.InitStorageConfigPublisher();

            //this.ProcessSchemaUpgrade();
        }

        protected override AbstractJobManager CreateJobManager()
        {
            var threadService = this._serviceProvider.GetService<IThreadService>();
            return new JobManager(this.InstanceName, this.HostName, this.WorkerRoleId, this._serviceProvider, threadService);
        }

        private void InitStorageConfigPublisher()
        {
            LogUtil.LogInfo("WorkerRole entry point called - DeploymentId = " + this._deploymentId);
        }
    }
}
