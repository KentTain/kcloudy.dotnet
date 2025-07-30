using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KC.Service.DTO.Pay
{
    public class CreateReturnModel
    {
        [DataMember]
        public bool Success { get; set; }
        [DataMember]
        public string BillNo { get; set; }
        [DataMember]
        public string OrderTime { get; set; }
        [DataMember]
        public string OrderAmount { get; set; }

        [DataMember]
        public string ErrorMessage { get; set; }
    }
}
