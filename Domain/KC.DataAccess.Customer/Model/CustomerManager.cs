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
    [Table(Tables.CustomerManager)]
    public class CustomerManager : EntityBase
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(128)]
        public string CustomerManagerId { get; set; }  

        [MaxLength(512)]
        public string CustomerManagerName { get; set; }
        public int? OrganizationId { get; set; }

        [MaxLength(256)]
        public string OrganizationName { get; set; }
        public int? CustomerId { get; set; }

        public bool? IsInSeas { get; set; }

        [ForeignKey("CustomerId")]
        public CustomerInfo CustomerInfo { get; set; }
    }
}
