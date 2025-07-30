using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using KC.Framework.Base;
using KC.Model.Customer.Constants;

namespace KC.Model.Customer
{
    [Table(Tables.CustomerExtInfo)]
    public class CustomerExtInfo : PropertyAttributeBase
    {
        public int CustomerId { get; set; }
        [ForeignKey("CustomerId")]
        public CustomerInfo CustomerInfo { get; set; }

        public int? CustomerExtInfoProviderId { get; set; }
        [ForeignKey("CustomerExtInfoProviderId")]
        public CustomerExtInfoProvider CustomerExtInfoProvider { get; set; }
    }
}
