using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using KC.Enums.CodeGenerate;
using KC.Framework.Base;
using KC.Framework.Tenant;
using KC.Model.CodeGenerate.Constants;

namespace KC.Model.CodeGenerate
{
    [Table(Tables.ApiOutParam)]
    public class ApiOutParam : TreeNode<ApiOutParam>
    {
        /// <summary>
        /// 属性数据类型
        /// </summary>
        [DataMember]
        public AttributeDataType DataType { get; set; }

        /// <summary>
        /// 显示名
        /// </summary>
        [DataMember]
        [MaxLength(100)]
        public string DisplayName { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        [DataMember]
        [MaxLength(4000)]
        public string Description { get; set; }
        /// <summary>
        /// 是否数组
        /// </summary>
        [DataMember]
        public bool IsArray { get; set; }
        /// <summary>
        /// 返回值类型：Json\Text\File\None
        /// </summary>
        [DataMember]
        public ApiOutReturnType ReturnType { get; set; }

        [DataMember]
        public int ApiDefId { get; set; }
        [DataMember]
        [ForeignKey("ApiDefId")]
        public ApiDefinition ApiDefinition { get; set; }
    }
}
