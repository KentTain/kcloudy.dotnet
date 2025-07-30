using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KC.Web.SSO.Models
{
    public class JwtRequestViewModel
    {
        public string GrantType { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public string ClientId { get; set; }
        public string Token { get; set; }
    }

    public class JwtResponseViewModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }

        public string Token { get; set; }

        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }

        public int Expiration { get; set; }

    }
}
