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
    [Table(Tables.ApiInputParam)]
    public class ApiInputParam : TreeNode<ApiInputParam>
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
        /// 是否必填
        /// </summary>
        [DataMember]
        public bool IsNotNull { get; set; }
        /// <summary>
        /// 请求类型：Body、Query、Header
        /// </summary>
        [DataMember]
        public ApiInputRequestType RequestType { get; set; }
        /// <summary>
        /// Body、Query类型
        /// </summary>
        [DataMember]
        public ApiInputBodyType BodyType { get; set; }

        [DataMember]
        public int ApiDefId { get; set; }
        [DataMember]
        [ForeignKey("ApiDefId")]
        public ApiDefinition ApiDefinition { get; set; }
    }
}
