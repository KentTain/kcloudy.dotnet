using Xunit;
using System;
using System.Collections.Generic;
using System.Text;

namespace KC.UnitTest.CodeGenerate
{
    
    public class AutoMapperConfigurationTest
    {
        [Xunit.Fact]
        public void MapperProfileTest()
        {
            var config = Service.CodeGenerate.AutoMapper.AutoMapperConfiguration.Configure();
            config.AssertConfigurationIsValid();
        }
    }
}
