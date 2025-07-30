using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KC.Enums.Account
{
    [Flags]
    public enum OrganizationDisplay
    {
        [Display(Name = "融资系统")]
        [Description("融资系统")]
        [EnumMember]
        融资系统 = 0x1,
        [Display(Name = "商城系统")]
        [Description("商城系统")]
        [EnumMember]
        商城系统 = 0x2,
        [Display(Name = "CRM系统")]
        [Description("CRM系统")]
        [EnumMember]
        CRM系统 = 0x4,
        [Display(Name = "ERP系统")]
        [Description("ERP系统")]
        [EnumMember]
        ERP系统 = 0x8,
        [Display(Name = "工作流")]
        [Description("工作流")]
        [EnumMember]
        工作流 = 0x10,
    }
}
