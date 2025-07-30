using KC.Framework.Extension;
using KC.Framework.Tenant;
using KC.Model.Admin;
using KC.Framework.Base;
using KC.Service.DTO.Admin;
using KC.Service.DTO;
using System.Linq;
using System;

namespace KC.Service.Admin.AutoMapper.Profile
{
    public partial class AdminMapperProfile : global::AutoMapper.Profile
    {
        public AdminMapperProfile()
        {
            CreateMap<Entity, EntityDTO>();
            CreateMap<EntityDTO, Entity>();

            CreateMap<ProcessLogBase, ProcessLogBaseDTO>()
                .Include<TenantUserOperationLog, TenantUserOperationLogDTO>()
                .Include<TenantUserOpenAppErrorLog, TenantUserOpenAppErrorLogDTO>()
                .ForMember(target => target.TypeString, config => config.Ignore());
            CreateMap<ProcessLogBaseDTO, ProcessLogBase>()
                .Include<TenantUserOperationLogDTO, TenantUserOperationLog>()
                .Include<TenantUserOpenAppErrorLogDTO, TenantUserOpenAppErrorLog>();

            CreateMap<PropertyAttributeBase, PropertyAttributeBaseDTO>()
                .IncludeBase<Entity, EntityDTO>();
            CreateMap<PropertyAttributeBaseDTO, PropertyAttributeBase>()
                .IncludeBase<EntityDTO, Entity>();

            //CreateMap<TreeNode, TreeNodeDTO>()
            //    .IncludeBase<Entity, EntityDTO>();
            //CreateMap<TreeNodeDTO, TreeNode>()
            //    .IncludeBase<EntityDTO, Entity>();

            CreateMap<CodeRepositoryPool, CodeRepositoryPoolDTO>()
                .IncludeBase<Entity, EntityDTO>()
                .ForMember(target => target.IsEditMode, config => config.Ignore())
                .ForMember(target => target.CloudTypeString, config => config.Ignore());
            CreateMap<CodeRepositoryPoolDTO, CodeRepositoryPool>()
                .IncludeBase<EntityDTO, Entity>();

            CreateMap<VodPool, VodPoolDTO>()
                .IncludeBase<Entity, EntityDTO>()
                .ForMember(target => target.IsEditMode, config => config.Ignore())
                .ForMember(target => target.CloudTypeString, config => config.Ignore());
            CreateMap<VodPoolDTO, VodPool>()
                .IncludeBase<EntityDTO, Entity>();

            CreateMap<NoSqlPool, NoSqlPoolDTO>()
                .IncludeBase<Entity, EntityDTO>()
                .ForMember(target => target.IsEditMode, config => config.Ignore())
                .ForMember(target => target.CloudTypeString, config => config.Ignore());
            CreateMap<NoSqlPoolDTO, NoSqlPool>()
                .IncludeBase<EntityDTO, Entity>();

            CreateMap<QueuePool, QueuePoolDTO>()
                .IncludeBase<Entity, EntityDTO>()
                .ForMember(target => target.IsEditMode, config => config.Ignore())
                .ForMember(target => target.CloudTypeString, config => config.Ignore());
            CreateMap<QueuePoolDTO, QueuePool>()
                .IncludeBase<EntityDTO, Entity>();

            CreateMap<ServiceBusPool, ServiceBusPoolDTO>()
                .IncludeBase<Entity, EntityDTO>()
                .ForMember(target => target.IsEditMode, config => config.Ignore())
                .ForMember(target => target.CloudTypeString, config => config.Ignore());
            CreateMap<ServiceBusPoolDTO, ServiceBusPool>()
                .IncludeBase<EntityDTO, Entity>();

            CreateMap<StoragePool, StoragePoolDTO>()
                .IncludeBase<Entity, EntityDTO>()
                .ForMember(target => target.IsEditMode, config => config.Ignore())
                .ForMember(target => target.CloudTypeString, config => config.Ignore());
            CreateMap<StoragePoolDTO, StoragePool>()
                .IncludeBase<EntityDTO, Entity>();

            CreateMap<DatabasePool, DatabasePoolDTO>()
                .IncludeBase<Entity, EntityDTO>()
                .ForMember(target => target.IsEditMode, config => config.Ignore())
                .ForMember(target => target.CloudTypeString, config => config.Ignore())
                //.ForMember(target => target.UserPassword, config => config.Ignore())
                //.ForMember(target => target.DatabaseConnectionString, config => config.Ignore())
                ;
            CreateMap<DatabasePoolDTO, DatabasePool>()
                .IncludeBase<EntityDTO, Entity>()
                .ForMember(target => target.ConnectionString, config => config.Ignore());

            //CreateMap<TenantUserAppModule, TenantUserAppModuleDTO>()
            //    .ForMember(target => target.ApplicationName,
            //        config => config.MapFrom(m =>
            //            m.Application != null ? m.Application.ApplicationName : string.Empty))
            //    .ForMember(target => target.Application, config => config.Ignore());
            //CreateMap<TenantUserAppModuleDTO, TenantUserAppModule>()
            //    .ForMember(target => target.Application, config => config.Ignore());

            CreateMap<TenantUserApplication, TenantUserApplicationDTO>()
                .ForMember(target => target.AppStatusName, config => config.MapFrom(m => m.AppStatus.ToDescription()))
                .ForMember(target => target.TenantUser, config => config.Ignore())
                .ForMember(target => target.DomainIp, config => config.Ignore())
                //.ForMember(target => target.TenantUserOperationLog, config => config.Ignore())
                ;
            CreateMap<TenantUserApplicationDTO, TenantUserApplication>()
                //.ForMember(target => target.ApplicationModules, config => config.Ignore())
                //.ForMember(target => target.TenantUserOperationLogs, config => config.Ignore())
                ;

            CreateMap<TenantUserSetting, TenantUserSettingDTO>()
                .IncludeBase<PropertyAttributeBase, PropertyAttributeBaseDTO>()
                .ForMember(target => target.Tenant, config => config.Ignore());
            CreateMap<TenantUserSettingDTO, TenantUserSetting>()
                .IncludeBase<PropertyAttributeBaseDTO, PropertyAttributeBase>()
                .ForMember(target => target.TenantUser, config => config.Ignore());

            CreateMap<TenantUser, TenantUserDTO>()
                .IncludeBase<Entity, EntityDTO>()
                .ForMember(target => target.OwnDomainName,
                    config => config.MapFrom(m => m.GetOwnDomain()))
                .ForMember(target => target.IsEnterprise,
                    config =>
                        config.MapFrom(
                            m =>
                                m.TenantType == TenantType.Enterprise)) 
                .ForMember(target => target.CloudTypeString, config => config.MapFrom(m => m.CloudType.ToDescription()))
                .ForMember(target => target.VersionString, config => config.MapFrom(m => m.Version.ToDescription()))
                .ForMember(target => target.IsEditMode, config => config.Ignore())
                .ForMember(target => target.ConnectionString, config => config.Ignore());
            CreateMap<TenantUserDTO, TenantUser>()
                .IncludeBase<EntityDTO, Entity>()
                .ForMember(target => target.AuthenticationInfo, config => config.Ignore())
                .ForMember(target => target.ConnectionString, config => config.Ignore());

            CreateMap<TenantUserOperationLog, TenantUserOperationLogDTO>()
                 .ForMember(target => target.logtypeName,
                    config => config.MapFrom(m => m.LogType.ToDescription()))
                .ForMember(target => target.AppStatusName, config => config.Ignore())
                  .ForMember(target => target.TenantUserApplication, config => config.Ignore());
            CreateMap<TenantUserOperationLogDTO, TenantUserOperationLog>()
                .ForMember(target => target.TenantUserApplication, config => config.Ignore());

            CreateMap<TenantUserChargeSms, TenantUserCallChargeDTO>()
                .IncludeBase<Entity, EntityDTO>();
            CreateMap<TenantUserCallChargeDTO, TenantUserChargeSms>()
                .IncludeBase<EntityDTO, Entity>();

            CreateMap<TenantUserOpenAppErrorLog, TenantUserOpenAppErrorLogDTO>()
                .ForMember(target => target.OpenServerTypeName,
                    config => config.MapFrom(m => m.OpenServerType.ToDescription()))
                .ForMember(target => target.TenantName,
                    config => config.MapFrom(m =>
                        m.TenantUser != null ? m.TenantUser.TenantName : string.Empty))
                .ForMember(target => target.IsShowUpgradeDbBtn, config => config.Ignore())
                .ForMember(target => target.IsShowRollbackDbBtn, config => config.Ignore())
                .ForMember(target => target.IsShowUpgradeWebBtn, config => config.Ignore())
                .ForMember(target => target.IsShowResendOpenAppEmailBtn, config => config.Ignore());
            CreateMap<TenantUserOpenAppErrorLogDTO, TenantUserOpenAppErrorLog>()
                .ForMember(target => target.TenantUser, config => config.Ignore());

            CreateMap<TenantUserServiceApplication, TenantUserServiceApplicationDTO>()
                .IncludeBase<Entity, EntityDTO>();
            CreateMap<TenantUserServiceApplicationDTO, TenantUserServiceApplication>()
                .IncludeBase<EntityDTO, Entity>();

            CreateMap<TenantUser, TenantSimpleDTO>()
                .ForMember(target => target.OwnDomainName,
                    config => config.MapFrom(m => m.GetOwnDomain()))
                .ForMember(target => target.IsEnterprise,
                    config =>
                        config.MapFrom(
                            m =>
                                m.TenantType == TenantType.Enterprise )) 
                .ForMember(target => target.TenantTypeString, config => config.Ignore())
                .ForMember(target => target.CloudTypeString, config => config.Ignore())
                .ForMember(target => target.VersionString, config => config.Ignore())
                .ForMember(target => target.TenantTypeString, config => config.Ignore());

            CreateMap<TenantUserLoopTask, TenantUserLoopTaskDTO>()
                .IncludeBase<Entity, EntityDTO>();
            CreateMap<TenantUserLoopTaskDTO, TenantUserLoopTask>()
                .IncludeBase<EntityDTO, Entity>();

            CreateMap<TenantUserAuthentication, TenantUserAuthenticationDTO>()
                .IncludeBase<Entity, EntityDTO>()
                .ForMember(target => target.CompanyId,
                    config => config.MapFrom(m => m.TenantId))
                .ForMember(target => target.CompanyCode,
                    config => config.MapFrom(m =>
                        m.TenantUser != null ? m.TenantUser.TenantName : string.Empty))
                .ForMember(target => target.CompanyName,
                    config => config.MapFrom(m =>
                        m.TenantUser != null ? m.TenantUser.TenantDisplayName : string.Empty))
                .ForMember(target => target.IsEditMode, config => config.Ignore());
            CreateMap<TenantUserAuthenticationDTO, TenantUserAuthentication>()
                .IncludeBase<EntityDTO, Entity>()
                .ForMember(target => target.TenantId,
                    config => config.MapFrom(m => m.CompanyId))
                .ForMember(target => target.TenantName,
                    config => config.MapFrom(m => m.CompanyCode))
                .ForMember(target => target.TenantUser, config => config.Ignore());

        }
    }
}
