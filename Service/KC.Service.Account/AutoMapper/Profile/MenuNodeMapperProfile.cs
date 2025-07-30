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
    public class MenuNodeMapperProfile : global::AutoMapper.Profile
    {
        public MenuNodeMapperProfile()
        {
            CreateMap<MenuNode, MenuNodeDTO>()
                .ForMember(target => target.@checked, config => config.Ignore())
                .ForMember(target => target.iconCls, config => config.Ignore())
                .ConvertUsing<MenuNodeConverter>();
            CreateMap<MenuNodeDTO, MenuNode>()
                .ForMember(target => target.ParentNode, config => config.Ignore())
                .ForMember(target => target.MenuRoles, config => config.Ignore())
                .ConvertUsing<MenuNodeDTOConverter>(); 

            CreateMap<MenuNode, MenuNodeSimpleDTO>()
                .ForMember(target => target.Id, config => config.MapFrom(src => src.Id))
                .ForMember(target => target.Text, config => config.MapFrom(src => src.Name))
                .ForMember(target => target.ParentName,
                    config => config.MapFrom(src => src.ParentNode != null ? src.ParentNode.Name : string.Empty))
                .ForMember(target => target.Children, config => config.MapFrom(src => src.ChildNodes))
                .ForMember(target => target.iconCls, config => config.Ignore())
                .ForMember(target => target.@checked, config => config.Ignore());
            
            CreateMap<MenuNodeSimpleDTO, TreeSimpleDTO>()
                .ForMember(target => target.Id, config => config.MapFrom(src => src.Id))
                .ForMember(target => target.Text, config => config.MapFrom(src => src.Text))
                .ForMember(target => target.@checked, config => config.MapFrom(src => src.@checked))
                .ForMember(target => target.Children, config => config.MapFrom(src => src.Children));
            
        }
    }
}
