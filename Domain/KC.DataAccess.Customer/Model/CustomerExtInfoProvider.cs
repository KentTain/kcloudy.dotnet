using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.Framework.Base;
using KC.Model.Customer.Constants;

namespace KC.Model.Customer
{
    [Table(Tables.CustomerExtInfoProvider)]
    public class CustomerExtInfoProvider : PropertyAttributeBase
    {
    }
}
