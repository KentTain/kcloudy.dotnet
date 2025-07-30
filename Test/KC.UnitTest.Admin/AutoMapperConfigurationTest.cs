using Xunit;
using System;
using System.Collections.Generic;
using System.Text;

namespace KC.UnitTest.Admin
{
    
    public class AutoMapperConfigurationTest
    {
        [Xunit.Fact]
        public void MapperProfileTest()
        {
            var config = Service.Admin.AutoMapper.AutoMapperConfiguration.Configure();
            config.AssertConfigurationIsValid();
        }

        [Xunit.Fact]
        public void ServiceMapperTest()
        {
            var config = Service.AutoMapper.AutoMapperConfiguration.Configure();
            config.AssertConfigurationIsValid();
        }

        [Xunit.Fact]
        public void ComponetMapperTest()
        {
            var config = Service.Component.AutoMapper.AutoMapperConfiguration.Configure();
            config.AssertConfigurationIsValid();
        }
    }
}
