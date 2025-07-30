using KC.Framework.Extension;
using KC.Framework.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace KC.Common.HttpHelper
{
    public static class HttpWebRequestHelper
    {

        /// <summary>
        /// 执行HTTP POST请求。
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="parameters">请求参数</param>
        /// <param name="charset">编码字符集</param>
        /// <returns>HTTP响应</returns>
        public static string DoPost(string url, IDictionary<string, string> parameters, string charset = "utf-8")
        {
            HttpWebRequest req = GetWebRequest(url, "POST");
            req.ContentType = "application/x-www-form-urlencoded;charset=" + charset;

            byte[] postData = Encoding.GetEncoding(charset).GetBytes(BuildQuery(parameters, charset));
            Stream reqStream = req.GetRequestStream();
            reqStream.Write(postData, 0, postData.Length);
            reqStream.Close();

            var rsp = (HttpWebResponse)req.GetResponse();
            var code = rsp.CharacterSet.IsNullOrEmpty() ? charset : rsp.CharacterSet;
            var encoding = Encoding.GetEncoding(code);
            return GetResponseAsString(rsp, encoding);
        }

        /// <summary>
        /// 执行HTTP POST请求。
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="json">请求参数</param>
        /// <param name="charset">编码字符集</param>
        /// <returns>HTTP响应</returns>
        public static string DoPost(string url, string json, string charset = "utf-8")
        {
            HttpWebRequest req = GetWebRequest(url, "POST");
            req.ContentType = "application/json;charset=" + charset;

            byte[] postData = Encoding.GetEncoding(charset).GetBytes(json);
            Stream reqStream = req.GetRequestStream();
            reqStream.Write(postData, 0, postData.Length);
            reqStream.Close();

            var rsp = (HttpWebResponse)req.GetResponse();
            var code = rsp.CharacterSet.IsNullOrEmpty() ? charset : rsp.CharacterSet;
            var encoding = Encoding.GetEncoding(code);
            return GetResponseAsString(rsp, encoding);
        }

        /// <summary>
        /// 执行HTTP POST请求。(异步)
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="json">请求参数</param>
        /// <param name="charset">编码字符集</param>
        /// <returns>HTTP响应</returns>
        public static async Task<string> DoPostAsync(string url, string json, string charset = "utf-8")
        {
            HttpWebRequest req = GetWebRequest(url, "POST");
            req.ContentType = "application/json;charset=" + charset;

            byte[] postData = Encoding.GetEncoding(charset).GetBytes(json);
            Stream reqStream = req.GetRequestStream();
            reqStream.Write(postData, 0, postData.Length);
            reqStream.Close();

            HttpWebResponse rsp =
                (HttpWebResponse)
                    await Task<WebResponse>.Factory.FromAsync(req.BeginGetResponse, req.EndGetResponse, null);
            var code = rsp.CharacterSet.IsNullOrEmpty() ? charset : rsp.CharacterSet;
            var encoding = Encoding.GetEncoding(code);
            return GetResponseAsString(rsp, encoding);
        }

        /// <summary>
        /// 执行HTTP GET请求。
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="parameters">请求参数</param>
        /// <param name="charset">编码字符集</param>
        /// <returns>HTTP响应</returns>
        public static string DoGet(string url, IDictionary<string, string> parameters, string charset = "utf-8")
        {
            if (parameters != null && parameters.Count > 0)
            {
                if (url.Contains("?"))
                {
                    url = url + "&" + BuildQuery(parameters, charset);
                }
                else
                {
                    url = url + "?" + BuildQuery(parameters, charset);
                }
            }

            var req = GetWebRequest(url, "GET");
            req.ContentType = "application/x-www-form-urlencoded;charset=" + charset;

            var rsp = (HttpWebResponse)req.GetResponse();
            var code = rsp.CharacterSet.IsNullOrEmpty() ? charset : rsp.CharacterSet;
            var encoding = Encoding.GetEncoding(code);
            return GetResponseAsString(rsp, encoding);
        }


        public static Tuple<bool, string> WebClientDownload(string url, string postString)
        {
            byte[] postData = Encoding.UTF8.GetBytes(postString);
            var webClient = new WebClient();
            webClient.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            try
            {
                byte[] responseData = webClient.UploadData(url, "POST", postData);
                return new Tuple<bool, string>(true, Encoding.UTF8.GetString(responseData));
            }
            catch (Exception ex)
            {
                LogUtil.LogException(ex);
                return new Tuple<bool, string>(false, ex.Message);
            }
        }

        public static byte[] WebClientDownload(string url)
        {
            var webClient = new WebClient();
            try
            {
                webClient.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
                byte[] responseData = webClient.DownloadData(url);
                return responseData;
            }
            catch (Exception ex)
            {
                LogUtil.LogException(ex);
                return null;
            }
        }


        private static HttpWebRequest GetWebRequest(string url, string method)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.ServicePoint.Expect100Continue = false;
            req.Method = method;
            req.KeepAlive = true;
            req.Timeout = 100000;
            return req;
        }

        /// <summary>
        /// 把响应流转换为文本。
        /// </summary>
        /// <param name="rsp">响应流对象</param>
        /// <param name="encoding">编码方式</param>
        /// <returns>响应文本</returns>
        private static string GetResponseAsString(HttpWebResponse rsp, Encoding encoding)
        {
            StringBuilder result = new StringBuilder();
            Stream stream = null;
            StreamReader reader = null;

            try
            {
                // 以字符流的方式读取HTTP响应
                stream = rsp.GetResponseStream();
                reader = new StreamReader(stream, encoding);

                // 按字符读取并写入字符串缓冲
                int ch = -1;
                while ((ch = reader.Read()) > -1)
                {
                    // 过滤结束符
                    char c = (char)ch;
                    if (c != '\0')
                    {
                        result.Append(c);
                    }
                }
            }
            finally
            {
                // 释放资源
                if (reader != null) reader.Close();
                if (stream != null) stream.Close();
                if (rsp != null) rsp.Close();
            }

            return result.ToString();
        }

        /// <summary>
        /// 组装普通文本请求参数。
        /// </summary>
        /// <param name="parameters">Key-Value形式请求参数字典</param>
        /// <returns>URL编码后的请求数据</returns>
        private static string BuildQuery(IDictionary<string, string> parameters, string charset)
        {
            StringBuilder postData = new StringBuilder();
            bool hasParam = false;

            IEnumerator<KeyValuePair<string, string>> dem = parameters.GetEnumerator();
            while (dem.MoveNext())
            {
                string name = dem.Current.Key;
                string value = dem.Current.Value;
                // 忽略参数名或参数值为空的参数
                if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(value))
                {
                    if (hasParam)
                    {
                        postData.Append("&");
                    }

                    postData.Append(name);
                    postData.Append("=");

                    string encodedValue = HttpUtility.UrlEncode(value, Encoding.GetEncoding(charset));

                    postData.Append(encodedValue);
                    hasParam = true;
                }
            }

            return postData.ToString();
        }
    }
}
