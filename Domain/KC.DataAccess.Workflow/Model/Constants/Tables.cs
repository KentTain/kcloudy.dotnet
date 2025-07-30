using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Model.Workflow.Constants
{
    public sealed class Tables
    {
        private const string Prx = "wf_";

        public const string ModelDefinition = Prx + "ModelDefinition";
        public const string ModelDefField = Prx + "ModelDefField";
        public const string ModelDefLog = Prx + "ModelDefLog";

        public const string WorkflowCategory = Prx + "WorkflowCategory";
        public const string WorkflowDefinition = Prx + "WorkflowDefinition";
        public const string WorkflowDefField = Prx + "WorkflowDefField";
        public const string WorkflowDefNode = Prx + "WorkflowDefNode";
        public const string WorkflowDefNodeRule = Prx + "WorkflowDefNodeRule";
        public const string WorkflowDefLog = Prx + "WorkflowDefLog";

        public const string WorkflowVerDefinition = Prx + "WorkflowVerDefinition";
        public const string WorkflowVerDefField = Prx + "WorkflowVerDefField";
        public const string WorkflowVerDefNode = Prx + "WorkflowVerDefNode";
        public const string WorkflowVerDefNodeRule = Prx + "WorkflowVerDefNodeRule";

        public const string WorkflowProcess = Prx + "WorkflowProcess";
        public const string WorkflowProField = Prx + "WorkflowProField";
        public const string WorkflowProTask = Prx + "WorkflowProTask";
        public const string WorkflowProTaskRule = Prx + "WorkflowProTaskRule";
        public const string WorkflowProTaskExecute = Prx + "WorkflowProTaskExecute";
        public const string WorkflowProRequestLog = Prx + "WorkflowProRequestLog";

        

    }
}
