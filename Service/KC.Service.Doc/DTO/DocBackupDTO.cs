using KC.Service.DTO;
using KC.Enums.Doc;
using KC.Framework.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text;

namespace KC.Service.DTO.Doc
{
    [Serializable]
    [DataContract(IsReference = true)]
    public class DocBackupDTO : EntityBaseDTO
    {
        [DataMember]
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
        /// 说明
        /// </summary>
        [MaxLength(1000)]
        [DataMember]
        public string Comment { get; set; }
        /// <summary>
        /// 上传时间
        /// </summary>
        [DataMember]
        public DateTime? CreatedDateTime { get; set; }
        /// <summary>
        /// 过期时间
        /// </summary>
        [DataMember]
        public DateTime? ExpiredDateTime { get; set; }
        /// <summary>
        /// 附件存储Id
        /// </summary>
        [MaxLength(1000)]
        [DataMember]
        public string AttachmentBlob { get; set; }
        [DataMember]
        public int DocumentId { get; set; }

        [ForeignKey("DocumentId")]
        public DocumentInfoDTO DocumentInfo { get; set; }
    }
}
