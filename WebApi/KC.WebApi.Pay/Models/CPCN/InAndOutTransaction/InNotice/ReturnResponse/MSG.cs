using KC.WebApi.Pay.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KC.WebApi.Pay.Models.CPCN.InAndOutTransaction.InNotice.ReturnResponse
{
    public class MSG
    {
        public MSGHD MSGHD { get; set; }

        public Srl Srl { get; set; }
    }
}