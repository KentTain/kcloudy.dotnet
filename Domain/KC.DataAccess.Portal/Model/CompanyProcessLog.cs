using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.Framework.Base;
using KC.Model.Portal.Constants;

namespace KC.Model.Portal
{
    [Table(Tables.CompanyProcessLog)]
    public class  CompanyProcessLog : ProcessLogBase
    {
        [MaxLength(1024)]
        public string CompanyName { get; set; }

        public string CompanyCode { get; set; }
    }
}
