using System.ComponentModel.DataAnnotations;
using KC.Framework.Base;
using System.Runtime.Serialization;
using KC.Enums.Doc;
using System;
using System.Collections.Generic;
using KC.Service.DTO;
using KC.Framework.Extension;
using KC.Common;

namespace KC.Service.DTO.Doc
{
    [Serializable]
    [DataContract(IsReference = true)]
    public class DocumentInfoDTO : DataPermitEntityDTO
    {
        public DocumentInfoDTO()
        {
            DocumentLogs = new List<DocumentLogDTO>();
            DocBackups = new List<DocBackupDTO>();
        }

        [DataMember]
        public bool IsArchive { get; set; }
        [DataMember]
        public bool IsEditMode { get; set; }
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        [MaxLength(20)]
        public string DocCode { get; set; }
        /// <summary>
        /// 资料名称
        /// </summary>
        [DataMember]
        [MaxLength(1000)]
        public string Name { get; set; }

        [DataMember]
        public LableType Type { get; set; }
        [DataMember]
        public string TypeName
        {
            get
            {
                return Type.ToDescription();
            }
        }
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

        /// <summary>
        /// 文件扩展名
        /// </summary>
        [DataMember]
        [MaxLength(50)]
        public string Ext { get; set; }

        /// <summary>
        /// 文件类型：KC.Common.FileHelper.FileType
        ///     Image=1、Word=2、Excel=3、PDF=4、PPT=5、Text=6、Xml=7、Audio=8、Video=9、Unknown
        /// </summary>
        [DataMember]
        public string FileType { get; set; }
        /// <summary>
        /// 文件格式：
        ///     KC.Common.FileHelper.ImageFormat(Bmp = 1, Gif = 2, Icon = 3, Jpeg = 4, Png = 5, Tiff = 6, Wmf = 7, Unknown = 99)
        ///     KC.Common.FileHelper.DocFormat(Doc = 1, Docx = 2, Xls = 3, Xlsx = 4, Ppt = 5, Pptx = 6, Pdf = 7, Unknown = 99)
        ///     KC.Common.FileHelper.AudioFormat(Basic = 1, Wav = 2, Mpeg = 3, Ram = 4, Rmi = 5, Aif = 6, Unknown = 99)
        /// </summary>
        [DataMember]
        public string FileFormat { get; set; }

        /// <summary>
        /// 文件大小
        /// </summary>
        [DataMember]
        public long Size { get; set; }

        /// <summary>
        /// 模版
        /// </summary>
        [MaxLength(1000)]
        [DataMember]
        public string TemplateBlob { get; set; }
        /// <summary>
        /// 附件对象
        /// </summary>
        [DataMember]
        public BlobInfoDTO Template
        {
            get
            {
                if (TemplateBlob.IsNullOrEmpty())
                    return null;

                return SerializeHelper.FromJson<BlobInfoDTO>(TemplateBlob);
            }
        }
        /// <summary>
        /// 是否有模版
        /// </summary>
        [DataMember]
        public bool HasTemplates { get; set; }

        /// <summary>
        /// 是否归档文件
        /// </summary>
        [DataMember]
        public bool IsValid { get; set; }

        /// <summary>
        /// 是否公开
        /// </summary>
        [DataMember]
        public bool IsPublic { get; set; }

        /// <summary>
        /// 是否可编辑
        /// </summary>
        [DataMember]
        public bool CanEdit { get; set; }

          /// <summary>
        /// 是否可外发
        /// </summary>
        [DataMember]
        public bool CanSend { get; set; }

        [DataMember]
        public string TenantName { get; set; }

        [DataMember]
        public int? DocCategoryId { set; get; }

        [DataMember]
        public DocCategoryDTO DocCategory { get; set; }

        [DataMember]
        public string UserNamesString { get; set; }

        public virtual ICollection<DocumentLogDTO> DocumentLogs { get; set; }

        public virtual ICollection<DocBackupDTO> DocBackups { get; set; }
    }
}
