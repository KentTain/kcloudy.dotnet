using AutoMapper;
using KC.Framework.Extension;
using KC.Framework.Tenant;
using KC.Framework.Base;
using KC.Service.DTO.Admin;
using KC.Service.DTO;
using KC.Model.Supplier;
using KC.Service.DTO.Supplier;

namespace KC.Service.Supplier.AutoMapper.Profile
{
    public partial class SupplierMapperProfile : global::AutoMapper.Profile
    {
        public SupplierMapperProfile()
        {
            CreateMap<Entity, EntityDTO>();
            CreateMap<EntityDTO, Entity>();

            CreateMap<ProcessLogBase, ProcessLogBaseDTO>()
                .ForMember(target => target.TypeString, config => config.Ignore());
            CreateMap<ProcessLogBaseDTO, ProcessLogBase>();

            CreateMap<TreeNode<IndustryClassfication>, TreeNodeDTO<IndustryClassficationDTO>>()
                .IncludeBase<Entity, EntityDTO>();
            CreateMap<TreeNodeDTO<IndustryClassficationDTO>, TreeNode<IndustryClassfication>>()
                .IncludeBase<EntityDTO, Entity>();

            CreateMap<Province, ProvinceDTO>();
            CreateMap<ProvinceDTO, Province>();

            CreateMap<City, CityDTO>();
            CreateMap<CityDTO, City>();

            CreateMap<IndustryClassfication, IndustryClassficationDTO>()
                .IncludeBase<TreeNode<IndustryClassfication>, TreeNodeDTO<IndustryClassficationDTO>>();
            CreateMap<IndustryClassficationDTO, IndustryClassfication>()
                .IncludeBase<TreeNodeDTO<IndustryClassficationDTO>, TreeNode<IndustryClassfication>>();

            CreateMap<MobileLocation, MobileLocationDTO>();
            CreateMap<MobileLocationDTO, MobileLocation>();
        }
    }
}
