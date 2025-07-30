using System.Runtime.Serialization;
using KC.Enums.Portal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.Framework.Extension;
using KC.Service.DTO;
using ProtoBuf;

namespace KC.Service.DTO.Portal
{
    [Serializable, DataContract(IsReference = true)]
    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public class WebSiteLinkDTO : EntityDTO
    {
        [DataMember]
        public bool IsEditMode { get; set; }
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public string Content { get; set; }
        [DataMember]
        public string Links { get; set; }
        [DataMember]
        public LinkType LinkType { get; set; }
        [DataMember]
        public string LinkTypeString
        {
            get;
            set;
        }
        [DataMember]
        public string MetaKeywords { get; set; }
        [DataMember]
        public string MetaDescription { get; set; }
        [DataMember]
        public string MetaTitle { get; set; }
        [DataMember]
        public bool IsEnable { get; set; }
        [DataMember]
        public string IsEnableString { get; set; }
        [DataMember]
        public bool IsNav { get; set; }
        [DataMember]
        public string IsNavString { get; set; }
    }
}
