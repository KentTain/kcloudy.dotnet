using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace KC.Service.DTO.Account
{
    [Serializable, DataContract(IsReference = true)]
    public class UserLoginLogDTO : ProcessLogBaseDTO
    {
        [DataMember]
        [MaxLength(100)]
        public string IPAddress { get; set; }

        [DataMember]
        [MaxLength(500)]
        public string BrowserInfo { get; set; }
    }
}
