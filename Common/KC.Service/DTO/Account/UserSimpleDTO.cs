using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using KC.Service.DTO;
using ProtoBuf;
using KC.Framework.Base;

namespace KC.Service.DTO.Account
{
    [Serializable, DataContract(IsReference = true)]
    [ProtoContract]
    public class UserSimpleDTO : EntityBaseDTO
    {
        public UserSimpleDTO()
        {
            UserOrgIds = new List<int>();
            UserRoleIds = new List<string>();
            UserOrgCodes = new List<string>();
            UserOrgTreeCodes = new List<string>();
            UserSettings = new List<UserSettingDTO>();
        }

        /// <summary>
        /// 用户类型
        /// </summary>
        [Display(Name = "用户类型")]
        [DataMember]
        [ProtoMember(10)]
        public UserType UserType { get; set; }

        [DataMember]
        [ProtoMember(1)]
        public PositionLevel PositionLevel { get; set; }

        [DataMember]
        [ProtoMember(2)]
        public string PositionLevelName { get; set; }

        /// <summary>
        /// 会员Guid
        /// </summary>
        [DataMember]
        [ProtoMember(4)]
        [MaxLength(128)]
        public string UserId { get; set; }
        /// <summary>
        /// 会员登陆名
        /// </summary>
        [DataMember]
        [ProtoMember(5)]
        [MaxLength(256)]
        public string UserName { get; set; }
        /// <summary>
        /// 会员编号
        /// </summary>
        [DataMember]
        [ProtoMember(6)]
        [MaxLength(20)]
        public string MemberId { get; set; }
        /// <summary>
        /// 会员显示名
        /// </summary>
        [DataMember]
        [ProtoMember(7)]
        [MaxLength(512)]
        public string DisplayName { get; set; }
        /// <summary>
        /// 会员Email
        /// </summary>
        [DataMember]
        [ProtoMember(8)]
        [MaxLength(128)]
        public string Email { get; set; }
        /// <summary>
        /// 会员手机号
        /// </summary>
        [DataMember]
        [ProtoMember(9)]
        [MaxLength(512)]
        public string PhoneNumber { get; set; }
        /// <summary>
        /// 是否锁定
        /// </summary>
        [DataMember]
        [ProtoMember(15)]
        public bool LockoutEnabled { get; set; }
        /// <summary>
        /// 锁定日期
        /// </summary>
        [DataMember]
        [ProtoMember(16)]
        public DateTime? LockoutEndDateUtc { get; set; }
        /// <summary>
        /// 登陆错误次数
        /// </summary>
        [DataMember]
        [ProtoMember(18)]
        public int AccessFailedCount { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        [DataMember]
        [ProtoMember(19)]
        public DateTime CreateDate { get; set; }
        [DataMember]
        [ProtoMember(20)]
        [MaxLength(20)]
        public string ContactQQ { get; set; }
        [DataMember]
        [ProtoMember(21)]
        [MaxLength(20)]
        public string Telephone { get; set; }
        /// <summary>
        /// 微信公众号
        /// </summary>
        [MaxLength(128)]
        [DataMember]
        [ProtoMember(23)]
        public string OpenId { get; set; }
        /// <summary>
        /// 是否为客户
        /// </summary>
        [Display(Name = "是否为客户")]
        [ProtoMember(26)]
        public bool IsClient { get; set; }
        /// <summary>
        /// 外部编号1
        /// </summary>
        [MaxLength(128)]
        [DataMember]
        [ProtoMember(27)]
        [Display(Name = "外部编号1")]
        public string ReferenceId1 { get; set; }

        /// <summary>
        /// 外部编号2
        /// </summary>
        [MaxLength(128)]
        [DataMember]
        [ProtoMember(28)]
        [Display(Name = "外部编号2")]
        public string ReferenceId2 { get; set; }
        /// <summary>
        /// 外部编号3
        /// </summary>
        [MaxLength(128)]
        [DataMember]
        [ProtoMember(29)]
        [Display(Name = "外部编号3")]
        public string ReferenceId3 { get; set; }

        [DataMember]
        [ProtoMember(30)]
        public Framework.Base.WorkflowBusStatus Status { get; set; }

        [DataMember]
        [ProtoMember(31)]
        public string TenantName { get; set; }

        [DataMember]
        [ProtoMember(32)]
        public bool IsModifyPassword { get; set; }
        
        [DataMember]
        [ProtoMember(34)]
        public List<string> UserRoleIds { get; set; }

        [DataMember]
        [ProtoMember(35)]
        public List<string> UserRoleNames { get; set; }

        [DataMember]
        [ProtoMember(36)]
        public List<int> UserOrgIds { get; set; }

        [DataMember]
        [ProtoMember(37)]
        public List<string> UserOrgCodes { get; set; }

        [DataMember]
        [ProtoMember(38)]
        public List<string> UserOrgNames { get; set; }
        [DataMember]
        [ProtoMember(39)]
        public List<string> UserOrgTreeCodes { get; set; }
        [DataMember]
        [ProtoMember(40)]
        public List<UserSettingDTO> UserSettings { get; set; }
    }

}
