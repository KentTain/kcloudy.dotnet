using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Service.Constants
{
    public static class OAuthConstants
    {
        public const string AuthorizePath = "/OAuth/Authorize";
        public const string TokenPath = "/OAuth/Token";
        public const string LoginPath = "/Account/SignIn";
        public const string LogoutPath = "/Account/SignOut";

        public const string WeixinServer = "https://api.weixin.qq.com/";

        public const string AppTokenUrl = "cgi-bin/token?grant_type=client_credential&appid={appid}&secret={secret}";
        public const string JsapiTicketUrl = "cgi-bin/ticket/getticket?access_token={token}&type=jsapi";

        public const string WeixinOAuth = "sns/oauth2/access_token?appid={appid}&secret={secret}&code={code}&grant_type=authorization_code";
        public const string GetUserInfoUrl = "sns/userinfo?access_token={token}&openid={openid}&lang=zh_CN";

        public const string SnsapiBaseUrl = "https://open.weixin.qq.com/connect/oauth2/authorize?appid={appid}&redirect_uri={redirect_uri}?u={memberId}&response_type=code&scope=snsapi_base&state=state#wechat_redirect";

        public const string SSOBindWx = "account/BingWX?openId={openId}&openType=wx&returnUrl={refUrl}";
        public const string SSORegisterAndBing = "account/RegisterAndBing?openId={openId}&openType=wx&returnUrl={refUrl}";
        public const string SSOUnBindWx = "account/UnBingWX?openId={openId}&openType=wx&returnUrl={refUrl}";

        public const string GetWXUserInfoUrl = "/Home/GetWXUserInfo";
        public const string GetWXCode = "/Home/GetWXCode";
    }
}
