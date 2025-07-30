using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace KC.Enums.Offering
{
    [DataContract]
    public enum ServiceDataType
    {
        /// <summary>
        /// 文本框列表
        /// </summary>
        [Display(Name = "文本框列表")]
        [Description("文本框列表")]
        [EnumMember]
        TextList = 0,

        /// <summary>
        /// 单选列表
        /// </summary>
        [Display(Name = "单选列表")]
        [Description("单选列表")]
        [EnumMember]
        RadioList = 1,

        /// <summary>
        /// 下拉列表
        /// </summary>
        [Display(Name = "下拉列表")]
        [Description("下拉列表")]
        [EnumMember]
        DropdownList = 2,

        /// <summary>
        /// 复选框
        /// </summary>
        [Display(Name = "复选框")]
        [Description("复选框")]
        [EnumMember]
        CheckBoxList = 3,

        /// <summary>
        /// 最大最小值范围
        /// </summary>
        [Display(Name = "最大最小值范围")]
        [Description("最大最小值范围")]
        [EnumMember]
        Range = 4,

        /// <summary>
        /// 图片上传
        /// </summary>
        [Display(Name = "图片上传")]
        [Description("图片上传")]
        [EnumMember]
        ImageUpload = 5,

        /// <summary>
        /// 文件上传
        /// </summary>
        [Display(Name = "文件上传")]
        [Description("文件上传")]
        [EnumMember]
        FileUpload = 6,

        /// <summary>
        /// 文本编辑
        /// </summary>
        [Display(Name = "文本编辑")]
        [Description("文本编辑")]
        [EnumMember]
        TextArea = 7,
    }
}
