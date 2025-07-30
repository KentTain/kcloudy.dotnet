using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using AutoMapper.Internal;
using KC.Service.Account.AutoMapper.Profile;
using AutoProfile = AutoMapper.Profile;

namespace KC.Service.Account.AutoMapper
{
    public static class AutoMapperConfiguration
    {
        public static IEnumerable<AutoProfile> GetAllProfiles()
        {
            return new List<AutoProfile>()
            {
                new MenuNodeMapperProfile(),
                new PermissionMapperProfile(),
                new AccountMapperProfile(),
            };
        }
        public static MapperConfiguration Configure()
        {
            var profiles = GetAllProfiles();
            return new MapperConfiguration(cfg =>
            {
                cfg.AddProfiles(profiles);
            });
        }
    }
}
