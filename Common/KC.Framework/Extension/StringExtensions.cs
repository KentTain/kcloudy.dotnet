using KC.Framework.Tenant;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace KC.Framework.Extension
{
    public static class StringExtensions
    {
        #region 处理域名
        /// <summary>
        /// 根据访问的url地址返回host
        /// </summary>
        /// <param name="url">Url地址，如：http://www.xxxx.com/aaa/bbb.html</param>
        /// <returns>域名url地址，如：www.xxxx.com</returns>
        public static string GetHost(this string url)
        {
            //如果为空，认为验证合格
            if (string.IsNullOrEmpty(url))
            {
                return null;
            }

            url = url.ToLower();
            if (!(url.StartsWith("http://") || url.StartsWith("https://")))
            {
                url = "http://" + url;
            }

            string pattern = "(?<=//|)((\\w)+\\.)+\\w+";
            Match match = Regex.Match(url, pattern, RegexOptions.IgnoreCase);
            if (match.Success)
            {
                return match.Value;
            }

            return null;
        }

        /// <summary>
        /// 根据访问的url地址返回域名url地址
        /// </summary>
        /// <param name="url">Url地址，如：http://www.xxxx.com/aaa/bbb.html</param>
        /// <returns>域名url地址，如：http://www.xxxx.com</returns>
        public static string GetHostUrl(string url)
        {
            string host = GetHost(url);
            if (string.IsNullOrEmpty(host))
                return null;

            return url.StartsWith("http://")
                    ? "http://" + host
                    : url.StartsWith("https://")
                        ? "https://" + host
                        : url;
        }

        /// <summary>
        /// 获取域名的顶级域名
        /// </summary>
        /// <param name="domain"></param>
        /// <returns></returns>
        public static string GetTopDomainName(this string domain)
        {
            domain = domain.Trim().ToLower();
            //顶级域名太多了，暂时列出常用的
            string rootDomain = ".com.cn|.gov.cn|.cn|.com|.net|.org|.so|.co|.mobi|.tel|.biz|.info|.name|.me|.cc|.tv|.asiz|.hk";
            if (domain.StartsWith("http://")) domain = domain.Replace("http://", "");
            if (domain.StartsWith("https://")) domain = domain.Replace("https://", "");
            if (domain.StartsWith("www.")) domain = domain.Replace("www.", "");
            //safsd.asdfasdf.baidu.com.cn/ssssd/s/b/d/hhh.html?domain=sfsdf.com.cn&id=1
            if (domain.IndexOf("/") > 0)
                domain = domain.Substring(0, domain.IndexOf("/"));
            //safsd.asdfasdf.baidu.com.cn
            foreach (string item in rootDomain.Split('|'))
            {
                if (domain.EndsWith(item))
                {
                    domain = domain.Replace(item, "");
                    if (domain.LastIndexOf(".") > 0)
                    {
                        domain = domain.Replace(domain.Substring(0, domain.LastIndexOf(".") + 1), "");
                    }
                    return domain + item;
                }
            }
            return "";
        }

        /// <summary>
        /// 根据请求的的Host获取TenantName </br>
        ///     例如：localhost：1001 为 cDba 的本地域名的租户代码 </br>
        ///     cdba.localhost：1001 为 cDba 的本地域名的租户代码 </br>
        ///     sso.kcloudy.com 为 cDba 的租户域名的租户代码 </br>
        ///     cdba.kcloudy.com 为 cDba 的租户域名的租户代码</br>
        ///     sso.xxx.com 为 sso.xxx.com 的独立域名的租户代码
        /// </summary>
        /// <param name="requestHost">请求的Host的租户代码</param>
        /// <returns></returns>
        public static string GetTenantNameByHost(this string requestHost)
        {
            if (string.IsNullOrWhiteSpace(requestHost))
                return string.Empty;
            if (requestHost.Contains("127.0.0.1"))
                return TenantConstant.DbaTenantName;

            var isLocal = requestHost.Contains("localhost", StringComparison.OrdinalIgnoreCase);
            var isKCloudy = requestHost.Contains("kcloudy.com", StringComparison.OrdinalIgnoreCase);
            requestHost = requestHost.Replace("https://", "").Replace("http://", "");
            var urlTenantSplits = requestHost.Split(".");
            if (isLocal)
            {
                if (urlTenantSplits.Length == 1)
                {
                    //顶级域名，例如：localhost:1001
                    return TenantConstant.DbaTenantName;
                }
                else if (urlTenantSplits.Length > 1)
                {
                    //二级域名，例如：cdba.localhost:1001
                    return urlTenantSplits[0];
                }
            }
            else if (isKCloudy)
            {
                if (urlTenantSplits.Length == 4)
                {
                    //三级级域名，例如：cdba.sso.kcloudy.com
                    return urlTenantSplits[0];
                }
                else if (urlTenantSplits.Length < 4)
                {
                    //二级域名，例如：sso.kcloudy.com
                    return TenantConstant.DbaTenantName;
                }
            }
            return requestHost;
        }

        /// <summary>
        /// 获取域名中的业务代码 </br>
        ///     例如：http://cdba.doc.kcoudy.com 为 doc 的本地域名业务代码 </br>
        ///     http://cdba.localhost:2001 为 2001 的租户域名业务代码 </br>
        ///     http://doc.xxx.com 为 doc 的独立域名业务代码
        /// </summary>
        /// <param name="requestHost"></param>
        /// <returns></returns>
        public static string GetBusNameByHost(this string requestHost)
        {
            if (string.IsNullOrWhiteSpace(requestHost))
                return string.Empty;
            if (requestHost.Contains("127.0.0.1"))
                return string.Empty;

            var isLocal = requestHost.Contains("localhost", StringComparison.OrdinalIgnoreCase);
            var isKCloudy = requestHost.Contains("kcloudy.com", StringComparison.OrdinalIgnoreCase);
            requestHost = requestHost.Replace("https://", "").Replace("http://", "").TrimEndSlash();
            if (isLocal)
            {
                var urlPortSplits = requestHost.Split(":");
                //返回端口号
                if (urlPortSplits.Length == 2)
                {
                    //域名，例如：localhost:1001、cdba.localhost:1001
                    return urlPortSplits[1];
                }
                else
                {
                    //二级域名，例如：cdba.localhost
                    return "";
                }
            }
            else if (isKCloudy)
            {
                var urlTenantSplits = requestHost.Split(".");
                if (urlTenantSplits.Length == 4)
                {
                    //三级级域名，例如：cdba.sso.kcloudy.com
                    return urlTenantSplits[1];
                }
                else
                {
                    //二级域名，例如：sso.kcloudy.com
                    return urlTenantSplits[0];
                }
            }
            else
            {
                var urlTenantSplits = requestHost.Split(".");
                //独立域名，例如：sso.xxx.com、sso.xxx.xxx.com
                if (urlTenantSplits.Length >= 3)
                    return urlTenantSplits[0];
                else
                    return "";
            }
        }
        #endregion

        /// <summary>
        /// 获取16位的Guid
        /// </summary>
        /// <returns></returns>
        public static string GenerateStringID()
        {
            long i = 1;
            foreach (byte b in Guid.NewGuid().ToByteArray())
            {
                i *= ((int)b + 1);
            }
            return string.Format("{0:x}", i - DateTime.Now.Ticks);
        }

        /// <summary>
        /// 获取座机号中的分机号
        ///     例如：获取座机号82680051-9003的分机号吗：9003
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string GetExtensionNumber(this string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return null;

            var tels = text.Split('-');
            return tels.LastOrDefault();
        }

        #region Trim
        /// <summary>
        /// 获取以/结尾的Url路径，
        ///     例如：www.xxxx.com/
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string EndWithSlash(this string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return null;

            return text.EndsWith("/") ? text : text + "/";
        }

        /// <summary>
        /// 获取以无/结尾的Url路径，
        ///     例如：www.xxxx.com
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string TrimEndSlash(this string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return null;

            return text.EndsWith("/") ? text.TrimEnd('/') : text;
        }

        /// <summary>
        /// 小数去掉末尾的0和.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string TrimEndZero(this string source)
        {
            if (string.IsNullOrWhiteSpace(source))
                return string.Empty;

            return source.TrimEnd('0').TrimEnd('.');
        }
        #endregion

        #region 字符串转换（列表、字典、DataTable）
        /// <summary>
        /// 将aaa,bbb,ccc转换为列表项 string[]{aaa,bbb,ccc}
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string[] ArrayFromCommaDelimitedStrings(this string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return new string[] { };

            string[] tokens = text.Trim().Split(',');
            string[] array = (from token in tokens where !string.IsNullOrWhiteSpace(token) select token.Trim()).ToArray();
            return array;
        }
        /// <summary>
        /// 将aaa,bbb,ccc转换为列表项 string[]{aaa,bbb,ccc}
        /// </summary>
        /// <param name="text">aaa,bbb,ccc 或  aaa-bbb-ccc</param>
        /// <param name="separator">分隔符</param>
        /// <returns></returns>
        public static string[] ArrayFromCommaDelimitedStringsBySplitChar(this string text, char separator)
        {
            if (string.IsNullOrWhiteSpace(text))
                return new string[] { };

            string[] tokens = text.Trim().Split(separator);
            string[] array = (from token in tokens where !string.IsNullOrWhiteSpace(token) select token.Trim()).ToArray();
            return array;
        }

        /// <summary>
        /// 将1,2,3转换为列表项 int[]{1,2,3}
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static int[] ArrayFromCommaDelimitedIntegers(this string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return new int[] { };

            List<int> result = new List<int>();
            string[] tokens = text.Trim().Split(',');

            foreach (string token in tokens.Where(token => !string.IsNullOrWhiteSpace(token)))
            {
                int val;
                if (Int32.TryParse(token, out val))
                {
                    result.Add(val);
                }
                else
                {
                    throw new ApplicationException(string.Format(CultureInfo.InvariantCulture, "Delimited integers '{0}' has invalid integer value '{1}'", text, token));
                }
            }

            return result.ToArray();
        }

        /// <summary>
        /// 将1,2,3转换为列表项 int[]{1,2,3}
        /// </summary>
        /// <param name="text">1,2,3 或者 1-2-3</param>
        /// <param name="separator">分隔符</param>
        /// <returns></returns>
        public static int[] ArrayFromCommaDelimitedIntegersBySplitChar(this string text, char separator)
        {
            if (string.IsNullOrWhiteSpace(text))
                return new int[] { };

            List<int> result = new List<int>();
            string[] tokens = text.Trim().Split(separator);

            foreach (string token in tokens.Where(token => !string.IsNullOrWhiteSpace(token)))
            {
                int val;
                if (Int32.TryParse(token, out val))
                {
                    result.Add(val);
                }
                else
                {
                    throw new ApplicationException(string.Format(CultureInfo.InvariantCulture, "Delimited integers '{0}' has invalid integer value '{1}'", text, token));
                }
            }

            return result.ToArray();
        }
        /// <summary>
        /// 根据Key值获取连接字符串对应的值，例如：
        /// 将AccessId=xxxxxx;AccessKey=yyyyy;  获取AccessId的值：xxxxxx
        /// </summary>
        /// <param name="connectionString">接字符串：AccessId=xxxxxx;AccessKey=yyyyy;</param>
        /// <param name="key">key值：AccessId</param>
        /// <returns></returns>
        public static string GetValueFromConnectionString(this string connectionString, string key)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                return null;

            var keyValuePair = KeyValuePairFromConnectionString(connectionString);
            if (null == keyValuePair)
                return null;

            return keyValuePair[key];
        }

        /// <summary>
        /// 将AccessId=xxxxxx;AccessKey=xxxxxx;转换为列表项 Dictionary<string, string>()
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static Dictionary<string, string> KeyValuePairFromConnectionString(this string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                return null;

            var result = new Dictionary<string, string>();
            var keyPairs = connectionString.Split(';');
            foreach (var keyPair in keyPairs)
            {
                if (!string.IsNullOrWhiteSpace(keyPair))
                {
                    var index = keyPair.IndexOf("=");
                    var key = keyPair.Substring(0, index).ToLower();
                    var value = keyPair.Substring(index + 1);
                    result.Add(key, value);
                }
            }

            return result;
        }

        /// <summary>
        /// 扩展方法，将一个Json字符串反序列化为DataTable
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="str"></param>
        /// <returns>DataTable</returns>
        public static DataTable ToDataTable<T>(this string str)
        {
            var dt = new DataTable();
            if (str[0] == '[')//如果str的第一个字符是'['，则说明str里存放有多个model数据
            {
                //删除最后一个']'和第一个'['，顺序不能错。不然字符串的长度就不对了。
                //因为每个model与model之间是用 ","分隔的，所以改为用 ";"分隔
                str = str.Remove(str.Length - 1, 1).Remove(0, 1).Replace("},{", "};{");
            }

            string[] items = str.Split(';');//用";"分隔开多条数据
            foreach (var property in typeof(T).GetProperties())//反射，获得T类型的所有属性
            {
                var columnType = property.PropertyType;
                //判断是否可为空类型
                if (columnType.IsGenericType && columnType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                {
                    columnType = property.PropertyType.GetGenericArguments()[0];
                }

                var attribute = property.GetCustomAttribute<DisplayNameAttribute>();

                string displayName = string.Empty;
                if (attribute != null)
                {
                    displayName = attribute.DisplayName;
                }


                //创建一个新列，列名为属性名，类型为属性的类型。
                if (!string.IsNullOrEmpty(displayName))
                {
                    var col = new DataColumn(displayName, columnType);//property.Name, property.PropertyType);
                    dt.Columns.Add(col);
                }

            }

            var options = new System.Text.Json.JsonSerializerOptions();
            options.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(System.Text.Unicode.UnicodeRanges.All);

            //循环，一个一个的反序列化
            for (int i = 0; i < items.Length; i++)
            {
                //创建新行
                var dr = dt.NewRow();

                //反序列化为一个T类型对象
                T temp = System.Text.Json.JsonSerializer.Deserialize<T>(items[i], options);
                foreach (var property in typeof(T).GetProperties())
                {
                    //赋值
                    //dr[property.Name] = property.GetValue(temp, null);
                    var attribute = property.GetCustomAttribute<DisplayNameAttribute>();
                    var displayName = string.Empty;
                    if (attribute != null)
                    {
                        displayName = attribute.DisplayName;
                        if (dr[displayName] != null)
                        {
                            dr[displayName] = property.GetValue(temp, null);
                        }
                    }
                }
                dt.Rows.Add(dr);
            }
            return dt;
        }

        /// <summary>
        /// HTML转行成TEXT
        /// </summary>
        /// <param name="strHtml"></param>
        /// <returns></returns>
        public static string HtmlToTxt(this string strHtml)
        {
            string[] aryReg ={
            @"<script[^>]*?>.*?</script>",
            @"<(\/\s*)?!?((\w+:)?\w+)(\w+(\s*=?\s*(([""'])(\\[""'tbnr]|[^\7])*?\7|\w+)|.{0})|\s)*?(\/\s*)?>",
            @"([\r\n])[\s]+",
            @"&(quot|#34);",
            @"&(amp|#38);",
            @"&(lt|#60);",
            @"&(gt|#62);",
            @"&(nbsp|#160);",
            @"&(iexcl|#161);",
            @"&(cent|#162);",
            @"&(pound|#163);",
            @"&(copy|#169);",
            @"&#(\d+);",
            @"-->",
            @"<!--.*\n"
            };

            string newReg = aryReg[0];
            string strOutput = strHtml;
            for (int i = 0; i < aryReg.Length; i++)
            {
                Regex regex = new Regex(aryReg[i], RegexOptions.IgnoreCase);
                strOutput = regex.Replace(strOutput, string.Empty);
            }

            strOutput.Replace("<", "");
            strOutput.Replace(">", "");
            strOutput.Replace("\r\n", "");

            return strOutput;
        }
        #endregion

        #region  字符串替换
        /// <summary>
        /// 将中文字符转化Unicode编码
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ChineseToUnicode(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return string.Empty;

            //中文轉為UNICODE
            string outStr = "";
            for (int i = 0; i < str.Length; i++)
            {
                //將中文轉為10進制整數，然後轉為16進制unicode
                outStr += "\\u" + ((int)str[i]).ToString("x");
            }

            return outStr;
        }

        /// <summary>
        /// 将Unicode编码字符转化中文
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string UnicodeToChinese(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return string.Empty;

            //UNICODE轉為中文(最直接的方法Regex.Unescape(input);)
            string outStr = "";
            string[] strlist = str.Replace("\\", "").Split('u');
            try
            {
                for (int i = 1; i < strlist.Length; i++)
                {
                    //將unicode轉為10進制整數，然後轉為char中文
                    outStr += (char)Int32.Parse(strlist[i], NumberStyles.HexNumber);
                }

                return outStr;
            }
            catch (FormatException ex)
            {
                throw;
            }
        }

        /// <summary>
        /// 将以oldValue开头字符中的替换为已newValue为开头
        /// </summary>
        /// <param name="input"></param>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        /// <returns></returns>
        public static string ReplaceFirst(this string input, string oldValue, string newValue)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            Regex regEx = new Regex(oldValue, RegexOptions.Multiline);
            return regEx.Replace(input, newValue ?? "", 1);
        }

        /// <summary>
        /// 将以oldValue结尾字符中的替换为已newValue为结尾
        /// </summary>
        /// <param name="input"></param>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        /// <returns></returns>
        public static string ReplaceLast(this string input, string oldValue, string newValue)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            int index = input.LastIndexOf(oldValue);
            if (index < 0)
            {
                return input;
            }
            else
            {
                StringBuilder sb = new StringBuilder(input.Length - oldValue.Length + newValue.Length);
                sb.Append(input.Substring(0, index));
                sb.Append(newValue);
                sb.Append(input.Substring(index + oldValue.Length,
                    input.Length - index - oldValue.Length));

                return sb.ToString();
            }
        }

        /// <summary>
        /// 将传入的字符串中间部分字符替换成特殊字符
        /// </summary>
        /// <param name="input">需要替换的字符串</param>
        /// <param name="startLen">前保留长度</param>
        /// <param name="endLen">尾保留长度</param>
        /// <param name="specialChar">特殊字符：默认为*</param>
        /// <returns>被特殊字符替换的字符串</returns>
        public static string ReplaceWithSpecialChar(this string input, int startLen = 4, int endLen = 4, char specialChar = '*')
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            try
            {
                int lenth = input.Length - startLen - endLen;
                string replaceStr = input.Substring(startLen, lenth);
                string specialStr = string.Empty;
                for (int i = 0; i < replaceStr.Length; i++)
                {
                    specialStr += specialChar;
                }

                input = input.Replace(replaceStr, specialStr);
            }
            catch (Exception)
            {
                return input;
            }

            return input;
        }

        /// <summary>
        /// 截取指定长度字符串
        /// </summary>
        /// <param name="inputString">要处理的字符串</param>
        /// <param name="len">指定长度</param>
        /// <returns>返回处理后的字符串</returns>
        public static string ClipString(string inputString, int len)
        {
            bool isShowFix = false;
            if (len % 2 == 1)
            {
                isShowFix = true;
                len--;
            }
            ASCIIEncoding ascii = new ASCIIEncoding();
            int tempLen = 0;
            string tempString = "";
            byte[] s = ascii.GetBytes(inputString);
            for (int i = 0; i < s.Length; i++)
            {
                if ((int)s[i] == 63)
                    tempLen += 2;
                else
                    tempLen += 1;

                try
                {
                    tempString += inputString.Substring(i, 1);
                }
                catch
                {
                    break;
                }

                if (tempLen > len)
                    break;
            }

            byte[] mybyte = Encoding.Default.GetBytes(inputString);
            if (isShowFix && mybyte.Length > len)
                tempString += "…";
            return tempString;
        }
        #endregion

        #region 字符串验证

        /// <summary>
        /// 验证身份证是否合法
        /// </summary>
        /// <param name="idCard">要验证的身份证</param>
        public static bool IsIdCard(this string idCard)
        {
            //如果为空，认为验证合格
            if (string.IsNullOrEmpty(idCard))
            {
                return false;
            }

            //清除要验证字符串中的空格
            idCard = idCard.Trim();

            //模式字符串
            StringBuilder pattern = new StringBuilder();
            pattern.Append(@"^(11|12|13|14|15|21|22|23|31|32|33|34|35|36|37|41|42|43|44|45|46|");
            pattern.Append(@"50|51|52|53|54|61|62|63|64|65|71|81|82|91)");
            pattern.Append(@"(\d{13}|\d{15}[\dx])$");

            //验证
            return Regex.IsMatch(idCard, pattern.ToString(), RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// 验证EMail是否合法
        /// </summary>
        /// <param name="email">要验证的Email</param>
        public static bool IsEmail(string email)
        {
            //如果为空，认为验证不合格
            if (string.IsNullOrEmpty(email))
            {
                return false;
            }

            //清除要验证字符串中的空格
            email = email.Trim();

            //模式字符串
            string pattern = @"^([0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$";

            //验证
            return Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// 验证银行帐号是否合法
        /// </summary>
        /// <param name="bankAccount">银行账号</param>
        /// <returns></returns>
        public static bool IsBankAccount(this string bankAccount)
        {
            if(string.IsNullOrEmpty(bankAccount))
                return false;

            bankAccount=bankAccount.Trim();

            string pattern = @"^([1-9]{1})(\d{14}|\d{15}|\d{16}|\d{17}|\d{18})$";

            return Regex.IsMatch(bankAccount, pattern, RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// 验证url是否合法
        /// </summary>
        /// <param name="url">URL地址</param>
        /// <returns></returns>
        public static bool IsUrl(this string url)
        {
            if (string.IsNullOrEmpty(url))
                return false;

            url = url.Trim();

            string pattern =
                @"^(https?|ftp):\/\/(((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&'\(\)\*\+,;=]|:)*@)?(((\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5])\.(\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5])\.(\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5])\.(\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5]))|((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?)(:\d*)?)(\/((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&'\(\)\*\+,;=]|:|@)+(\/(([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&'\(\)\*\+,;=]|:|@)*)*)?)?(\?((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&'\(\)\*\+,;=]|:|@)|[\uE000-\uF8FF]|\/|\?)*)?(\#((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&'\(\)\*\+,;=]|:|@)|\/|\?)*)?$";

            return Regex.IsMatch(url, pattern, RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// 验证座机号
        /// </summary>
        /// <param name="phone">座机号</param>
        /// <returns></returns>
        public static bool IsTelephone(this string phone)
        {
            return Regex.IsMatch(phone, @"^((\(\d{2,3}\))|(\d{3}\-))?(\(0\d{2,3}\)|0\d{2,3}-)?[1-9]\d{6,7}(\-\d{1,4})?$");
        }

        /// <summary>
        /// 验证手机号
        /// </summary>
        /// <param name="phone">手机号</param>
        /// <returns></returns>
        public static bool IsMobile(this string phone)
        {
            return Regex.IsMatch(phone, @"^(13|14|15|17|18)\d{9}$");
        }

        /// <summary>
        /// 验证手机号或座机号
        /// </summary>
        /// <param name="phone">要验证的手机号或座机号</param>
        /// <returns></returns>
        public static bool IsTelOrMobile(this string phone)
        {
            return IsTelephone(phone) || IsMobile(phone);
        }
        /// <summary>
        /// 验证QQ号
        /// </summary>
        /// <param name="qq">要验证的的QQ号</param>
        /// <returns></returns>
        public static bool IsQQ(this string qq)
        {
            return Regex.IsMatch(qq, @"^[1-9]\d{4,11}$");
        }

        // <summary>
        /// 验证IP地址是否合法
        /// </summary>
        /// <param name="ip">要验证的IP地址</param>
        public static bool IsIP(string ip)
        {
            //如果为空，认为验证合格
            if (string.IsNullOrEmpty(ip))
            {
                return true;
            }

            //清除要验证字符串中的空格
            ip = ip.Trim();

            //模式字符串
            string pattern = @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$";

            //验证
            return Regex.IsMatch(ip, pattern, RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// 是否为SaaS系统的主域名
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsOwnDomain(this string source)
        {
            var isOwnDomain = !source.EndsWith("kcloudy.com")
                              || !source.EndsWith("kcloudy.cn");
            return isOwnDomain;
        }

        /// <summary>
        /// 验证字符串是否有sql注入字段
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsValidInput(this string input)
        {
            try
            {
                if (input.IsNullOrEmpty())
                    return false;
                else
                {
                    //替换单引号
                    input = input.Replace("'", "''").Trim();

                    //检测攻击性危险字符串
                    string testString = "and |or |exec |insert |select |delete |update |count |chr |mid |master |truncate |char |declare ";
                    string[] testArray = testString.Split('|');
                    foreach (string testStr in testArray)
                    {
                        if (input.ToLower().IndexOf(testStr) != -1)
                        {
                            //检测到攻击字符串,清空传入的值
                            input = "";
                            return false;
                        }
                    }

                    //未检测到攻击字符串
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 检测客户输入的字符串是否有效,并将原始字符串修改为有效字符串或空字符串。
        /// 当检测到客户的输入中有攻击性危险字符串,则返回false,有效返回true。
        /// </summary>
        /// <param name="input">要检测的字符串</param>
        /// <param name="isNullValid">是否需要做非空判断,true表示要,false表示不要</param>
        public static bool IsValidInput(this string input, bool isNullValid)
        {
            if (isNullValid == false)
            {
                return true;
            }
            try
            {
                if (isNullValid && string.IsNullOrEmpty(input))
                {
                    //如果是空值,则跳出
                    return false;
                }
                else
                {
                    //替换单引号
                    input = input.Replace("'", "''").Trim();

                    //检测攻击性危险字符串
                    string testString = "and |or |exec |insert |select |delete |update |count |chr |mid |master |truncate |char |declare ";
                    string[] testArray = testString.Split('|');
                    foreach (string testStr in testArray)
                    {
                        if (input.ToLower().IndexOf(testStr) != -1)
                        {
                            //检测到攻击字符串,清空传入的值
                            input = "";
                            return false;
                        }
                    }

                    //未检测到攻击字符串
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static bool EngNum(this string engnum)
        {
            return Regex.IsMatch(engnum, "^[0-9a-zA-Z]*$");
        }

        public static bool LenEngNum(this string lenEngNum, int length)
        {
            return length >= lenEngNum.Length && EngNum(lenEngNum);
        }
        #endregion 字符串验证

        #region 字符串处理

        /// <summary>
        ///  前台显示邮箱的掩码替换(由tzh@qq.com等替换成t***@qq.com)
        /// </summary>
        /// <param name="email">邮箱</param>
        /// <returns></returns>
        public static string ToHideEmail(this string email)
        {
            if (string.IsNullOrEmpty(email))
                return string.Empty;

            string strArg = "";
            string SendEmail = "";
            Match match = Regex.Match(email, @"(\w)\w+@");

            if (match.Success)
            {
                strArg = match.Groups[1].Value + "***@";
                SendEmail = Regex.Replace(email, @"\w+@", strArg);
            }
            else
                SendEmail = email;
            return SendEmail;
        }

        /// <summary>
        ///  前台显示手机的掩码替换(由13655555555等替换成136***555)
        /// </summary>
        /// <param name="phone">手机号</param>
        /// <returns></returns>
        public static string ToHidePhone(this string phone)
        {
            if (string.IsNullOrEmpty(phone))
                return string.Empty;

            string prefix = phone.Substring(0, 3);
            string suffix = phone.Substring(phone.Length - 3);

            return prefix + "***" + suffix;
        }

        /// <summary>
        /// 隐藏敏感信息
        /// </summary>
        /// <param name="info">信息实体</param>
        /// <param name="left">左边保留的字符数</param>
        /// <param name="right">右边保留的字符数</param>
        /// <param name="basedOnLeft">当长度异常时，是否显示左边 </param>
        /// <returns></returns>
        public static string ToHideSensitiveInfo(this string info, int left, int right, bool basedOnLeft = true)
        {
            if (String.IsNullOrEmpty(info))
            {
                return "";
            }
            StringBuilder sbText = new StringBuilder();
            int hiddenCharCount = info.Length - left - right;
            if (hiddenCharCount > 0)
            {
                string prefix = info.Substring(0, left), suffix = info.Substring(info.Length - right);
                sbText.Append(prefix);
                for (int i = 0; i < hiddenCharCount; i++)
                {
                    sbText.Append("*");
                }
                sbText.Append(suffix);
            }
            else
            {
                if (basedOnLeft)
                {
                    if (info.Length > left && left > 0)
                    {
                        sbText.Append(info.Substring(0, left) + "***");
                    }
                    else
                    {
                        sbText.Append(info.Substring(0, 1) + "***");
                    }
                }
                else
                {
                    if (info.Length > right && right > 0)
                    {
                        sbText.Append("***" + info.Substring(info.Length - right));
                    }
                    else
                    {
                        sbText.Append("***" + info.Substring(info.Length - 1));
                    }
                }
            }
            return sbText.ToString();
        }

        /// <summary>
        /// 下划线转驼峰
        /// user_name  ---->  userName
        /// userName   --->  userName
        /// </summary>
        /// <param name="underlineStr">带有下划线的字符串</param>
        /// <param name="isUpperFirstCase">首字母是否大写</param>
        /// <returns>驼峰字符串</returns>
        public static string ToCamelCase(string underlineStr, bool isUpperFirstCase)
        {
            if (string.IsNullOrEmpty(underlineStr))
                return "";
            // 分成数组
            char[] charArray = underlineStr.ToCharArray();
            // 判断上次循环的字符是否是"_"
            bool underlineBefore = false;
            StringBuilder buffer = new StringBuilder();
            for (int i = 0, l = charArray.Length; i < l; i++)
            {
                // 判断当前字符是否是"_",如果跳出本次循环
                if (charArray[i] == 95)
                {
                    underlineBefore = true;
                }
                else if (underlineBefore)
                {
                    // 如果为true，代表上次的字符是"_",当前字符需要转成大写
                    var upperChar = (char)(charArray[i] - 32);
                    buffer.Append(upperChar);
                    underlineBefore = false;
                }
                else
                {
                    // 不是"_"后的字符就直接追加
                    buffer.Append(charArray[i]);
                }
            }
            return isUpperFirstCase
                    ? ToUpperFirstCase(buffer.ToString())
                    : buffer.ToString();
        }

        /// <summary>
        /// 驼峰转 下划线
        /// userName  ---->  user_name
        /// user_name   --->  user_name
        /// </summary>
        /// <param name="underlineStr">驼峰字符串</param>
        /// <param name="isUpperFirstCase">是否小写下划线字符串</param>
        /// <returns>带下滑线的字符串</returns>
        public static string ToUnderlineCase(string camelCaseStr, bool isLowerCase)
        {
            if (string.IsNullOrEmpty(camelCaseStr))
                return "";
            // 将驼峰字符串转换成数组
            char[] charArray = camelCaseStr.ToCharArray();
            StringBuilder buffer = new StringBuilder();
            //处理字符串
            for (int i = 0, l = charArray.Length; i < l; i++)
            {
                if (i != 0 && charArray[i] >= 65 && charArray[i] <= 90)
                {
                    if (isLowerCase)
                    {
                        var lowerChar = (char)(charArray[i] + 32);
                        buffer.Append("_").Append(lowerChar);
                    }
                    else
                    {
                        buffer.Append("_").Append(charArray[i]);
                    }
                }
                else
                {
                    buffer.Append(charArray[i]);
                }
            }

            return isLowerCase
                    ? buffer.ToString().ToLower()
                    : buffer.ToString();
        }


        /// <summary>
        /// ⾸字母⼤写(进⾏字母的ascii编码前移，效率是最⾼的)
        /// </summary>
        /// <param name="source">需要转化的字符串</param>
        /// <returns>带下滑线的字符串</returns>
        public static string ToUpperFirstCase(string source)
        {
            char[] chars = source.ToCharArray();
            chars[0] = (char)(chars[0] - 32);
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(chars);
            return sb.ToString();
        }

        /// <summary>
        /// ⾸字母小写(进⾏字母的ascii编码前移，效率是最⾼的)
        /// </summary>
        /// <param name="source">需要转化的字符串</param>
        /// <returns>带下滑线的字符串</returns>
        public static string ToLowerFirstCase(string source)
        {
            char[] chars = source.ToCharArray();
            chars[0] = (char)(chars[0] + 32);
            System.Text.StringBuilder sb = new System.Text.StringBuilder(); 
            sb.Append(chars);
            return sb.ToString();
        }

        #endregion 字符串处理

        #region 数据类型判断

        /// <summary>
        /// 验证日期是否合法,对不规则的作了简单处理
        /// </summary>
        /// <param name="date">日期</param>
        public static bool IsDate(this string date)
        {
            //如果为空，认为验证合格
            if (string.IsNullOrEmpty(date))
            {
                return false;
            }

            //清除要验证字符串中的空格
            date = date.Trim();

            //替换\
            date = date.Replace(@"\", "-");
            //替换/
            date = date.Replace(@"/", "-");

            //如果查找到汉字"今",则认为是当前日期
            if (date.IndexOf("今") != -1)
            {
                date = DateTime.Now.ToString();
            }

            try
            {
                //用转换测试是否为规则的日期字符
                date = Convert.ToDateTime(date).ToString("d");
                return true;
            }
            catch
            {
                //如果日期字符串中存在非数字，则返回false
                //if (!IsInt(date))
                //{
                //    return false;
                //}

                #region 对纯数字进行解析

                //对8位纯数字进行解析
                if (date.Length == 8)
                {
                    //获取年月日
                    string year = date.Substring(0, 4);
                    string month = date.Substring(4, 2);
                    string day = date.Substring(6, 2);

                    //验证合法性
                    if (Convert.ToInt32(year) < 1900 || Convert.ToInt32(year) > 2100)
                    {
                        return false;
                    }
                    if (Convert.ToInt32(month) > 12 || Convert.ToInt32(day) > 31)
                    {
                        return false;
                    }

                    //拼接日期
                    date = Convert.ToDateTime(year + "-" + month + "-" + day).ToString("d");
                    return true;
                }

                //对6位纯数字进行解析
                if (date.Length == 6)
                {
                    //获取年月
                    string year = date.Substring(0, 4);
                    string month = date.Substring(4, 2);

                    //验证合法性
                    if (Convert.ToInt32(year) < 1900 || Convert.ToInt32(year) > 2100)
                    {
                        return false;
                    }
                    if (Convert.ToInt32(month) > 12)
                    {
                        return false;
                    }

                    //拼接日期
                    date = Convert.ToDateTime(year + "-" + month).ToString("d");
                    return true;
                }

                //对5位纯数字进行解析
                if (date.Length == 5)
                {
                    //获取年月
                    string year = date.Substring(0, 4);
                    string month = date.Substring(4, 1);

                    //验证合法性
                    if (Convert.ToInt32(year) < 1900 || Convert.ToInt32(year) > 2100)
                    {
                        return false;
                    }

                    //拼接日期
                    date = year + "-" + month;
                    return true;
                }

                //对4位纯数字进行解析
                if (date.Length == 4)
                {
                    //获取年
                    string year = date.Substring(0, 4);

                    //验证合法性
                    if (Convert.ToInt32(year) < 1900 || Convert.ToInt32(year) > 2100)
                    {
                        return false;
                    }

                    //拼接日期
                    date = Convert.ToDateTime(year).ToString("d");
                    return true;
                }

                #endregion 对纯数字进行解析

                return false;
            }
        }

        /// <summary>
        /// 验证是否为数字
        /// </summary>
        /// <param name="number">要验证的数字</param>
        public static bool IsNumber(this string number)
        {
            //如果为空，认为验证不合格
            if (string.IsNullOrEmpty(number))
            {
                return false;
            }

            //清除要验证字符串中的空格
            number = number.Trim();

            //模式字符串
            string pattern = @"^[0-9]+[0-9]*[.]?[0-9]*$";

            //验证
            return Regex.IsMatch(number, pattern, RegexOptions.IgnoreCase);
        }

        #endregion 数据类型判断

    }
}