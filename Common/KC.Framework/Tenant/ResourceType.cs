using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text;

namespace KC.Framework.Tenant
{
    [DataContract]
    public enum DatabaseType
    {
        [Description("Sql Server")]
        [EnumMember]
        SqlServer = 0,
        [Description("MySql")]
        [EnumMember]
        MySql = 1, 
        [Description("PostgreSql")]
        [EnumMember]
        PostgreSql = 2,
        [Description("未知")]
        [EnumMember]
        UNKNOWN = 99
    }
    [DataContract]
    public enum StorageType
    {
        [Description("Azure Blob")]
        [EnumMember]
        Blob = 0,
        [Description("本地文件")]
        [EnumMember]
        File = 1,
        [Description("FTP")]
        [EnumMember]
        FTP = 2,
        [Description("AWS S3")]
        [EnumMember]
        AWSS3 = 3,
        [Description("阿里云OSS")]
        [EnumMember]
        AliOSS = 4,
        [Description("腾讯云存储")]
        [EnumMember]
        Tencent = 5,
        [Description("华为云存储")]
        [EnumMember]
        Huawei = 6,
        [Description("未知")]
        [EnumMember]
        UNKNOWN = 99
    }
    [DataContract]
    public enum QueueType
    {
        [Description("Azure Queue")]
        [EnumMember]
        AzureQueue = 0,
        [Description("Azure ServiceBus")]
        [EnumMember]
        ServiceBus = 1,
        [Description("Redis")]
        [EnumMember]
        Redis = 2,
        [Description("Kafka")]
        [EnumMember]
        Kafka = 3,
        [Description("RabbitMQ")]
        [EnumMember]
        RabbitMQ = 4,
        [Description("未知")]
        [EnumMember]
        UNKNOWN = 99
    }
    [DataContract]
    public enum NoSqlType
    {
        [Description("AzureTable")]
        [EnumMember]
        AzureTable = 0,
        [Description("MongDB")]
        [EnumMember]
        MongDB = 1,
        [Description("Redis")]
        [EnumMember]
        Redis = 2, 
        [Description("Elastic")]
        [EnumMember]
        Elastic = 3, 
        [Description("Database")]
        [EnumMember]
        Database = 4,
        [Description("未知")]
        [EnumMember]
        UNKNOWN = 99
    }
    [DataContract]
    public enum ServiceBusType
    {
        [Description("ServiceBus")]
        [EnumMember]
        ServiceBus = 0,
        [Description("Kafka")]
        [EnumMember]
        Kafka = 1,
        [Description("Redis")]
        [EnumMember]
        Redis = 2,
        [Description("RabbitMQ")]
        [EnumMember]
        RabbitMQ = 3,
        [Description("未知")]
        [EnumMember]
        UNKNOWN = 99
    }

    [DataContract]
    public enum VodType
    {
        [Description("Azure Blob")]
        [EnumMember]
        Azure = 0,
        [Description("本地文件")]
        [EnumMember]
        File = 1,
        [Description("AWS Vod")]
        [EnumMember]
        AWS = 3,
        [Description("阿里云Vod")]
        [EnumMember]
        Aliyun = 4,
        [Description("腾讯云Vod")]
        [EnumMember]
        Tencent = 6,
        [Description("华为云Vod")]
        [EnumMember]
        Huawei = 7,
        [Description("未知")]
        [EnumMember]
        UNKNOWN = 99
    }

    [DataContract]
    public enum CodeType
    {
        [Description("Gitlab")]
        [EnumMember]
        Gitlab = 0,
        [Description("SVN")]
        [EnumMember]
        SVN = 1,
        [Description("未知")]
        [EnumMember]
        UNKNOWN = 99
    }
}
