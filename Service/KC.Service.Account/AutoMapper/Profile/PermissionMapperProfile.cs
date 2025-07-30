using AutoMapper;
using KC.Framework.Base;
using KC.Model.Account;
using KC.Service.Account.AutoMapper.Converter;
using KC.Service.DTO.Account;
using KC.Service.DTO;
using System.Collections.Generic;
using System.Linq;

namespace KC.Service.Account.AutoMapper.Profile
{
    public class PermissionMapperProfile : global::AutoMapper.Profile
    {
        public PermissionMapperProfile()
        {
            CreateMap<Permission, PermissionDTO>()
                .ConvertUsing<PermissionConverter>();
            CreateMap<PermissionDTO, Permission>()
                .ConvertUsing<PermissionDTOConverter>(); ;

            CreateMap<Permission, PermissionSimpleDTO>()
                .ForMember(target => target.Id, config => config.MapFrom(src => src.Id))
                .ForMember(target => target.Text, config => config.MapFrom(src => src.Name))
                .ForMember(target => target.ParentName,
                    config => config.MapFrom(src => src.ParentNode != null ? src.ParentNode.Name : string.Empty))
                .ForMember(target => target.Children, config => config.MapFrom(src => src.ChildNodes))
                .ForMember(target => target.@checked, config => config.Ignore());

            CreateMap<PermissionSimpleDTO, TreeSimpleDTO>()
                .ForMember(target => target.Id, config => config.MapFrom(src => src.Id))
                .ForMember(target => target.Text, config => config.MapFrom(src => src.Text))
                .ForMember(target => target.@checked, config => config.MapFrom(src => src.@checked))
                .ForMember(target => target.Children, config => config.MapFrom(src => src.Children));

            
        }
    }
}
