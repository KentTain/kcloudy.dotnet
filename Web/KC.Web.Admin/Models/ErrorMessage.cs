using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KC.Web.Admin.Models
{
    public class ErrorMessage
    {
        public ErrorMessage() { }

        public string DisplayMode { get; set; }
        public string UiLocales { get; set; }
        public string Error { get; set; }
        public string ErrorDescription { get; set; }
        public string RequestId { get; set; }
        public string RedirectUri { get; set; }
        public string ResponseMode { get; set; }
        public string ClientId { get; set; }
    }
}
