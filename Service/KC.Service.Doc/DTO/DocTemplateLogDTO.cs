using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.Framework.Base;
using KC.Enums.Doc;
using System.Runtime.Serialization;
using KC.Service.DTO;

namespace KC.Service.DTO.Doc
{
    [Serializable]
    [DataContract(IsReference = true)]
    public class DocTemplateLogDTO : ProcessLogBaseDTO
    {
        [DataMember]
        public DocOperType DocOperType { get; set; }
        [DataMember]
        public int DocTemplateId { get; set; }
        [DataMember]
        public DocTemplateDTO DocumentInfo { get; set; }
    }
}
