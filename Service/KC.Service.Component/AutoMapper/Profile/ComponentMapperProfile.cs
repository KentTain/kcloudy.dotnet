using AutoMapper;
using KC.Framework.Base;
using KC.Component.Base;
using KC.Model.Job.Table;
using KC.Model.Component.Table;
using KC.Service.Component.DTO;

namespace KC.Service.Component.AutoMapper.Profile
{
    public class ComponentMapperProfile : global::AutoMapper.Profile
    {
        public ComponentMapperProfile()
        {
            CreateMap<Entity, StorageEntityDTO>();
            CreateMap<StorageEntityDTO, Entity>();

            CreateMap<AzureTableEntity, AzureTableEntityDTO>()
                .IncludeBase<Entity, StorageEntityDTO>();
            CreateMap<AzureTableEntityDTO, AzureTableEntity>()
                .IncludeBase<StorageEntityDTO, Entity>();


            CreateMap<QueueErrorMessageTable, QueueErrorMessageDTO>()
                .IncludeBase<AzureTableEntity, AzureTableEntityDTO>();
            CreateMap<QueueErrorMessageDTO, QueueErrorMessageTable>()
                .IncludeBase<AzureTableEntityDTO, AzureTableEntity>();

        }
    }
}
