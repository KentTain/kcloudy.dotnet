using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using KC.Common;
using KC.Framework.Base;
using KC.Framework.Extension;
using KC.Framework.Tenant;
using ProtoBuf;

namespace KC.Service.DTO.Admin
{
    [Serializable, DataContract(IsReference = true)]
    [ProtoContract]
    public class TenantSimpleDTO : EntityBaseDTO
    {
        [DataMember]
        [ProtoMember(1)]
        public int TenantId { get; set; }

        /// <summary>
        ///  Client's MemberId
        /// </summary>
        [DataMember]
        [ProtoMember(2)]
        [MaxLength(20)]
        public string TenantName { get; set; }
        [DataMember]
        [ProtoMember(3)]
        [MaxLength(512)]
        public string TenantDisplayName { get; set; }

        /// <summary>
        ///  租户Logo
        /// </summary>
        [DataMember]
        [ProtoMember(8)]
        [MaxLength(2000)]
        public string TenantLogo { get; set; }

        /// <summary>
        /// 租户Logo对象
        /// </summary>

        [DataMember]
        public BlobInfoDTO TenantLogoBlob
        {
            get
            {
                if (TenantLogo.IsNullOrEmpty())
                    return null;

                return SerializeHelper.FromJson<BlobInfoDTO>(TenantLogo);
            }
        }

        /// <summary>
        ///  租户简介
        /// </summary>
        [DataMember]
        [ProtoMember(9)]
        [MaxLength(4000)]
        public string TenantIntroduction { get; set; }

        /// <summary>
        /// 企业经营模式
        /// </summary>
        [DataMember]
        [ProtoMember(41)]
        public BusinessModel BusinessModel { get; set; }

        [MaxLength(128)]
        [DataMember]
        [ProtoMember(42)]
        public string IndustryId { get; set; }

        [MaxLength(1024)]
        [DataMember]
        [ProtoMember(43)]
        public string IndustryName { get; set; }

        [DataMember]
        [ProtoMember(5)]
        [MaxLength(50)]
        public string ContactName { get; set; }
        [DataMember]
        [ProtoMember(6)]
        [MaxLength(200)]
        public string ContactEmail { get; set; }
        [DataMember]
        [ProtoMember(7)]
        [MaxLength(20)]
        public string ContactPhone { get; set; }

        [DataMember]
        [ProtoMember(8)]
        public TenantType TenantType { get; set; }

        [DataMember]
        [ProtoMember(44)]
        public CloudType CloudType { get; set; }

        [DataMember]
        [ProtoMember(48)]
        public TenantVersion Version { get; set; }
        [DataMember]
        //[ProtoMember(62)]
        public string TenantTypeString { get { return TenantType.ToDescription(); } }
        [DataMember]
        //[ProtoMember(49)]
        public string VersionString { get { return Version.ToDescription(); } }

        [DataMember]
        //[ProtoMember(50)]
        public string CloudTypeString { get { return CloudType.ToDescription(); } }

        [DataMember]
        [ProtoMember(58)]
        [MaxLength(1000)]
        public string OwnDomainName { get; set; }

        [DataMember]
        [ProtoMember(60)]
        [MaxLength(50)]
        public string NickName { get; set; }
        [DataMember]
        [ProtoMember(61)]
        public DateTime? NickNameLastModifyDate { get; set; }

        [DataMember]
        [ProtoMember(63)]
        public bool IsEnterprise { get; set; }

        [DataMember]
        [ProtoMember(64)]
        public DatabaseType DatabaseType { get; set; }
        [DataMember]
        [ProtoMember(65)]
        public StorageType StorageType { get; set; }
        [DataMember]
        [ProtoMember(66)]
        public QueueType QueueType { get; set; }
        [DataMember]
        [ProtoMember(67)]
        public NoSqlType NoSqlType { get; set; }
        [DataMember]
        [ProtoMember(68)]
        public ServiceBusType ServiceBusType { get; set; }
        [DataMember]
        [ProtoMember(69)]
        public VodType VodType { get; set; }
    }
}
