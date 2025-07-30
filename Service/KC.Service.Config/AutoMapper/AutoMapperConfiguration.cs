using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using AutoMapper.Internal;
using KC.Service.Config.AutoMapper.Profile;
using AutoProfile = AutoMapper.Profile;

namespace KC.Service.Config.AutoMapper
{
    public static class AutoMapperConfiguration
    {
        public static IEnumerable<AutoProfile> GetAllProfiles()
        {
            return new List<AutoProfile>()
            {
                new ConfigMapperProfile()
            };
        }
        public static MapperConfiguration Configure()
        {
            return new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ConfigMapperProfile>();
            });
        }

       
    }
}
