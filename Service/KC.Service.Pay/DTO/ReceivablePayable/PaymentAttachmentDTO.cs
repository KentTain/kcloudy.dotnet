using KC.Enums.Pay;
using KC.Framework.Extension;
using KC.Service.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KC.Service.DTO.Pay
{
    [Serializable]
    [DataContract(IsReference = true)]
    public class PaymentAttachmentDTO: EntityDTO
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string BusinessNumber{ get; set; }

        [DataMember]
        public string BlobId { get; set; }

        [DataMember]
        public string FileName { get; set; }

        [DataMember]
        public string Url { get; set; }
    }
}
