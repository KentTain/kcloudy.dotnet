using System;
using System.Collections.Generic;
using System.Text;

namespace KC.UnitTest.Supplier
{
    public class AutoMapperConfigurationTest
    {
        [Xunit.Fact]
        public void MapperProfileTest()
        {
            var config = KC.Service.Supplier.AutoMapper.AutoMapperConfiguration.Configure();
            config.AssertConfigurationIsValid();
        }
    }
}
