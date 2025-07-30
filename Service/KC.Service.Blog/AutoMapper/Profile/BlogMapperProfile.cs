using AutoMapper;
using KC.Service.DTO.Blog;
using KC.Framework.Base;
using KC.Model.Blog;
using KC.Service.DTO;

namespace KC.Service.Blog.AutoMapper.Profile
{
    public class BlogMapperProfile : global::AutoMapper.Profile
    {
        public BlogMapperProfile()
        {
            CreateMap<Entity, EntityDTO>();
            CreateMap<EntityDTO, Entity>();

            CreateMap<Category, CategoryDTO>()
                .ForMember(target => target.IsEditMode, config => config.Ignore());
            CreateMap<CategoryDTO, Category>();

            CreateMap<Comment, CommentDTO>();
            CreateMap<CommentDTO, Comment>();

            CreateMap<KC.Model.Blog.Blog, BlogDTO>()
                .ForMember(target => target.CategoryName,
                    config => config.MapFrom(m =>
                        m.Category != null ? m.Category.Name : string.Empty))
                .ForMember(target => target.IsEditMode, config => config.Ignore())
                .ForMember(target => target.TagList, config => config.Ignore());
            CreateMap<BlogDTO, KC.Model.Blog.Blog>()
                .ForMember(target => target.Category, config => config.Ignore());

            CreateMap<KC.Model.Blog.Blog, BlogSimpleDTO>()
                .ForMember(target => target.CategoryName,
                    config => config.MapFrom(m =>
                        m.Category != null ? m.Category.Name : string.Empty))
                .ForMember(target => target.TagList, config => config.Ignore());

            CreateMap<Setting, SettingDTO>();
            CreateMap<SettingDTO, Setting>();
        }
    }
}
