using Xunit;
using System;
using System.Collections.Generic;
using System.Text;

namespace KC.UnitTest.Training
{
    
    public class AutoMapperConfigurationTest
    {
        [Xunit.Fact]
        public void MapperProfileTest()
        {
            var config = KC.Service.Training.AutoMapper.AutoMapperConfiguration.Configure();
            config.AssertConfigurationIsValid();
        }
    }
}
