using AutoMapper;
using KC.Service.DTO.Config;
using KC.Framework.Base;
using KC.Service.AutoMapper.Converter;
using KC.Service.Base;

namespace KC.Service.AutoMapper.Profile
{
    public class ConfigMapperProfile : global::AutoMapper.Profile
    {
        public ConfigMapperProfile()
        {
            //Model
           CreateMap<ConfigEntity, EmailConfig>()
                .ConvertUsing<EmailConfigConverter>();
           CreateMap<EmailConfig, ConfigEntity>()
                .ConvertUsing<ConfigEntityToMailConfigConverter>();

           CreateMap<ConfigEntity, SmsConfig>()
                .ConvertUsing<SmsConfigConverter>();
           CreateMap<SmsConfig, ConfigEntity>()
                .ConvertUsing<ConfigEntityToSmsConfigConverter>();

           CreateMap<ConfigEntity, CallConfig>()
                .ConvertUsing<CallConfigConverter>();
           CreateMap<CallConfig, ConfigEntity>()
                .ConvertUsing<ConfigEntityToCallConfigConverter>();

           CreateMap<ConfigEntity, WeixinConfig>()
                .ConvertUsing<WeixinConfigConverter>();
           CreateMap<WeixinConfig, ConfigEntity>()
                .ConvertUsing<ConfigEntityToWeixinConfigConverter>();

            //DTO
           CreateMap<ConfigEntityDTO, EmailConfig>()
                .ConvertUsing<EmailConfigDTOConverter>();
           CreateMap<EmailConfig, ConfigEntityDTO>()
                .ConvertUsing<ConfigEntityDTOToMailConfigConverter>();

           CreateMap<ConfigEntityDTO, SmsConfig>()
                .ConvertUsing<SmsConfigDTOConverter>();
           CreateMap<SmsConfig, ConfigEntityDTO>()
                .ConvertUsing<ConfigEntityDTOToSmsConfigConverter>();

           //CreateMap<ConfigEntityDTO, CallConfig>()
           //     .ConvertUsing<CallConfigDTOConverter>();
           //CreateMap<CallConfig, ConfigEntityDTO>()
           //     .ConvertUsing<ConfigEntityDTOToCallConfigConverter>();

           //CreateMap<ConfigEntityDTO, WeixinConfig>()
           //     .ConvertUsing<WeixinConfigDTOConverter>();
           //CreateMap<WeixinConfig, ConfigEntityDTO>()
           //     .ConvertUsing<ConfigEntityDTOToWeixinConfigConverter>();
        }
    }
}
