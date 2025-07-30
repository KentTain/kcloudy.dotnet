using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.Model.Account.Constants;
using KC.Framework.Base;
using KC.Framework.Tenant;

namespace KC.Model.Account
{
    [Table(Tables.Organization)]
    public class Organization : TreeNode<Organization>
    {
        public Organization()
        {
            BusinessType = BusinessType.None;
            OrganizationType = OrganizationType.Internal;
            OrganizationUsers = new List<UsersInOrganizations>();
        }

        /// <summary>
        /// 组织编号（SequenceName--Organization：ORG2018120100001）
        /// </summary>
        [MaxLength(20)]
        [Display(Name = "组织编号")]
        public string OrganizationCode { get; set; }
        /// <summary>
        /// 组织类型
        /// </summary>
        public OrganizationType OrganizationType { get; set; }
        /// <summary>
        /// 业务类型
        /// </summary>
        [Display(Name = "业务类型")]
        public BusinessType BusinessType { get; set; }

        public WorkflowBusStatus Status { get; set; }

        /// <summary>
        /// 外部编号1
        /// </summary>
        [MaxLength(50)]
        [Display(Name = "外部编号1")]
        public string ReferenceId1 { get; set; }

        /// <summary>
        /// 外部编号2
        /// </summary>
        [MaxLength(50)]
        [Display(Name = "外部编号2")]
        public string ReferenceId2 { get; set; }
        /// <summary>
        /// 外部编号3
        /// </summary>
        [MaxLength(50)]
        [Display(Name = "外部编号3")]
        public string ReferenceId3 { get; set; }

        public ICollection<UsersInOrganizations> OrganizationUsers { get; set; }
        [NotMapped]
        public IEnumerable<User> Users => OrganizationUsers.Select(e => e.User);
    }

    public class OrganizationEquality : EqualityComparer<Organization>
    {
        public override bool Equals(Organization x, Organization y)
        {
            return x.Id == y.Id;
        }

        public override int GetHashCode(Organization obj)
        {
            return obj.GetHashCode();
        }
    }
}
