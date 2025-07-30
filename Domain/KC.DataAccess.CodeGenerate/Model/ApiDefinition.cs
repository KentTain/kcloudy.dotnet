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
    [Table(Tables.ApiDefinition)]
    public class ApiDefinition : Entity
    {
        public ApiDefinition()
        {
            ApiInputParams = new List<ApiInputParam>();
            ApiOutParams = new List<ApiOutParam>();
        }

        [Key]
        [DataMember]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] //数据库生成默认值
        public int Id { get; set; }

        /// <summary>
        /// 接口状态
        /// </summary>
        [DataMember]
        public ApiStatus ApiStatus { get; set; }
        /// <summary>
        /// Api接口类型
        /// </summary>
        [DataMember]
        public ApiMethodType ApiMethodType { get; set; }

        /// <summary>
        /// http类型
        /// </summary>
        [DataMember]
        public ApiHttpType ApiHttpType { get; set; }

        /// <summary>
        /// 接口名称
        /// </summary>
        [DataMember]
        [MaxLength(50)]
        [Display(Name = "接口名称")]
        public string Name { get; set; }

        /// <summary>
        /// 显示名称
        /// </summary>
        [DataMember]
        [MaxLength(200)]
        [Display(Name = "显示名称")]
        public string DisplayName { get; set; }

        /// <summary>
        /// Url地址
        /// </summary>
        [DataMember]
        [MaxLength(2000)]
        [Display(Name = "Url")]
        public string Url { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [MaxLength(4000)]
        [DataMember]
        public string Description { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [DataMember]
        public int Index { get; set; }

        /// <summary>
        /// 分组Id
        /// </summary>
        [DataMember]
        public int? CategoryId { get; set; }

        [DataMember]
        [ForeignKey("CategoryId")]
        public ModelCategory ModelCategory { get; set; }

        /// <summary>
        /// 应用Id
        /// </summary>
        [DataMember]
        public string ApplicationId { get; set; }

        [DataMember]
        public virtual ICollection<ApiInputParam> ApiInputParams { get; set; }
        [DataMember]
        public virtual ICollection<ApiOutParam> ApiOutParams { get; set; }
    }
}
