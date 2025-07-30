using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using KC.Framework.Extension;
using KC.Framework.SecurityHelper;
using KC.Framework.Util;

namespace KC.Framework.Base
{
    [Serializable, DataContract]
    public class ServiceRequestToken
    {
        private string _key = EncryptPasswordUtil.Key;

        public ServiceRequestToken(int userId, string memberId, string key)
        {
            if (!string.IsNullOrWhiteSpace(key))
                this._key = key;
            this.UserId = userId;
            this.MemberId = memberId;
            this.RandomId = key;
            //this.ExpiredDateTime = expiredDateTime;
        }

        public ServiceRequestToken(string signatureJson, string key)
        {
            if (string.IsNullOrEmpty(signatureJson))
                throw new ArgumentNullException("signatureJson", "signature argument is empty.");
            try
            {
                if (!string.IsNullOrWhiteSpace(key))
                    this._key = key;
                var encrypt = DESProvider.DecryptString(signatureJson, _key);
                InitObject(encrypt);
            }
            catch (Exception)
            {
                throw new ArgumentException("signature argument is not the json string of the SignatureObject class."); ;
            }
        }

        [DataMember]
        public int UserId { get; set; }
        [DataMember]
        public string MemberId { get; set; }
        [DataMember]
        public string RandomId { get; set; }
        //[DataMember]
        //public DateTime? ExpiredDateTime { get; set; }

        public string GetEncrptSignature()
        {
            string objStr = ObjToString();
            return DESProvider.EncryptString(objStr, _key);
        }

        public string IsValid(string memberId)
        {
            //if (HttpContext.Current != null)
            //{
            //    var url = HttpContext.Current.Request.Url.Host;
            //    var referrerurl = HttpContext.Current.Request.Headers["Origin"];
            //    var userHostName = HttpContext.Current.Request.UserHostName;
            //    var clientIp = HttpContext.Current.Request.UserHostAddress;
            //    LogUtil.LogInfo("ServiceRequestToken--IsValid--memberId：" + memberId + Environment.NewLine
            //                     + "userHostName：" + userHostName + Environment.NewLine
            //                     + "clientIp：" + clientIp + Environment.NewLine
            //                     + "url：" + url + Environment.NewLine
            //                     + "referrerurl：" + referrerurl + Environment.NewLine
            //                     + "ExpiredDateTime：" + ExpiredDateTime);
            //}

            var result = string.Empty;
            //if (ExpiredDateTime.HasValue && ExpiredDateTime < DateTime.UtcNow)
            //    return string.Format("抱歉，服务账号（{0}）已经超期了(过期时间：{1})，请重新申请。", memberId, ExpiredDateTime.Value.ToLocalTime());

            var isValid = MemberId.Equals(memberId, StringComparison.OrdinalIgnoreCase);
            if (!isValid)
                return string.Format("抱歉，服务账号({0})未通过认证，请提供正确的服务账号。", memberId);

            //isValid = ApplicationDomain.Equals(url, StringComparison.OrdinalIgnoreCase);
            //if (!isValid)
            //    return "抱歉，该域名(" + url + ")未被授权访问服务。";

            return result;
        }

        private string ObjToString()
        {
            return string.Format("key={0};UserId={1};MemberId={2};RandomId={3}", _key, UserId, MemberId, RandomId);
        }

        private bool InitObject(string objStr)
        {
            try
            {
                var dictObj = StringExtensions.KeyValuePairFromConnectionString(objStr);
                _key = dictObj["key"];
                UserId = int.Parse(dictObj["userid"]);
                MemberId = dictObj["memberid"];
                RandomId = dictObj["randomid"];
                return true;
            }
            catch (Exception ex)
            {
                LogUtil.LogException(ex);
                return false;
            }
        }
    }
}
