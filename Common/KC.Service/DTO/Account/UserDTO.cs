using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using KC.Service.DTO;
using KC.Framework.Base;
using ProtoBuf;

namespace KC.Service.DTO.Account
{
    [Serializable, DataContract(IsReference = true)]
    [ProtoContract]
    public  class UserDTO : EntityBaseDTO
    {
        public UserDTO()
        {
            OrganizationIds = new List<int>();
            Organizations = new List<OrganizationDTO>();
            RoleIds = new List<string>();
            RoleNames = new List<string>();
            UserSettings = new List<UserSettingDTO>();
            EmailConfirmed = false;
            LockoutEnabled = false;
            TwoFactorEnabled = false;
            IsDefaultMobile = true;
            Status = Framework.Base.WorkflowBusStatus.Approved;
        }

        [DataMember]
        public bool IsEditMode { get; set; }

        /// <summary>
        /// 用户类型
        /// </summary>
        [Display(Name = "用户类型")]
        [DataMember]
        [ProtoMember(1)]
        public UserType UserType { get; set; }

        [DataMember]
        [ProtoMember(2)]
        public PositionLevel PositionLevel { get; set; }

        [DataMember]
        [ProtoMember(3)]
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
        [Required(ErrorMessage = "用户名不能为空！")]
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
        [Required(ErrorMessage = "姓名不能为空！")]
        public string DisplayName { get; set; }
        /// <summary>
        /// 会员Email
        /// </summary>
        [DataMember]
        [ProtoMember(8)]
        [MaxLength(128)]
        [Required(ErrorMessage = "邮箱不能为空！")]
        public string Email { get; set; }
        /// <summary>
        /// 邮箱是否验证
        /// </summary>
        [DataMember]
        [ProtoMember(9)]
        public bool EmailConfirmed { get; set; }
        /// <summary>
        /// 邮箱验证过期时间
        /// </summary>
        [DataMember]
        [ProtoMember(10)]
        public DateTime? EmailConfirmedExpired { get; set; }
        /// <summary>
        /// 邮箱验证时间
        /// </summary>
        [DataMember]
        [ProtoMember(11)]
        public DateTime? EmailConfirmedDate { get; set; }
        /// <summary>
        /// 会员手机号
        /// </summary>
        [DataMember]
        [ProtoMember(12)]
        [MaxLength(512)]
        [Required(ErrorMessage = "手机不能为空！")]
        public string PhoneNumber { get; set; }
        /// <summary>
        /// 手机号是否验证
        /// </summary>
        [DataMember]
        [ProtoMember(13)]
        public bool PhoneNumberConfirmed { get; set; }
        [DataMember]
        [ProtoMember(14)]
        public string Password { get; set; }
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
        [DataMember]
        [ProtoMember(17)]
        [MaxLength(512)]
        public string SecurityStamp { get; set; }
        [DataMember]
        [ProtoMember(18)]
        public bool TwoFactorEnabled { get; set; }
        /// <summary>
        /// 登陆错误次数
        /// </summary>
        [DataMember]
        [ProtoMember(19)]
        public int AccessFailedCount { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        [DataMember]
        [ProtoMember(20)]
        public DateTime CreateDate { get; set; }
        [DataMember]
        [ProtoMember(21)]
        [MaxLength(20)]
        public string ContactQQ { get; set; }
        [DataMember]
        [ProtoMember(22)]
        [MaxLength(20)]
        public string Telephone { get; set; }
        [DataMember]
        [ProtoMember(23)]
        public bool IsDefaultMobile { get; set; }
        /// <summary>
        /// 微信公众号
        /// </summary>
        [MaxLength(128)]
        [DataMember]
        [ProtoMember(24)]
        public string OpenId { get; set; }

        [DataMember]
        [ProtoMember(25)]
        public bool IsSystemAdmin { get; set; }

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
        public WorkflowBusStatus Status { get; set; }

        [DataMember]
        [ProtoMember(31)]
        public string TenantName { get; set; }

        [DataMember]
        [ProtoMember(32)]
        public bool IsModifyPassword { get; set; }

        [DataMember]
        [ProtoMember(33)]
        public string TenantNickName { get; set; }


        [DataMember]
        [ProtoMember(34)]
        public List<string> RoleIds { get; set; }

        [DataMember]
        [ProtoMember(35)]
        public List<string> RoleNames { get; set; }

        [DataMember]
        [ProtoMember(36)]
        public List<int> OrganizationIds { get; set; }

        [DataMember]
        [ProtoMember(38)]
        public List<OrganizationDTO> Organizations { get; set; }

        [DataMember]
        [ProtoMember(39)]
        public List<UserSettingDTO> UserSettings { get; set; }
    }

}
