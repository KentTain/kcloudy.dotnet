using KC.Enums.Portal;
using KC.Framework.Base;
using KC.Model.Portal.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KC.Model.Portal
{
    [Table(Tables.WebSiteLink)]
    public class WebSiteLink: Entity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [DataMember]
        public int Id { get; set; }
        [MaxLength(128)]
        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public LinkType LinkType { get; set; }
        [MaxLength(512)]
        [DataMember]
        public string Links { get; set; }
        [MaxLength(4000)]
        [DataMember]
        public string Content { get; set; }
        [MaxLength(1000)]
        [DataMember]
        public string MetaKeywords { get; set; }
        [MaxLength(2000)]
        [DataMember]
        public string MetaDescription { get; set; }
        [MaxLength(1000)]
        [DataMember]
        public string MetaTitle { get; set; }

        [DataMember]
        public bool IsEnable { get; set; }

        [DataMember]
        public bool IsNav { get; set; }
    }
}
