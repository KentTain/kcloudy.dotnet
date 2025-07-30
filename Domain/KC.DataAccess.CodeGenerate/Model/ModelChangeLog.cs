using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using KC.Enums.CodeGenerate;
using KC.Framework.Base;
using KC.Framework.Tenant;
using KC.Model.CodeGenerate.Constants;

namespace KC.Model.CodeGenerate
{
    [Table(Tables.ModelChangeLog)]
    public class ModelChangeLog : ProcessLogBase
    {
        public ModelType ModelType { get; set; }

        [MaxLength(128)]
        public String ReferenceId { get; set; }

        /// <summary>
        /// 模型名称
        /// </summary>
        [MaxLength(256)]
        public string ReferenceName { get; set; }

        /// <summary>
        /// 引用对象Json
        /// </summary>
        public string RefObjectJson { get; set; }
    }
}
