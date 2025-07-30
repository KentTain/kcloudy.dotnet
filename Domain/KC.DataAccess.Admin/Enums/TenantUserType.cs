using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Enums.App
{
    public enum TenantUserType
    {
        [Display(Name = "企业/核心企业")]
        [Description("企业")]
        Company = 2,

        [Display(Name = "机构")]
        [Description("机构")]
        Institution = 3
    }
}
