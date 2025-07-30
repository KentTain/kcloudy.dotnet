using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.Model.Account.Constants;
using KC.Framework.Base;

namespace KC.Model.Account
{
    [Table(Tables.UserTracingLog)]
    public class UserTracingLog : ProcessLogBase
    {
        
    }
}
