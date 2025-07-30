using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using KC.Enums.Message;
using KC.Framework.Base;
using KC.Model.Message.Constants;

namespace KC.Model.Message
{
    [Table(Tables.NewsBulletinCategory)]
    public class NewsBulletinCategory : TreeNode<NewsBulletinCategory>
    {
        public NewsBulletinCategory()
        {
            NewsBulletins = new List<NewsBulletin>();
        }

        /// <summary>
        /// 文章类型
        /// </summary>
        [DataMember]
        [Required]
        public NewsBulletinType Type { get; set; }

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

        public virtual ICollection<NewsBulletin> NewsBulletins { get; set; }
    }
}
