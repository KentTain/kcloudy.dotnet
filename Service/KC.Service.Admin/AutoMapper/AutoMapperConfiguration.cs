using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using KC.Service.Admin.AutoMapper.Profile;
using AutoProfile = AutoMapper.Profile;

namespace KC.Service.Admin.AutoMapper
{
    public static class AutoMapperConfiguration
    {
        public static List<AutoProfile> GetAllProfiles()
        {
            return new List<AutoProfile>() { new AdminMapperProfile() };
        }

        public static MapperConfiguration Configure()
        {
            return new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<AdminMapperProfile>();
            });
        }
    }

}
