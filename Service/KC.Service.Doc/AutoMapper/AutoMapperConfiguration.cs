using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.Configuration;
using KC.Service.Doc.AutoMapper.Profile;
using AutoProfile = AutoMapper.Profile;

namespace KC.Service.Doc.AutoMapper
{
    public static class AutoMapperConfiguration
    {
        public static List<AutoProfile> GetAllProfiles()
        {
            return new List<AutoProfile>() { new DocMapperProfile() };
        }
        public static MapperConfiguration Configure()
        {
            return new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<DocMapperProfile>();
            });
        }
    }

}
