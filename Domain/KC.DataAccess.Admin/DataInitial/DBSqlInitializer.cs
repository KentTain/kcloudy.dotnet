using System;
using System.Collections.Generic;
using KC.Framework.Util;
using KC.Framework.SecurityHelper;

namespace KC.DataAccess.Admin.DataInitial
{
    public static class DBSqlInitializer
    {
        public static List<string> lines = EmbeddedFileHelper.GetEmbeddedFileContents("KC.DataAccess.Admin.DataInitial.CurrentVersion.txt");
        public static string GetPreMigrationVersion()
        {
            //List<string> lines = EmbeddedFileHelper.GetEmbeddedFileContents("KC.DataAccess.Admin.DataInitial.CurrentVersion.txt");
            return lines[0].Trim();
        }
        public static string GetLatestMigrationVersion()
        {
            //List<string> lines = EmbeddedFileHelper.GetEmbeddedFileContents("KC.DataAccess.Admin.DataInitial.CurrentVersion.txt");
            return lines.Count > 1 ? lines[1].Trim() : string.Empty;
        }
        public static List<string> GetSchemaUpgradeSqlScript(int version)
        {
            return EmbeddedFileHelper.GetEmbeddedFileContents("KC.DataAccess.Admin.DataInitial.SchemaUpgrade.SQL_" + version + ".sql");
        }
        public static List<string> GetStructureSqlScript()
        {
            return EmbeddedFileHelper.GetEmbeddedFileContents("KC.DataAccess.Admin.DataInitial.SQL_Structure.sql");
        }
        public static List<string> GetInitialDataSqlScript()
        {
            return EmbeddedFileHelper.GetEmbeddedFileContents("KC.DataAccess.Admin.DataInitial.SQL_InitialData.sql");
        }
    }
}
