using AutoMapper;
using KC.Service.DTO.Config;
using KC.Framework.Base;
using KC.Model.Config;
using KC.Service.DTO.Account;
using KC.Service.DTO;

namespace KC.Service.Config.AutoMapper.Profile
{
    public class ConfigMapperProfile : global::AutoMapper.Profile
    {
        public ConfigMapperProfile()
        {
            CreateMap<Entity, EntityDTO>();
            CreateMap<EntityDTO, Entity>();

            CreateMap<ProcessLogBase, ProcessLogBaseDTO>()
                .ForMember(target => target.TypeString, config => config.Ignore());
            CreateMap<ProcessLogBaseDTO, ProcessLogBase>();


            #region 应用及配置
            CreateMap<ConfigAttribute, ConfigAttributeDTO>()
               .IncludeBase<Entity, EntityDTO>()
               .ForMember(target => target.ConfigName,
                    config =>
                        config.MapFrom(m => m.ConfigEntity != null
                            ? m.ConfigEntity.ConfigName
                            : string.Empty));
            CreateMap<ConfigAttributeDTO, ConfigAttribute>()
                .IncludeBase<EntityDTO, Entity>()
                .ForMember(target => target.ConfigEntity, config => config.Ignore());

            CreateMap<ConfigEntity, ConfigEntityDTO>()
                .IncludeBase<Entity, EntityDTO>()
                .ForMember(target => target.IsEditMode, config => config.Ignore());
            CreateMap<ConfigEntityDTO, ConfigEntity>()
                .IncludeBase<EntityDTO, Entity>();

            CreateMap<ConfigLog, ConfigLogDTO>()
                .IncludeBase<ProcessLogBase, ProcessLogBaseDTO>();
            CreateMap<ConfigLogDTO, ConfigLog>()
                .IncludeBase<ProcessLogBaseDTO, ProcessLogBase>();

            #endregion

            CreateMap<SysSequence, SysSequenceDTO>()
                .ForMember(target => target.IsEditMode, config => config.Ignore());
            CreateMap<SysSequenceDTO, SysSequence>();
        }
    }
}
