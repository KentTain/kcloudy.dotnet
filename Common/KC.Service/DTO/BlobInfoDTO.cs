using KC.Framework.Base;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;

namespace KC.Service.DTO
{
    [Serializable, DataContract(IsReference = true)]
    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public class BlobInfoDTO : ICloneable
    {
        /// <summary>
        /// 对应的PropertyId
        /// </summary>
        [DataMember]
        [ProtoMember(1)]
        public int? PropertyId;

        /// <summary>
        /// 文件Id
        /// </summary>
        [DataMember]
        [ProtoMember(2)]
        [MaxLength(128)]
        public string BlobId { get; set; }

        /// <summary>
        /// 文件名称
        /// </summary>
        [DataMember]
        [ProtoMember(3)]
        [MaxLength(256)]
        public string BlobName { get; set; }

        /// <summary>
        /// 文件扩展名
        /// </summary>
        [DataMember]
        [ProtoMember(4)]
        [MaxLength(50)]
        public string Ext { get; set; }

        /// <summary>
        /// 文件类型：KC.Common.FileHelper.FileType
        ///     Image=1、Word=2、Excel=3、PDF=4、PPT=5、Text=6、Xml=7、Audio=8、Video=9、Unknown
        /// </summary>
        [DataMember]
        [ProtoMember(5)]
        public string FileType { get; set; }
        /// <summary>
        /// 文件格式：
        ///     KC.Common.FileHelper.ImageFormat(Bmp = 1, Gif = 2, Icon = 3, Jpeg = 4, Png = 5, Tiff = 6, Wmf = 7, Unknown = 99)
        ///     KC.Common.FileHelper.DocFormat(Doc = 1, Docx = 2, Xls = 3, Xlsx = 4, Ppt = 5, Pptx = 6, Pdf = 7, Unknown = 99)
        ///     KC.Common.FileHelper.AudioFormat(Basic = 1, Wav = 2, Mpeg = 3, Ram = 4, Rmi = 5, Aif = 6, Unknown = 99)
        /// </summary>
        [DataMember]
        [ProtoMember(6)]
        public string FileFormat { get; set; }

        /// <summary>
        /// 文件大小
        /// </summary>
        [DataMember]
        [ProtoMember(7)]
        public long Size { get; set; }

        [DataMember]
        [ProtoMember(8)]
        public bool IsSelect { get; set; }

        /// <summary>
        /// 本地Id
        /// </summary>
        [DataMember]
        [ProtoMember(9)]
        public string LocalId { get; set; }

        /// <summary>
        /// 租户编码
        /// </summary>
        [DataMember]
        [ProtoMember(10)]
        public string TenantName { get; set; }

        /// <summary>
        /// 图片查看地址
        /// </summary>
        [DataMember]
        [ProtoMember(11)]
        public string ShowImageUrl
        {
            get
            {
                if (string.IsNullOrEmpty(TenantName))
                    return "";
                
                var docApiDomain = GlobalConfig.GetTenantWebApiDomain(GlobalConfig.DocWebDomain, TenantName);
                return docApiDomain + "Resources/ShowImage?id=" + BlobId;
            }
        }

        /// <summary>
        /// 文件下载地址
        /// </summary>
        [DataMember]
        [ProtoMember(12)]
        public string DownloadFileUrl
        {
            get
            {
                if (string.IsNullOrEmpty(TenantName))
                    return "";

                var docApiDomain = GlobalConfig.GetTenantWebApiDomain(GlobalConfig.DocWebDomain, TenantName);
                return docApiDomain + "Resources/DownloadFile?id=" + BlobId;
            }
        }

        public BlobInfo Clone()
        {
            return (BlobInfo)this.MemberwiseClone();
        }

        object ICloneable.Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
