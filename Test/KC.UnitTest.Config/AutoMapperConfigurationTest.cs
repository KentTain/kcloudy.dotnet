using Xunit;
using System;
using System.Collections.Generic;
using System.Text;

namespace KC.UnitTest.Config
{
    
    public class AutoMapperConfigurationTest
    {
        [Xunit.Fact]
        public void MapperProfileTest()
        {
            var config = Service.Config.AutoMapper.AutoMapperConfiguration.Configure();
            config.AssertConfigurationIsValid();
        }
    }
}
