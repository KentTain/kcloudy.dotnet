using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using KC.Framework.Extension;
using KC.Service.DTO;
using KC.Service.Enums.Message;

namespace KC.Service.DTO.Message
{
    [Serializable, DataContract(IsReference = true)]
    public class MessageDTO : EntityDTO
    {
        [DataMember]
        public string MessageClassCode { get; set; }

        [DataMember]
        public string MessageClassName { get; set; }

        [DataMember]
        public int TemplateId { get; set; }
        
        [DataMember]
        public MessageTemplateType TemplateType { get; set; }

        [DataMember]
        public string TemplateTypeStr
        {
            get { return TemplateType.ToDescription(); }
        }

        /// <summary>
        /// 模版名称
        /// </summary>
        [MaxLength(128)]
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// 模版主题
        /// </summary>
        [MaxLength(256)]
        [DataMember]
        public string Subject { get; set; }

        /// <summary>
        /// 模版内容
        /// </summary>
        [DataMember]
        public string Content { get; set; }

        /// <summary>
        /// 可替换参数对照字典表的Json字符串（key：可替换参数；value：替换参数值）
        ///     例如：{'code':'1001', 'name':'test'}
        /// </summary>
        [DataMember]
        public string ReplaceValueString { get; set; }

        
    }
}
