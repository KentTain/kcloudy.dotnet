using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using AutoMapper.Internal;
using KC.Service.Blog.AutoMapper.Profile;
using AutoProfile = AutoMapper.Profile;

namespace KC.Service.Blog.AutoMapper
{
    public static class AutoMapperConfiguration
    {
        public static IEnumerable<AutoProfile> GetAllProfiles()
        {
            return new List<AutoProfile>()
            {
                new BlogMapperProfile()
            };
        }
        public static MapperConfiguration Configure()
        {
            return new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<BlogMapperProfile>();
            });
        }

       
    }
}
