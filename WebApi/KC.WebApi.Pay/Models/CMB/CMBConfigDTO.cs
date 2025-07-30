using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KC.WebApi.Pay.Models.CMB
{
    public class CMBConfigDTO : CMBBaseModel
    {
        public int ConfigId { get; set; }

        public int ConfigSign { get; set; }

        public string PayUrl { get; set; }

        public string Key { get; set; }

    }
}