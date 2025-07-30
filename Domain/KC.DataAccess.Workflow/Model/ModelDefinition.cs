using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using KC.Framework.Base;
using KC.Framework.Tenant;
using KC.Model.Workflow.Constants;

namespace KC.Model.Workflow
{
    [Table(Tables.ModelDefinition)]
    public class ModelDefinition : PropertyBase<ModelDefField>
    {
        /// <summary>
        /// 业务类型
        /// </summary>
        [DataMember]
        [Display(Name = "业务类型")]
        public BusinessType BusinessType { get; set; }

    }
}
