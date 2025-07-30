using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using KC.Framework.SecurityHelper;
using KC.Framework.Tenant;
using ProtoBuf;
using KC.Framework.Extension;
using System.ComponentModel.DataAnnotations;
using KC.Framework.Base;
using KC.Common;

namespace KC.Service.DTO.Admin
{
    [Serializable, DataContract(IsReference = true)]
    [ProtoContract]
    public class TenantUserDTO : EntityDTO
    {
        public TenantUserDTO()
        {
            //PrivateEncryptKey = EncryptPasswordUtil.GetRandomString();
            Applications = new List<TenantUserApplicationDTO>();
            OpenApplicationErrorLogs = new List<TenantUserOpenAppErrorLogDTO>();
            TenantSettings = new List<TenantUserSettingDTO>();
            IsEnterprise = true;
        }

        #region 租户基本信息
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
        [DataMember]
        [ProtoMember(4)]
        [MaxLength(2000)]
        public string TenantSignature { get; set; }

        [DataMember]
        [ProtoMember(5)]
        public TenantType TenantType { get; set; }
        [DataMember]
        public string TypeString
        {
            get
            {
                return TenantType.ToDescription();
            }
        }

        [DataMember]
        [ProtoMember(6)]
        public CloudType CloudType { get; set; }
        [DataMember]
        public string CloudTypeString
        {
            get
            {
                return CloudType.ToDescription();
            }
        }

        [DataMember]
        [ProtoMember(7)]
        public TenantVersion Version { get; set; }
        [DataMember]
        public string VersionString
        {
            get
            {
                return Version.ToDescription();
            }
        }

        [DataMember]
        [ProtoMember(8)]
        [MaxLength(1000)]
        public string PrivateEncryptKey { get; set; }

        /// <summary>
        ///  租户Logo
        /// </summary>
        [DataMember]
        [ProtoMember(9)]
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
        [ProtoMember(10)]
        [MaxLength(4000)]
        public string TenantIntroduction { get; set; }

        [DataMember]
        [ProtoMember(11)]
        public string[] Hostnames { get; set; }

        [DataMember]
        [ProtoMember(45)]
        public Dictionary<string, string> Scopes { get; set; }

        #endregion

        #region 数据库
        [DataMember]
        [ProtoMember(12)]
        [MaxLength(1000)]
        public string Server { get; set; }
        [DataMember]
        [ProtoMember(13)]
        [MaxLength(1000)]
        public string Database { get; set; }
        [DataMember]
        [ProtoMember(14)]
        [MaxLength(4000)]
        public string DatabasePasswordHash { get; set; }
        [XmlIgnore]
        //[JsonIgnore]
        public string ConnectionString
        {
            get
            {
                if (string.IsNullOrEmpty(Server) || string.IsNullOrEmpty(Database))
                    return null;

                const string conn = @"Server={0};Database={1};User ID={2};Password={3};MultipleActiveResultSets=true;";
                return string.Format(conn, Server, Database, TenantName, EncryptPasswordUtil.DecryptPassword(DatabasePasswordHash, PrivateEncryptKey));
            }
        }
        #endregion

        #region 存储
        [DataMember]
        [ProtoMember(15)]
        [MaxLength(2000)]
        public string StorageEndpoint { get; set; }
        [DataMember]
        [ProtoMember(16)]
        [MaxLength(1000)]
        public string StorageAccessName { get; set; }
        [DataMember]
        [ProtoMember(17)]
        [MaxLength(4000)]
        public string StorageAccessKeyPasswordHash { get; set; }

        #endregion

        #region 队列
        [DataMember]
        [ProtoMember(18)]
        [MaxLength(2000)]
        public string QueueEndpoint { get; set; }
        [DataMember]
        [ProtoMember(19)]
        [MaxLength(1000)]
        public string QueueAccessName { get; set; }
        [DataMember]
        [ProtoMember(20)]
        [MaxLength(4000)]
        public string QueueAccessKeyPasswordHash { get; set; }
        #endregion

        #region NoSql
        [DataMember]
        [ProtoMember(21)]
        [MaxLength(2000)]
        public string NoSqlEndpoint { get; set; }
        [DataMember]
        [ProtoMember(22)]
        [MaxLength(1000)]
        public string NoSqlAccessName { get; set; }
        [DataMember]
        [ProtoMember(23)]
        [MaxLength(4000)]
        public string NoSqlAccessKeyPasswordHash { get; set; }
        #endregion

        #region ServiceBus
        [DataMember]
        [ProtoMember(24)]
        [MaxLength(2000)]
        public string ServiceBusEndpoint { get; set; }
        [DataMember]
        [ProtoMember(25)]
        [MaxLength(1000)]
        public string ServiceBusAccessName { get; set; }
        [DataMember]
        [ProtoMember(26)]
        [MaxLength(4000)]
        public string ServiceBusAccessKeyPasswordHash { get; set; }
        #endregion

        #region VOD
        /// <summary>
        /// 租户的使用的VOD连接地址
        /// </summary>
        [DataMember]
        [ProtoMember(27)]
        [MaxLength(2000)]
        public string VodEndpoint { get; set; }
        /// <summary>
        /// 租户的使用的VOD连接名称
        /// </summary>
        [DataMember]
        [ProtoMember(28)]
        [MaxLength(1000)]
        public string VodAccessName { get; set; }
        /// <summary>
        /// 租户的使用的VOD连接秘钥
        /// </summary>
        [DataMember]
        [ProtoMember(29)]
        [MaxLength(4000)]
        public string VodAccessKeyPasswordHash { get; set; }

        #endregion

        #region 数据库服务器配置
        [DataMember]
        [ProtoMember(30)]
        public int DatabasePoolId { get; set; }
        [DataMember]
        [ProtoMember(31)]
        public DatabasePoolDTO OwnDatabase { get; set; }
        #endregion

        #region 存储服务器配置
        [DataMember]
        [ProtoMember(32)]
        public int? StoragePoolId { get; set; }
        [DataMember]
        [ProtoMember(33)]
        public StoragePoolDTO OwnStorage { get; set; }
        #endregion

        #region 队列服务器配置
        [DataMember]
        [ProtoMember(34)]
        public int? QueuePoolId { get; set; }
        [DataMember]
        [ProtoMember(35)]
        public QueuePoolDTO OwnQueue { get; set; }
        #endregion

        #region NoSql服务器配置
        [DataMember]
        [ProtoMember(36)]
        public int? NoSqlPoolId { get; set; }
        [DataMember]
        [ProtoMember(37)]
        public NoSqlPoolDTO OwnNoSql { get; set; }
        #endregion

        #region ServiceBus服务器配置
        [DataMember]
        [ProtoMember(38)]
        public int? ServiceBusPoolId { get; set; }
        [DataMember]
        [ProtoMember(39)]
        public ServiceBusPoolDTO ServiceBusPool { get; set; }
        #endregion

        #region Vod服务器配置
        [DataMember]
        [ProtoMember(40)]
        public int? VodPoolId { get; set; }
        [DataMember]
        [ProtoMember(41)]
        public VodPoolDTO VodPool { get; set; }
        #endregion

        #region 联系人信息
        [DataMember]
        [ProtoMember(42)]
        [MaxLength(50)]
        public string ContactName { get; set; }
        [DataMember]
        [ProtoMember(43)]
        [MaxLength(200)]
        public string ContactEmail { get; set; }
        [DataMember]
        [ProtoMember(44)]
        [MaxLength(20)]
        public string ContactPhone { get; set; }

        /// <summary>
        /// 企业经营模式
        /// </summary>
        [DataMember]
        [ProtoMember(46)]
        public BusinessModel BusinessModel { get; set; }

        [MaxLength(128)]
        [DataMember]
        [ProtoMember(47)]
        public string IndustryId { get; set; }

        [MaxLength(1024)]
        [DataMember]
        [ProtoMember(48)]
        public string IndustryName { get; set; }
        #endregion

        #region 代码仓库
        /// <summary>
        /// 租户的使用的代码仓库连接地址
        /// </summary>
        [DataMember]
        [ProtoMember(49)]
        [MaxLength(2000)]
        public string CodeEndpoint { get; set; }
        /// <summary>
        /// 租户的使用的代码仓库连接名称
        /// </summary>
        [DataMember]
        [ProtoMember(50)]
        [MaxLength(1000)]
        public string CodeAccessName { get; set; }
        /// <summary>
        /// 租户的使用的代码仓库连接秘钥
        /// </summary>
        [DataMember]
        [ProtoMember(51)]
        [MaxLength(4000)]
        public string CodeAccessKeyPasswordHash { get; set; }

        #endregion

        #region 代码仓库服务器配置
        [DataMember]
        [ProtoMember(52)]
        public int? CodePoolId { get; set; }
        [DataMember]
        [ProtoMember(53)]
        public CodeRepositoryPoolDTO CodePool { get; set; }
        #endregion

        #region 租户域名设置（别名）
        [DataMember]
        [ProtoMember(54)]
        [MaxLength(1000)]
        public string OwnDomainName { get; set; }

        [DataMember]
        [ProtoMember(55)]
        [MaxLength(50)]
        public string NickName { get; set; }
        [DataMember]
        [ProtoMember(56)]
        public DateTime? NickNameLastModifyDate { get; set; }

        #endregion

        //[MaxLength(128)]
        [DataMember]
        [ProtoMember(57)]
        public string ReferenceId { get; set; }

        [DataMember]
        [ProtoMember(58)]
        public DateTime? PasswordExpiredTime { get; set; }

        [DataMember]
        [ProtoMember(59)]
        public bool CanEdit { get; set; }

        [DataMember]
        [ProtoMember(60)]
        public bool IsEditMode { get; set; }

        [DataMember]
        [ProtoMember(61)]
        public bool IsEnterprise { get; set; }

        [DataMember]
        [ProtoMember(62)]
        public DatabaseType DatabaseType { get; set; }
        [DataMember]
        [ProtoMember(63)]
        public StorageType StorageType { get; set; }
        [DataMember]
        [ProtoMember(64)]
        public QueueType QueueType { get; set; }
        [DataMember]
        [ProtoMember(65)]
        public NoSqlType NoSqlType { get; set; }
        [DataMember]
        [ProtoMember(66)]
        public ServiceBusType ServiceBusType { get; set; }
        [DataMember]
        [ProtoMember(67)]
        public VodType VodType { get; set; }
        [DataMember]
        [ProtoMember(68)]
        public virtual List<TenantUserApplicationDTO> Applications { get; set; }

        [DataMember]
        [ProtoMember(69)]
        public virtual List<TenantUserSettingDTO> TenantSettings { get; set; }

        [DataMember]
        public virtual IList<TenantUserOpenAppErrorLogDTO> OpenApplicationErrorLogs { get; set; }

    }
}
