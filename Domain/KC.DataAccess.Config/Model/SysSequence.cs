using KC.Framework.Base;
using KC.Model.Config.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace KC.Model.Config
{
    [Table(Tables.SysSequence)]
    public class SysSequence : EntityBase
    {
        [Key]
        [MaxLength(32)]
        public string SequenceName { get; set; }
        /// <summary>
        /// 当前值
        /// </summary>
        public int CurrentValue { get; set; }
        /// <summary>
        /// 初试值
        /// </summary>
        public int InitValue { get; set; }
        /// <summary>
        /// 最大值
        /// </summary>
        public int MaxValue { get; set; }
        /// <summary>
        /// 步长
        /// </summary>
        public int StepValue { get; set; }
        /// <summary>
        /// 前缀
        /// </summary>
        [MaxLength(12)]
        public string PreFixString { get; set; }
        /// <summary>
        /// 后缀
        /// </summary>
        [MaxLength(12)]
        public string PostFixString { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        [MaxLength(256)]
        public string Comments { get; set; }
        /// <summary>
        /// 当前时间
        /// </summary>
        public string CurrDate { get; set; }
    }
}
