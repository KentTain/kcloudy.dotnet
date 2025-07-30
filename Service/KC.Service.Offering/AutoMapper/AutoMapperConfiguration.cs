using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.Configuration;
using KC.Service.Offering.AutoMapper.Profile;
using AutoProfile = AutoMapper.Profile;

namespace KC.Service.Offering.AutoMapper
{
    public static class AutoMapperConfiguration
    {
        public static List<AutoProfile> GetAllProfiles()
        {
            return new List<AutoProfile>() { new OfferingMapperProfile() };
        }
        public static MapperConfiguration Configure()
        {
            return new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<OfferingMapperProfile>();
            });
        }
    }

}
