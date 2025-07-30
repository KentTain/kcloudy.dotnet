using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using KC.Framework;
using KC.Framework.Util;
using KC.Framework.Tenant;
using KC.Framework.Base;

namespace KC.Component.FileRespository
{
    public abstract class FileRespositoryBase<T> : IRepository<T>, IDisposable where T : AzureTableEntity
    {
        protected string RootPath { get; private set; }
        protected string ContainerName { get; private set; }
        public Tenant Tenant { get; private set; }

        protected FileRespositoryBase(string rootName, string containerName)
        {
            RootPath = rootName;
            ContainerName = containerName;
        }

        protected FileRespositoryBase(string rootName, Tenant tenant)
        {
            RootPath = rootName;
            ContainerName = tenant.TenantName;
            Tenant = tenant;
        }

        public IList<T> FindAll()
        {
            try
            {
                var blobIds = ListBlobIds();

                var objStringList = blobIds.Select(ReadBlob);

                return objStringList.Select(DeserializerToObject)
                    .Where(threadCfg => threadCfg != null)
                    .ToList();
            }
            catch (Exception ex)
            {
                LogUtil.LogException(ex);
                return null;
            }
        }

        public IList<T> FindAll(System.Linq.Expressions.Expression<Func<T, bool>> predicate)
        {
            return FindAll().AsQueryable().Where(predicate).ToList();
        }

        public IList<T> FindAll<K>(System.Linq.Expressions.Expression<Func<T, K>> keySelector, bool ascending = true)
        {
            if (ascending)
            {
                return FindAll().OrderBy(t => keySelector).ToList();
            }
            else
            {
                return FindAll().OrderByDescending(t => keySelector).ToList();
            }
        }

        public IList<T> FindAll<K>(System.Linq.Expressions.Expression<Func<T, bool>> predicate, System.Linq.Expressions.Expression<Func<T, K>> keySelector, bool ascending = true)
        {
            if (ascending)
            {
                return FindAll().AsQueryable().Where(predicate).OrderBy(t => keySelector).ToList();
            }
            else
            {
                return FindAll().AsQueryable().Where(predicate).OrderByDescending(t => keySelector).ToList();
            }
        }

        public bool Add(T entity, bool isSave = true)
        {
            try
            {
                var data = SerializerToXmlString(entity);
                WriteBlob(entity.RowKey, data);
                return true;
            }
            catch (Exception ex)
            {
                LogUtil.LogException(ex);
                return false;
            }

        }

        public bool Modify(T entity, bool isSave = true)
        {
            try
            {
                var data = SerializerToXmlString(entity);
                WriteBlob(entity.RowKey, data);
                return true;
            }
            catch (Exception ex)
            {
                LogUtil.LogException(ex);
                return false;
            }
        }

        public bool Remove(T entity, bool isSave = true)
        {
            try
            {
                DeleteBlob(entity.RowKey);
                return true;
            }
            catch (Exception ex)
            {
                LogUtil.LogException(ex);
                return false;
            }
        }

        public int Remove(IEnumerable<T> entities, bool isSave = true)
        {
            var i = 0;
            try
            {
                foreach (var entity in entities)
                {
                    DeleteBlob(entity.RowKey);
                    i++;
                }
                
                return i;
            }
            catch (Exception ex)
            {
                LogUtil.LogException(ex);
                return i;
            }
        }

        public bool RemoveAll()
        {
            try
            {
                string fileName = RootPath + ContainerName;
                var dir = new DirectoryInfo(fileName);
                if (dir.Exists)
                {
                    var childs = dir.GetDirectories();
                    foreach (var child in childs)
                    {
                        child.Delete(true);
                    }

                    dir.Delete(true);
                }

                return true;
            }
            catch (Exception ex)
            {
                LogUtil.LogException(ex);
                return false;
            }
        }

        #region private methods
        private void CreateContainer()
        {
            string dirName = RootPath + ContainerName;
            if (!Directory.Exists(dirName))
            {
                Directory.CreateDirectory(dirName);
            }
        }

        private string ReadBlob(string blobId)
        {
            CreateContainer();
            string fileName = RootPath + ContainerName + "\\" + blobId;

            return File.ReadAllText(fileName, Encoding.Default);
        }

        private void WriteBlob(string blobId, byte[] blobData)
        {
            CreateContainer();
            string fileName = RootPath + ContainerName + "\\" + blobId;
            File.WriteAllBytes(fileName, blobData);
        }

        private void DeleteBlob(string blobId)
        {
            CreateContainer();
            string fileName = RootPath + ContainerName + "\\" + blobId;
            File.Delete(fileName);
        }

        private byte[] SerializerToXmlString(T xmlObj)
        {
            byte[] blobData;
            var stream = new MemoryStream();
            var setting = new XmlWriterSettings { Encoding = Encoding.UTF8, Indent = true };
            using (var writer = XmlWriter.Create(stream, setting))
            {
                var xs = new XmlSerializer(typeof(T));
                var ns = new XmlSerializerNamespaces();
                ns.Add("", "");
                xs.Serialize(writer, xmlObj, ns);
                blobData = stream.ToArray();
            }

            return blobData;
        }

        private T DeserializerToObject(string blobId)
        {
            string fileName = RootPath + ContainerName + "\\" + blobId;

            if (!File.Exists(fileName))
                return null;

            using (var reader = XmlReader.Create(fileName))
            {
                var xs = new XmlSerializer(typeof(T));
                var ns = new XmlSerializerNamespaces();
                ns.Add("", "");
                var obj = xs.Deserialize(reader);
                return (T)obj;
            }
        }

        private List<string> ListBlobIds()
        {
            string dirName = RootPath + ContainerName;
            List<string> allFileNames = System.IO.Directory.GetFiles(dirName).ToList().ConvertAll(Path.GetFileName);
            return allFileNames.Where(f => !f.EndsWith(".metadata")).ToList();
        }
        #endregion

        ~FileRespositoryBase()
        {
            this.Dispose();
        }

        private bool _disposed = false;
        public void Dispose()
        {
            if (!this._disposed)
            {
                this._disposed = true;
            }
        }
    }

    public abstract class CommonFileRepository<T> : FileRespositoryBase<T> where T : AzureTableEntity
    {
        private static readonly string rootName = KC.Framework.Base.GlobalConfig.TempFilePath;
        protected CommonFileRepository(string container)
            : base(rootName, container)
        {

        }

        protected CommonFileRepository(Tenant tenant)
            : base(rootName, tenant)
        {

        }

        protected CommonFileRepository(string rootName, string container)
            : base(rootName, container)
        {

        }

    }
}