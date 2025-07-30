using Xunit;
using System;
using System.Collections.Generic;
using System.Text;

namespace KC.UnitTest.App
{
    
    public class AutoMapperConfigurationTest
    {
        [Xunit.Fact]
        public void MapperProfileTest()
        {
            var config = Service.App.AutoMapper.AutoMapperConfiguration.Configure();
            config.AssertConfigurationIsValid();
        }
    }
}
