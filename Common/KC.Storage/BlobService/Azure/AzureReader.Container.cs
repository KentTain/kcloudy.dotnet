using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace KC.Storage.BlobService
{
    internal partial class AzureReader
    {
        public override List<string> ListContainers()
        {
            List<string> containerNames = new List<string>();
            var resultSegment = Account.GetBlobContainers(BlobContainerTraits.Metadata, null, default).AsPages(default, 200);
            foreach (Azure.Page<BlobContainerItem> containerPage in resultSegment)
            {
                foreach (BlobContainerItem containerItem in containerPage.Values)
                {
                    containerNames.Add(containerItem.Name);
                }
            }
            return containerNames;
        }

        public override bool ExistContainer(string containerName)
        {
            if (string.IsNullOrEmpty(containerName))
                return false;

            var container = Account.GetBlobContainerClient(containerName);
            return container.Exists();
        }

        public override List<string> ListBlobIds(string containerName)
        {
            if (string.IsNullOrEmpty(containerName)) return new List<string>();

            List<string> blobIds = new List<string>();
            var container = Account.GetBlobContainerClient(containerName);
            foreach (var blobItem in container.GetBlobs())
            {
                blobIds.Add(blobItem.Name);
            }

            return blobIds;
        }

        public override List<string> ListBlobIdsWithMetadata(string containerName)
        {
            if (string.IsNullOrEmpty(containerName)) return new List<string>();

            List<string> blobIds = new List<string>();
            var container = Account.GetBlobContainerClient(containerName);
            foreach (var blobItem in container.GetBlobs(BlobTraits.Metadata))
            {
                blobIds.Add(blobItem.Name);
            }

            return blobIds;
        }
    }
}
