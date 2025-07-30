using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using KC.Framework.Base;
using KC.Framework.Tenant;
using KC.Model.CodeGenerate.Constants;

namespace KC.Model.CodeGenerate
{
    [Table(Tables.RelationDefinition)]
    public class RelationDefinition : Entity
    {
        public RelationDefinition()
        {
            DefDetails = new List<RelationDefDetail>();
        }

        [Key]
        [DataMember]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] //数据库生成默认值
        public int Id { get; set; }

        /// <summary>
        /// 类名
        /// </summary>
        [DataMember]
        [MaxLength(50)]
        [Display(Name = "类名")]
        public string Name { get; set; }

        /// <summary>
        /// 显示名称
        /// </summary>
        [DataMember]
        [MaxLength(200)]
        [Display(Name = "显示名称")]
        public string DisplayName { get; set; }

        /// <summary>
        /// 主表Id
        /// </summary>
        [DataMember]
        [Display(Name = "主表Id")]
        public int MainModelDefId { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [MaxLength(4000)]
        [DataMember]
        public string Description { get; set; }

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
        public virtual ICollection<RelationDefDetail> DefDetails { get; set; }
    }
}
