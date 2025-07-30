using AutoMapper;
using KC.Framework.Base;
using KC.Service.DTO;
using KC.Model.Customer;
using KC.Service.DTO.Customer;
using System;
using KC.Enums.CRM;
using System.Collections.Generic;

namespace KC.Service.Customer.AutoMapper.Profile
{
    public partial class CustomerMapperProfile : global::AutoMapper.Profile
    {
        public CustomerMapperProfile()
        {
            CreateMap<Entity, EntityDTO>();
            CreateMap<EntityDTO, Entity>();

            CreateMap<ProcessLogBase, ProcessLogBaseDTO>()
                .ForMember(target => target.TypeString, config => config.Ignore());
            CreateMap<ProcessLogBaseDTO, ProcessLogBase>();

            //Customer Change Log
            CreateMap<CustomerChangeLog, CustomerChangeLogDTO>()
                .ForMember(target => target.CustomerName,
                    config =>
                        config.MapFrom(m => m.CustomerInfo != null
                            ? m.CustomerInfo.CustomerName
                            : string.Empty));
            CreateMap<CustomerChangeLogDTO, CustomerChangeLog>()
                .ForMember(target => target.CustomerInfo, config => config.Ignore());

            //Customer Tracing's Log
            CreateMap<CustomerTracingLog, CustomerTracingLogDTO>()
                .IncludeBase<ProcessLogBase, ProcessLogBaseDTO>()
                .ForMember(target => target.TypeString, config => config.Ignore())
                .ForMember(target => target.TracingTypeName, config => config.Ignore())
                .ForMember(target => target.CompanyTypeName, config => config.Ignore())
                .ForMember(target => target.StartTime,
                    src =>
                        src.MapFrom(
                            source =>
                                !source.StartTime.HasValue ? default(DateTime?) : source.StartTime.Value.ToLocalTime()));

            CreateMap<CustomerTracingLogDTO, CustomerTracingLog>()
                .IncludeBase<ProcessLogBaseDTO, ProcessLogBase>()
                .ForMember(target => target.CustomerInfo, config => config.Ignore());

            //Customer SendToTenant Log
            CreateMap<CustomerSendToTenantLog, CustomerSendToTenantLogDTO>()
                .IncludeBase<ProcessLogBase, ProcessLogBaseDTO>()
                .ForMember(target => target.TypeString, config => config.Ignore());
            CreateMap<CustomerSendToTenantLogDTO, CustomerSendToTenantLog>()
                .IncludeBase<ProcessLogBaseDTO, ProcessLogBase>();

            //Customer Contact Info
            CreateMap<CustomerContact, CustomerContactDTO>()
                .IncludeBase<Entity, EntityDTO>();
            CreateMap<CustomerContactDTO, CustomerContact>()
                .IncludeBase<EntityDTO, Entity>()
                .ForMember(target => target.CustomerInfo, config => config.Ignore());

            //Customer Account
            CreateMap<CustomerAccount, CustomerAccountDTO>()
                .IncludeBase<Entity, EntityDTO>();
            CreateMap<CustomerAccountDTO, CustomerAccount>()
                .IncludeBase<EntityDTO, Entity>()
                .ForMember(target => target.CustomerInfo, config => config.Ignore());

            //Customer Address
            CreateMap<CustomerAddress, CustomerAddressDTO>()
                .IncludeBase<Entity, EntityDTO>();
            CreateMap<CustomerAddressDTO, CustomerAddress>()
                .IncludeBase<EntityDTO, Entity>()
                .ForMember(target => target.CustomerInfo, config => config.Ignore());

            //Customer Authentication
            CreateMap<CustomerAuthentication, CustomerAuthenticationDTO>()
                .IncludeBase<Entity, EntityDTO>();
            CreateMap<CustomerAuthenticationDTO, CustomerAuthentication>()
                .IncludeBase<EntityDTO, Entity>();

            //Customer Extend Info Provider
            CreateMap<CustomerExtInfoProvider, CustomerExtInfoProviderDTO>()
                .IncludeBase<Entity, EntityDTO>();
            CreateMap<CustomerExtInfoProviderDTO, CustomerExtInfoProvider>()
                .IncludeBase<EntityDTO, Entity>();

            //Customer Extend Info 
            CreateMap<CustomerExtInfo, CustomerExtInfoDTO>()
                .IncludeBase<Entity, EntityDTO>()
                .ForMember(target => target.CustomerName,
                    config =>
                        config.MapFrom(m => m.CustomerInfo != null
                            ? m.CustomerInfo.CustomerName
                            : string.Empty));
            CreateMap<CustomerExtInfoDTO, CustomerExtInfo>()
                .IncludeBase<EntityDTO, Entity>()
                .ForMember(target => target.CustomerInfo, config => config.Ignore())
                .ForMember(target => target.CustomerExtInfoProvider, config => config.Ignore());

            //Convert the extend info's Provider to extend info
            CreateMap<CustomerExtInfoProvider, CustomerExtInfo>()
                .ForMember(target => target.CustomerExtInfoProviderId,
                    config => config.MapFrom(m => m.PropertyAttributeId))
                .ForMember(target => target.PropertyAttributeId, config => config.Ignore())
                .ForMember(target => target.CustomerId, config => config.Ignore())
                .ForMember(target => target.CustomerInfo, config => config.Ignore())
                .ForMember(target => target.CustomerExtInfoProvider, config => config.Ignore());

            CreateMap<CustomerInfo, CustomerInfoDTO>()
                .IncludeBase<Entity, EntityDTO>()
                .ForMember(target => target.RecommandedUserName,
                    source =>
                        source.MapFrom(
                            src =>
                                !string.IsNullOrEmpty(src.RecommandedUserName) ? src.RecommandedUserName : src.CreatedBy));
            CreateMap<CustomerInfoDTO, CustomerInfo>()
                .IncludeBase<EntityDTO, Entity>();

            CreateMap<NotificationApplication, NotificationApplicationDTO>()
                .IncludeBase<Entity, EntityDTO>()
                .ForMember(target => target.ApplicantDateTime,
                    source => source.MapFrom(src => src.ApplicantDateTime.ToLocalTime()));
            CreateMap<NotificationApplicationDTO, NotificationApplication>()
                .IncludeBase<EntityDTO, Entity>();

            //CustomerManager
            CreateMap<CustomerManager, CustomerManagerDTO>();
            CreateMap<CustomerManagerDTO, CustomerManager>()
                .ForMember(target => target.CustomerInfo, config => config.Ignore());

            //CustomerSeas
            CreateMap<CustomerSeas, CustomerSeasDTO>();
            CreateMap<CustomerSeasDTO, CustomerSeas>()
                .ForMember(target => target.CustomerInfo, config => config.Ignore());

            //CustomerSeasInfo
            CreateMap<CustomerSeas, CustomerSeasInfoDTO>()
                .ForMember(target => target.CustomerName,
                    source => source.MapFrom(
                        src => src.CustomerInfo != null 
                            ? src.CustomerInfo.CustomerName : string.Empty))
                .ForMember(target => target.ContactName,
                    source => source.MapFrom(
                        src => src.CustomerInfo != null
                            ? src.CustomerInfo.ContactName : string.Empty))
                .ForMember(target => target.CustomerId,
                    source => source.MapFrom(
                        src => src.CustomerInfo != null
                            ? src.CustomerInfo.ContactId : string.Empty))
                .ForMember(target => target.CustomerType,
                    source => source.MapFrom(
                        src => src.CustomerInfo != null
                            ? src.CustomerInfo.CompanyType : CompanyType.Supplier));

            //CustomerContactInfo
            CreateMap<CustomerContact, CustomerContactInfoDTO>()
                .ForMember(target => target.CustomerContactId, config => config.MapFrom(m => m.Id))
                .ForMember(target => target.ContactPhoneNumber, config => config.MapFrom(m => m.ContactPhoneNumber))
                .ForMember(target => target.IsDefaultCustomerContact, config => config.MapFrom(m => m.IsDefault))
                .ForMember(target => target.CustomerName,
                    source => source.MapFrom(
                        src => src.CustomerInfo != null
                            ? src.CustomerInfo.CustomerName : string.Empty))
                .ForMember(target => target.CompanyType,
                    source => source.MapFrom(
                        src => src.CustomerInfo != null
                            ? src.CustomerInfo.CompanyType : CompanyType.Supplier))
                .ForMember(target => target.ClientType,
                    source => source.MapFrom(
                        src => src.CustomerInfo != null
                            ? src.CustomerInfo.ClientType : ClientType.Potential))
                .ForMember(target => target.CustomerManagers,
                    source => source.MapFrom(
                        src => src.CustomerInfo != null
                            ? src.CustomerInfo.CustomerManagers : new List<CustomerManager>()))
                .ForMember(target => target.CompanyTypeStr, config => config.Ignore())
                .ForMember(target => target.ClientTypeStr, config => config.Ignore());
            //CreateMap<CustomerContactInfoDTO, CustomerContact>();
        }
    }
}
