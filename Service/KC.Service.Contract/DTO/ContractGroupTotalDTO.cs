using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace KC.Service.DTO.Contract
{
    /// <summary>
    /// 合同类型统计
    /// </summary>
    [ProtoContract]
    [Serializable]
    [DataContract(IsReference = true)]
    public class ContractGroupTotalDTO
    {
        [DataMember]
        public int All { get; set; }

        [DataMember]
        public int Electronic { get; set; }
        [DataMember]
        public int AccountStatement { get; set; }

        [DataMember]
        public int Quartet { get; set; }

        [DataMember]
        public int Seller { get; set; }

        [DataMember]
        public int Purchase { get; set; }

        [DataMember]
        public int Agreement { get; set; }

        [DataMember]
        public int Lending { get; set; }
    }
}
