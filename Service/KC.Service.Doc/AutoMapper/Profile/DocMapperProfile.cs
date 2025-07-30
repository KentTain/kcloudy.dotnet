using AutoMapper;
using KC.Framework.Extension;
using KC.Framework.Tenant;
using KC.Framework.Base;
using KC.Service.DTO;
using KC.Model.Doc;
using KC.Service.DTO.Doc;
using KC.Service.Doc.AutoMapper.Converter;

namespace KC.Service.Doc.AutoMapper.Profile
{
    public partial class DocMapperProfile : global::AutoMapper.Profile
    {
        public DocMapperProfile()
        {
            CreateMap<Entity, EntityDTO>();
            CreateMap<EntityDTO, Entity>();

            CreateMap<ProcessLogBase, ProcessLogBaseDTO>()
                .ForMember(target => target.TypeString, config => config.Ignore());
            CreateMap<ProcessLogBaseDTO, ProcessLogBase>();

            //CreateMap<TreeNode, TreeNodeDTO>()
            //    .IncludeBase<Entity, EntityDTO>();
            //CreateMap<TreeNodeDTO, TreeNode>()
            //    .IncludeBase<EntityDTO, Entity>();

            CreateMap<DocTemplateLog, DocTemplateLogDTO>();
            CreateMap<DocTemplateLogDTO, DocTemplateLog>();

            CreateMap<DocTemplate, DocTemplateDTO>()
                .ForMember(target => target.StatusName, config => config.Ignore())
                .ForMember(target => target.LevelName, config => config.Ignore());
            CreateMap<DocTemplateDTO, DocTemplate>();

            CreateMap<DocumentLog, DocumentLogDTO>()
                .IncludeBase<ProcessLogBase, ProcessLogBaseDTO>();
            CreateMap<DocumentLogDTO, DocumentLog>()
                .IncludeBase<ProcessLogBaseDTO, ProcessLogBase>();

            CreateMap<DocBackup, DocBackupDTO>();
            CreateMap<DocBackupDTO, DocBackup>();

            CreateMap<DocumentInfo, DocumentInfoDTO>()
                .IncludeBase<Entity, EntityDTO>()
                .ForMember(target => target.IsArchive, config => config.Ignore())
                .ForMember(target => target.IsEditMode, config => config.Ignore())
                .ForMember(target => target.TypeName, config => config.Ignore())
                .ForMember(target => target.LevelName, config => config.Ignore());
            CreateMap<DocumentInfoDTO, DocumentInfo>()
                .IncludeBase<EntityDTO, Entity>();

            CreateMap<DocCategory, DocCategoryDTO>()
                .IncludeBase<Entity, EntityDTO>()
                .ForMember(target => target.IsEditMode, config => config.Ignore())
                .ForMember(target => target.@checked, config => config.Ignore())
                .ConvertUsing<DocCategoryConverter>();
            CreateMap<DocCategoryDTO, DocCategory>()
                .IncludeBase<EntityDTO, Entity>()
                .ForMember(target => target.ParentNode, config => config.Ignore())
                .ConvertUsing<DocCategoryDTOConverter>();
        }
    }
}
