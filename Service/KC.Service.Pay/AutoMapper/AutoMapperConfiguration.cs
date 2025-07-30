using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.Configuration;
using KC.Service.Pay.AutoMapper.Profile;
using AutoProfile = AutoMapper.Profile;

namespace KC.Service.Pay.AutoMapper
{
    public static class AutoMapperConfiguration
    {
        public static List<AutoProfile> GetAllProfiles()
        {
            return new List<AutoProfile>() { new PayMapperProfile() };
        }
        public static MapperConfiguration Configure()
        {
            return new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<PayMapperProfile>();
            });
        }
    }

}
