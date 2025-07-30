
using KC.Framework.Base;
using KC.Service.DTO;
using KC.Model.Portal;
using KC.Service.DTO.Portal;
using KC.Framework.Extension;

namespace KC.Service.Portal.AutoMapper.Profile
{
    public partial class PortalMapperProfile : global::AutoMapper.Profile
    {
        public PortalMapperProfile()
        {
            CreateMap<Entity, EntityDTO>();
            CreateMap<EntityDTO, Entity>();

            CreateMap<ProcessLogBase, ProcessLogBaseDTO>()
                .ForMember(target => target.TypeString, config => config.Ignore());
            CreateMap<ProcessLogBaseDTO, ProcessLogBase>();

            CreateMap<TreeNode<RecommendCategory>, TreeNodeDTO<RecommendCategoryDTO>>()
                .IncludeBase<Entity, EntityDTO>()
                .ForMember(target => target.Text, config => config.MapFrom(src => src.Name))
                .ForMember(target => target.Children, config => config.MapFrom(src => src.ChildNodes))
                .ForMember(target => target.@checked, config => config.Ignore());
            CreateMap<TreeNodeDTO<RecommendCategoryDTO>, TreeNode<RecommendCategory>>()
                .IncludeBase<EntityDTO, Entity>()
                .ForMember(target => target.Name, config => config.MapFrom(src => src.Text))
                .ForMember(target => target.ChildNodes, config => config.Ignore())
                .ForMember(target => target.ParentNode, config => config.Ignore());

            #region Recommend to WebSiteItem
            CreateMap<RecommendCustomer, WebSiteItemDTO>()
                .IncludeBase<Entity, EntityDTO>()
                .ForMember(target => target.Title, config => config.MapFrom(src => src.RecommendName))
                .ForMember(targe => targe.SubTitle, config => config.MapFrom(src => src.Description))
                .ForMember(target => target.IsImage, config => config.MapFrom(src => true))
                .ForMember(target => target.ImageOrIConCls, config => config.MapFrom(src => src.CompanyLogo))
                .ForMember(target => target.Link, config => config.MapFrom(src => "/Home/CompanyInfo/" + src.RecommendId))
                .ForMember(target => target.LinkIsOpenNewPage, config => config.MapFrom(src => true))
                .ForMember(target => target.IsShow, config => config.MapFrom(src => true))
                .ForMember(target => target.Id, config => config.Ignore())
                .ForMember(target => target.CanEdit, config => config.Ignore())
                .ForMember(target => target.Description, config => config.Ignore())
                .ForMember(target => target.CustomizeContent, config => config.Ignore())
                .ForMember(target => target.WebSiteColumnId, config => config.Ignore())
                .ForMember(target => target.WebSiteColumnName, config => config.Ignore());

            CreateMap<RecommendOffering, WebSiteItemDTO>()
                .IncludeBase<Entity, EntityDTO>()
                .ForMember(target => target.Title, config => config.MapFrom(src => src.RecommendName))
                .ForMember(targe => targe.SubTitle, config => config.MapFrom(src => src.OfferingPrice.HasValue ? src.OfferingPrice.Value.ToString() : "面议"))
                .ForMember(target => target.IsImage, config => config.MapFrom(src => true))
                .ForMember(target => target.ImageOrIConCls, config => config.MapFrom(src => src.OfferingImage))
                .ForMember(target => target.Link, config => config.MapFrom(src => "/Home/ProductDetail/" + src.RecommendId))
                .ForMember(target => target.LinkIsOpenNewPage, config => config.MapFrom(src => true))
                .ForMember(target => target.IsShow, config => config.MapFrom(src => true))
                .ForMember(target => target.Id, config => config.Ignore())
                .ForMember(target => target.CanEdit, config => config.Ignore())
                .ForMember(target => target.Description, config => config.Ignore())
                .ForMember(target => target.CustomizeContent, config => config.Ignore())
                .ForMember(target => target.WebSiteColumnId, config => config.Ignore())
                .ForMember(target => target.WebSiteColumnName, config => config.Ignore());

            CreateMap<RecommendRequirement, WebSiteItemDTO>()
                .IncludeBase<Entity, EntityDTO>()
                .ForMember(target => target.Title, config => config.MapFrom(src => src.RecommendName))
                .ForMember(targe => targe.SubTitle, config => config.MapFrom(src => src.RequirementType.ToDescription()))
                .ForMember(target => target.IsImage, config => config.MapFrom(src => true))
                .ForMember(target => target.ImageOrIConCls, config => config.MapFrom(src => src.RequirementImage))
                .ForMember(target => target.Link, config => config.MapFrom(src => "/Home/RequirementDetail/" + src.RecommendId))
                .ForMember(target => target.LinkIsOpenNewPage, config => config.MapFrom(src => true))
                .ForMember(target => target.IsShow, config => config.MapFrom(src => true))
                .ForMember(target => target.Id, config => config.Ignore())
                .ForMember(target => target.CanEdit, config => config.Ignore())
                .ForMember(target => target.Description, config => config.Ignore())
                .ForMember(target => target.CustomizeContent, config => config.Ignore())
                .ForMember(target => target.WebSiteColumnId, config => config.Ignore())
                .ForMember(target => target.WebSiteColumnName, config => config.Ignore());
            #endregion

            #region WebSite

            //--WebTemplateSite
            CreateMap<WebSiteTemplateItem, WebSiteTemplateItemDTO>()
                .IncludeBase<Entity, EntityDTO>()
                .ForMember(target => target.WebSiteTemplateColumnName, config => config.MapFrom(src => src.WebSiteColumn != null ? src.WebSiteColumn.Title : string.Empty));
            CreateMap<WebSiteTemplateItemDTO, WebSiteTemplateItem>()
                .IncludeBase<EntityDTO, Entity>()
                .ForMember(target => target.WebSiteColumn, config => config.Ignore());

            //--WebTemplateSiteGroup
            CreateMap<WebSiteTemplateColumn, WebSiteTemplateColumnDTO>();
            CreateMap<WebSiteTemplateColumnDTO, WebSiteTemplateColumn>();

            CreateMap<WebSiteTemplateItem, WebSiteItem>()
                .ForMember(target => target.WebSiteColumnId, config => config.Ignore());
            CreateMap<WebSiteTemplateItemDTO, WebSiteItemDTO>()
                .ForMember(target => target.WebSiteColumnId, config => config.Ignore())
                .ForMember(target => target.WebSiteColumnName, config => config.Ignore());
            
            CreateMap<WebSiteTemplateColumn, WebSiteColumn>()
                .ForMember(target => target.WebSitePageId, config => config.Ignore())
                .ForMember(target => target.WebSitePage, config => config.Ignore());
            CreateMap<WebSiteTemplateColumnDTO, WebSiteColumnDTO>()
                .ForMember(target => target.WebSitePageId, config => config.Ignore())
                .ForMember(target => target.WebSitePageName, config => config.Ignore());

            //--WebSiteItem
            CreateMap<WebSiteItem, WebSiteItemDTO>()
                .IncludeBase<Entity, EntityDTO>()
                .ForMember(target => target.WebSiteColumnName, config => config.MapFrom(src => src.WebSiteColumn != null ? src.WebSiteColumn.Title : string.Empty));
            CreateMap<WebSiteItemDTO, WebSiteItem>()
                .IncludeBase<EntityDTO, Entity>()
                .ForMember(target => target.WebSiteColumn, config => config.Ignore());

            //--WebSiteColumn
            CreateMap<WebSiteColumn, WebSiteColumnDTO>()
                .IncludeBase<Entity, EntityDTO>()
                .ForMember(target => target.WebSiteItems, config => config.MapFrom(src => src.WebSiteItems.SingleOrderBy("Index", true)))
                .ForMember(target => target.WebSitePageName, config => config.MapFrom(src =>
                    src.WebSitePage != null ? src.WebSitePage.Name : string.Empty));
            CreateMap<WebSiteColumnDTO, WebSiteColumn>()
                .IncludeBase<EntityDTO, Entity>()
                .ForMember(target => target.WebSitePage, config => config.Ignore());

            //--WebSitePage
            CreateMap<WebSitePage, WebSitePageDTO>()
                .IncludeBase<Entity, EntityDTO>()
                .ForMember(target => target.WebSiteColumns, config => config.MapFrom(src => src.WebSiteColumns.SingleOrderBy("Index", true)))
                .ForMember(target => target.IsEditMode, config => config.Ignore());
            CreateMap<WebSitePageDTO, WebSitePage>()
                .IncludeBase<EntityDTO, Entity>();
            
            #endregion

            #region Company Info

            //CompanyAuthentication
            CreateMap<CompanyAuthentication, CompanyAuthenticationDTO>()
                .IncludeBase<Entity, EntityDTO>()
                .ForMember(target => target.CompanyName,
                    config =>
                        config.MapFrom(src => src.CompanyInfo != null
                            ? src.CompanyInfo.CompanyName
                            : string.Empty))
                .ForMember(target => target.IsEditMode, config => config.Ignore());
            CreateMap<CompanyAuthenticationDTO, CompanyAuthentication>()
                .IncludeBase<EntityDTO, Entity>()
                .ForMember(target => target.CompanyInfo, config => config.Ignore());

            //CompanyAccount
            CreateMap<CompanyAccount, CompanyAccountDTO>()
                .IncludeBase<Entity, EntityDTO>()
                .ForMember(target => target.CompanyName,
                    config =>
                        config.MapFrom(src => src.CompanyInfo != null
                            ? src.CompanyInfo.CompanyName
                            : string.Empty))
                .ForMember(target => target.BankTypeString, config => config.Ignore())
                .ForMember(target => target.BankShowNumber, config => config.Ignore())
                .ForMember(target => target.IsEditMode, config => config.Ignore());
            CreateMap<CompanyAccountDTO, CompanyAccount>()
                .IncludeBase<EntityDTO, Entity>()
                .ForMember(target => target.CompanyInfo, config => config.Ignore());

            //CompanyAddress
            CreateMap<CompanyAddress, CompanyAddressDTO>()
                .IncludeBase<Entity, EntityDTO>()
                .ForMember(target => target.CompanyName,
                    config =>
                        config.MapFrom(src => src.CompanyInfo != null
                            ? src.CompanyInfo.CompanyName
                            : string.Empty))
                .ForMember(target => target.AddressTypeString, config => config.Ignore())
                .ForMember(target => target.IsEditMode, config => config.Ignore());
            CreateMap<CompanyAddressDTO, CompanyAddress>()
                .IncludeBase<EntityDTO, Entity>()
                .ForMember(target => target.CompanyInfo, config => config.Ignore());

            //CompanyContact
            CreateMap<CompanyContact, CompanyContactDTO>()
                .IncludeBase<Entity, EntityDTO>()
                .ForMember(target => target.CompanyName,
                    config =>
                        config.MapFrom(src => src.CompanyInfo != null
                            ? src.CompanyInfo.CompanyName
                            : string.Empty))
                .ForMember(target => target.BusinessTypeString, config => config.Ignore())
                .ForMember(target => target.IsEditMode, config => config.Ignore());
            CreateMap<CompanyContactDTO, CompanyContact>()
                .IncludeBase<EntityDTO, Entity>()
                .ForMember(target => target.CompanyInfo, config => config.Ignore());

            //Company's Process Log
            CreateMap<CompanyProcessLog, CompanyProcessLogDTO>()
                .IncludeBase<ProcessLogBase, ProcessLogBaseDTO>();
            CreateMap<CompanyProcessLogDTO, CompanyProcessLog>()
                .IncludeBase<ProcessLogBaseDTO, ProcessLogBase>();

            //Company Information
            CreateMap<CompanyInfo, CompanyInfoDTO>()
                .ForMember(target => target.AuthenticationInfo, config => config.MapFrom(src => src.AuthenticationInfo))
                .ForMember(target => target.CompanyContacts, config => config.MapFrom(src => src.CompanyContacts))
                .ForMember(target => target.CompanyAccounts, config => config.MapFrom(src => src.CompanyAccounts))
                .ForMember(target => target.CompanyAddresses, config => config.MapFrom(src => src.CompanyAddresses))
                .ForMember(target => target.IsEditMode, config => config.Ignore());
            CreateMap<CompanyInfoDTO, CompanyInfo>();
            #endregion

            #region Recommend
            //--RecommendCustomerLog
            CreateMap<RecommendCustomerLog, RecommendCustomerLogDTO>()
                .IncludeBase<ProcessLogBase, ProcessLogBaseDTO>();
            CreateMap<RecommendCustomerLogDTO, RecommendCustomerLog>()
                .IncludeBase<ProcessLogBaseDTO, ProcessLogBase>();

            //--RecommendCustomer
            CreateMap<RecommendCustomer, RecommendCustomerDTO>()
                .IncludeBase<Entity, EntityDTO>()
                .ForMember(target => target.CategoryName,
                    config => config.MapFrom(
                        src => src.Category != null ? src.Category.Name : string.Empty))
                .ForMember(target => target.IsEditMode, config => config.Ignore());
            CreateMap<RecommendCustomerDTO, RecommendCustomer>()
                .IncludeBase<EntityDTO, Entity>()
                .ForMember(targe => targe.Category, config => config.Ignore());

            CreateMap<RecommendCustomer, RecommendInfoDTO>()
                .IncludeBase<Entity, EntityDTO>()
                .ForMember(target => target.RecommendImage, config => config.MapFrom(src => src.CompanyLogo))
                .ForMember(target => target.CategoryName,
                    config => config.MapFrom(
                        src => src.Category != null ? src.Category.Name : string.Empty))
                .ForMember(targe => targe.Type, config => config.MapFrom(src => RecommendType.Customer))
                .ForMember(target => target.RecommendFile, config => config.Ignore())
                .ForMember(target => target.IsEditMode, config => config.Ignore());

            //--RecommendRequirementLog
            CreateMap<RecommendRequirementLog, RecommendRequirementLogDTO>()
                .IncludeBase<ProcessLogBase, ProcessLogBaseDTO>();
            CreateMap<RecommendRequirementLogDTO, RecommendRequirementLog>()
                .IncludeBase<ProcessLogBaseDTO, ProcessLogBase>();

            //--RecommendMaterial
            CreateMap<RecommendMaterial, RecommendMaterialDTO>()
                .IncludeBase<Entity, EntityDTO>()
                .ForMember(target => target.CategoryName,
                    config => config.MapFrom(
                        src => src.Category != null ? src.Category.Name : string.Empty))
                .ForMember(targe => targe.IsEditMode, config => config.Ignore());
            CreateMap<RecommendMaterialDTO, RecommendMaterial>()
                .IncludeBase<EntityDTO, Entity>()
                .ForMember(targe => targe.Category, config => config.Ignore())
                .ForMember(targe => targe.RequirementForMaterials, config => config.Ignore())
                .ForMember(targe => targe.RecommendRequirements, config => config.Ignore());

            //--RecommendRequirement
            CreateMap<RecommendRequirement, RecommendRequirementDTO>()
                .IncludeBase<Entity, EntityDTO>()
                .ForMember(target => target.RecommendMaterials, config => config.MapFrom(src => src.RecommendMaterials))
                .ForMember(target => target.CategoryName,
                    config => config.MapFrom(
                        src => src.Category != null ? src.Category.Name : string.Empty))
                .ForMember(target => target.IsEditMode, config => config.Ignore());
            CreateMap<RecommendRequirementDTO, RecommendRequirement>()
                .IncludeBase<EntityDTO, Entity>()
                .ForMember(target => target.Category, config => config.Ignore())
                .ForMember(targe => targe.RecommendMaterials, config => config.Ignore())
                .ForMember(target => target.RequirementForMaterials, config => config.Ignore());

            CreateMap<RecommendRequirement, RecommendInfoDTO>()
                .IncludeBase<Entity, EntityDTO>()
                .ForMember(target => target.RecommendImage, config => config.MapFrom(src => src.RequirementImage))
                .ForMember(target => target.RecommendFile, config => config.MapFrom(src => src.RequirementFile))
                .ForMember(target => target.CategoryName,
                    config => config.MapFrom(
                        src => src.Category != null ? src.Category.Name : string.Empty))
                .ForMember(targe => targe.Type, config => config.MapFrom(src => RecommendType.Requirement))
                .ForMember(target => target.IsEditMode, config => config.Ignore());

            //--RecommendOfferingLog
            CreateMap<RecommendOfferingLog, RecommendOfferingLogDTO>()
                .IncludeBase<ProcessLogBase, ProcessLogBaseDTO>();
            CreateMap<RecommendOfferingLogDTO, RecommendOfferingLog>()
                .IncludeBase<ProcessLogBaseDTO, ProcessLogBase>();

            //--RecommendOffering
            CreateMap<RecommendOffering, RecommendOfferingDTO>()
                .IncludeBase<Entity, EntityDTO>()
                .ForMember(target => target.CategoryName,
                    config => config.MapFrom(
                        src => src.Category != null ? src.Category.Name : string.Empty))
                .ForMember(target => target.IsEditMode, config => config.Ignore());
            CreateMap<RecommendOfferingDTO, RecommendOffering>()
                .IncludeBase<EntityDTO, Entity>()
                .ForMember(targe => targe.Category, config => config.Ignore());

            CreateMap<RecommendOffering, RecommendInfoDTO>()
                .IncludeBase<Entity, EntityDTO>()
                .ForMember(target => target.RecommendImage, config => config.MapFrom(src => src.OfferingImage))
                .ForMember(target => target.RecommendFile, config => config.MapFrom(src => src.OfferingFile))
                .ForMember(target => target.CategoryName,
                    config => config.MapFrom(
                        src => src.Category != null ? src.Category.Name : string.Empty))
                .ForMember(targe => targe.Type, config => config.MapFrom(src => RecommendType.Offering))
                .ForMember(target => target.IsEditMode, config => config.Ignore());

            //--RecommendCategory
            CreateMap<RecommendCategory, RecommendCategoryDTO>()
                .IncludeBase<TreeNode<RecommendCategory>, TreeNodeDTO<RecommendCategoryDTO>>()
                .ForMember(target => target.Id, config => config.MapFrom(src => src.Id))
                .ForMember(target => target.Text, config => config.MapFrom(src => src.Name))
                .ForMember(target => target.ParentName,
                    config => config.MapFrom(
                        src => src.ParentNode != null ? src.ParentNode.Name : string.Empty))
                .ForMember(target => target.Children, config => config.MapFrom(src => src.ChildNodes))
                .ForMember(target => target.RecommendOfferings, config => config.MapFrom(src => src.RecommendOfferings))
                .ForMember(target => target.RecommendCustomers, config => config.MapFrom(src => src.RecommendCustomers))
                .ForMember(target => target.RecommendRequirements, config => config.MapFrom(src => src.RecommendRequirements))
                .ForMember(target => target.IsEditMode, config => config.Ignore())
                .ForMember(target => target.@checked, config => config.Ignore());
            CreateMap<RecommendCategoryDTO, RecommendCategory>()
                .IncludeBase<TreeNodeDTO<RecommendCategoryDTO>, TreeNode<RecommendCategory>>()
                .ForMember(target => target.Id, config => config.MapFrom(src => src.Id))
                .ForMember(target => target.Name, config => config.MapFrom(src => src.Text))
                .ForMember(target => target.ChildNodes, config => config.MapFrom(src => src.Children))
                .ForMember(target => target.RecommendOfferings, config => config.MapFrom(src => src.RecommendOfferings))
                .ForMember(target => target.RecommendCustomers, config => config.MapFrom(src => src.RecommendCustomers))
                .ForMember(target => target.RecommendRequirements, config => config.MapFrom(src => src.RecommendRequirements));


            #endregion

            #region  Web Info
            //WebSiteInfoDTO
            CreateMap<WebSiteInfo, WebSiteInfoDTO>()
                .IncludeBase<Entity, EntityDTO>()
                .ForMember(target => target.IsEditMode, config => config.Ignore())
                .ForMember(target => target.HomePageSlideBlobs, config => config.Ignore());
            CreateMap<WebSiteInfoDTO, WebSiteInfo>()
                .IncludeBase<EntityDTO, Entity>();
            
            //WebSiteLink
            CreateMap<WebSiteLink, WebSiteLinkDTO>()
                .IncludeBase<Entity, EntityDTO>()
                .ForMember(target => target.IsEnableString, config => config.MapFrom(src => src.IsEnable ? "是" : "否"))
                .ForMember(target => target.IsNavString, config => config.MapFrom(src => src.IsNav ? "是" : "否"))
                .ForMember(target => target.LinkTypeString, config => config.MapFrom(src => src.LinkType.ToDescription()))
                .ForMember(target => target.IsEditMode, config => config.Ignore());
            CreateMap<WebSiteLinkDTO, WebSiteLink>()
                .IncludeBase<EntityDTO, Entity>();

            CreateMap<FavoriteInfo, FavoriteInfoDTO>()
                .IncludeBase<Entity, EntityDTO>();
            CreateMap<FavoriteInfoDTO, FavoriteInfo>()
                .IncludeBase<EntityDTO, Entity>();
            #endregion

        }
    }
}
