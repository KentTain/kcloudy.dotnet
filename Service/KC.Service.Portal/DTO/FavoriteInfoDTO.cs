using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using KC.Framework.Extension;
using KC.Enums.Portal;
using KC.Service.DTO;

namespace KC.Service.DTO.Portal
{
    [Serializable, DataContract(IsReference = true)]
    public class FavoriteInfoDTO : EntityDTO
    {
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// 被关注类型
        /// </summary>
        [DataMember]
        public FavoriteType FavoriteType { get; set; }
        [DataMember]
        public string FavoriteTypeStr { get { return FavoriteType.ToDescription(); } }

        /// <summary>
        /// 被关注Id
        /// </summary>
        [MaxLength(128)]
        [DataMember]
        public string RelationId { get; set; }

        /// <summary>
        ///被关注名称
        /// </summary>
        [MaxLength(512)]
        [DataMember]
        public string RelationName { get; set; }

        /// <summary>
        /// 被关注图片
        /// </summary>
        [MaxLength(512)]
        [DataMember]
        public string RelationImg { get; set; }

        /// <summary>
        /// 被关注商铺的所属TenantName
        /// </summary>
        [MaxLength(128)]
        [DataMember]
        public string CompanyCode { get; set; }

        /// <summary>
        /// 被关注商铺的所属企业名称
        /// </summary>
        [MaxLength(200)]
        [DataMember]
        public string CompanyName { get; set; }
        /// <summary>
        /// 被关注商铺的所属域名
        /// </summary>
        [MaxLength(200)]
        [DataMember]
        public string CompanyDomain { get; set; }

        /// <summary>
        /// 关注者id
        /// </summary>
        [MaxLength(128)]
        [DataMember]
        public string ConcernedUserId { get; set; }

        /// <summary>
        /// 关注者名称
        /// </summary>
        [MaxLength(128)]
        [DataMember]
        public string ConcernedUserName { get; set; }
        public string CreatedDateStr
        {
            get { return CreatedDate.ToLocalDateTimeStr("yyyy-MM-dd HH:mm"); }
        }
    }

    public class CurrentFavoriteDTO : EntityBaseDTO
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public int StoreCount { get; set; }

        public int RelationCount { get; set; }
    }
}
