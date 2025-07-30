using KC.Framework.Base;
using KC.Service.DTO;
using KC.Model.Offering;
using KC.Service.DTO.Offering;

namespace KC.Service.Offering.AutoMapper.Profile
{
    public partial class OfferingMapperProfile : global::AutoMapper.Profile
    {
        public OfferingMapperProfile()
        {
            CreateMap<Entity, EntityDTO>();
            CreateMap<EntityDTO, Entity>();

            CreateMap<TreeNode<Category>, TreeNodeDTO<CategoryDTO>>();
            CreateMap<TreeNodeDTO<CategoryDTO>, TreeNode<Category>>();

            CreateMap<ProcessLogBase, ProcessLogBaseDTO>()
                .ForMember(target => target.TypeString, config => config.Ignore());
            CreateMap<ProcessLogBaseDTO, ProcessLogBase>();

            #region Category
            CreateMap<CategoryOperationLog, CategoryOperationLogDTO>()
                .IncludeBase<ProcessLogBase, ProcessLogBaseDTO>();
            CreateMap<CategoryOperationLogDTO, CategoryOperationLog>()
                .IncludeBase<ProcessLogBaseDTO, ProcessLogBase>();

            CreateMap<CategoryManager, CategoryManagerDTO>();
            CreateMap<CategoryManagerDTO, CategoryManager>();

            CreateMap<PropertyProviderAttr, PropertyProviderAttrDTO>();
            CreateMap<PropertyProviderAttrDTO, PropertyProviderAttr>();

            CreateMap<PropertyProvider, PropertyProviderDTO>();
            CreateMap<PropertyProviderDTO, PropertyProvider>();
            
            CreateMap<Category, CategoryDTO>()
                .ForMember(target => target.IsEditMode, config => config.Ignore())
                .ForMember(target => target.Offerings, config => config.Ignore())
                .IncludeBase<TreeNode<Category>, TreeNodeDTO<CategoryDTO>>();
            CreateMap<CategoryDTO, Category>()
                .ForMember(targe => targe.Offerings, config => config.Ignore())
                .IncludeBase<TreeNodeDTO<CategoryDTO>, TreeNode<Category>>();
            #endregion

            #region Product

            CreateMap<ProductProperty, ProductPropertyDTO>();
            CreateMap<ProductPropertyDTO, ProductProperty>();

            CreateMap<Product, ProductDTO>()
                .IncludeBase<Entity, EntityDTO>();
            CreateMap<ProductDTO, Product>()
                .IncludeBase<EntityDTO, Entity>();

            #endregion 

            #region Offering
            CreateMap<OfferingOperationLog, OfferingOperationLogDTO>()
                .IncludeBase<ProcessLogBase, ProcessLogBaseDTO>();
            CreateMap<OfferingOperationLogDTO, OfferingOperationLog>()
                .IncludeBase<ProcessLogBaseDTO, ProcessLogBase>();

            CreateMap<PropertyProviderAttr, PropertyProviderAttrDTO>();
            CreateMap<PropertyProviderAttrDTO, PropertyProviderAttr>();

            CreateMap<PropertyProvider, PropertyProviderDTO>();
            CreateMap<PropertyProviderDTO, PropertyProvider>();

            CreateMap<OfferingProperty, OfferingPropertyDTO>();
            CreateMap<OfferingPropertyDTO, OfferingProperty>();

            CreateMap<Model.Offering.Offering, OfferingDTO>()
                .ForMember(target => target.CategoryName,
                    config => config.MapFrom(
                        src =>
                            src.Category != null
                                ? src.Category.Name
                                : null))
                .ForMember(target => target.IsEditMode, config => config.Ignore())
                .IncludeBase<Entity, EntityDTO>();
            CreateMap<OfferingDTO, Model.Offering.Offering>()
                .ForMember(targe => targe.Category, config => config.Ignore())
                .IncludeBase<EntityDTO, Entity>();
            #endregion

        }
    }
}
