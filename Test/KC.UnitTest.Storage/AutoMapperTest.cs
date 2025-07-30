using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using KC.Model.Component.Table;
using KC.Model.Job.Table;
using KC.Service.Component.AutoMapper;
using KC.Service.Component.DTO;
using Xunit;

namespace KC.Service.Component.UnitTest
{
    
    public class AutoMapperTest
    {
        [Xunit.Fact]
        public void AutoMapper_Test()
        {
            var config = AutoMapperConfiguration.Configure();
            var mapper = config.CreateMapper();
            //var nlog = new NLogEntity()
            //{
            //    LoggerName = "test",
            //    Level = "I",
            //    MachineName = "Kent",
            //    CreatedBy = "admin",
            //    CreatedDate = DateTime.UtcNow,
            //    ModifiedBy = "admin-test",
            //    ModifiedDate = DateTime.UtcNow
            //};

            //var nlogDto = mapper.Map<NLogEntityDTO>(nlog);
            //Assert.Equal(nlog.LoggerName, nlogDto.LoggerName);
            //Assert.Equal(nlog.Level, nlogDto.Level);
            //Assert.Equal(nlog.CreatedBy, nlogDto.CreatedBy);
            //Assert.Equal(nlog.CreatedDate, nlogDto.CreatedDate);
        }
    }
}
