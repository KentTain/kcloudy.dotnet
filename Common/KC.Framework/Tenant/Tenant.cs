using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using KC.Framework.SecurityHelper;
using KC.Framework.Util;
using KC.Framework.Base;
using ProtoBuf;
using System.ComponentModel.DataAnnotations;

namespace KC.Framework.Tenant
{
    [Serializable, DataContract(IsReference = true), ProtoContract]
    [ProtoInclude(101, typeof(Tenant))]
    public class TenantApiAccessInfo : Entity
    {
        public TenantApiAccessInfo()
        {
            CloudType = CloudType.Azure;
            TenantType = TenantType.Enterprise;
            Version = TenantVersion.Standard;
            Scopes = new Dictionary<string, string>();
        }
        /// <summary>
        /// 租户Id
        /// </summary>
        //[Key]
        [DataMember]
        [ProtoMember(1)]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)] //数据库生成默认值
        public int TenantId { get; set; }

        /// <summary>
        ///  租户Code：Client's MemberId（如：USR2014073100001）
        /// </summary>
        [DataMember]
        [ProtoMember(2)]
        [MaxLength(20)]
        public string TenantName { get; set; }
        /// <summary>
        /// 租户名称
        /// </summary>
        [DataMember]
        [ProtoMember(3)]
        [MaxLength(512)]
        public string TenantDisplayName { get; set; }
        /// <summary>
        /// 租户的共用（访问WebApi时使用）
        /// </summary>
        [DataMember]
        [ProtoMember(4)]
        [MaxLength(2000)]
        public string TenantSignature { get; set; }

        /// <summary>
        /// 租户类型：KC.Framework.Tenant.TenantType
        /// </summary>
        [DataMember]
        [ProtoMember(5)]
        public TenantType TenantType { get; set; }

        /// <summary>
        /// 租户版本：KC.Framework.Tenant.TenantVersion
        /// </summary>
        [DataMember]
        [ProtoMember(6)]
        public TenantVersion Version { get; set; }

        /// <summary>
        /// 云类型：KC.Framework.Tenant.CloudType
        /// </summary>
        [DataMember]
        [ProtoMember(7)]
        public CloudType CloudType { get; set; }

        /// <summary>
        ///  租户Logo
        /// </summary>
        [DataMember]
        [ProtoMember(8)]
        [MaxLength(2000)]
        public string TenantLogo { get; set; }

        /// <summary>
        ///  租户简介
        /// </summary>
        [DataMember]
        [ProtoMember(9)]
        [MaxLength(4000)]
        public string TenantIntroduction { get; set; }

        /// <summary>
        /// 租户私钥，用于加密租户敏感数据用
        /// </summary>
        [DataMember]
        [ProtoMember(11)]
        [MaxLength(1000)]
        public string PrivateEncryptKey { get; set; }

        /// <summary>
        /// 租户别名最后的修改时间
        /// </summary>
        [DataMember]
        [ProtoMember(12)]
        [NotMapped]
        public string[] Hostnames { get; set; }

        [DataMember]
        [ProtoMember(13)]
        [NotMapped]
        public Dictionary<string, string> Scopes { get; set; }

        public string GenerateSignature()
        {
            var s = new ServiceRequestToken(TenantId, TenantName, PrivateEncryptKey);
            return s.GetEncrptSignature();
        }

        public ServiceRequestToken GetServiceToken()
        {
            return new ServiceRequestToken(TenantSignature, PrivateEncryptKey);
        }
    }

    [Serializable, DataContract(IsReference = true), ProtoContract]
    public class Tenant : TenantApiAccessInfo
    {
        public Tenant()
        {
            //PrivateEncryptKey = EncryptPasswordUtil._key;
            //TenantSettings = new List<TenantSetting>();
        }

        #region 数据库
        [DataMember]
        [ProtoMember(1)]
        public DatabaseType DatabaseType { get; set; }
        /// <summary>
        /// 租户的使用的数据库服务器名称
        /// </summary>
        [DataMember]
        [ProtoMember(12)]
        [MaxLength(1000)]
        public string Server { get; set; }
        /// <summary>
        /// 租户的使用的数据库实例名称
        /// </summary>
        [DataMember]
        [ProtoMember(13)]
        [MaxLength(1000)]
        public string Database { get; set; }
        /// <summary>
        /// 租户的使用的数据库服务器密码
        /// </summary>
        [DataMember]
        [ProtoMember(14)]
        [MaxLength(4000)]
        public string DatabasePasswordHash { get; set; }
        /// <summary>
        /// 租户使用的数据库连接字符串
        /// </summary>
        [XmlIgnore]
        [NotMapped]
        //[ScriptIgnore]
        public string ConnectionString
        {
            get
            {
                if (string.IsNullOrEmpty(Server) || string.IsNullOrEmpty(Database))
                    return null;

                try
                {
                    switch (DatabaseType)
                    {
                        case DatabaseType.MySql:
                            var mysqlConn = @"Server={0};Database={1};User ID={2};Password={3};Port=3306;sslMode=None;";
                            return string.Format(mysqlConn, Server, Database, TenantName,
                                EncryptPasswordUtil.DecryptPassword(DatabasePasswordHash, PrivateEncryptKey));
                        case DatabaseType.SqlServer:
                            var sqlConn = @"Server={0};Database={1};User ID={2};Password={3};MultipleActiveResultSets=true;";
                            return string.Format(sqlConn, Server, Database, TenantName,
                                EncryptPasswordUtil.DecryptPassword(DatabasePasswordHash, PrivateEncryptKey));
                    }

                    return null;
                }
                catch (Exception ex)
                {
                    LogUtil.LogException(ex);
                    return string.Empty;
                }
            }
        }
        #endregion

        #region 存储
        [DataMember]
        [ProtoMember(2)]
        public StorageType StorageType { get; set; }
        /// <summary>
        /// 租户的使用的存储连接地址
        /// </summary>
        [DataMember]
        [ProtoMember(15)]
        [MaxLength(2000)]
        public string StorageEndpoint { get; set; }
        /// <summary>
        /// 租户的使用的存储连接名称
        /// </summary>
        [DataMember]
        [ProtoMember(16)]
        [MaxLength(1000)]
        public string StorageAccessName { get; set; }
        /// <summary>
        /// 租户的使用的存储连接秘钥
        /// </summary>
        [DataMember]
        [ProtoMember(17)]
        [MaxLength(4000)]
        public string StorageAccessKeyPasswordHash { get; set; }

        public string GetDecryptStorageConnectionString()
        {
            if (string.IsNullOrEmpty(StorageEndpoint)
                || string.IsNullOrEmpty(StorageAccessName)
                || string.IsNullOrEmpty(StorageAccessKeyPasswordHash))
                return KC.Framework.Base.GlobalConfig.GetDecryptStorageConnectionString();

            var azureConn = @"DefaultEndpointsProtocol=https;BlobEndpoint={0};AccountName={1};AccountKey={2}";

            try
            {
                return string.Format(azureConn, StorageEndpoint, StorageAccessName, EncryptPasswordUtil.DecryptPassword(StorageAccessKeyPasswordHash, PrivateEncryptKey));
            }
            catch (Exception ex)
            {
                LogUtil.LogException(ex);
                return string.Empty;
            }
        }
        #endregion

        #region 队列
        [DataMember]
        [ProtoMember(3)]
        public QueueType QueueType { get; set; }
        /// <summary>
        /// 租户的使用的队列连接地址
        /// </summary>
        [DataMember]
        [ProtoMember(18)]
        [MaxLength(2000)]
        public string QueueEndpoint { get; set; }
        /// <summary>
        /// 租户的使用的队列连接名称
        /// </summary>
        [DataMember]
        [ProtoMember(19)]
        [MaxLength(1000)]
        public string QueueAccessName { get; set; }
        /// <summary>
        /// 租户的使用的队列连接秘钥
        /// </summary>
        [DataMember]
        [ProtoMember(20)]
        [MaxLength(4000)]
        public string QueueAccessKeyPasswordHash { get; set; }

        public string GetDecryptQueueConnectionString()
        {
            if (string.IsNullOrEmpty(QueueEndpoint)
                || string.IsNullOrEmpty(QueueAccessName)
                || string.IsNullOrEmpty(QueueAccessKeyPasswordHash))
                return KC.Framework.Base.GlobalConfig.GetDecryptQueueConnectionString();

            var azureConn = @"DefaultEndpointsProtocol=https;QueueEndpoint={0};AccountName={1};AccountKey={2}";

            try
            {
                return string.Format(azureConn, QueueEndpoint, QueueAccessName, EncryptPasswordUtil.DecryptPassword(QueueAccessKeyPasswordHash, PrivateEncryptKey));
            }
            catch (Exception ex)
            {
                LogUtil.LogException(ex);
                return string.Empty;
            }
        }
        #endregion

        #region NoSql
        [DataMember]
        [ProtoMember(4)]
        public NoSqlType NoSqlType { get; set; }
        /// <summary>
        /// 租户的使用的非结构化数据库连接地址
        /// </summary>
        [DataMember]
        [ProtoMember(21)]
        [MaxLength(2000)]
        public string NoSqlEndpoint { get; set; }
        /// <summary>
        /// 租户的使用的非结构化数据库连接名称
        /// </summary>
        [DataMember]
        [ProtoMember(22)]
        [MaxLength(1000)]
        public string NoSqlAccessName { get; set; }
        /// <summary>
        /// 租户的使用的非结构化数据库连接秘钥
        /// </summary>
        [DataMember]
        [ProtoMember(23)]
        [MaxLength(4000)]
        public string NoSqlAccessKeyPasswordHash { get; set; }

        public string GetDecryptNoSqlConnectionString()
        {
            if (string.IsNullOrEmpty(NoSqlEndpoint)
                    || string.IsNullOrEmpty(NoSqlAccessName)
                    || string.IsNullOrEmpty(NoSqlAccessKeyPasswordHash))
                return KC.Framework.Base.GlobalConfig.GetDecryptNoSqlConnectionString();

            var azureConn = @"DefaultEndpointsProtocol=https;TableEndpoint={0};AccountName={1};AccountKey={2}";

            try
            {
                return string.Format(azureConn, NoSqlEndpoint, NoSqlAccessName, EncryptPasswordUtil.DecryptPassword(NoSqlAccessKeyPasswordHash, PrivateEncryptKey));
            }
            catch (Exception ex)
            {
                LogUtil.LogException(ex);
                return string.Empty;
            }
        }
        #endregion

        #region ServiceBus
        [DataMember]
        [ProtoMember(5)]
        public ServiceBusType ServiceBusType { get; set; }

        /// <summary>
        /// 租户的使用的分布式连接地址
        /// </summary>
        [DataMember]
        [ProtoMember(24)]
        [MaxLength(2000)]
        public string ServiceBusEndpoint { get; set; }
        /// <summary>
        /// 租户的使用的分布式连接名称
        /// </summary>
        [DataMember]
        [ProtoMember(25)]
        [MaxLength(1000)]
        public string ServiceBusAccessName { get; set; }
        /// <summary>
        /// 租户的使用的分布式连接秘钥
        /// </summary>
        [DataMember]
        [ProtoMember(26)]
        [MaxLength(4000)]
        public string ServiceBusAccessKeyPasswordHash { get; set; }

        public string GetDecryptServiceBusConnectionString()
        {
            if (string.IsNullOrEmpty(ServiceBusEndpoint)
                    || string.IsNullOrEmpty(ServiceBusAccessName)
                    || string.IsNullOrEmpty(ServiceBusAccessKeyPasswordHash))
                return KC.Framework.Base.GlobalConfig.GetDecryptServiceBusConnectionString();

            var azureConn = @"Endpoint={0};SharedAccessKeyName={1};SharedAccessKey={2}";

            try
            {
                return string.Format(azureConn, ServiceBusEndpoint, ServiceBusAccessName, EncryptPasswordUtil.DecryptPassword(ServiceBusAccessKeyPasswordHash, PrivateEncryptKey));
            }
            catch (Exception ex)
            {
                LogUtil.LogException(ex);
                return string.Empty;
            }
        }
        #endregion

        #region VOD
        [DataMember]
        [ProtoMember(6)]
        public VodType VodType { get; set; }

        /// <summary>
        /// 租户的使用的分布式连接地址
        /// </summary>
        [DataMember]
        [ProtoMember(27)]
        [MaxLength(2000)]
        public string VodEndpoint { get; set; }
        /// <summary>
        /// 租户的使用的分布式连接名称
        /// </summary>
        [DataMember]
        [ProtoMember(28)]
        [MaxLength(1000)]
        public string VodAccessName { get; set; }
        /// <summary>
        /// 租户的使用的分布式连接秘钥
        /// </summary>
        [DataMember]
        [ProtoMember(29)]
        [MaxLength(4000)]
        public string VodAccessKeyPasswordHash { get; set; }

        public string GetVodConnectionString()
        {
            if (string.IsNullOrEmpty(VodEndpoint)
                    || string.IsNullOrEmpty(VodAccessName)
                    || string.IsNullOrEmpty(VodAccessKeyPasswordHash))
                return KC.Framework.Base.GlobalConfig.GetDecryptVodConnectionString();

            var azureConn = @"VodEndpoint={0};AccountName={1};AccountKey={2}";
            try
            {
                return string.Format(azureConn, VodEndpoint, VodAccessName, EncryptPasswordUtil.DecryptPassword(VodAccessKeyPasswordHash, PrivateEncryptKey));
            }
            catch (Exception ex)
            {
                LogUtil.LogException(ex);
                return string.Empty;
            }
        }
        #endregion

        #region 代码仓库
        [DataMember]
        [ProtoMember(7)]
        public CodeType CodeType { get; set; }

        /// <summary>
        /// 租户的使用的分布式连接地址
        /// </summary>
        [DataMember]
        [ProtoMember(30)]
        [MaxLength(2000)]
        public string CodeEndpoint { get; set; }
        /// <summary>
        /// 租户的使用的分布式连接名称
        /// </summary>
        [DataMember]
        [ProtoMember(31)]
        [MaxLength(1000)]
        public string CodeAccessName { get; set; }
        /// <summary>
        /// 租户的使用的分布式连接秘钥
        /// </summary>
        [DataMember]
        [ProtoMember(32)]
        [MaxLength(4000)]
        public string CodeAccessKeyPasswordHash { get; set; }

        public string GetDecryptCodeConnectionString()
        {
            if (string.IsNullOrEmpty(CodeEndpoint)
                    || string.IsNullOrEmpty(CodeAccessName)
                    || string.IsNullOrEmpty(CodeAccessKeyPasswordHash))
                return KC.Framework.Base.GlobalConfig.GetDecryptCodeConnectionString();

            var azureConn = @"CodeEndpoint={0};AccountName={1};AccountKey={2}";
            try
            {
                return string.Format(azureConn, CodeEndpoint, CodeAccessName, EncryptPasswordUtil.DecryptPassword(CodeAccessKeyPasswordHash, PrivateEncryptKey));
            }
            catch (Exception ex)
            {
                LogUtil.LogException(ex);
                return string.Empty;
            }
        }
        #endregion

        #region 联系人信息
        /// <summary>
        /// 租户的联系人姓名
        /// </summary>
        [DataMember]
        [ProtoMember(40)]
        [MaxLength(50)]
        public string ContactName { get; set; }
        /// <summary>
        /// 租户的联系人邮箱
        /// </summary>
        [DataMember]
        [ProtoMember(41)]
        [MaxLength(200)]
        public string ContactEmail { get; set; }
        /// <summary>
        /// 租户的联系人电话
        /// </summary>
        [DataMember]
        [ProtoMember(42)]
        [MaxLength(20)]
        public string ContactPhone { get; set; }
        #endregion

        #region 租户域名设置（别名/独立域名）
        /// <summary>
        /// 租户别名
        /// </summary>
        [DataMember]
        [ProtoMember(50)]
        [MaxLength(50)]
        public string NickName { get; set; }
        /// <summary>
        /// 租户别名最后的修改时间
        /// </summary>
        [DataMember]
        [ProtoMember(51)]
        public DateTime? NickNameLastModifyDate { get; set; }

        #endregion

        [DataMember]
        [ProtoMember(54)]
        public DateTime? PasswordExpiredTime { get; set; }


    }
}
