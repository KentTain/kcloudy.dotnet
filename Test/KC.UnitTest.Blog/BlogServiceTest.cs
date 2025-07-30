using System;
using System.Net.Http;
using System.Threading.Tasks;
using KC.Common;
using KC.Framework.Base;
using KC.Framework.Tenant;
using KC.Service.Constants;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace KC.UnitTest.Blog
{
    
    public class BlogServiceTest : BlogTestBase
    {
        private static ILogger _logger;
        public BlogServiceTest(CommonFixture data)
            : base(data)
        {
            _logger = LoggerFactory.CreateLogger(nameof(BlogServiceTest));
        }

    }
}
