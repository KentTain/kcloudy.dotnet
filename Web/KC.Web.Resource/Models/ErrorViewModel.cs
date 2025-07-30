using System;

namespace KC.Web.Resource.Models
{
    public class ErrorViewModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        public string TenantName { get; set; }

        public string ReturnUrl { get; set; }
    }
}