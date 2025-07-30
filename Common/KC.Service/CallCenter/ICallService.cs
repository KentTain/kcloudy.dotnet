using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using KC.Common;
using KC.Service.Base;
using KC.Service.WebApiService.Business;

namespace KC.Service.CallCenter
{
    [DataContract]
    public enum CallType
    {
        /// <summary>
        /// 座机
        /// </summary>
        [Description("座机")]
        [EnumMember]
        Telephone = 0,
        /// <summary>
        /// 手机
        /// </summary>
        [Description("手机")]
        [EnumMember]
        Mobile = 1,
    }

    public interface ICallService
    {
        CallConfig Config { get; }
        string IsNotBusy(string phoneNumber);
        string CallContact(string currentUserPhone, string phoneNumber, CallType type, out string sesstionId);
        bool StopCallContact(string phoneNumber, string sessionId);
        string GetCallVoiceFileUrl(string sessionId);
        string PopEvent(string phoneNumber);

        T GetCallVoiceFileInfo<T>(string sessionId, string caller, string callee, string startTime, string endTime)
            where T : class;
    }

    public abstract class AbstractCall : ICallService
    {
        private readonly TenantUserApiService _tenantUserApiService;
        public CallConfig Config { get; private set; }

        protected AbstractCall(CallConfig config)
        {
            //var dbaTenant = TenantConstant.DbaTenantApiAccessInfo;
            _tenantUserApiService = new TenantUserApiService(null, null);
            Config = config;
        }

        public abstract string IsNotBusy(string phoneNumber);
        public abstract string CallContact(string currentUserPhone, string phoneNumber, CallType type, out string sesstionId);
        public abstract bool StopCallContact(string phoneNumber, string sessionId);
        public abstract string PopEvent(string phoneNumber);
        public abstract string GetCallVoiceFileUrl(string sessionId);
        public abstract T GetCallVoiceFileInfo<T>(string sessionId, string caller, string callee, string startTime,
            string endTime) where T : class;

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

        //protected MobileLocationDTO GetMobileLocation(string mobilePhone)
        //{
        //    return _tenantUserApiService.GetMobileLocation(mobilePhone);
        //}

        protected string GetData(string resultJson)
        {
            return SerializeHelper.GetMapValueByKey(resultJson, "data");
        }

        protected int GetErrorCode(string resultJson)
        {
            return Convert.ToInt32(SerializeHelper.GetMapValueByKey(resultJson, "errorCode"));
        }
    }
}
