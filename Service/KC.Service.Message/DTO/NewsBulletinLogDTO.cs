using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using KC.Enums.Message;
using KC.Model.Message.Constants;
using KC.Framework.Base;
using KC.Framework.Extension;

namespace KC.Service.DTO.Message
{
    [Serializable]
    [DataContract(IsReference = true)]
    public class NewsBulletinLogDTO : ProcessLogBaseDTO
    {
        [DataMember]
        public int NewsBulletinId { get; set; }
        /// <summary>
        /// 文章类型
        /// </summary>
        [DataMember]
        [Required]
        public NewsBulletinType NewsBulletinType { get; set; }

        /// <summary>
        /// 文章标题
        /// </summary>
        [DataMember]
        [Required]
        [MaxLength(128)]
        public string NewsBulletinTitle { get; set; }

        /// <summary>
        /// 作者
        /// </summary>
        [DataMember]
        [MaxLength(128)]
        public string NewsBulletinAuthor { get; set; }

        /// <summary>
        /// 发布状态
        /// </summary>
        [DataMember]
        public WorkflowBusStatus NewsBulletinStatus { get; set; }
        [DataMember]
        public string StatusString { get { return NewsBulletinStatus.ToDescription(); } }

        /// <summary>
        /// 文章内容(显示时，填充html description)
        /// </summary>
        [DataMember]
        public string NewsBulletinContent { get; set; }

        
    }
}
