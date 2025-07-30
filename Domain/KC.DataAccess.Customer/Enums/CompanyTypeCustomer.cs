using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KC.Enums.CRM
{
    [DataContract]
    public enum BusinessState
    {
        [EnumMember] [Display(Name = "在业")] [Description("在业")] Normal = 0,

        [EnumMember] [Display(Name = "存续")] [Description("存续")] Survival = 1,

        [EnumMember] [Display(Name = "注销")] [Description("注销")] Cancellation = 2
    }

    [DataContract]
    public enum CompanyPersons
    {
        [EnumMember] [Display(Name = "0-50人")] [Description("0-50人")] Less = 0,

        [EnumMember] [Display(Name = "50-100人")] [Description("50-100人")] Normal = 1,

        [EnumMember] [Display(Name = "100-200人")] [Description("100-200人")] Much = 2,

        [EnumMember] [Display(Name = "200人以上")] [Description("200人以上")] VeryMuch = 3
    }
}
