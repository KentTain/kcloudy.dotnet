using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.Framework.Base;
using KC.Model.Customer.Constants;

namespace KC.Model.Customer
{
    [Table(Tables.CustomerSendToTenantLog)]
    public class CustomerSendToTenantLog : ProcessLogBase
    {
        public string TenantName { get; set; }
        public string TenantDisplayName { get; set; }

        public string CustomerIdList { get; set; }
    }
}
