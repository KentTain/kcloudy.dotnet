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
    [Table(Tables.RelationDefDetail)]
    public class RelationDefDetail : Entity
    {
        [Key]
        [DataMember]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] //数据库生成默认值
        public int Id { get; set; }

        /// <summary>
        /// 关系类型
        /// </summary>
        [DataMember]
        [Display(Name = "关系类型")]
        public RelationType RelationType { get; set; }

        /// <summary>
        /// 属性名
        /// </summary>
        [DataMember]
        [MaxLength(50)]
        [Display(Name = "属性名")]
        public string Name { get; set; }

        /// <summary>
        /// 显示名称
        /// </summary>
        [DataMember]
        [MaxLength(200)]
        [Display(Name = "显示名称")]
        public string DisplayName { get; set; }

        /// <summary>
        /// 主表属性Id
        /// </summary>
        [DataMember]
        [Display(Name = "主表属性Id")]
        public int MainModelDefFieldId { get; set; }

        /// <summary>
        /// 子表Id
        /// </summary>
        [DataMember]
        [Display(Name = "子表Id")]
        public int SubModelDefId { get; set; }

        /// <summary>
        /// 子表属性Id
        /// </summary>
        [DataMember]
        [Display(Name = "子表属性Id")]
        public int SubModelDefFieldId { get; set; }

        /// <summary>
        /// 关系模型Id
        /// </summary>
        [DataMember]
        public int RelationDefId { get; set; }

        [DataMember]
        [ForeignKey("RelationDefId")]
        public RelationDefinition RelationDefinition { get; set; }
    }
}
