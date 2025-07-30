using KC.Enums.Contract;
using KC.Service.DTO;
using System;
using System.Runtime.Serialization;
 

namespace KC.Service.DTO.Contract
{
    [Serializable]
    [DataContract(IsReference = true)]
    public  class ContractTemplateAPIModel : EntityDTO
    {
        /// <summary>
        /// 合同模板主键Id
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// 合同类型
        /// </summary>
        [DataMember]
        public ContractType Type { get; set; }
        /// <summary>
        /// 合同类型字符串格式
        /// </summary>
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
