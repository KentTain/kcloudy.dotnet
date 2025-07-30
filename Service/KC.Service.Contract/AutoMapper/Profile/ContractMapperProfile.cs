using AutoMapper;
using KC.Framework.Extension;
using KC.Framework.Base;
using KC.Service.DTO;
using KC.Model.Contract;
using KC.Service.DTO.Contract;
using System.Linq;

namespace KC.Service.Contract.AutoMapper.Profile
{
    public partial class ContractMapperProfile : global::AutoMapper.Profile
    {
        public ContractMapperProfile()
        {
            CreateMap<Entity, EntityDTO>();
            CreateMap<EntityDTO, Entity>();

            CreateMap<ProcessLogBase, ProcessLogBaseDTO>()
                .ForMember(target => target.TypeString, config => config.Ignore());
            CreateMap<ProcessLogBaseDTO, ProcessLogBase>();

            //CreateMap<TreeNode, TreeNodeDTO>()
            //    .IncludeBase<Entity, EntityDTO>();
            //CreateMap<TreeNodeDTO, TreeNode>()
            //    .IncludeBase<EntityDTO, Entity>();

            CreateMap<ContractGroup, ContractGroupDTO>()
                .ForMember(target => target.DownFileUrl, config => config.Ignore())
                .ForMember(target => target.IsEdit, config => config.Ignore())
                .ForMember(target => target.IsLast, config => config.Ignore())
                .ForMember(target => target.IsRelieveAll, config => config.Ignore())
                .ForMember(target => target.IsComfirmFrist, config => config.Ignore())
                .ForMember(target => target.IsRelieve, config => config.Ignore())
                .ForMember(target => target.IsComfirm, config => config.Ignore())
                .ForMember(target => target.IsReturn, config => config.Ignore())
                .ForMember(target => target.IsSign, config => config.Ignore())
                .ForMember(target => target.Opt, config => config.Ignore())
                .ForMember(target => target.CurrentUserId, config => config.Ignore())
                .ForMember(target => target.CreatedDateString, config => config.MapFrom(src => src.CreatedDate.AddHours(8).ToString("yyyy-MM-dd HH:mm:ss")))
                .ForMember(target => target.TypeStr, config => config.MapFrom(src => src.Type.ToDescription()))
                .ForMember(target => target.HasLogs, config => config.MapFrom(src => (src.ContractGroupOperationLog != null && src.ContractGroupOperationLog.Any())))
                .ForMember(target => target.StatuStr, config => config.MapFrom(src => src.Statu.ToDescription()))
                .ForMember(target => target.DateTimeStr, config => config.Ignore())
                .ForMember(target => target.DateTimeStr, config => config.MapFrom(src => src.CreatedDate.ToString()))
                .IncludeBase<Entity, EntityDTO>();

            CreateMap<ContractGroupDTO, ContractGroup>()
                .IncludeBase<EntityDTO, Entity>();
            CreateMap<ContractGroup, ContractGroupAPIModel>()
                 .ForMember(target => target.DownFileUrl, config => config.Ignore())
                .ForMember(target => target.IsEdit, config => config.Ignore())
                .ForMember(target => target.IsLast, config => config.Ignore())
                .ForMember(target => target.IsRelieveAll, config => config.Ignore())
                .ForMember(target => target.IsComfirmFrist, config => config.Ignore())
                .ForMember(target => target.IsRelieve, config => config.Ignore())
                .ForMember(target => target.IsComfirm, config => config.Ignore())
                .ForMember(target => target.IsReturn, config => config.Ignore())
                .ForMember(target => target.IsSign, config => config.Ignore())
                .ForMember(target => target.CreatedDateString, config => config.MapFrom(src => src.CreatedDate.AddHours(8).ToString("yyyy-MM-dd HH:mm:ss")))
                .ForMember(target => target.TypeStr, config => config.MapFrom(src => src.Type.ToDescription()))
                .ForMember(target => target.StatuStr, config => config.MapFrom(src => src.Statu.ToDescription()))
                .ForMember(target => target.DateTimeStr, config => config.Ignore())
                .ForMember(target => target.DateTimeStr,
                    config =>
                        config.MapFrom(src => src.CreatedDate.ToString()))
                .IncludeBase<Entity, EntityDTO>();
            CreateMap<ContractGroupAPIModel, ContractGroup>()
                .IncludeBase<EntityDTO, Entity>();

            CreateMap<UserContract, UserContractDTO>()
                .ForMember(target => target.IsEdit, config => config.Ignore())
                .IncludeBase<Entity, EntityDTO>();
            CreateMap<UserContractDTO, UserContract>()
                .IncludeBase<EntityDTO, Entity>();

            CreateMap<UserContract, UserContractAPIModel>()
               .ForMember(target => target.IsEdit, config => config.Ignore())
               .IncludeBase<Entity, EntityDTO>();
            CreateMap<UserContractAPIModel, UserContract>()
                .IncludeBase<EntityDTO, Entity>();

            CreateMap<ContractGroupOperationLog, ContractGroupOperationLogDTO>()
                .ForMember(target => target.TypeStr,
                    config =>
                        config.MapFrom(src => src.Type.ToDescription()))
                .IncludeBase<ProcessLogBase, ProcessLogBaseDTO>();
            CreateMap<ContractGroupOperationLogDTO, ContractGroupOperationLog>()
                .IncludeBase<ProcessLogBaseDTO, ProcessLogBase>();

            //CreateMap<ContractGroupTotal, ContractGroupTotalDTO>();

            CreateMap<ContractTemplate, ContractTemplateDTO>()
                .ForMember(target => target.TypeStr, config => config.MapFrom(src => src.Type.ToDescription()))
                .IncludeBase<Entity, EntityDTO>();
            CreateMap<ContractTemplateDTO, ContractTemplate>()
                .IncludeBase<EntityDTO, Entity>();

            CreateMap<ContractTemplate, ContractTemplateAPIModel>()
                 .ForMember(target => target.TypeStr, config => config.MapFrom(src => src.Type.ToDescription()))
                 .IncludeBase<Entity, EntityDTO>();
            CreateMap<ContractTemplateAPIModel, ContractTemplate>()
                .IncludeBase<EntityDTO, Entity>();

            CreateMap<ElectronicPerson, ElectronicPersonDTO>()
               .IncludeBase<Entity, EntityDTO>()
               .ForMember(target => target.IsEditMode, config => config.Ignore());
            CreateMap<ElectronicPersonDTO, ElectronicPerson>()
                .IncludeBase<EntityDTO, Entity>();

            CreateMap<ElectronicOrganization, ElectronicOrganizationDTO>()
                .ForMember(target => target.StatusStr, config => config.Ignore())
                .ForMember(target => target.IsEditMode, config => config.Ignore())
               .IncludeBase<Entity, EntityDTO>();
            CreateMap<ElectronicOrganizationDTO, ElectronicOrganization>()
                .IncludeBase<EntityDTO, Entity>();

        }
    }
}
