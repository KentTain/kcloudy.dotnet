using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using KC.Enums.Portal;
using KC.Framework.Base;
using KC.Model.Portal.Constants;

namespace KC.Model.Portal
{
    [Table(Tables.FavoriteInfo)]
    public class FavoriteInfo : Entity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [DataMember]
        public int Id { get; set; }

        [DisplayName("关注类型")]
        [DataMember]
        public FavoriteType FavoriteType { get; set; }

        [DisplayName("被关注的Id,产品Id,询价单Id或店铺TenantName")]
        [MaxLength(128)]
        [DataMember]
        public string RelationId { get; set; }

        [DisplayName("被关注的名称,产品名称,询价单标题或店铺名称等等")]
        [MaxLength(512)]
        [DataMember]
        public string RelationName { get; set; }

        [DisplayName("被关注的图片")]
        [MaxLength(512)]
        [DataMember]
        public string RelationImg { get; set; }


        [DisplayName("被关注的公司的TenantName")]
        [MaxLength(128)]
        [DataMember]
        public string CompanyCode { get; set; }

        [DisplayName("被关注的公司的TenantDisplayName")]
        [MaxLength(200)]
        [DataMember]
        public string CompanyName { get; set; }

        [MaxLength(200)]
        [DataMember]
        public string CompanyDomain { get; set; }

        [DisplayName("关注者的UserId")]
        [MaxLength(128)]
        [DataMember]
        public string ConcernedUserId { get; set; }

        [DisplayName("关注者的UserDisplayName")]
        [MaxLength(128)]
        [DataMember]
        public string ConcernedUserName { get; set; }

    }
}
