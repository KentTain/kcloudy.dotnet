using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using KC.Framework.Base;
using KC.Framework.Extension;
using KC.Service.Enums.Message;

namespace KC.Service.DTO.Message
{
    [Serializable, DataContract(IsReference = true)]
    public class NewsBulletinCategoryDTO : TreeNodeDTO<NewsBulletinCategoryDTO>
    {
        public NewsBulletinCategoryDTO()
        {
            NewsBulletins = new List<NewsBulletinDTO>();
        }

        [DataMember]
        public bool IsEditMode { get; set; }
        /// <summary>
        /// 文章类型
        /// </summary>
        [DataMember]
        [Required]
        public NewsBulletinType Type { get; set; }

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
        /// description
        /// </summary>
        [DataMember]
        [MaxLength(512)]
        public string Description { get; set; }

        /// <summary>
        /// 是否显示在导航栏
        /// </summary>
        [DataMember]
        [Required]
        public bool IsShow { get; set; }

        [DataMember]
        public List<NewsBulletinDTO> NewsBulletins { get; set; }
    }
}
