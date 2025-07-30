using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace KC.Storage.BlobService
{
    public abstract class WriterBase : BlobBase, IDisposable
    {
        public WriterBase()
        {
        }

        ~WriterBase()
        {
            Dispose(false);
        }

        private bool _disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            _disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected abstract void CreateContainer(string container);

        public void AddContainer(string container)
        {
            this.CreateContainer(container);

        }

        protected abstract void DeleteContainer(string container);

        public void RemoveContainer(string container)
        {
            this.DeleteContainer(container);
        }

        protected abstract void WriteBlob(string container, string blobId, byte[] blobData, Dictionary<string, string> metadata);

        public void SaveBlob(string container, string blobId, byte[] blobData, Dictionary<string, string> metadata)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            this.WriteBlob(container, blobId, blobData, metadata);

            //LogUtil.LogDebug("Saved Blob", "BlobId=" + blobId + " Size=" + blobData.Length, stopwatch.ElapsedMilliseconds);

            //if (this.BlobSaved != null)
            //    this.BlobSaved(this, new AzureBlobEventArgs(container, blobId, true, metadata != null));
        }

        protected abstract void DeleteBlob(string container, string blobId);

        public void RemoveBlob(string container, string blobId)
        {
            this.DeleteBlob(container, blobId);
        }

        public abstract void CopyBlob(string containerName, string desContainerName, string blobId);

        protected abstract void WriteBlobMetadata(string container, string blobId, bool clearExisting, Dictionary<string, string> blobMetadata);

        /// <summary>Merge blob metadata</summary>
        public void AppendBlobMetadata(string container, string blobId, Dictionary<string, string> blobMetadata)
        {
            this.WriteBlobMetadata(container, blobId, false, blobMetadata);
        }

        /// <summary>Will overwrite blob metadata</summary>
        public void SaveBlobMetadata(string container, string blobId, Dictionary<string, string> blobMetadata)
        {
            this.WriteBlobMetadata(container, blobId, true, blobMetadata);
        }

    }
}