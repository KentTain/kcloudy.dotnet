using System;
using System.Collections.Generic;
using System.Text;

namespace KC.UnitTest.Contract
{
    public class AutoMapperConfigurationTest
    {
        [Xunit.Fact]
        public void MapperProfileTest()
        {
            var config = KC.Service.Contract.AutoMapper.AutoMapperConfiguration.Configure();
            config.AssertConfigurationIsValid();
        }
    }
}
