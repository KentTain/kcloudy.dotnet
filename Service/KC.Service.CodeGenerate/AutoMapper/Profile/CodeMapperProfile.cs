using AutoMapper;
using KC.Framework.Base;
using KC.Service.DTO;
using KC.Model.CodeGenerate;
using KC.Service.DTO.CodeGenerate;
using KC.Service.DTO.App;

namespace KC.Service.CodeGenerate.AutoMapper.Profile
{
    public partial class CodeMapperProfile : global::AutoMapper.Profile
    {
        public CodeMapperProfile()
        {
            //基础类型映射
            CreateMap<Entity, EntityDTO>();
            CreateMap<EntityDTO, Entity>();

            CreateMap<PropertyAttributeBase, PropertyAttributeBaseDTO>()
                .IncludeBase<Entity, EntityDTO>();
            CreateMap<PropertyAttributeBaseDTO, PropertyAttributeBase>()
                .IncludeBase<EntityDTO, Entity>();

            //数据模型
            CreateMap<ModelDefinition, ModelDefinitionDTO>()
                .IncludeBase<Entity, EntityDTO>()
                .ForMember(target => target.IsEditMode, config => config.Ignore());
            CreateMap<ModelDefinitionDTO, ModelDefinition>()
                .IncludeBase<EntityDTO, Entity>();


            CreateMap<ModelDefField, ModelDefFieldDTO>()
                .IncludeBase<PropertyAttributeBase, PropertyAttributeBaseDTO>()
                .ForMember(target => target.ModelDefinitionName,
                    config =>
                        config.MapFrom(
                            src =>
                                src.ModelDefinition != null
                                    ? src.ModelDefinition.Name
                                    : string.Empty))
                .ForMember(target => target.IsEditMode, config => config.Ignore());
            CreateMap<ModelDefFieldDTO, ModelDefField>()
                .IncludeBase<PropertyAttributeBaseDTO, PropertyAttributeBase>()
                .ForMember(target => target.ModelDefinition, config => config.Ignore());

            //关系模型
            CreateMap<RelationDefinition, RelationDefinitionDTO>()
                .IncludeBase<Entity, EntityDTO>()
                .ForMember(target => target.IsEditMode, config => config.Ignore());
            CreateMap<RelationDefinitionDTO, RelationDefinition>()
                .IncludeBase<EntityDTO, Entity>();

            CreateMap<RelationDefDetail, RelationDefDetailDTO>()
                .IncludeBase<Entity, EntityDTO>()
                .ForMember(target => target.RelationDefinitionName,
                    config =>
                        config.MapFrom(
                            src =>
                                src.RelationDefinition != null
                                    ? src.RelationDefinition.Name
                                    : string.Empty))
                .ForMember(target => target.IsEditMode, config => config.Ignore());
            CreateMap<RelationDefDetailDTO, RelationDefDetail>()
                .IncludeBase<EntityDTO, Entity>()
                .ForMember(target => target.RelationDefinition, config => config.Ignore());

        }
    }
}
