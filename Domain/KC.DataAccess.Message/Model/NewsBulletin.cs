using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using KC.Enums.Message;
using KC.Framework.Base;
using KC.Model.Message.Constants;

namespace KC.Model.Message
{
    [Table(Tables.NewsBulletin)]
    public class NewsBulletin : Entity
    {
        public NewsBulletin()
        {
            Type = NewsBulletinType.Internal;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// 文章类型
        /// </summary>
        [DataMember]
        [Required]
        public NewsBulletinType Type { get; set; }

        /// <summary>
        /// 文章标题
        /// </summary>
        [DataMember]
        [Required]
        [MaxLength(128)]
        public string Title { get; set; }

        /// <summary>
        /// 作者
        /// </summary>
        [DataMember]
        [MaxLength(128)]
        public string Author { get; set; }

        /// <summary>
        /// 作者邮箱
        /// </summary>
        [DataMember]
        [MaxLength(128)]
        public string AuthorEmail { get; set; }

        /// <summary>
        /// 关键字
        /// </summary>
        [DataMember]
        [MaxLength(512)]
        public string Keywords { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [DataMember]
        [MaxLength(2000)]
        public string Description { get; set; }

        /// <summary>
        /// 文章配图
        /// </summary>
        [DataMember]
        [MaxLength(1000)]
        public string ImageBlob { get; set; }

        /// <summary>
        /// 文章附件
        /// </summary>
        [DataMember]
        [MaxLength(1000)]
        public string FileBlob { get; set; }

        /// <summary>
        /// 外链
        /// </summary>
        [DataMember]
        [MaxLength(512)]
        public string Link { get; set; }
        /// <summary>
        /// 文章内容(显示时，填充html description)
        /// </summary>
        [DataMember]
        [Required]
        public string Content { get; set; }
        /// <summary>
        /// 是否显示
        /// </summary>
        [DataMember]
        public bool IsShow { get; set; }

        /// <summary>
        /// 发布状态
        /// </summary>
        [DataMember]
        public WorkflowBusStatus Status { get; set; }

        [DataMember]
        public int? CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public virtual NewsBulletinCategory Category { get; set; }
    }
}
