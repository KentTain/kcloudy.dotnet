using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KC.WebApi.Pay.Models.BoHai
{
    public class VisTransferOutRetModel : BoHaiReturnModel
    {
        /// <summary>
        /// 预计到账日期 yyyy-mm-dd 此日期为预估日期
        /// </summary>
        public string ReceivedDate { get; set; }
    }
}