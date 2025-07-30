using System;
using System.Runtime.Serialization;
using System.Text;
using ProtoBuf;

namespace KC.Service.Base
{
    [Serializable, DataContract(IsReference = true)]
    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public class OAuthTokenResult
    {
        public OAuthTokenResult()
        {
            CreateTime = DateTime.UtcNow;
        }
        [DataMember(Name = "access_token")]
        public string AccessToken { get; set; }
        [DataMember(Name = "expires_in")]
        public string ExpiresIn { get; set; }
        [DataMember(Name = "refresh_token")]
        public string RefreshToken { get; set; }
        [DataMember(Name = "token_type")]
        public string TokenType { get; set; }
        [DataMember(Name = "scope")]
        public string Scope { get; set; }
        [DataMember(Name = "openid")]
        public string OpenId { get; set; }
        [DataMember(Name = "createtime")]
        public DateTime CreateTime { get; set; }
        public bool IsTimeOut()
        {
            if (string.IsNullOrWhiteSpace(ExpiresIn))
                return false;
            return DateTime.Now.AddSeconds(-int.Parse(ExpiresIn) - 60) > CreateTime;
        }
    }

    [Serializable, DataContract(IsReference = true)]
    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public class WeixinAccessToken
    {
        public WeixinAccessToken()
        {
            CreateTime = DateTime.UtcNow;
        }
        [DataMember(Name = "AppId")]
        public string AppId { get; set; }
        [DataMember(Name = "access_token")]
        public string AccessToken { get; set; }
        [DataMember(Name = "expires_in")]
        public int ExpiresIn { get; set; }
        [DataMember(Name = "CreateTime")]
        public DateTime CreateTime { get; set; }
        [DataMember(Name = "IsValid")]
        [ProtoIgnore]
        public bool IsValid
        {
            get
            {
                //减300是为了有个缓存区，不要等到失效了才去取，不然可能取到快失效的，业务处理不成功
                return !string.IsNullOrWhiteSpace(AccessToken) && CreateTime.AddSeconds(ExpiresIn - 300) > DateTime.UtcNow;
            }
        }

    }

    [Serializable, DataContract(IsReference = true)]
    [ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
    public class WeixinJsTicket
    {
        public WeixinJsTicket()
        {
            CreateTime = DateTime.UtcNow;
        }
        [DataMember(Name = "errcode")]
        public string ErrorCode { get; set; }
        [DataMember(Name = "errmsg")]
        public string ErrorMessage { get; set; }
        [DataMember(Name = "expires_in")]
        public int ExpiresIn { get; set; }
        [DataMember(Name = "ticket")]
        public string Ticket { get; set; }


        [DataMember(Name = "AppId")]
        public string AppId { get; set; }
        [DataMember(Name = "CreateTime")]
        public DateTime CreateTime { get; set; }
        [DataMember(Name = "access_token")]
        public string AccessToken { get; set; }
        [DataMember(Name = "Noncestr")]
        public string Noncestr
        {
            get
            {
                return "Q231fesdfsadfASDF321QEQASfadg";
            }
        }
        [DataMember(Name = "Timestamp")]
        public long Timestamp
        {
            get
            {
                return (CreateTime.AddHours(8).Ticks - 621355968000000000) / 10000000;
            }
        }
        [DataMember(Name = "Signature")]
        public string Signature { set; get; }

        public void SetSignature(string url)
        {
            string strValue = string.Format("jsapi_ticket={0}&noncestr={1}&timestamp={2}&url={3}"
                , Ticket, Noncestr, Timestamp, url);
            byte[] buffer = Encoding.UTF8.GetBytes(strValue);

            var sha1 = System.Security.Cryptography.SHA1.Create();
            var hash = sha1.ComputeHash(buffer);

            Signature = Encoding.UTF8.GetString(hash).ToLower();
            //Signature = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(strValue, "SHA1").ToLower();
        }
        [DataMember(Name = "IsValid")]
        public bool IsValid
        {
            get
            {
                //减300是为了有个缓存区，不要等到失效了才去取，不然可能取到快失效的，业务处理不成功
                return !string.IsNullOrWhiteSpace(Ticket) && CreateTime.AddSeconds(ExpiresIn - 300) > DateTime.UtcNow;
            }
        }
    }
}
