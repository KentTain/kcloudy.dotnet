using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using KC.Common;
using KC.Framework.Base;
using KC.Framework.Extension;
using KC.Service.Enums.Message;

namespace KC.Service.DTO.Message
{
    [Serializable, DataContract(IsReference = true)]
    public class NewsBulletinDTO : EntityDTO
    {
        public NewsBulletinDTO()
        {
            Type = NewsBulletinType.Internal;
        }

        [DataMember]
        public bool IsEditMode { get; set; }
        
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// 文章类型
        /// </summary>
        [DataMember]
        [Required]
        public NewsBulletinType Type { get; set; }
        [DataMember]
        public string TypeStr { get { return Type.ToDescription(); } }

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
        /// 附件对象
        /// </summary>
        [DataMember]
        public BlobInfoDTO Image
        {
            get
            {
                if (ImageBlob.IsNullOrEmpty())
                    return null;

                return SerializeHelper.FromJson<BlobInfoDTO>(ImageBlob);
            }
        }

        /// <summary>
        /// 文章附件
        /// </summary>
        [DataMember]
        [MaxLength(1000)]
        public string FileBlob { get; set; }
        /// <summary>
        /// 附件对象
        /// </summary>
        [DataMember]
        public BlobInfoDTO File
        {
            get
            {
                if (FileBlob.IsNullOrEmpty())
                    return null;

                return SerializeHelper.FromJson<BlobInfoDTO>(FileBlob);
            }
        }

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
        public string StatusStr
        {
            get { return Status.ToDescription(); }
        }

        [DataMember]
        public int? CategoryId { get; set; }
        [DataMember]
        public string CategoryName { get; set; }

    }
}
