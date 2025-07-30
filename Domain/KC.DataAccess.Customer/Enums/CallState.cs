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
    public enum CallState
    {
        [EnumMember] [Display(Name = "已接听")] [Description("已接听")] Succeed = 0,

        [EnumMember] [Display(Name = "未接听")] [Description("未接听")] Failed = 1
    }
}
