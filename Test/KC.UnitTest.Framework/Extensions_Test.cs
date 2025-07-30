using System;
using System.Collections.Generic;
using System.Linq;
using KC.Framework.Base;
using KC.Framework.Extension;
using KC.Framework.Tenant;
using Xunit;

namespace KC.Framework.UnitTest
{
    
    public class Extensions_Test : KC.UnitTest.Framework.FrameworkTestBase
    {
        public Extensions_Test(KC.UnitTest.CommonFixture data)
            : base(data)
        {
        }

        [Xunit.Fact]
        public void Test_TypeExtensions()
        {
            var dResult1 = typeof(TestQueueInfo).IsSameOrSubclass(typeof(TestQueueInfo));//true
            var dResult2 = typeof(TestQueueInfo).IsSameOrSubclass(typeof(QueueEntity));//true
            Assert.True(dResult1);
            Assert.True(dResult2);

            var bResult = typeof(Test).IsSubclassOf(typeof(GenericInterface<>));//false
            var bResult1 = typeof(Test).IsSubclassOf(typeof(GenericClass<>));//false
            var bResult2 = typeof(Test).IsSubclassOf(typeof(GenericInterface<TestQueueInfo>));//true
            var bResult3 = typeof(Test).IsSubclassOf(typeof(GenericClass<TestQueueInfo>));//true
            var bResult4 = typeof(Test).IsSubclassOf(typeof(GenericInterface<QueueEntity>));//false
            var bResult5 = typeof(Test).IsSubclassOf(typeof(GenericClass<QueueEntity>));//false
            var bResult6 = typeof(Test).IsSubclassOf(typeof(Test));//false
            Assert.False(bResult);
            Assert.False(bResult1);
            Assert.True(bResult2);
            Assert.True(bResult3);
            Assert.False(bResult4);
            Assert.False(bResult5);
            Assert.False(bResult6);

            var aResult = typeof(Test).IsSubclassOfRawGeneric(typeof(GenericInterface<>));//true
            var aResult1 = typeof(Test).IsSubclassOfRawGeneric(typeof(GenericClass<>));//true
            var aResult2 = typeof(Test).IsSubclassOfRawGeneric(typeof(GenericInterface<TestQueueInfo>));//false
            var aResult3 = typeof(Test).IsSubclassOfRawGeneric(typeof(GenericClass<TestQueueInfo>));//false
            var aResult4 = typeof(Test).IsSubclassOfRawGeneric(typeof(GenericInterface<QueueEntity>));//false
            var aResult5 = typeof(Test).IsSubclassOfRawGeneric(typeof(GenericClass<QueueEntity>));//false
            var aResult6 = typeof(Test).IsSubclassOfRawGeneric(typeof(Test));//true
            Assert.True(aResult);
            Assert.True(aResult1);
            Assert.False(aResult2);
            Assert.False(aResult3);
            Assert.False(aResult4);
            Assert.False(aResult5);
            Assert.True(aResult6);

            var cResult = typeof(Test).IsSubclassOfRawClass(typeof(GenericInterface<>));//true
            var cResult1 = typeof(Test).IsSubclassOfRawClass(typeof(GenericClass<>));//true
            var cResult2 = typeof(Test).IsSubclassOfRawClass(typeof(GenericInterface<TestQueueInfo>));//true
            var cResult3 = typeof(Test).IsSubclassOfRawClass(typeof(GenericClass<TestQueueInfo>));//true
            var cResult4 = typeof(Test).IsSubclassOfRawClass(typeof(GenericInterface<QueueEntity>));//true
            var cResult5 = typeof(Test).IsSubclassOfRawClass(typeof(GenericClass<QueueEntity>));//true
            var cResult6 = typeof(Test).IsSubclassOfRawClass(typeof(Test));//true
            Assert.True(cResult);
            Assert.True(cResult1);
            Assert.True(cResult2);
            Assert.True(cResult3);
            Assert.True(cResult4);
            Assert.True(cResult5);
            Assert.True(cResult6);
        }

        [Xunit.Fact]
        public void Test_DateTimeExtensions()
        {
            var nextZeroHoutDay = DateTime.UtcNow.GetZeroHourOfTheNextDay();
            Assert.Equal(DateTime.UtcNow.Day + 1, nextZeroHoutDay.Day);
            Assert.Equal(0, nextZeroHoutDay.Hour);
            Assert.Equal(0, nextZeroHoutDay.Minute);
            Assert.Equal(DateTimeKind.Utc, nextZeroHoutDay.Kind);
        }

        [Xunit.Fact]
        public void Test_StringExtension()
        {
            var result1 = "13611112222".IsMobile();
            Assert.True(result1);
            var result2 = "11211112222".IsMobile();
            Assert.False(result2);

            var result3 = "http://www.baidu.com/136111122.22.mp3".Split('.').LastOrDefault();
            Assert.Equal("mp3", result3);

            #region 根据url获取TenantName
            var result4 = "127.0.0.1".GetTenantNameByHost();
            Assert.Equal(TenantConstant.DbaTenantName, result4);
            result4 = "127.0.0.1".GetBusNameByHost();
            Assert.Equal("", result4);

            var result5 = "localhost:1001".GetTenantNameByHost();
            Assert.Equal(TenantConstant.DbaTenantName, result5);
            result5 = "localhost:1001".GetBusNameByHost();
            Assert.Equal("1001", result5);

            var result6 = "ctest.localhost:1001".GetTenantNameByHost();
            Assert.Equal("ctest", result6);
            result6 = "ctest.localhost:1001".GetBusNameByHost();
            Assert.Equal("1001", result6);

            var result7 = "sso.kcloudy.com".GetTenantNameByHost();
            Assert.Equal(TenantConstant.DbaTenantName, result7);
            result7 = "sso.kcloudy.com".GetBusNameByHost();
            Assert.Equal("sso", result7);

            var result8 = "ctest.sso.kcloudy.com".GetTenantNameByHost();
            Assert.Equal("ctest", result8);
            result8 = "ctest.sso.kcloudy.com".GetBusNameByHost();
            Assert.Equal("sso", result8);

            var result9 = "sso.kcloudy.cn".GetTenantNameByHost();
            Assert.Equal("sso.kcloudy.cn", result9);
            result9 = "sso.kcloudy.cn".GetBusNameByHost();
            Assert.Equal("sso", result9);
            #endregion

            var result10 = "aaa, bbb, ccc, ddd"
                .Replace("bbb", "")
                .Replace(",,", ",")
                .Replace(", ,", ",")
                .TrimStart(',')
                .Trim()
                .TrimEnd(',')
                .Trim();
            Assert.Equal("aaa, ccc, ddd", result10);
            Console.WriteLine(result10);
            var result11 = "aaa, bbb, ccc, ddd"
                .Replace("aaa", "")
                .Replace(",,", ",")
                .Replace(", ,", ", ")
                .TrimStart(',')
                .Trim()
                .TrimEnd(',')
                .Trim();
            Assert.Equal("bbb, ccc, ddd", result11);
            Console.WriteLine(result11);
            var result12 = "aaa, bbb, ccc, ddd"
                .Replace("ddd", "")
                .Replace(",,", ",")
                .Replace(", ,", ", ")
                .TrimStart(',')
                .Trim()
                .TrimEnd(',')
                .Trim();
            Assert.Equal("aaa, bbb, ccc", result12);
            Console.WriteLine(result12);

            var result13 = "aaa,bbb,ccc,ddd"
                .Replace("bbb", "")
                .Replace(",,", ",")
                .Replace(", ,", ",")
                .TrimStart(',')
                .Trim()
                .TrimEnd(',')
                .Trim();
            Assert.Equal("aaa,ccc,ddd", result13);
            Console.WriteLine(result13);
            var result14 = "aaa,bbb,ccc,ddd"
                .Replace("aaa", "")
                .Replace(",,", ",")
                .Replace(", ,", ", ")
                .TrimStart(',')
                .Trim()
                .TrimEnd(',')
                .Trim();
            Assert.Equal("bbb,ccc,ddd", result14);
            Console.WriteLine(result14);
            var result15 = "aaa,bbb,ccc,ddd"
                .Replace("ddd", "")
                .Replace(",,", ",")
                .Replace(", ,", ", ")
                .TrimStart(',')
                .Trim()
                .TrimEnd(',')
                .Trim();
            Assert.Equal("aaa,bbb,ccc", result15);
            Console.WriteLine(result15);

            string url = "http://gitlab.kcloudy.com/demo/net/kcloudy.demotest.net.git";
            string host = StringExtensions.GetHost(url);
            Assert.Equal("gitlab.kcloudy.com", host);

            string domainUrl = StringExtensions.GetHostUrl(url);
            Assert.Equal("http://gitlab.kcloudy.com", domainUrl);

            string domainUrl2 = StringExtensions.GetHostUrl("https://gitlab.kcloudy.com/demo/net/kcloudy.demotest.net.git");
            Assert.Equal("https://gitlab.kcloudy.com", domainUrl2);
        }

        [Xunit.Fact]
        public void Test_EnumrableExtension()
        {
            var list = new List<string> { "test1tt", "test2tt", "test3tt", "test4tt", "test1tt", "test2tt" };
            //ReplaceFirst
            var copy1 = new string[6];
            list.CopyTo(copy1);
            var result1 = copy1.ReplaceFirst("test", "T");
            var except1 = new List<string> { "T1tt", "T2tt", "T3tt", "T4tt", "T1tt", "T2tt" };
            Console.WriteLine("result1: " + result1.ToCommaSeparatedString());
            Console.WriteLine("except1: " + except1.ToCommaSeparatedString());
            Assert.Equal(except1.ToCommaSeparatedString(), result1.ToCommaSeparatedString());
            //ReplaceLast
            var copy2 = new string[6];
            list.CopyTo(copy2);
            var result2 = copy2.ReplaceLast("tt", "T");
            var except2 = new List<string> { "test1T", "test2T", "test3T", "test4T", "test1T", "test2T" };
            Console.WriteLine("result2: " + result2.ToCommaSeparatedString());
            Console.WriteLine("except2: " + except2.ToCommaSeparatedString());
            Assert.Equal(except2.ToCommaSeparatedString(), result2.ToCommaSeparatedString());
            //FixStringList
            var copy3 = new string[6];
            list.CopyTo(copy3);
            var result3 = copy3.FixStringList("T-", "-T");
            var except3 = new List<string> { "T-test1tt-T", "T-test2tt-T", "T-test3tt-T", "T-test4tt-T", "T-test1tt-T", "T-test2tt-T" };
            Console.WriteLine("result3: " + result3.ToCommaSeparatedString());
            Console.WriteLine("except3: " + except3.ToCommaSeparatedString());
            Assert.Equal(except3.ToCommaSeparatedString(), result3.ToCommaSeparatedString());
            //Distinct
            var copy4 = new string[6];
            list.CopyTo(copy4);
            var result4 = copy4.Distinct();
            var except4 = new List<string> { "test1tt", "test2tt", "test3tt", "test4tt" };
            Console.WriteLine("result4: " + result4.ToCommaSeparatedString());
            Console.WriteLine("except4: " + except4.ToCommaSeparatedString());
            Assert.Equal(except4.ToCommaSeparatedString(), result4.ToCommaSeparatedString());
            //SingleOrderBy
            var copy5 = new string[6];
            list.CopyTo(copy5);
            var result5 = copy5.SingleOrderBy();
            var except5 = new List<string> { "test1tt", "test1tt", "test2tt", "test2tt", "test3tt", "test4tt" };
            Console.WriteLine("result5: " + result5.ToCommaSeparatedString());
            Console.WriteLine("except5: " + except5.ToCommaSeparatedString());
            Assert.Equal(except5.ToCommaSeparatedString(), result5.ToCommaSeparatedString());
        }

        [Xunit.Fact]
        public void Test_GlobalConfig_Methods()
        {
            // 开发域名测试
            var webDomain1 = "http://subdomain.localhost:2001/";
            var result1 = GlobalConfig.GetTenantWebDomain(webDomain1, TenantConstant.DbaTenantName);
            Assert.Equal("http://cDba.localhost:2001/", result1);
            result1 = GlobalConfig.GetTenantWebApiDomain(webDomain1, TenantConstant.DbaTenantName);
            Assert.Equal("http://cDba.localhost:2002/api/", result1);
            // 租户域名测试
            var webDomain2 = "http://subdomain.acc.kcloudy.com/";
            var result2 = GlobalConfig.GetTenantWebDomain(webDomain2, TenantConstant.DbaTenantName);
            Assert.Equal("http://cDba.acc.kcloudy.com/", result2);
            result2 = GlobalConfig.GetTenantWebApiDomain(webDomain2, TenantConstant.DbaTenantName);
            Assert.Equal("http://cDba.accapi.kcloudy.com/api/", result2);
            // 独立域名测试
            var webDomain3 = "http://acc.xxx.com/";
            var result3 = GlobalConfig.GetTenantWebDomain(webDomain3, TenantConstant.DbaTenantName);
            Assert.Equal("http://acc.xxx.com/", result3);
            result3 = GlobalConfig.GetTenantWebApiDomain(webDomain3, TenantConstant.DbaTenantName);
            Assert.Equal("http://accapi.xxx.com/api/", result3);
            // 开发域名测试
            var webDomain4 = "http://localhost:2001/";
            var result4 = GlobalConfig.GetTenantWebDomain(webDomain4, TenantConstant.DbaTenantName);
            Assert.Equal("http://localhost:2001/", result4);
            result4 = GlobalConfig.GetTenantWebApiDomain(webDomain4, TenantConstant.DbaTenantName);
            Assert.Equal("http://localhost:2002/api/", result4);

            // SSO域名测试
            var webDomain5 = "http://sso.kcloudy.com/";
            var result5 = GlobalConfig.GetTenantWebDomain(webDomain5, TenantConstant.DbaTenantName);
            Assert.Equal("http://cDba.sso.kcloudy.com/", result5);
            result5 = GlobalConfig.GetTenantWebApiDomain(webDomain5, TenantConstant.DbaTenantName);
            Assert.Equal("http://cDba.ssoapi.kcloudy.com/api/", result5);
            // SSO域名测试
            var webDomain6 = "http://localhost:1001/";
            var result6 = GlobalConfig.GetTenantWebDomain(webDomain6, TenantConstant.DbaTenantName);
            Assert.Equal("http://cDba.localhost:1001/", result6);
            result6 = GlobalConfig.GetTenantWebApiDomain(webDomain6, TenantConstant.DbaTenantName);
            Assert.Equal("http://cDba.localhost:1002/api/", result6);
        }

        [Xunit.Fact]
        public void Test_ToCamel_ToUnderline()
        {
            string source = "user_name";
            string result = StringExtensions.ToCamelCase(source, false);
            Assert.Equal("userName", result);

            result = StringExtensions.ToCamelCase(source, true);
            Assert.Equal("UserName", result);

            source = "UserName";
            result = StringExtensions.ToUnderlineCase(source, false);
            Assert.Equal("User_Name", result);

            result = StringExtensions.ToUnderlineCase(source, true);
            Assert.Equal("user_name", result);

            source = "userName";
            result = StringExtensions.ToUpperFirstCase(source);
            Assert.Equal("UserName", result);

            source = "UserName";
            result = StringExtensions.ToLowerFirstCase(source);
            Assert.Equal("userName", result);
        }
    }


    public class GenericInterface<T> { }

    public class GenericClass<T> : GenericInterface<T> { }

    public class Test : GenericClass<TestQueueInfo> { }

    public class TestQueueInfo : QueueEntity
    {
        public string Name { get; set; }
    }

    public class QueueEntity : Entity
    {

    }
}
