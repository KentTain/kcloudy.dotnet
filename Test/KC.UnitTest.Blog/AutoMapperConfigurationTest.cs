using Xunit;
using System;
using System.Collections.Generic;
using System.Text;

namespace KC.UnitTest.Blog
{
    
    public class AutoMapperConfigurationTest
    {
        [Xunit.Fact]
        public void MapperProfileTest()
        {
            var config = Service.Blog.AutoMapper.AutoMapperConfiguration.Configure();
            config.AssertConfigurationIsValid();
        }
    }
}
