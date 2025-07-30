using System;
using System.Runtime.Serialization;
using ProtoBuf;

namespace KC.Framework.Base
{
    [Serializable, DataContract(IsReference = true)]
    [ProtoContract]
    public class BlobInfo : ICloneable
    {
        /// <summary>
        /// 文件Id
        /// </summary>
        [DataMember]
        [ProtoMember(1)]
        public string Id { get; set; }
        /// <summary>
        /// 文件类型：KC.Common.FileHelper.FileType
        ///     Image=1、Word=2、Excel=3、PDF=4、PPT=5、Text=6、Xml=7、Audio=8、Video=9、Zip=10、Unknown=99
        /// </summary>
        [DataMember]
        [ProtoMember(2)]
        public string FileType { get; set; }
        /// <summary>
        /// 文件格式：
        ///     KC.Common.FileHelper.ImageFormat(Bmp = 1, Gif = 2, Icon = 3, Jpeg = 4, Png = 5, Tiff = 6, Wmf = 7, Dwg=8, Unknown = 99)
        ///     KC.Common.FileHelper.DocFormat(Doc = 1, Docx = 2, Xls = 3, Xlsx = 4, Ppt = 5, Pptx = 6, Pdf = 7, Rar=40, Zip=41, Gzip=42,  Unknown = 99)
        ///     KC.Common.FileHelper.AudioFormat(Basic = 1, Wav = 2, Mpeg = 3, Ram = 4, Rmi = 5, Aif = 6, Unknown = 99)
        ///     KC.Common.FileHelper.VideoFormat(Wmv = 1, Mp4 = 2, Flv = 3, Avi = 4, Mov = 5, Unknown = 99)
        /// </summary>
        [DataMember]
        [ProtoMember(3)]
        public string FileFormat { get; set; }
        /// <summary>
        /// 文件名称
        /// </summary>
        [DataMember]
        [ProtoMember(4)]
        public string FileName { get; set; }
        /// <summary>
        /// 文件流
        /// </summary>
        [DataMember]
        [ProtoMember(5)]
        public byte[] Data { get; set; }
        /// <summary>
        /// 文件大小
        /// </summary>
        [DataMember]
        [ProtoMember(6)]
        public long Size { get; set; }
        /// <summary>
        /// 水印
        /// </summary>
        [DataMember]
        [ProtoMember(7)]
        public bool Placeholder { get; set; }
        /// <summary>
        /// 本地Id
        /// </summary>
        [DataMember]
        [ProtoMember(8)]
        public string LocalId { get; set; }

        /// <summary>
        /// 本地Id
        /// </summary>
        [DataMember]
        [ProtoMember(9)]
        public string Ext { get; set; }

        // default constuctor added for SoapContentService
        public BlobInfo()
        {

        }

        public BlobInfo(string id, string type, string format, string fileName, string ext)
            : this(id, type, format, fileName, ext, null)
        {
        }

        public BlobInfo(string id, string type, string format, string fileName, string ext, byte[] data)
        {
            Id = id;
            FileType = type;
            FileFormat = format;
            Data = data;
            Size = data == null ? 0 : data.Length;
            FileName = fileName;
            Ext = ext;
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