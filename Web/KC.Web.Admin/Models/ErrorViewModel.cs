using System;

namespace KC.Web.Admin.Models
{
    public class ErrorViewModel
    {
        public ErrorViewModel()
        {
        }

        public ErrorViewModel(string error)
        {
            Error = new ErrorMessage { Error = error };
        }

        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        public ErrorMessage Error { get; set; }

        public string TenantName { get; set; }

        public string ReturnUrl { get; set; }
    }
}