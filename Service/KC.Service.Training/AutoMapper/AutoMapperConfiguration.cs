using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using AutoMapper.Internal;
using KC.Service.Training.AutoMapper.Profile;
using AutoProfile = AutoMapper.Profile;

namespace KC.Service.Training.AutoMapper
{
    public static class AutoMapperConfiguration
    {
        public static IEnumerable<AutoProfile> GetAllProfiles()
        {
            return new List<AutoProfile>()
            {
                new TrainingMapperProfile()
            };
        }
        public static MapperConfiguration Configure()
        {
            return new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<TrainingMapperProfile>();
            });
        }

       
    }
}
