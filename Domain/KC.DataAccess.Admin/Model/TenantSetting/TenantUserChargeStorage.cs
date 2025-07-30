using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using KC.Model.Admin.Constants;
using KC.Framework.Base;
using System.Runtime.Serialization;

namespace KC.Model.Admin
{
    [Table(Tables.TenantUserChargeStorage)]
    [Serializable, DataContract(IsReference = true)]
    public class TenantUserChargeStorage : Entity
    {
        public TenantUserChargeStorage()
        {
            ChargeId = Guid.NewGuid();
        }

        [Key]
        [DataMember]
        public Guid ChargeId { get; set; }

        /// <summary>
        /// 来源租户编码
        /// </summary>
        [MaxLength(20)]
        [DataMember]
        public string TenantName { get; set; }
        /// <summary>
        /// 来源租户名称
        /// </summary>
        [MaxLength(512)]
        [DataMember]
        public string TenantDisplayName { get; set; }

        /// <summary>
        /// 文件Id
        /// </summary>
        [DataMember]
        [MaxLength(50)]
        public string FileId { get; set; }
        /// <summary>
        /// 文件类型：KC.Common.FileHelper.FileType
        ///     Image=1、Word=2、Excel=3、PDF=4、PPT=5、Text=6、Xml=7、Audio=8、Video=9、Zip=10、Unknown=99
        /// </summary>
        [DataMember]
        [MaxLength(50)]
        public string FileType { get; set; }
        /// <summary>
        /// 文件格式：
        ///     KC.Common.FileHelper.ImageFormat(Bmp = 1, Gif = 2, Icon = 3, Jpeg = 4, Png = 5, Tiff = 6, Wmf = 7, Dwg=8, Unknown = 99)
        ///     KC.Common.FileHelper.DocFormat(Doc = 1, Docx = 2, Xls = 3, Xlsx = 4, Ppt = 5, Pptx = 6, Pdf = 7, Rar=40, Zip=41, Gzip=42,  Unknown = 99)
        ///     KC.Common.FileHelper.AudioFormat(Basic = 1, Wav = 2, Mpeg = 3, Ram = 4, Rmi = 5, Aif = 6, Unknown = 99)
        ///     KC.Common.FileHelper.VideoFormat(Wmv = 1, Mp4 = 2, Flv = 3, Avi = 4, Mov = 5, Unknown = 99)
        /// </summary>
        [DataMember]
        [MaxLength(50)]
        public string FileFormat { get; set; }
        /// <summary>
        /// 文件名称
        /// </summary>
        [DataMember]
        [MaxLength(512)]
        public string FileName { get; set; }

        /// <summary>
        /// 文件大小
        /// </summary>
        [DataMember]
        public long Size { get; set; }

        /// <summary>
        /// 扩展名
        /// </summary>
        [DataMember]
        [MaxLength(20)]
        public string Ext { get; set; }

        [DataMember]
        public bool IsSuccess { get; set; }

        [DataMember]
        public string ErrorLog { get; set; }
    }
}
