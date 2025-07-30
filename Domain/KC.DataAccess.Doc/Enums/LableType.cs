using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;

namespace KC.Enums.Doc
{
    [DataContract]
    public enum LableType
    {
        [EnumMember]
        [Display(Name = "个人")]
        [Description("个人")]
        Private = 0,

        [EnumMember]
        [Display(Name = "部门")]
        [Description("部门")]
        Department = 1,

        [EnumMember]
        [Display(Name = "公司")]
        [Description("公司")]
        Company = 2,

        [EnumMember]
        [Display(Name = "外部企业")]
        [Description("外部企业")]
        OutCompany = 3,

        [EnumMember]
        [Display(Name = "其他")]
        [Description("其他")]
        Other = 4,
    }
}
