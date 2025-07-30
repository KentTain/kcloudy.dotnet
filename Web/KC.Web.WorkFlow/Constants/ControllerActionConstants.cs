using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using KC.Web.Constants;

namespace KC.Web.Workflow.Constants
{
    public sealed class ControllerName : ControllerNameBase
    {
        public const string ModelDefinition = "ModelDefinition";
        public const string WorkflowDefinition = "WorkflowDefinition";
        public const string WorkflowVersion = "WorkflowVersion";
        public const string WorkflowProcess = "WorkflowProcess";
        public const string WorkflowInspect = "WorkflowInspect";



    }

    public sealed class ActionName : ActionNameBase
    {
        public sealed class ModelDefinition
        {
            public const string LoadModelDefinitionList = "LoadModelDefinitionList";
            public const string GetModelDefinitionForm = "GetModelDefinitionForm";
            public const string SaveModelDefinition = "SaveModelDefinition";
            public const string RemoveModelDefinition = "RemoveModelDefinition";

            public const string LoadModelDefFieldList = "LoadModelDefFieldList";
            public const string GetModelDefFieldForm = "GetModelDefFieldForm";
            public const string SaveModelDefField = "SaveModelDefField";
            public const string RemoveModelDefField = "RemoveModelDefField";

            public const string LoadModelDefLogList = "LoadModelDefLogList";
            public const string RemoveModelDefLog = "RemoveModelDefLog";
        }

        public sealed class WorkflowDefinition
        {
            //流程分类
            public const string CategoryList = "CategoryList";
            public const string LoadCategoryList = "LoadCategoryList";
            public const string GetCategoryForm = "GetCategoryForm";
            public const string SaveCategoryForm = "SaveCategoryForm";
            public const string RemoveCategory = "RemoveCategory";
            public const string ExistCategoryName = "ExistCategoryName";

            public const string LoadCategoryTree = "LoadCategoryTree";
            
            //流程列表
            public const string LoadWorkflowDefinitionList = "LoadWorkflowDefinitionList";
            public const string LoadWorkflowFieldList = "LoadWorkflowFieldList";
            public const string RemoveWorkflowField = "RemoveWorkflowField";
            public const string RemoveWorkflowDefinition = "RemoveWorkflowDefinition";
            public const string WorkflowDefinitionDetail = "WorkflowDefinitionDetail";
            
            //流程基本信息
            public const string WorkflowDefinitionOrganizationTree = "WorkflowDefinitionOrganizationTree";
            public const string GetWorkflowDefinitionForm = "GetWorkflowDefinitionForm";
            public const string ExistWorkflowDefinitionName = "ExistWorkflowDefinitionName";
            public const string SaveWorkflowBasicInfo = "SaveWorkflowBasicInfo";
            
            //流程定义
            public const string WorkflowDesigner = "WorkflowDesigner";
            public const string GetWorkflowStartNode = "GetWorkflowStartNode";
            public const string SaveWorkflowDefinition = "SaveWorkflowDefinition";

            //流程验证
            public const string WorkflowValidator = "WorkflowValidator";
            public const string GetWorkflowTaskStartData = "GetWorkflowTaskStartData";
            public const string GetWorkflowTaskStartForm = "GetWorkflowTaskStartForm";
            public const string WorkflowTaskStartForm = "WorkflowTaskStartForm";
            public const string StartWorkflowValidator = "StartWorkflowValidator";

            //流程版本
            public const string LoadWorkflowVerDefinitionList = "LoadWorkflowVerDefinitionList";
            public const string LoadWorkflowVerFieldList = "LoadWorkflowVerFieldList";
            public const string RemoveWorkflowVerField = "RemoveWorkflowVerField";
            public const string RemoveWorkflowVerDefinition = "RemoveWorkflowVerDefinition";
            public const string WorkflowVerDefinitionDetail = "WorkflowVerDefinitionDetail";
            public const string WorkflowVerDesigner = "WorkflowVerDesigner";

            //流程日志
            public const string WorkflowDefLog = "WorkflowDefLog";
            public const string LoadWorkflowDefLogList = "LoadWorkflowDefLogList";
        }


        public sealed class WorkflowProcess
        {
            public const string LoadWfProcessTaskList = "LoadWfProcessTaskList";

            public const string WorkflowTaskInfo = "WorkflowTaskInfo";
            public const string LoadWfProcessFieldList = "LoadWfProcessFieldList";
            public const string GetWorkflowTaskStartNode = "GetWorkflowTaskStartNode";
            public const string LoadWfProcessTaskExecuteList = "LoadWfProcessTaskExecuteList";

            public const string GetWorkflowTaskStartForm = "GetWorkflowTaskStartForm";
            public const string GetWorkflowTaskAuditForm = "GetWorkflowTaskAuditForm";
            public const string AuditWorkflowTask = "AuditWorkflowTask";
            
            public const string MyTaskList = "MyTaskList";
            public const string LoadMyTasks = "LoadMyTasks";
  
        }

        public sealed class WorkflowInspect
        {
            public const string SaveDictType = "SaveDictType";
            public const string RemoveDictType = "RemoveDictType";
            public const string LoadDictTypeList = "LoadDictTypeList";
            public const string GetDictTypeForm = "GetDictTypeForm";

            public const string LoadDictValueList = "LoadDictValueList";
            public const string GetDictValueForm = "GetDictValueForm";
            public const string SaveDictValue = "SaveDictValue";
            public const string RemoveDictValue = "RemoveDictValue";
        }
    }
}