using KC.Service.DTO;
using KC.Enums.Doc;
using KC.Framework.Base;
using KC.Framework.Extension;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text;
using KC.Common;

namespace KC.Service.DTO.Doc
{
    [Serializable]
    [DataContract(IsReference = true)]
    public class DocTemplateDTO : DataPermitEntityDTO
    {
        public DocTemplateDTO()
        {
            DocTemplateLogs = new List<DocTemplateLogDTO>();
        }

        [DataMember]
        public bool IsEditMode { get; set; }

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
        [DataMember]
        public DocLevel Level { get; set; }
        [DataMember]
        public string LevelName
        {
            get
            {
                return Level.ToDescription();
            }
        }
        /// <summary>
        /// 模板状态
        /// </summary>
        [DataMember]
        public WorkflowBusStatus Status { get; set; }
        [DataMember]
        public string StatusName
        {
            get
            {
                return Status.ToDescription();
            }
        }
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
        /// 附件存储Id
        /// </summary>
        [MaxLength(1000)]
        [DataMember]
        public string AttachmentBlob { get; set; }

        /// <summary>
        /// 附件对象
        /// </summary>
        [DataMember]
        public BlobInfoDTO Attachment
        {
            get
            {
                if (AttachmentBlob.IsNullOrEmpty())
                    return null;

                return SerializeHelper.FromJson<BlobInfoDTO>(AttachmentBlob);
            }
        }

        public virtual ICollection<DocTemplateLogDTO> DocTemplateLogs { get; set; }
    }
}
