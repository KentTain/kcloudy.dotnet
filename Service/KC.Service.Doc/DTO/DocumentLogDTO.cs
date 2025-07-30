using System;
using System.Collections.Generic;
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
    public class DocumentLogDTO : ProcessLogBaseDTO
    {
        [DataMember]
        public DocOperType DocOperType { get; set; }

        [DataMember]
        public int DocumentId { get; set; }

        [DataMember]
        public DocumentInfoDTO DocumentInfo { get; set; }
    }
}
