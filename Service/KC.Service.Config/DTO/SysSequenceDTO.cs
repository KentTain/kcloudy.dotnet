using KC.Service.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text;

namespace KC.Service.DTO.Config
{
    [Serializable, DataContract(IsReference = true)]
    public class SysSequenceDTO : EntityBaseDTO
    {
        [DataMember]
        public bool IsEditMode { get; set; }
        [MaxLength(32)]
        [DataMember]
        public string SequenceName { get; set; }
        /// <summary>
        /// 当前值
        /// </summary>
        [DataMember]
        public int CurrentValue { get; set; }
        /// <summary>
        /// 初试值
        /// </summary>
        [DataMember]
        public int InitValue { get; set; }
        /// <summary>
        /// 最大值
        /// </summary>
        [DataMember]
        public int MaxValue { get; set; }
        /// <summary>
        /// 步长
        /// </summary>
        [DataMember]
        public int StepValue { get; set; }
        /// <summary>
        /// 前缀
        /// </summary>
        [MaxLength(12)]
        [DataMember]
        public string PreFixString { get; set; }
        /// <summary>
        /// 后缀
        /// </summary>
        [MaxLength(12)]
        [DataMember]
        public string PostFixString { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        [MaxLength(256)]
        [DataMember]
        public string Comments { get; set; }
        /// <summary>
        /// 当前时间
        /// </summary>
        [DataMember]
        public string CurrDate { get; set; }
    }
}
