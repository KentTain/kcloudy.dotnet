using Xunit;
using System;
using System.Collections.Generic;
using System.Text;
using KC.Model.Account;
using KC.Service.DTO.Account;

namespace KC.UnitTest.Account
{
    
    public class AutoMapperConfigurationTest
    {
        [Xunit.Fact]
        public void MapperTest()
        {
            var config = Service.Account.AutoMapper.AutoMapperConfiguration.Configure();
            config.AssertConfigurationIsValid();
        }


        
    }
}
