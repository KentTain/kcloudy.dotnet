using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using KC.Enums.App;
using KC.Framework.Extension;
using ProtoBuf;

namespace KC.Service.DTO.App
{
    [Serializable, DataContract(IsReference = true)]
    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public class ApplicationLogDTO : ProcessLogBaseDTO
    {
        [DataMember]
        [Display(Name = "日志类型")] 
        public AppLogType AppLogType { get; set; }

        [DataMember]
        public string AppLogTypeString { get { return AppLogType.ToDescription(); } }

        [DataMember]
        [Display(Name = "应用Id")] 
        public Guid ApplicationId { get; set; }

        /// <summary>
        /// 应用名称
        /// </summary>
        [MaxLength(128)]
        [DataMember]
        [Display(Name = "应用名称")] 
        public string ApplicationName { get; set; }
    }
}
