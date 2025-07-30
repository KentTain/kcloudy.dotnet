using KC.Enums.Contract;
using KC.Service.DTO;
using System;
using System.Runtime.Serialization;
 

namespace KC.Service.DTO.Contract
{
    [Serializable]
    [DataContract(IsReference = true)]
    public  class ContractTemplateDTO : EntityDTO
    {

        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// 合同名称
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// 合同类型
        /// </summary>
        [DataMember]
        public ContractType Type { get; set; }

        [DataMember]
        public string TypeStr { get; set; }
        /// <summary>
        /// 交易类型
        /// </summary>
        [DataMember]
        public string TransactionTypeName { get; set; }

        /// <summary>
        /// 合同模板内容
        /// </summary>
        [DataMember]
        public string ContractValue { get; set; }

     }
}
