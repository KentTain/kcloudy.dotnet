using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.Model.CodeGenerate.Constants;
using KC.Framework.Base;
using KC.Framework.Tenant;
using KC.Enums.CodeGenerate;
using System.Runtime.Serialization;

namespace KC.Model.CodeGenerate
{
    [Table(Tables.ModelCategory)]
    public class ModelCategory : TreeNode<ModelCategory>
    {
        public ModelCategory()
        {
        }

        /// <summary>
        /// 模型类型
        /// </summary>
        [DataMember]
        public ModelType ModelType { get; set; }


        /// <summary>
        /// 应用Id
        /// </summary>
        [DataMember]
        public string ApplicationId { get; set; }
    }
}
