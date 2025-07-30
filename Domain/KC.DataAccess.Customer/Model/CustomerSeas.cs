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
    [Table(Tables.CustomerSeas)]
    public class CustomerSeas : EntityBase
    {
        [Key]
        public int Id { get; set; }
        public int CustomerId { get; set; }

        [MaxLength(128)]
        public string OperatorId { get; set; }

        [MaxLength(50)]
        public string Operator { get; set; }
        public DateTime? OperateDate { get; set; }
        public int? OrganizationId { get; set; } 

        [ForeignKey("CustomerId")]
        public CustomerInfo CustomerInfo { get; set; }
    }
}
