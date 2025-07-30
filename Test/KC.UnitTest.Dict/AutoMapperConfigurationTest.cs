using System;
using System.Collections.Generic;
using System.Text;

namespace KC.UnitTest.Dict
{
    public class AutoMapperConfigurationTest
    {
        [Xunit.Fact]
        public void MapperProfileTest()
        {
            var config = KC.Service.Dict.AutoMapper.AutoMapperConfiguration.Configure();
            config.AssertConfigurationIsValid();
        }
    }
}
