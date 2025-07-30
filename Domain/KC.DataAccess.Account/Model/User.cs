using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.Framework.Base;
using KC.Model.Account.Constants;
using Microsoft.AspNetCore.Identity;

namespace KC.Model.Account
{
    [Table(Tables.User)]
    public class User : IdentityUser
    {
        public User()
        {
            UserOrganizations = new List<UsersInOrganizations>();
            UserRoles = new List<UsersInRoles>();
            IsDefaultMobile = true;
            Status = Framework.Base.WorkflowBusStatus.Approved;
        }
        /// <summary>
        /// 用户类型
        /// </summary>
        [Display(Name = "用户类型")]
        public UserType UserType { get; set; }

        /// <summary>
        /// 岗位类型
        /// </summary>
        [Display(Name = "岗位类型")]
        public PositionLevel PositionLevel { get; set; }
        /// <summary>
        /// 用户系统编号（SequenceName--Member：USR2018120100001）
        /// </summary>
        [MaxLength(20)]
        [Display(Name = "用户系统编号")]
        public string MemberId { get; set; }
        /// <summary>
        /// 用户显示名
        /// </summary>
        [MaxLength(512)]
        [Display(Name = "用户显示名")]
        public string DisplayName { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [Display(Name = "创建时间")]
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// 推荐人编号
        /// </summary>
        [MaxLength(20)]
        [Display(Name = "推荐人编号")]
        public string Recommended { get; set; }
        /// <summary>
        /// 邮箱验证过期时间
        /// </summary>
        [Display(Name = "邮箱验证过期时间")]
        public DateTime? EmailConfirmedExpired { get; set; }
        /// <summary>
        /// 邮箱验证时间
        /// </summary>
        [Display(Name = "邮箱验证时间")]
        public DateTime? EmailConfirmedDate { get; set; }
        /// <summary>
        /// 用户QQ号
        /// </summary>
        [MaxLength(20)]
        [Display(Name = "用户QQ号")]
        public string ContactQQ { get; set; }
        /// <summary>
        /// 用户手机号
        /// </summary>
        [MaxLength(20)]
        [Display(Name = "用户手机号")]
        public string Telephone { get; set; }
        /// <summary>
        /// 是否为默认手机
        /// </summary>
        [Display(Name = "是否为默认手机")]
        public bool IsDefaultMobile { get; set; }
        /// <summary>
        /// 用户微信号
        /// </summary>
        [MaxLength(128)]
        [Display(Name = "用户微信号")]
        public string OpenId { get; set; }
        
        /// <summary>
        /// 用户状态
        /// </summary>
        [Display(Name = "用户状态")]
        public Framework.Base.WorkflowBusStatus Status { get; set; }

        /// <summary>
        /// 第一次登陆是否修改密码
        /// </summary>
        [Display(Name = "第一次登陆是否修改密码")]
        public bool IsModifyPassword { get; set; }

        /// <summary>
        /// 是否为客户
        /// </summary>
        [Display(Name = "是否为客户")]
        public bool IsClient { get; set; }

        /// <summary>
        /// 外部编号1
        /// </summary>
        [MaxLength(128)]
        [Display(Name = "外部编号1")]
        public string ReferenceId1 { get; set; }

        /// <summary>
        /// 外部编号2
        /// </summary>
        [MaxLength(128)]
        [Display(Name = "外部编号2")]
        public string ReferenceId2 { get; set; }
        /// <summary>
        /// 外部编号3
        /// </summary>
        [MaxLength(128)]
        [Display(Name = "外部编号3")]
        public string ReferenceId3 { get; set; }

        public ICollection<UsersInOrganizations> UserOrganizations { get; set; }
        [NotMapped]
        public IEnumerable<Organization> Organizations => UserOrganizations.Select(e => e.Organization);
        
        public ICollection<UsersInRoles> UserRoles { get; set; }
        [NotMapped]
        public IEnumerable<Role> Roles => UserRoles.Select(e => e.Role);

        [NotMapped]
        public ICollection<System.Security.Claims.Claim> Claims
        {
            get
            {
                var result = new List<System.Security.Claims.Claim>();
                foreach(var role in Roles)
                {
                    foreach (var permission in role.Permissions)
                    {
                        if (permission.ParentNode != null)
                        {
                            result.Add(new System.Security.Claims.Claim
                                (permission.ParentNode.Name, permission.Name));
                        }
                    }
                }

                return result;
            }
        }

        public ICollection<UserSetting> UserSettings { get; set; }

    }
}
