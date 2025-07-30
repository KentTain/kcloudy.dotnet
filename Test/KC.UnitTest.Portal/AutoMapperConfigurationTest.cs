using System;
using System.Collections.Generic;
using System.Text;

namespace KC.UnitTest.Portal
{
    public class AutoMapperConfigurationTest
    {
        [Xunit.Fact]
        public void MapperProfileTest()
        {
            var config = KC.Service.Portal.AutoMapper.AutoMapperConfiguration.Configure();
            config.AssertConfigurationIsValid();
        }
    }
}
