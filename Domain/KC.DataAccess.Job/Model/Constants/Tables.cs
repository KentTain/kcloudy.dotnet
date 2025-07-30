using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Model.Job.Constants
{
    public sealed class Tables
    {
        private const string Prx = "job_";

        public const string DatabaseVersionInfo = Prx + "DatabaseVersionInfo";
        public const string QueueErrorMessage = Prx + "QueueErrorMessage";

        public const string ThreadConfigInfo = Prx + "ThreadConfigInfo";
        public const string ThreadStatusInfo = Prx + "ThreadStatusInfo";
        public const string NLogEntity = Prx + "NLogEntity";

        
    }
}
