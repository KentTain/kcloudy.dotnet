using System.Linq;
using KC.Storage.BlobService;
using KC.Framework.Tenant;

namespace KC.Storage.Util.BlobHelper
{
    public partial class BlobProvider
    {
        private IBlobProvider Provider { get; set; }
        internal WriterBase Writer { get { return Provider.GetWriter(); } }

        public Tenant Tenant { get; set; }
        public BlobProvider(Tenant tenant)
        {
            Tenant = tenant;
            Provider = GetProvider(tenant);
        }

        internal IBlobProvider GetProvider(Tenant tenant)
        {
            IBlobProvider result = null;
            var storageType = tenant.StorageType;
            switch (storageType)
            {
                case StorageType.File:
                    {
                        result = new FileSystemProvider(tenant);
                    }
                    break;
                case StorageType.FTP:
                    {
                        result = new FtpSystemProvider(tenant);
                    }
                    break;
                case StorageType.Blob:
                    {
                        result = new AzureProvider(tenant);
                    }
                    break;
                case StorageType.AWSS3:
                    {
                        result = new AwsS3Provider(tenant);
                    }
                    break;
                case StorageType.AliOSS:
                    {
                        result = new AliyunOSSProvider(tenant);
                    }
                    break;
                default:
                    {
                        result = new FileSystemProvider(tenant);
                    }
                    break;
            }

            return result;
        }

        public void CopyBlobsToOtherTenant(Tenant targetTenant, string[] blobIds, string userId, string encryptionKey, bool isInternal = true, bool singleHandler = false)
        {
            var sourceTenant = Tenant.TenantName.ToLower();
            var targetProvider = GetProvider(targetTenant);
            var targetTenantName = targetTenant.TenantName.ToLower();
            if (!singleHandler)
            {
                var blobs = Provider.GetBlobs(sourceTenant, encryptionKey, isInternal, userId, 
                    blobIds.ToList(), false, false);
                if (blobs == null || !blobs.Any()) return;
                foreach (var blobInfo in blobs)
                {
                    if (blobInfo == null) continue;

                    targetProvider.SaveBlob(targetTenantName, encryptionKey, isInternal, userId, blobInfo, false, -1);
                }
            }
            else
            {
                foreach (var blobid in blobIds)
                {
                    var blobInfo = Provider.GetBlob(sourceTenant, encryptionKey, isInternal, userId, blobid, false);
                    if (blobInfo == null) continue;

                    targetProvider.SaveBlob(targetTenantName, encryptionKey, isInternal, userId, blobInfo, false, -1);
                }
            }
        }
    }
}
