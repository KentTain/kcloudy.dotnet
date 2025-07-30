using AutoMapper;
using KC.Framework.Extension;
using KC.Framework.Tenant;
using KC.Framework.Base;
using KC.Service.DTO.Admin;
using KC.Service.DTO;
using KC.Model.Dict;
using KC.Service.DTO.Dict;

namespace KC.Service.Dict.AutoMapper.Profile
{
    public partial class DictMapperProfile : global::AutoMapper.Profile
    {
        public DictMapperProfile()
        {
            CreateMap<Entity, EntityDTO>();
            CreateMap<EntityDTO, Entity>();

            CreateMap<ProcessLogBase, ProcessLogBaseDTO>()
                .ForMember(target => target.TypeString, config => config.Ignore());
            CreateMap<ProcessLogBaseDTO, ProcessLogBase>();

            CreateMap<TreeNode<IndustryClassfication>, TreeNodeDTO<IndustryClassficationDTO>>()
                .IncludeBase<Entity, EntityDTO>()
                .ForMember(target => target.Text, config => config.MapFrom(src => src.Name))
                .ForMember(target => target.Children, config => config.MapFrom(src => src.ChildNodes))
                .ForMember(target => target.@checked, config => config.Ignore());
            CreateMap<TreeNodeDTO<IndustryClassficationDTO>, TreeNode<IndustryClassfication>>()
                .IncludeBase<EntityDTO, Entity>()
                .ForMember(target => target.Name, config => config.MapFrom(src => src.Text))
                .ForMember(target => target.ChildNodes, config => config.Ignore())
                .ForMember(target => target.ParentNode, config => config.Ignore());

            CreateMap<DictValue, DictValueDTO>()
                     .ForMember(target => target.DictTypeName,
                        config => config.MapFrom(
                            src =>
                                src.DictType != null
                                    ? src.DictType.Name
                                    : string.Empty));
            CreateMap<DictValueDTO, DictValue>()
                     .ForMember(target => target.DictType, config => config.Ignore());

            CreateMap<DictType, DictTypeDTO>();
            CreateMap<DictTypeDTO, DictType>();

            CreateMap<City, CityDTO>();
            CreateMap<CityDTO, City>()
                .ForMember(target => target.Province, config => config.Ignore())
                .ForMember(target => target.IsDeleted, config => config.Ignore())
                .ForMember(target => target.CreatedBy, config => config.Ignore())
                .ForMember(target => target.CreatedName, config => config.Ignore())
                .ForMember(target => target.CreatedDate, config => config.Ignore())
                .ForMember(target => target.ModifiedBy, config => config.Ignore())
                .ForMember(target => target.ModifiedName, config => config.Ignore())
                .ForMember(target => target.ModifiedDate, config => config.Ignore());

            CreateMap<Province, ProvinceDTO>();
            CreateMap<ProvinceDTO, Province>()
                .ForMember(target => target.IsDeleted, config => config.Ignore())
                .ForMember(target => target.CreatedBy, config => config.Ignore())
                .ForMember(target => target.CreatedName, config => config.Ignore())
                .ForMember(target => target.CreatedDate, config => config.Ignore())
                .ForMember(target => target.ModifiedBy, config => config.Ignore())
                .ForMember(target => target.ModifiedName, config => config.Ignore())
                .ForMember(target => target.ModifiedDate, config => config.Ignore());

            CreateMap<IndustryClassfication, IndustryClassficationDTO>()
                .IncludeBase<TreeNode<IndustryClassfication>, TreeNodeDTO<IndustryClassficationDTO>>()
                .ForMember(target => target.ParentName, config => config.Ignore());
            CreateMap<IndustryClassficationDTO, IndustryClassfication>()
                .IncludeBase<TreeNodeDTO<IndustryClassficationDTO>, TreeNode<IndustryClassfication>>();

            CreateMap<MobileLocation, MobileLocationDTO>();
            CreateMap<MobileLocationDTO, MobileLocation>();
        }
    }
}
