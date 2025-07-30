DROP VIEW IF EXISTS [cTest].[V_WorkflowDefinitionDetail]
GO
CREATE VIEW [cTest].[V_WorkflowDefinitionDetail]
AS
SELECT [cTest].wf_WorkflowDefinition.CategoryId,[cTest].wf_WorkflowDefinition.Id,[cTest].wf_WorkflowDefinition.Code,[cTest].wf_WorkflowDefinition.Version,[cTest].wf_WorkflowDefinition.Name,[cTest].wf_WorkflowDefinition.Status,[cTest].wf_WorkflowDefNode.Id AS NodeId,[cTest].wf_WorkflowDefNode.Code AS NodeCode,[cTest].wf_WorkflowDefNode.Name AS NodeName,[cTest].wf_WorkflowDefNode.NodeType,[cTest].wf_WorkflowDefNode.WorkflowDefId,[cTest].wf_WorkflowDefNodeRule.Id AS NodeRuleId,[cTest].wf_WorkflowDefNodeRule.RuleType,[cTest].wf_WorkflowDefNodeRule.FieldName,[cTest].wf_WorkflowDefNodeRule.FieldDisplayName,[cTest].wf_WorkflowDefNodeRule.OperatorType,[cTest].wf_WorkflowDefNodeRule.FieldValue
FROM [cTest].wf_WorkflowDefinition LEFT JOIN [cTest].wf_WorkflowDefNode ON [cTest].wf_WorkflowDefNode.WorkflowDefId = [cTest].wf_WorkflowDefinition.Id 
LEFT JOIN [cTest].wf_WorkflowDefNodeRule ON [cTest].wf_WorkflowDefNodeRule.WorkflowNodeId = [cTest].wf_WorkflowDefNode.Id
GO


DROP VIEW IF EXISTS [cTest].[V_WorkflowProcessTaskExecuteDetail]
GO
CREATE VIEW [cTest].[V_WorkflowProcessTaskExecuteDetail]
AS
SELECT [cTest].wf_WorkflowProcess.CategoryId,[cTest].wf_WorkflowProcess.Id,[cTest].wf_WorkflowProcess.Code,[cTest].wf_WorkflowProcess.Name,[cTest].wf_WorkflowProcess.Version,[cTest].wf_WorkflowProcess.ProcessStatus,[cTest].wf_WorkflowProTask.Id AS TaskId,[cTest].wf_WorkflowProTask.Code AS TaskCode,[cTest].wf_WorkflowProTask.Name AS TaskName,[cTest].wf_WorkflowProTaskExecute.Id AS ExecuteId,[cTest].wf_WorkflowProTaskExecute.ExecuteUserId,[cTest].wf_WorkflowProTaskExecute.ExecuteUserName,[cTest].wf_WorkflowProTaskExecute.IsAgree,[cTest].wf_WorkflowProTaskExecute.ExecuteStatus
FROM [cTest].wf_WorkflowProcess LEFT JOIN [cTest].wf_WorkflowProTask ON [cTest].wf_WorkflowProTask.ProcessId = [cTest].wf_WorkflowProcess.Id
LEFT JOIN [cTest].wf_WorkflowProTaskExecute ON [cTest].wf_WorkflowProTaskExecute.TaskId =[cTest].wf_WorkflowProTask.Id
GO

