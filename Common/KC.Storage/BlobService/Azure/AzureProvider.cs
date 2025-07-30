using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.Framework.Util;
using KC.Framework.Tenant;

namespace KC.Storage.BlobService
{
    public partial class AzureProvider : ProviderBase, IBlobProvider
    {
        private readonly string TempFilePath;
        private readonly string StorageConnectionString;
        private readonly BlobCache Cache;
        private readonly AzureReader Reader;
        private readonly AzureWriter Writer;

        //public AzureProvider()
        //{
        //    this.TempFilePath = ConfigUtil.GetConfigItem("TempFilePath") + "\\";
        //    this.StorageConnectionString = ConfigUtil.GetConfigItem("StorageConnectionString");
        //    this.Cache = new AzureCache(this.TempFilePath);
        //    this.Reader = new AzureReader(this.StorageConnectionString, this.Cache);
        //    this.Writer = new AzureWriter(this.StorageConnectionString, this.Cache);
        //}

        public AzureProvider(Tenant tenant)
        {
            try
            {
                this.StorageConnectionString = tenant.GetDecryptStorageConnectionString();
                this.TempFilePath = KC.Framework.Base.GlobalConfig.TempFilePath + "\\";
                //this.Cache = new BlobCache(this.TempFilePath);
                this.Reader = new AzureReader(this.StorageConnectionString, this.Cache);
                this.Writer = new AzureWriter(this.StorageConnectionString, this.Cache);
            }
            catch (Exception ex)
            {
                LogUtil.LogError("Initilize the Azure Provider connection throw error. " + ex.Message);
            }
        }

        public AzureProvider(string connectionString)
        {
            this.TempFilePath = KC.Framework.Base.GlobalConfig.TempFilePath + "\\";
            this.StorageConnectionString = connectionString;
            //this.Cache = new BlobCache(this.TempFilePath);
            this.Reader = new AzureReader(this.StorageConnectionString, this.Cache);
            this.Writer = new AzureWriter(this.StorageConnectionString, this.Cache);
        }

        public override WriterBase GetWriter()
        {
            return this.Writer;
        }

    }
}
