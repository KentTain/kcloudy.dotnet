using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.Model.Doc.Constants;
using KC.Framework.Base;
using System.Runtime.Serialization;
using KC.Enums.Doc;

namespace KC.Model.Doc
{
    [Serializable]
    [DataContract(IsReference = true)]
    [Table(Tables.DocCategory)]
    public class DocCategory : TreeNode<DocCategory>
    {
        public DocCategory()
        {
            DocumentInfos = new List<DocumentInfo>();
        }

        /// <summary>
        /// 0: 公司 1：部门  2：个人  3：外部企业   4：其他
        /// </summary>
        [DataMember]
        public LableType Type { get; set; }

        /// <summary>
        /// 说明
        /// </summary>
        [MaxLength(256)]
        [DataMember]
        public string Comment { get; set; }

        public virtual ICollection<DocumentInfo> DocumentInfos { get; set; }
    }
}
