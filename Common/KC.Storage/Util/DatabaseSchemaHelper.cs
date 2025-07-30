using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using CFW.Common.Util;
using Ionic.Utils.Zip;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;

namespace CFW.Azure.Common.Util
{
    class DatabaseSchemaHelper
    {
        private void SaveSchemaToZip(DatabaseType schemaType, int schemaVersion)
        {
            string zipFileName = Enum.GetName(typeof(DatabaseType), schemaType) + "." + schemaVersion + ".zip";
            string zipFilePath = ConfigUtil.GetConfigItem("TempFilePath") + "\\" + zipFileName;

            ZipFile zip = new ZipFile(zipFilePath);
            zip.AddFileStream("CurrentVersion.txt", string.Empty, SeismicDeployment.GetEmbeddedFileStream("Nuage.Common.DataLayer.SQLServer.Schema.CurrentVersion.txt"));
            zip.AddFileStream("SQL_Conversion.sql", string.Empty, SeismicDeployment.GetEmbeddedFileStream("Nuage.Common.DataLayer.SQLServer.Schema.SQL_Conversion.sql"));
            zip.AddFileStream("SQL_InitialData.sql", string.Empty, SeismicDeployment.GetEmbeddedFileStream("Nuage.Common.DataLayer.SQLServer.Schema.SQL_InitialData.sql"));
            zip.AddFileStream("SQL_Structure.sql", string.Empty, SeismicDeployment.GetEmbeddedFileStream("Nuage.Common.DataLayer.SQLServer.Schema.SQL_Structure.sql"));
            zip.Save();

            CloudBlobClient client = CloudStorageAccount.FromConfigurationSetting("DataConnectionString").CreateCloudBlobClient();
            CloudBlobContainer container = client.GetContainerReference("schema");
            container.CreateIfNotExist();
            CloudBlob blob = container.GetBlobReference(zipFileName);
            blob.UploadFile(zipFilePath);

            File.Delete(zipFilePath);
        }
    }
}
