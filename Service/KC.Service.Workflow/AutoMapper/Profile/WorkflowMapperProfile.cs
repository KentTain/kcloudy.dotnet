using AutoMapper;
using KC.Framework.Extension;
using KC.Framework.Tenant;
using KC.Framework.Base;
using KC.Service.DTO.Admin;
using KC.Service.DTO;
using KC.Model.Workflow;
using KC.Service.Workflow.DTO;
using System;
using KC.Service.DTO.Workflow;

namespace KC.Service.Workflow.AutoMapper.Profile
{
    public partial class WorkflowMapperProfile : global::AutoMapper.Profile
    {
        public WorkflowMapperProfile()
        {
            CreateMap<Entity, EntityDTO>();
            CreateMap<EntityDTO, Entity>();

            CreateMap<TreeNode<WorkflowCategory>, TreeNodeDTO<WorkflowCategoryDTO>>()
                .IncludeBase<Entity, EntityDTO>();
            CreateMap<TreeNodeDTO<WorkflowCategoryDTO>, TreeNode<WorkflowCategory>>()
                .IncludeBase<EntityDTO, Entity>();

            CreateMap<ProcessLogBase, ProcessLogBaseDTO>()
                .ForMember(target => target.TypeString, config => config.Ignore());
            CreateMap<ProcessLogBaseDTO, ProcessLogBase>();

            CreateMap<PropertyAttributeBase, PropertyAttributeBaseDTO>();
            CreateMap<PropertyAttributeBaseDTO, PropertyAttributeBase>();

            CreateMap<DefinitionBase, DefinitionBaseDTO>()
                .IncludeBase<Entity, EntityDTO>()
                .ForMember(target => target.CategoryName,
                    config =>
                        config.MapFrom(src => src.WorkflowCategory != null
                            ? src.WorkflowCategory.Name
                            : string.Empty));
            CreateMap<DefinitionBaseDTO, DefinitionBase>()
                .IncludeBase<EntityDTO, Entity>()
                .ForMember(target => target.WorkflowCategory, config => config.Ignore());

            #region Model Definition

            CreateMap<ModelDefField, ModelDefFieldDTO>();
            CreateMap<ModelDefFieldDTO, ModelDefField>();

            CreateMap<ModelDefinition, ModelDefinitionDTO>();
            CreateMap<ModelDefinitionDTO, ModelDefinition>();

            CreateMap<ModelDefLog, ModelDefLogDTO>()
                .IncludeBase<ProcessLogBase, ProcessLogBaseDTO>();
            CreateMap<ModelDefLogDTO, ModelDefLog>()
                .IncludeBase<ProcessLogBaseDTO, ProcessLogBase>();
            #endregion

            #region Workflow Definition

            CreateMap<WorkflowCategory, WorkflowCategoryDTO>()
                .IncludeBase<TreeNode<WorkflowCategory>, TreeNodeDTO<WorkflowCategoryDTO>>()
                .ForMember(target => target.Id, config => config.MapFrom(src => src.Id))
                .ForMember(target => target.Text, config => config.MapFrom(src => src.Name))
                .ForMember(target => target.ParentName, config => config.MapFrom(src => src.ParentNode.Name))
                .ForMember(target => target.Children, config => config.MapFrom(src => src.ChildNodes))
                .ForMember(target => target.@checked, config => config.Ignore());

            CreateMap<WorkflowCategoryDTO, WorkflowCategory>()
                .IncludeBase<TreeNodeDTO<WorkflowCategoryDTO>, TreeNode<WorkflowCategory>>()
                .ForMember(target => target.Id, config => config.MapFrom(src => src.Id))
                .ForMember(target => target.Name, config => config.MapFrom(src => src.Text))
                .ForMember(target => target.ChildNodes, config => config.Ignore())
                .ForMember(target => target.ParentNode, config => config.Ignore());


            CreateMap<WorkflowDefNodeRule, WorkflowDefNodeRuleDTO>();
            CreateMap<WorkflowDefNodeRuleDTO, WorkflowDefNodeRule>();
            
            CreateMap<WorkflowDefNode, WorkflowDefNodeDTO>();
            CreateMap<WorkflowDefNodeDTO, WorkflowDefNode>();

            CreateMap<WorkflowDefField, WorkflowDefFieldDTO>()
                //.IncludeBase<DefFieldBase<WorkflowDefField>, DefFieldBaseDTO<WorkflowDefFieldDTO>>()
                //.IncludeBase<TreeNode<WorkflowDefField>, TreeNodeDTO<WorkflowDefFieldDTO>>()
                .ForMember(target => target.Text, config => config.MapFrom(src => src.Name))
                .ForMember(target => target.Children, config => config.MapFrom(src => src.ChildNodes))
                .ForMember(target => target.@checked, config => config.Ignore())
                .ForMember(target => target.WorkflowDefinition, config => config.Ignore());
            CreateMap<WorkflowDefFieldDTO, WorkflowDefField>()
                //.IncludeBase<DefFieldBaseDTO<WorkflowDefFieldDTO>, DefFieldBase<WorkflowDefField>>()
                //.IncludeBase<TreeNodeDTO<WorkflowDefFieldDTO>, TreeNode<WorkflowDefField>>()
                .ForMember(target => target.Name, config => config.MapFrom(src => src.Text))
                .ForMember(target => target.ChildNodes, config => config.MapFrom(src => src.Children))
                .ForMember(target => target.ParentNode, config => config.Ignore())
                .ForMember(target => target.WorkflowDefinition, config => config.Ignore());

            CreateMap<WorkflowDefinition, WorkflowDefinitionDTO>()
                .IncludeBase<DefinitionBase, DefinitionBaseDTO>();
            CreateMap<WorkflowDefinitionDTO, WorkflowDefinition>()
                .IncludeBase<DefinitionBaseDTO, DefinitionBase>();

            CreateMap<WorkflowDefLog, WorkflowDefLogDTO>()
                .IncludeBase<ProcessLogBase, ProcessLogBaseDTO>();
            CreateMap<WorkflowDefLogDTO, WorkflowDefLog>()
                .IncludeBase<ProcessLogBaseDTO, ProcessLogBase>();

            #endregion

            #region Workflow Verson
            CreateMap<WorkflowVerDefNodeRule, WorkflowVerDefNodeRuleDTO>();
            CreateMap<WorkflowVerDefNodeRuleDTO, WorkflowVerDefNodeRule>();
            
            CreateMap<WorkflowVerDefNode, WorkflowVerDefNodeDTO>();
            CreateMap<WorkflowVerDefNodeDTO, WorkflowVerDefNode>();

            CreateMap<WorkflowVerDefField, WorkflowVerDefFieldDTO>()
                //.IncludeBase<DefFieldBase<WorkflowVerDefField>, DefFieldBaseDTO<WorkflowVerDefFieldDTO>>()
                //.IncludeBase<TreeNode<WorkflowVerDefField>, TreeNodeDTO<WorkflowVerDefFieldDTO>>()
                .ForMember(target => target.Text, config => config.MapFrom(src => src.Name))
                .ForMember(target => target.Children, config => config.MapFrom(src => src.ChildNodes))
                .ForMember(target => target.@checked, config => config.Ignore())
                .ForMember(target => target.WorkflowVerDefinition, config => config.Ignore());
            CreateMap<WorkflowVerDefFieldDTO, WorkflowVerDefField>()
                //.IncludeBase<DefFieldBaseDTO<WorkflowVerDefFieldDTO>, DefFieldBase<WorkflowVerDefField>>()
                //.IncludeBase<TreeNodeDTO<WorkflowVerDefFieldDTO>, TreeNode<WorkflowVerDefField>>()
                .ForMember(target => target.Name, config => config.MapFrom(src => src.Text))
                .ForMember(target => target.ChildNodes, config => config.MapFrom(src => src.Children))
                .ForMember(target => target.ParentNode, config => config.Ignore())
                .ForMember(target => target.WorkflowVerDefinition, config => config.Ignore());

            CreateMap<WorkflowVerDefinition, WorkflowVerDefinitionDTO>()
                .IncludeBase<DefinitionBase, DefinitionBaseDTO>();
            CreateMap<WorkflowVerDefinitionDTO, WorkflowVerDefinition>()
                .IncludeBase<DefinitionBaseDTO, DefinitionBase>();
            #endregion

            #region Workflow Process
            CreateMap<WorkflowProTaskExecute, WorkflowProTaskExecuteDTO>()
                .ForMember(target => target.TaskCode,
                    config =>
                        config.MapFrom(src => src.Task != null
                            ? src.Task.Code
                            : string.Empty))
                .ForMember(target => target.TaskName,
                    config =>
                        config.MapFrom(src => src.Task != null
                            ? src.Task.Name
                            : string.Empty));
            CreateMap<WorkflowProTaskExecuteDTO, WorkflowProTaskExecute>();

            CreateMap<WorkflowProTaskRule, WorkflowProTaskRuleDTO>();
            CreateMap<WorkflowProTaskRuleDTO, WorkflowProTaskRule>();

            CreateMap<WorkflowProTask, WorkflowProTaskDTO>()
                .ForMember(target => target.ProcessCode,
                    config =>
                        config.MapFrom(src => src.Process != null
                            ? src.Process.Code
                            : string.Empty))
                .ForMember(target => target.ProcessVersion,
                    config =>
                        config.MapFrom(src => src.Process != null
                            ? src.Process.Version
                            : string.Empty))
                .ForMember(target => target.ProcessName,
                    config =>
                        config.MapFrom(src => src.Process != null
                            ? src.Process.Name
                            : string.Empty));
            CreateMap<WorkflowProTaskDTO, WorkflowProTask>()
                .ForMember(target => target.Process, config => config.Ignore());

            CreateMap<WorkflowProField, WorkflowProFieldDTO>()
                //.IncludeBase<DefFieldBase<WorkflowProField>, DefFieldBaseDTO<WorkflowProFieldDTO>>()
                //.IncludeBase<TreeNode<WorkflowProField>, TreeNodeDTO<WorkflowProFieldDTO>>()
                .ForMember(target => target.Text, config => config.MapFrom(src => src.Name))
                .ForMember(target => target.Children, config => config.MapFrom(src => src.ChildNodes))
                .ForMember(target => target.@checked, config => config.Ignore())
                .ForMember(target => target.Process, config => config.Ignore());
            CreateMap<WorkflowProFieldDTO, WorkflowProField>()
                //.IncludeBase<DefFieldBaseDTO<WorkflowProFieldDTO>, DefFieldBase<WorkflowProField>>()
                //.IncludeBase<TreeNodeDTO<WorkflowProFieldDTO>, TreeNode<WorkflowProField>>()
                .ForMember(target => target.Name, config => config.MapFrom(src => src.Text))
                .ForMember(target => target.ChildNodes, config => config.MapFrom(src => src.Children))
                .ForMember(target => target.ParentNode, config => config.Ignore())
                .ForMember(target => target.Process, config => config.Ignore());

            CreateMap<WorkflowProcess, WorkflowProcessDTO>()
                .IncludeBase<DefinitionBase, DefinitionBaseDTO>();
            CreateMap<WorkflowProcessDTO, WorkflowProcess>()
                .IncludeBase<DefinitionBaseDTO, DefinitionBase>();

            CreateMap<WorkflowProRequestLog, WorkflowProRequestLogDTO>()
                .IncludeBase<ProcessLogBase, ProcessLogBaseDTO>();
            CreateMap<WorkflowProRequestLogDTO, WorkflowProRequestLog>()
                .IncludeBase<ProcessLogBaseDTO, ProcessLogBase>();
            #endregion

            #region Workflow Definition to Version Def
            CreateMap<WorkflowDefNodeRule, WorkflowVerDefNodeRule>()
                .ForMember(target => target.WorkflowVerNode, config => config.Ignore());
            CreateMap<WorkflowDefNode, WorkflowVerDefNode>()
                .ForMember(target => target.Rules, config => config.MapFrom(src => src.Rules))
                .ForMember(target => target.WorkflowVerDefinition, config => config.Ignore());
            CreateMap<WorkflowDefField, WorkflowVerDefField>()
                //.ForMember(target => target.ChildNodes, config => config.MapFrom(src => src.ChildNodes))
                .ForMember(target => target.WorkflowVerDefinition, config => config.Ignore());
            CreateMap<WorkflowDefinition, WorkflowVerDefinition>()
                .ForMember(target => target.WorkflowVerNodes, config => config.MapFrom(src => src.WorkflowNodes))
                .ForMember(target => target.WorkflowVerFields, config => config.MapFrom(src => src.WorkflowFields))
                .ForMember(target => target.WorkflowDefinition, config => config.Ignore());
            #endregion

            #region Workflow Definition to Process
            CreateMap<WorkflowDefNodeRule, WorkflowProTaskRule>()
                .ForMember(target => target.TaskCode, config => config.Ignore())
                .ForMember(target => target.TaskId, config => config.Ignore())
                .ForMember(target => target.Task, config => config.Ignore())
                .ForMember(target => target.Id, config => config.Ignore());
            CreateMap<WorkflowDefNode, WorkflowProTask>()
                //.ForMember(target => target.Id, config => config.MapFrom(src => Guid.NewGuid()))
                .ForMember(target => target.Rules, config => config.MapFrom(src => src.Rules))
                .ForMember(target => target.TaskStatus, config => config.Ignore())
                .ForMember(target => target.ProcessId, config => config.Ignore())
                .ForMember(target => target.Process, config => config.Ignore())
                .ForMember(target => target.TaskExecutes, config => config.Ignore())
                .ForMember(target => target.Id, config => config.Ignore());
            CreateMap<WorkflowDefField, WorkflowProField>()
                .ForMember(target => target.ChildNodes, config => config.MapFrom(src => src.ChildNodes))
                .ForMember(target => target.ParentNode, config => config.MapFrom(src => src.ParentNode))
                //.ForMember(target => target.ParentId,
                //    config => config.MapFrom(
                //        src => src.ParentNode != null 
                //            ? src.ParentNode.Id 
                //            : default(int?)))
                .ForMember(target => target.ParentNode, config => config.Ignore())
                .ForMember(target => target.ProcessId, config => config.Ignore())
                .ForMember(target => target.Process, config => config.Ignore())
                .ForMember(target => target.Id, config => config.Ignore());
            CreateMap<WorkflowDefinition, WorkflowProcess>()
                //.ForMember(target => target.Id, config => config.MapFrom(src => Guid.NewGuid()))
                .ForMember(target => target.WorkflowDefId, config => config.MapFrom(src => src.Id))
                .ForMember(target => target.WorkflowDefCode, config => config.MapFrom(src => src.Code))
                .ForMember(target => target.WorkflowDefName, config => config.MapFrom(src => src.Name))
                .ForMember(target => target.Version, config => config.MapFrom(src => src.Version))
                .ForMember(target => target.Tasks, config => config.MapFrom(src => src.WorkflowNodes))
                .ForMember(target => target.Context, config => config.MapFrom(src => src.WorkflowFields))
                .ForMember(target => target.CurrentTaskId, config => config.Ignore())
                .ForMember(target => target.StartDateTime, config => config.Ignore())
                .ForMember(target => target.EndDateTime, config => config.Ignore())
                .ForMember(target => target.ProcessStatus, config => config.Ignore())
                .ForMember(target => target.Id, config => config.Ignore());


            CreateMap<WorkflowDefNodeRuleDTO, WorkflowProTaskRuleDTO>()
                .ForMember(target => target.TaskCode, config => config.Ignore())
                .ForMember(target => target.TaskId, config => config.Ignore())
                .ForMember(target => target.Task, config => config.Ignore())
                .ForMember(target => target.Id, config => config.Ignore());
            CreateMap<WorkflowDefNodeDTO, WorkflowProTaskDTO>()
                .ForMember(target => target.Rules, config => config.MapFrom(src => src.Rules))
                .ForMember(target => target.TaskStatus, config => config.Ignore())
                .ForMember(target => target.ProcessId, config => config.Ignore())
                .ForMember(target => target.ProcessCode, config => config.Ignore())
                .ForMember(target => target.ProcessName, config => config.Ignore())
                .ForMember(target => target.ProcessVersion, config => config.Ignore())
                .ForMember(target => target.TaskExecutes, config => config.Ignore())
                .ForMember(target => target.Id, config => config.Ignore());
            CreateMap<WorkflowDefFieldDTO, WorkflowProFieldDTO>()
                .ForMember(target => target.Children, config => config.MapFrom(src => src.Children))
                //.ForMember(target => target.ParentId,
                //    config => config.MapFrom(
                //        src => src.ParentNode != null 
                //            ? src.ParentNode.Id 
                //            : default(int?)))
                .ForMember(target => target.ProcessId, config => config.Ignore())
                .ForMember(target => target.Process, config => config.Ignore())
                .ForMember(target => target.Id, config => config.Ignore());
            CreateMap<WorkflowDefinitionDTO, WorkflowProcessDTO>()
                .ForMember(target => target.WorkflowDefId, config => config.MapFrom(src => src.Id))
                .ForMember(target => target.WorkflowDefCode, config => config.MapFrom(src => src.Code))
                .ForMember(target => target.WorkflowDefName, config => config.MapFrom(src => src.Name))
                .ForMember(target => target.Version, config => config.MapFrom(src => src.Version))
                .ForMember(target => target.Tasks, config => config.MapFrom(src => src.WorkflowNodes))
                .ForMember(target => target.Context, config => config.MapFrom(src => src.WorkflowFields))
                .ForMember(target => target.CurrentTaskId, config => config.Ignore())
                .ForMember(target => target.StartDateTime, config => config.Ignore())
                .ForMember(target => target.EndDateTime, config => config.Ignore())
                .ForMember(target => target.ProcessStatus, config => config.Ignore())
                .ForMember(target => target.Id, config => config.Ignore());
            #endregion
        }
    }
}
