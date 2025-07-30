using AutoMapper;
using KC.Framework.Extension;
using KC.Framework.Tenant;
using KC.Framework.Base;
using KC.Service.DTO;
using KC.Model.Job;

namespace KC.Service.Job.AutoMapper.Profile
{
    public partial class JobMapperProfile : global::AutoMapper.Profile
    {
        public JobMapperProfile()
        {
            CreateMap<Entity, EntityDTO>();
            CreateMap<EntityDTO, Entity>();

            CreateMap<ProcessLogBase, ProcessLogBaseDTO>()
                .ForMember(target => target.TypeString, config => config.Ignore());
            CreateMap<ProcessLogBaseDTO, ProcessLogBase>();

            
        }
    }
}
