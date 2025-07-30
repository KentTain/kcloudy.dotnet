using AutoMapper;
using KC.Service.Component.AutoMapper.Profile;
using System.Collections.Generic;
using AutoProfile = AutoMapper.Profile;

namespace KC.Service.Component.AutoMapper
{
    public static class AutoMapperConfiguration
    {
        public static IEnumerable<AutoProfile> GetAllProfiles()
        {
            return new List<AutoProfile>()
            {
                new ComponentMapperProfile(),
            };
        }
        public static MapperConfiguration Configure()
        {
            return new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ComponentMapperProfile>();
            });
        }
    }
}
