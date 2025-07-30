
using KC.Enums.Doc;
using KC.Framework.Base;
using KC.Model.Doc.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text;

namespace KC.Model.Doc
{
    [Table(Tables.DocTemplate)]
    public class DocTemplate : DataPermitEntity
    {
        public DocTemplate()
        {
            DocTemplateLogs = new List<DocTemplateLog>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] //数据库生成默认值
        public int Id { get; set; }
        /// <summary>
        /// 资料名称
        /// </summary>
        [DataMember]
        [MaxLength(1000)]
        public string Name { get; set; }
        /// <summary>
        /// 资料类型
        /// </summary>
        public DocLevel Level { get; set; }

        /// <summary>
        /// 模板状态
        /// </summary>
        public WorkflowBusStatus Status { get; set; }

        /// <summary>
        /// 说明
        /// </summary>
        [MaxLength(1000)]
        [DataMember]
        public string Comment { get; set; }
        /// <summary>
        /// 上传时间
        /// </summary>
        [DataMember]
        public DateTime? UploadedTime { get; set; }
        /// <summary>
        /// 附件存储（Json字符串）
        /// </summary>
        [MaxLength(1000)]
        [DataMember]
        public string AttachmentBlob { get; set; }

        public virtual ICollection<DocTemplateLog> DocTemplateLogs { get; set; }
    }
}
