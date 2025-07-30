using KC.Enums.Contract;
using KC.Framework.Base;
using KC.Model.Contract.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KC.Model.Contract
{
    [Serializable]
    [DataContract(IsReference = true)]
    [Table(Tables.ElectronicOrganization)]
    public class ElectronicOrganization : Entity //企业(个人)用户注册
    {
        [Key]
        public int Id { get; set; }

        [DataMember]
        public string TenantName { get; set; }

        /// <summary>
        /// 机构名称  不可空 length(512)
        /// </summary>
        [MaxLength(512)]
        [Display(Name = "机构名称")]
        public string Name { get; set; }

        [DataMember]
        public bool IsSync { get; set; }

        /// <summary>
        /// 印章状态
        /// </summary>
        public ElectronicSignStatus Status { get; set; }

        /// <summary>
        /// 授权书Id
        /// </summary>
        public string blobId { get; set; }

        /// <summary>
        /// 企业注册类型  可空 企业注册类型，0-组织机构代码号, 1-多证合一，默认组织机构代码号
        /// </summary>
        public RegType RegType { get; set; }

        /// <summary>
        /// 企业相关编号，如：统一社会信用代码，组织机构代码号
        /// </summary>
        [MaxLength(128)]
        [Display(Name = "企业相关编号")]
        public string OrgNumber { get; set; }

        [MaxLength(13)]
        [DataMember]
        public string Mobile { get; set; }

        /// <summary>
        /// 下弦文
        /// </summary>
        [MaxLength(128)]
        public string QText { get; set; }

        /// <summary>
        /// 印章数据：Base64格式
        /// </summary>
        [DataType(DataType.Text)]
        public string Data { get; set; }

        /// <summary>
        /// 外部系统唯一Id
        /// </summary>
        [MaxLength(128)]
        public string OrgId { get; set; }

        /// <summary>
        /// 印章Id
        /// </summary>
        [MaxLength(256)]
        public string SealId { get; set; }

        /// <summary>
        /// 法人名
        /// </summary>
        [MaxLength(128)]
        [DataMember]
        public string OrgLegalName { get; set; }

        /// <summary>
        /// 法人身份证
        /// </summary>
        [MaxLength(56)]
        [DataMember]
        public string OrgLegalIdNumber { get; set; }

        /// <summary>
        /// 印章退回说明
        /// </summary>
        [MaxLength(512)]
        public string Remark { get; set; }

        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public ElectronicPerson ElectronicPerson { get; set; }
    }


}
