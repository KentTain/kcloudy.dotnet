using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;
using KC.Service.Base;

namespace KC.Service.Message
{
    public interface ISmsService
    {
        SmsConfig Config { get;}
        string Send(List<long> mobiles, string tempCode, string msgContent);
        string SendVoice(List<long> mobiles, string tempCode, string msgContent);
    }

    public abstract class AbstractSms : ISmsService
    {
        public SmsConfig Config { get; private set; }

        protected AbstractSms(SmsConfig config)
        {
            Config = config;
        }

        public abstract string Send(List<long> mobiles, string tempCode, string msgContent);
        public abstract string SendVoice(List<long> mobiles, string tempCode, string msgContent);
        /// <summary>
        /// 通过get方式请求页面
        /// </summary>
        /// <param name="getUrl"></param>
        /// <returns></returns>
        public string GetHtmlData(string getUrl)
        {
            HttpWebRequest request;
            HttpWebResponse response;
            request = WebRequest.Create(getUrl) as HttpWebRequest;
            if (request == null)
                throw new Exception("创建连接失败");
            request.Method = "GET";
            request.UserAgent = "Mozilla/4.0";
            request.KeepAlive = true;
            request.Timeout = 60000;

            //http://stackoverflow.com/questions/3345387/how-to-change-originating-ip-in-httpwebrequest
            request.ServicePoint.BindIPEndPointDelegate = delegate(
                ServicePoint servicePoint,
                IPEndPoint remoteEndPoint,
                int retryCount)
            {
                if (remoteEndPoint.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                {
                    return new IPEndPoint(IPAddress.IPv6Any, 0);
                }
                else
                {
                    return new IPEndPoint(IPAddress.Any, 0);
                }
            };

            //获取服务器返回的资源
            using (response = (HttpWebResponse)request.GetResponse())
            {
                var st = response.GetResponseStream();
                if (st == null)
                {
                    throw new Exception("获取远程返回结果失败");
                }
                using (var reader = new StreamReader(st, Encoding.UTF8))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        public object Deserialize(Type type, string xml)
        {
            using (var sr = new StringReader(xml))
            {
                var xmldes = new XmlSerializer(type);
                return xmldes.Deserialize(sr);
            }
        }

        public string Sha1Encrypt(string sourceStr)
        {
            byte[] strRes = Encoding.Default.GetBytes(sourceStr);
            var provider = new SHA1CryptoServiceProvider();
            strRes = provider.ComputeHash(strRes);
            var enText = new StringBuilder();
            foreach (byte iByte in strRes)
            {
                enText.AppendFormat("{0:x2}", iByte);
            }
            return enText.ToString();
        }
    }
}
