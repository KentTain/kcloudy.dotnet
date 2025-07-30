using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Framework.Tenant
{
    public static class ConnectionKeyConstant
    {
        public static readonly string Endpoint = "endpoint";
        public static readonly string QueueEndpoint = "queueendpoint";
        public static readonly string BlobEndpoint = "blobendpoint";
        public static readonly string NoSqlEndpoint = "tableendpoint";
        public static readonly string RedisEndpoint = "redisendpoint";
        public static readonly string VodEndpoint = "vodendpoint";
        public static readonly string CodeEndpoint = "codeendpoint";

        public static readonly string AccessKey = "accountkey";
        public static readonly string AccessName = "accountname";

        public static readonly string ServiceBusEndpoint = "endpoint";
        public static readonly string ServiceBusAccessKey = "sharedaccesskey";
        public static readonly string ServiceBusAccessName = "sharedaccesskeyname";

        public static readonly string DatabaseEndpoint = "server";
        public static readonly string DatabaseName = "database";
        public static readonly string DatabaseUserID = "user id";
        public static readonly string DatabasePassword = "password";
        public static readonly string DatabasePort = "port";


        public static readonly string DefaultMigrationsHistoryTable = "__{0}MigrationsHistory";

        public static readonly string IMAGE_CODE = "code";
        public static readonly string PHONE_CODE_RERGISTER = "register_";
    }
}
