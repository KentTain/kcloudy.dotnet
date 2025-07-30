using AutoMapper;
using KC.Framework.Extension;
using KC.Framework.Tenant;
using KC.Framework.Base;
using KC.Service.DTO;
using KC.Model.Message;
using KC.Service.DTO.Message;

namespace KC.Service.Message.AutoMapper.Profile
{
    public partial class MessageMapperProfile : global::AutoMapper.Profile
    {
        public MessageMapperProfile()
        {
            CreateMap<Entity, EntityDTO>();
            CreateMap<EntityDTO, Entity>();

            CreateMap<TreeNode<MessageCategory>, TreeNodeDTO<MessageCategoryDTO>>()
                .IncludeBase<Entity, EntityDTO>();
            CreateMap<TreeNodeDTO<MessageCategoryDTO>, TreeNode<MessageCategory>>()
                .IncludeBase<EntityDTO, Entity>();

            CreateMap<TreeNode<NewsBulletinCategory>, TreeNodeDTO<NewsBulletinCategoryDTO>>()
                .IncludeBase<Entity, EntityDTO>();
            CreateMap<TreeNodeDTO<NewsBulletinCategoryDTO>, TreeNode<NewsBulletinCategory>>()
                .IncludeBase<EntityDTO, Entity>();

            CreateMap<ProcessLogBase, ProcessLogBaseDTO>()
                .ForMember(target => target.TypeString, config => config.Ignore());
            CreateMap<ProcessLogBaseDTO, ProcessLogBase>();

            #region Message
            //Message Category
            CreateMap<MessageCategory, MessageCategoryDTO>()
                .IncludeBase<TreeNode<MessageCategory>, TreeNodeDTO<MessageCategoryDTO>>()
                .ForMember(target => target.Id, config => config.MapFrom(src => src.Id))
                .ForMember(target => target.Text, config => config.MapFrom(src => src.Name))
                .ForMember(target => target.Children, config => config.MapFrom(src => src.ChildNodes));
            CreateMap<MessageCategoryDTO, MessageCategory>()
                .IncludeBase<TreeNodeDTO<MessageCategoryDTO>, TreeNode<MessageCategory>>()
                .ForMember(target => target.Id, config => config.MapFrom(src => src.Id))
                .ForMember(target => target.Name, config => config.MapFrom(src => src.Text))
                .ForMember(target => target.ChildNodes, config => config.Ignore())
                .ForMember(target => target.ParentNode, config => config.Ignore());

            // Message Template
            CreateMap<MessageTemplate, MessageTemplateDTO>()
                .IncludeBase<Entity, EntityDTO>()
                .ForMember(target => target.MessageClassCode,
                    config =>
                        config.MapFrom(m => m.MessageClass != null
                            ? m.MessageClass.Code
                            : string.Empty))
                .ForMember(target => target.MessageClassName,
                    config =>
                        config.MapFrom(m => m.MessageClass != null
                            ? m.MessageClass.Name
                            : string.Empty))
                .ForMember(target => target.ReplaceParametersString,
                    config =>
                        config.MapFrom(m => m.MessageClass != null
                            ? m.MessageClass.ReplaceParametersString
                            : string.Empty));
            CreateMap<MessageTemplateDTO, MessageTemplate>()
                .IncludeBase<EntityDTO, Entity>()
                .ForMember(target => target.MessageClass, config => config.Ignore());

            //Message Class
            CreateMap<MessageClass, MessageClassDTO>()
                .IncludeBase<Entity, EntityDTO>()
                .ForMember(target => target.MessageCategoryName,
                    config =>
                        config.MapFrom(m => m.MessageCategory != null
                            ? m.MessageCategory.Name
                            : string.Empty));
            CreateMap<MessageClassDTO, MessageClass>()
                .IncludeBase<EntityDTO, Entity>()
                .ForMember(target => target.MessageCategory, config => config.Ignore());

            //Message Log
            CreateMap<MessageTemplateLog, MessageTemplateLogDTO>()
                .IncludeBase<ProcessLogBase, ProcessLogBaseDTO>();
            CreateMap<MessageTemplateLogDTO, MessageTemplateLog>()
                .IncludeBase<ProcessLogBaseDTO, ProcessLogBase>();

            //Member Remind Message
            CreateMap<MemberRemindMessage, MemberRemindMessageDTO>()
                .IncludeBase<Entity, EntityDTO>();

            CreateMap<MemberRemindMessageDTO, MemberRemindMessage>()
                .IncludeBase<EntityDTO, Entity>();
            #endregion

            #region NewsBulletin
            //NewsBulletin Category
            CreateMap<NewsBulletinCategory, NewsBulletinCategoryDTO>()
                .IncludeBase<TreeNode<NewsBulletinCategory>, TreeNodeDTO<NewsBulletinCategoryDTO>>()
                .ForMember(target => target.Id, config => config.MapFrom(src => src.Id))
                .ForMember(target => target.Text, config => config.MapFrom(src => src.Name))
                .ForMember(target => target.Children, config => config.MapFrom(src => src.ChildNodes));
            CreateMap<NewsBulletinCategoryDTO, NewsBulletinCategory>()
                .IncludeBase<TreeNodeDTO<NewsBulletinCategoryDTO>, TreeNode<NewsBulletinCategory>>()
                .ForMember(target => target.Id, config => config.MapFrom(src => src.Id))
                .ForMember(target => target.Name, config => config.MapFrom(src => src.Text))
                .ForMember(target => target.ChildNodes, config => config.Ignore())
                .ForMember(target => target.ParentNode, config => config.Ignore());

            // NewsBulletin
            CreateMap<NewsBulletin, NewsBulletinDTO>()
                .IncludeBase<Entity, EntityDTO>()
                .ForMember(target => target.CategoryName,
                    config =>
                        config.MapFrom(m => m.Category != null
                            ? m.Category.Name
                            : string.Empty));
            CreateMap<NewsBulletinDTO, NewsBulletin>()
                .IncludeBase<EntityDTO, Entity>()
                .ForMember(target => target.Category, config => config.Ignore());

            //Message Log
            CreateMap<NewsBulletinLog, NewsBulletinLogDTO>()
                .IncludeBase<ProcessLogBase, ProcessLogBaseDTO>();
            CreateMap<NewsBulletinLogDTO, NewsBulletinLog>()
                .IncludeBase<ProcessLogBaseDTO, ProcessLogBase>();
            #endregion

        }
    }
}
