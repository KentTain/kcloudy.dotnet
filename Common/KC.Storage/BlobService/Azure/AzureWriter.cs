using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Blobs.Models;
using Azure;

namespace KC.Storage.BlobService
{
    internal class AzureWriter : WriterBase
    {
        private readonly BlobServiceClient Account;
        private readonly BlobCache Cache;

        public AzureWriter(string connectionString, BlobCache cache)
            : base()
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("Azure Storage's connect string is empy or null.", "connectionString");

            this.Cache = cache;

            // Create a client that can authenticate with a connection string
            this.Account = new BlobServiceClient(connectionString);
        }

        private BlobClient GetBlobClient(string containerName, string blobId, bool isBlockBlob = false)
        {
            if (string.IsNullOrWhiteSpace(containerName))
            {
                return null;
            }
            else
            {
                // Create the container and return a container client object
                var container = Account.GetBlobContainerClient(containerName);
                container.CreateIfNotExists();

                return isBlockBlob ? container.GetBlobClient(blobId) : container.GetBlobClient(blobId);
            }
        }

        protected override void CreateContainer(string containerName)
        {
            BlobContainerClient container = this.Account.CreateBlobContainer(containerName);
        }

        protected async override void DeleteContainer(string containerName)
        {
            var container = Account.GetBlobContainerClient(containerName);
            try
            {
                var response = await container.DeleteAsync();

            }
            catch (RequestFailedException e)
            {
                if (e.ErrorCode == BlobErrorCode.ResourceNotFound)
                    return;
                else
                    throw e;
            }
        }

        protected override void WriteBlob(string container, string blobId, byte[] blobData, Dictionary<string, string> blobMetadata)
        {
            var blob = GetBlobClient(container, blobId, true);
            var bData = new BinaryData(blobData);
            blob.Upload(bData);
            if (blobMetadata != null)
            {
                blob.SetMetadataAsync(blobMetadata);
            }
        }

        protected async override void WriteBlobMetadata(string container, string blobId, bool clearExisting, Dictionary<string, string> blobMetadata)
        {
            var blob = GetBlobClient(container, blobId);
            if (await blob.ExistsAsync())
            {
                await blob.SetMetadataAsync(blobMetadata);
            }
        }

        protected async override void DeleteBlob(string container, string blobId)
        {
            string realBlobId = GetActualBlobId(blobId);
            if (!string.IsNullOrWhiteSpace(realBlobId))
            {
                var blob = GetBlobClient(container, realBlobId);
                if (blob != null)
                {
                    try
                    {
                        await blob.DeleteIfExistsAsync();
                    }
                    catch (RequestFailedException e)
                    {
                        if (e.ErrorCode == BlobErrorCode.ResourceNotFound)
                            return;
                        else
                            throw e;
                    }

                }
                if (this.Cache != null)
                    this.Cache.Delete(container, realBlobId);
            }
        }

        #region Copy Blob

        public override void CopyBlob(string containerName, string desContainerName, string blobId)
        {
            if (string.IsNullOrEmpty(blobId)) return;

            var sourceBlob = GetBlobClient(containerName, blobId, true);
            if (sourceBlob.Exists())
            {
                // Lease the source blob for the copy operation 
                // to prevent another client from modifying it.
                var lease = sourceBlob.GetBlobLeaseClient();

                // Specifying -1 for the lease interval creates an infinite lease.
                lease.Acquire(TimeSpan.FromSeconds(-1));

                // Get the source blob's properties and display the lease state.
                BlobProperties sourceProperties = sourceBlob.GetProperties();
                Console.WriteLine($"Lease state: {sourceProperties.LeaseState}");

                var destBlob = GetBlobClient(desContainerName, blobId, true);
                destBlob.StartCopyFromUriAsync(sourceBlob.Uri);
            }
        }

        #endregion

        
    }
}