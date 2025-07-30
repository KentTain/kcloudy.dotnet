using AutoMapper;
using KC.Framework.Base;
using KC.Service.DTO;
using KC.Model.App;
using KC.Service.DTO.App;

namespace KC.Service.App.AutoMapper.Profile
{
    public partial class AppMapperProfile : global::AutoMapper.Profile
    {
        public AppMapperProfile()
        {
            //基础类型映射
            CreateMap<Entity, EntityDTO>();
            CreateMap<EntityDTO, Entity>();

            CreateMap<ProcessLogBase, ProcessLogBaseDTO>();
            CreateMap<ProcessLogBaseDTO, ProcessLogBase>();

            CreateMap<PropertyAttributeBase, PropertyAttributeBaseDTO>()
                .IncludeBase<Entity, EntityDTO>();
            CreateMap<PropertyAttributeBaseDTO, PropertyAttributeBase>()
                .IncludeBase<EntityDTO, Entity>();

            #region 应用业务类型（removed）
            //CreateMap<ApplicationBusiness, AppBusinessSimpleDTO>()
            //    .IncludeBase<Entity, EntityDTO>()
            //    .ForMember(target => target.BusinessTypeStr, config => config.Ignore());

            //CreateMap<PushMapping, PushMappingDTO>();
            //CreateMap<PushMappingDTO, PushMapping>();

            //CreateMap<SecurityParameter, SecurityParameterDTO>();
            //CreateMap<SecurityParameterDTO, SecurityParameter>();

            //CreateMap<AppTargetApiSetting, AppTargetApiSettingDTO>()
            //    .ForMember(target => target.IsEditMode, config => config.Ignore())
            //    .ForMember(target => target.SecurityTypeString, config => config.Ignore());
            //CreateMap<AppTargetApiSettingDTO, AppTargetApiSetting>();

            //CreateMap<AppApiPushLog, AppApiPushLogDTO>();
            //CreateMap<AppApiPushLogDTO, AppApiPushLog>();

            //CreateMap<AppApiPushSetting, AppApiPushSettingDTO>()
            //    .IncludeBase<Entity, EntityDTO>()
            //    .ForMember(target => target.Business, config => config.MapFrom(src => src.Business))
            //    .ForMember(target => target.BusinessCode, config => config.Ignore())
            //    .ForMember(target => target.BusinessName, config => config.Ignore())
            //    .ForMember(target => target.BusinessTypeString, config => config.Ignore());
            //CreateMap<AppApiPushSettingDTO, AppApiPushSetting>()
            //    .IncludeBase<EntityDTO, Entity>()
            //    .ForMember(target => target.Business, config => config.Ignore());

            //CreateMap<ApplicationBusiness, ApplicationBusinessDTO>()
            //    .IncludeBase<Entity, EntityDTO>()
            //    .ForMember(target => target.PushObjectNameSpace,
            //        config =>
            //            config.MapFrom(
            //                src =>
            //                    src.PushSetting != null
            //                        ? src.PushSetting.PushObjectNameSpace
            //                        : string.Empty))
            //    .ForMember(target => target.IsEditMode, config => config.Ignore())
            //    .ForMember(target => target.BusinessTypeStr, config => config.Ignore());
            //CreateMap<ApplicationBusinessDTO, ApplicationBusiness>()
            //    .IncludeBase<EntityDTO, Entity>()
            //    .ForMember(target => target.PushSetting, config => config.Ignore());
            #endregion

            #region 应用Git仓库
            CreateMap<AppGitBranch, AppGitBranchDTO>()
                .IncludeBase<Entity, EntityDTO>()
                .ForMember(target => target.AppGitAddress,
                    config =>
                        config.MapFrom(
                            src =>
                                src.AppGit != null
                                    ? src.AppGit.GitAddress
                                    : string.Empty))
                .ForMember(target => target.IsEditMode, config => config.Ignore());
            CreateMap<AppGitBranchDTO, AppGitBranch>()
                .IncludeBase<EntityDTO, Entity>();

            CreateMap<AppGitUser, AppGitUserDTO>()
                .IncludeBase<Entity, EntityDTO>()
                .ForMember(target => target.AppGitAddress,
                    config =>
                        config.MapFrom(
                            src =>
                                src.AppGit != null
                                    ? src.AppGit.GitAddress
                                    : string.Empty))
                .ForMember(target => target.IsEditMode, config => config.Ignore());
            CreateMap<AppGitUserDTO, AppGitUser>()
                .IncludeBase<EntityDTO, Entity>();

            CreateMap<AppGit, AppGitDTO>()
                .IncludeBase<Entity, EntityDTO>()
                .ForMember(target => target.ApplicationName,
                    config =>
                        config.MapFrom(
                            src =>
                                src.Application != null
                                    ? src.Application.ApplicationName
                                    : string.Empty))
                .ForMember(target => target.IsEditMode, config => config.Ignore());
            CreateMap<AppGitDTO, AppGit>()
                .IncludeBase<EntityDTO, Entity>();

            #endregion

            #region 应用模板
            CreateMap<DevTemplate, DevTemplateDTO>()
                .IncludeBase<Entity, EntityDTO>()
                .ForMember(target => target.IsEditMode, config => config.Ignore());
            CreateMap<DevTemplateDTO, DevTemplate>()
                .IncludeBase<EntityDTO, Entity>();

            #endregion

            #region 应用设置
            CreateMap<AppSettingProperty, AppSettingPropertyDTO>()
                .IncludeBase<PropertyAttributeBase, PropertyAttributeBaseDTO>()
                .ForMember(target => target.AppSettingName,
                    config =>
                        config.MapFrom(
                            src =>
                                src.AppSetting != null
                                    ? src.AppSetting.Name
                                    : string.Empty))
                .ForMember(target => target.IsEditMode, config => config.Ignore());
            CreateMap<AppSettingPropertyDTO, AppSettingProperty>()
                .IncludeBase<PropertyAttributeBaseDTO, PropertyAttributeBase>();

            CreateMap<AppSetting, AppSettingDTO>()
                .IncludeBase<Entity, EntityDTO>()
                .ForMember(target => target.ApplicationName,
                    config =>
                        config.MapFrom(
                            src =>
                                src.Application != null
                                    ? src.Application.ApplicationName
                                    : string.Empty))
                .ForMember(target => target.IsEditMode, config => config.Ignore());
            CreateMap<AppSettingDTO, AppSetting>()
                .IncludeBase<EntityDTO, Entity>();

            #endregion

            #region 应用
            CreateMap<Application, ApplicationDTO>()
                .IncludeBase<Entity, EntityDTO>()
                .ForMember(target => target.IsEditMode, config => config.Ignore())
                .ForMember(target => target.IconCls, config => config.Ignore())
                .ForMember(target => target.BigIconUrl, config => config.Ignore());
            CreateMap<ApplicationDTO, Application>()
                .IncludeBase<EntityDTO, Entity>();
            #endregion

            #region 应用日志
            CreateMap<ApplicationLog, ApplicationLogDTO>()
                .IncludeBase<ProcessLogBase, ProcessLogBaseDTO>();
            CreateMap<ApplicationLogDTO, ApplicationLog>()
                .IncludeBase<ProcessLogBaseDTO, ProcessLogBase>();
            #endregion

        }
    }
}
