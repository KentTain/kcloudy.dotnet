using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.Framework.Base;
using System.Runtime.Serialization;
using KC.Enums.Doc;
using KC.Service.DTO;
using KC.Framework.Extension;

namespace KC.Service.DTO.Doc
{
    [Serializable]
    [DataContract(IsReference = true)]
    public class DocCategoryDTO : TreeNodeDTO<DocCategoryDTO>
    {
        public DocCategoryDTO()
        {
            DocumentInfos = new List<DocumentInfoDTO>();
        }

        [DataMember]
        public bool IsEditMode { get; set; }

        /// <summary>
        /// 0: 公司 1：部门  2：个人  3：外部企业   4：其他
        /// </summary>
        [DataMember]
        public LableType Type { get; set; }

        private string _typeString;
        [DataMember]
        public string TypeString
        {
            get
            {
                if (_typeString != null)
                    return _typeString;
                return Type.ToDescription();
            }
            set
            {
                _typeString = value;
            }
        }
        /// <summary>
        /// 说明
        /// </summary>
        [MaxLength(256)]
        [DataMember]
        public string Comment { get; set; }

        public ICollection<DocumentInfoDTO> DocumentInfos { get; set; }
    }
}
