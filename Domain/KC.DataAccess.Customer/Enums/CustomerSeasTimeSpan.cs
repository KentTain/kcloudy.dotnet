using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;

namespace KC.Enums.CRM
{
    public enum CustomerSeasTimeSpan
    {
        [EnumMember] [Display(Name = "一周内")] [Description("一周内")] WithinOneWeek = 1,
        [EnumMember] [Display(Name = "一月内")] [Description("一月内")] WithinOneMonth = 2,
        [EnumMember] [Display(Name = "三月内")] [Description("三月内")] WithinThreeMonth = 3,
        [EnumMember] [Display(Name = "六月内")] [Description("六月内")] WithinSixMonth = 4,
        [EnumMember] [Display(Name = "六月以上")] [Description("六月以上")] WithOutSixMonth = 5
    }
}
