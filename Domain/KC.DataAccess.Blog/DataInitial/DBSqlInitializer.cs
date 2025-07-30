using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.Framework.Util;
using KC.Framework.SecurityHelper;

namespace KC.DataAccess.Blog.DataInitial
{
    public static class DBSqlInitializer
    {
        //public static string GetSchemaConnectionString()
        //{
        //    return ConfigUtil.GetConfigItem("ComAccountContext"); //System.Configuration.ConfigurationManager.ConnectionStrings["CFWinContext"].ConnectionString;
        //}
        public static List<string> lines = EmbeddedFileHelper.GetEmbeddedFileContents("KC.DataAccess.Blog.DataInitial.CurrentVersion.txt");
        public static string GetPreMigrationVersion()
        {
            //List<string> lines = EmbeddedFileHelper.GetEmbeddedFileContents("KC.DataAccess.Blog.DataInitial.CurrentVersion.txt");
            return lines[0].Trim();
        }
        public static string GetLatestMigrationVersion()
        {
            //List<string> lines = EmbeddedFileHelper.GetEmbeddedFileContents("KC.DataAccess.Blog.DataInitial.CurrentVersion.txt");
            return lines.Count > 1 ? lines[1].Trim() : string.Empty;
        }
        public static bool IsNeedUpdataDatabase()
        {
            var preMigration = GetPreMigrationVersion();
            var latestMigration = GetLatestMigrationVersion();
            if (!preMigration.EndsWith(latestMigration))
                return true;

            return false;
        }
        public static List<string> GetSchemaUpgradeSqlScript(int version)
        {
            var key = "KC.DataAccess.Blog.DataInitial.SchemaUpgrade.SQL_" + version + ".sql";
            var cache = GetCache(key);
            if (cache == null)
            {
                var sqls = EmbeddedFileHelper.GetEmbeddedFileContents(key);
                cache = sqls;
                try
                {
                    SetCache(key, cache);
                }
                catch (Exception)
                {
                }
            }
            
            return cache;
        }
        public static List<string> GetStructureSqlScript()
        {
            var key = "KC.DataAccess.Blog.DataInitial.SQL_Structure.sql";
            var cache = GetCache(key);
            if (cache == null)
            {
                var sqls = EmbeddedFileHelper.GetEmbeddedFileContents(key);
                cache = sqls;
                try
                {
                    SetCache(key, cache);
                }
                catch (Exception)
                {
                }
            }

            return cache;
        }
        public static List<string> GetInitialDataSqlScript()
        {
            var key = "KC.DataAccess.Blog.DataInitial.SQL_InitialData.sql";
            var cache = GetCache(key);
            if (cache == null)
            {
                var sqls = EmbeddedFileHelper.GetEmbeddedFileContents(key);
                cache = sqls;
                try
                {
                    SetCache(key, cache);
                }
                catch (Exception)
                {
                }
            } 

            return cache;
        }

        private static Dictionary<string, List<string>> CachedItems = new Dictionary<string, List<string>>();
        private static void SetCache(string key, List<string> cacheObj)
        {
            if (CachedItems.ContainsKey(key))
                return;

            CachedItems.Add(key, cacheObj);
        }
        private static List<string> GetCache(string key)
        {
            if (CachedItems.ContainsKey(key))
                return CachedItems[key];

            return null;
        }
    }
}
