using System;
using System.Collections.Generic;
using System.Text;

namespace KC.UnitTest.Pay
{
    public class AutoMapperConfigurationTest
    {
        [Xunit.Fact]
        public void MapperProfileTest()
        {
            var config = KC.Service.Pay.AutoMapper.AutoMapperConfiguration.Configure();
            config.AssertConfigurationIsValid();
        }
    }
}
