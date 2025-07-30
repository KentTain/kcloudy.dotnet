using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.Model.Account.Constants;
using KC.Framework.Base;
using System.ComponentModel.DataAnnotations;

namespace KC.Model.Account
{
    [Table(Tables.UserLoginLog)]
    public class UserLoginLog : ProcessLogBase
    {
        [MaxLength(100)]
        public string IPAddress { get; set; }

        [MaxLength(500)]
        public string BrowserInfo { get; set; }
    }
}
