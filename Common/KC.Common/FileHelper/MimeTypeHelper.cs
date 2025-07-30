using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Common.FileHelper
{
    public enum FileType : int
    {
        Image = 1,
        Word = 2,
        Excel = 3,
        PDF = 4,
        PPT = 5,
        Text = 6,
        Xml = 7,
        Audio = 8,
        Video = 9,
        Zip = 10,

        Unknown = 99,
    }

    public enum DocFormat : int
    {
        Doc = 1,
        Docx = 2,

        Xls = 3,
        Xlsx = 4,

        Ppt = 5,
        Pptx = 6,

        Pdf = 7,

        Dwg = 9,
        Bmp = 10,
        Gif = 11,
        Icon = 12,
        Jpeg = 13,
        Png = 14,
        Tiff = 15,
        Wmf = 16,

        Text = 17,
        Xml = 18,

        Basic = 19,
        Wav = 20,
        Mpeg = 21, //mp3
        Ram = 22,
        Rmi = 23,
        Aif = 24,

        Wmv = 30,
        Mp4 = 31,
        Flv = 32,
        Avi = 33,
        Mov = 34,

        Rar = 40,
        Zip = 41,
        Gzip = 42,

        Unknown = 99,
    }

    public enum ZipFormat
    {
        Rar = 1,
        Zip = 2,
        Gzip = 3,

        Unknown = 99,
    }

    public enum ImageFormat : int
    {
        Bmp = 1,
        Gif = 2,
        Icon = 3,
        Jpeg = 4,
        Png = 5,
        Tiff = 6,
        Wmf = 7,
        Dwg = 8,

        Unknown = 99,
    }

    public enum AudioFormat : int
    {
        Basic = 1,
        Wav = 2,
        Mpeg = 3, //mp3
        Ram = 4,
        Rmi = 5,
        Aif = 6,

        Unknown = 99,
    }

    public enum VideoFormat : int
    {
        Wmv = 1,
        Mp4 = 2,
        Flv = 3,
        Avi = 4,
        Mov = 5,

        Unknown = 99,
    }

    public static class MimeTypeHelper
    {
        /// <summary>
        /// 根据DocFormat类型，获取Http能解析的MineType
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string GetMineType(DocFormat format)
        {
            string result = string.Empty;
            switch (format)
            {
                case DocFormat.Doc:
                    result = "application/msword";
                    break;
                case DocFormat.Docx:
                    result = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                    break;
                case DocFormat.Xls:
                    result = "application/vnd.ms-excel";
                    break;
                case DocFormat.Xlsx:
                    result = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    break;
                case DocFormat.Ppt:
                    result = "application/vnd.ms-powerpoint";
                    break;
                case DocFormat.Pptx:
                    result = "application/vnd.openxmlformats-officedocument.presentationml.presentation";
                    break;
                case DocFormat.Pdf:
                    result = "application/pdf";
                    break;
                case DocFormat.Text:
                    result = "text/plain";
                    break;
                case DocFormat.Xml:
                    result = "text/xml";
                    break;

                //图片
                case DocFormat.Bmp:
                    result = "image/bmp";
                    break;
                case DocFormat.Gif:
                    result = "image/gif";
                    break;
                case DocFormat.Icon:
                    result = "image/vnd.microsoft.icon";
                    break;
                case DocFormat.Jpeg:
                    result = "image/jpeg";
                    break;
                case DocFormat.Png:
                    result = "image/png";
                    break;
                case DocFormat.Tiff:
                    result = "image/tiff";
                    break;
                case DocFormat.Wmf:
                    result = "image/wmf";
                    break;
                case DocFormat.Dwg:
                    result = "image/vnd.dwg";
                    break;

                //压缩
                case DocFormat.Rar:
                    result = "application/x-rar-compressed";
                    break;
                case DocFormat.Zip:
                    result = "application/zip";
                    break;
                case DocFormat.Gzip:
                    result = "application/x-gzip";
                    break;

                //语音
                case DocFormat.Basic:
                    result = "audio/basic";
                    break;
                case DocFormat.Wav:
                    result = "audio/x-wav";
                    break;
                case DocFormat.Mpeg:
                    result = "audio/mpeg";
                    break;
                case DocFormat.Ram:
                    result = "audio/x-pn-realaudio";
                    break;
                case DocFormat.Rmi:
                    result = "audio/mid";
                    break;
                case DocFormat.Aif:
                    result = "audio/x-aiff";
                    break;

                // 视频
                case DocFormat.Wmv:
                    result = "video/x-ms-wmv";
                    break;
                case DocFormat.Mp4:
                    result = "video/mp4";
                    break;
                case DocFormat.Flv:
                    result = "video/x-flv";
                    break;
                case DocFormat.Avi:
                    result = "video/x-msvideo";
                    break;
                case DocFormat.Mov:
                    result = "video/quicktime";
                    break;

                default:
                    result = "application/unknown";
                    break;
            }

            return result;
        }
        /// <summary>
        /// 根据ImageFormat类型，获取Http能解析的MineType
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string GetImageMineType(ImageFormat format)
        {
            string result = string.Empty;
            switch (format)
            {
                case ImageFormat.Bmp:
                    result = "image/bmp";
                    break;
                case ImageFormat.Gif:
                    result = "image/gif";
                    break;
                case ImageFormat.Icon:
                    result = "image/vnd.microsoft.icon";
                    break;
                case ImageFormat.Jpeg:
                    result = "image/jpeg";
                    break;
                case ImageFormat.Png:
                    result = "image/png";
                    break;
                case ImageFormat.Tiff:
                    result = "image/tiff";
                    break;
                case ImageFormat.Wmf:
                    result = "image/wmf";
                    break;
                case ImageFormat.Dwg:
                    result = "image/vnd.dwg";
                    break;
                default:
                    result = "application/unknown";
                    break;
            }

            return result;
        }
        /// <summary>
        /// 根据AudioFormat类型，获取Http能解析的MineType
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string GetAudioMineType(AudioFormat format)
        {
            string result = string.Empty;
            switch (format)
            {
                //语音
                case AudioFormat.Basic:
                    result = "audio/basic";
                    break;
                case AudioFormat.Wav:
                    result = "audio/x-wav";
                    break;
                case AudioFormat.Mpeg:
                    result = "audio/mpeg";
                    break;
                case AudioFormat.Ram:
                    result = "audio/x-pn-realaudio";
                    break;
                case AudioFormat.Rmi:
                    result = "audio/mid";
                    break;
                case AudioFormat.Aif:
                    result = "audio/x-aiff";
                    break;
                default:
                    result = "application/unknown";
                    break;
            }

            return result;
        }
        /// <summary>
        /// 根据VideoFormat类型，获取Http能解析的MineType
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string GetVideoMineType(VideoFormat format)
        {
            String result = "";
            switch (format)
            {
                // 视频
                case VideoFormat.Wmv:
                    result = "video/x-ms-wmv";
                    break;
                case VideoFormat.Mp4:
                    result = "video/mp4";
                    break;
                case VideoFormat.Flv:
                    result = "video/x-flv";
                    break;
                case VideoFormat.Avi:
                    result = "video/x-msvideo";
                    break;
                case VideoFormat.Mov:
                    result = "video/quicktime";
                    break;
                default:
                    result = "application/unknown";
                    break;
            }

            return result;
        }

        /// <summary>
        /// 根据ContentType类型，获取ImageFormat
        /// </summary>
        /// <param name="contentType"></param>
        /// <returns></returns>
        public static ImageFormat GetImageFormatByContentType(string contentType)
        {
            ImageFormat result = ImageFormat.Unknown;
            switch (contentType.ToLower())
            {
                case "image/bmp":
                    result = ImageFormat.Bmp;
                    break;
                case "image/gif":
                    result = ImageFormat.Gif;
                    break;
                case "image/vnd.microsoft.icon":
                    result = ImageFormat.Icon;
                    break;
                case "image/jpeg":
                    result = ImageFormat.Jpeg;
                    break;
                case "image/png":
                    result = ImageFormat.Png;
                    break;
                case "image/tiff":
                    result = ImageFormat.Tiff;
                    break;
                case "image/wmf":
                    result = ImageFormat.Wmf;
                    break;
                case "image/vnd.dwg":
                    result = ImageFormat.Dwg;
                    break;
                default:
                    result = ImageFormat.Unknown;
                    break;
            }

            return result;
        }
        /// <summary>
        /// 根据文件扩展名，获取ImageFormat
        /// </summary>
        /// <param name="extensionStr"></param>
        /// <returns></returns>
        public static ImageFormat GetImageFormatByExt(string extensionStr)
        {
            string[] Image = new string[] { "jpg", "jpeg", "png", "gif", "bmp", "JPEG2000", "TIFF", "PSD", "PNG", "SVG", "PCX", "DXF", "WMF", "DWG" };
            ImageFormat result = ImageFormat.Unknown;
            switch (extensionStr.ToLower())
            {
                case "bmp":
                    result = ImageFormat.Bmp;
                    break;
                case "gif":
                    result = ImageFormat.Gif;
                    break;
                case "jpg":
                case "jpeg":
                case "JPEG2000":
                    result = ImageFormat.Jpeg;
                    break;
                case "png":
                    result = ImageFormat.Png;
                    break;
                case "TIFF":
                    result = ImageFormat.Tiff;
                    break;
                case "wmf":
                    result = ImageFormat.Wmf;
                    break;
                case "dwg":
                    result = ImageFormat.Dwg;
                    break;
                default:
                    result = ImageFormat.Unknown;
                    break;
            }

            return result;
        }

        /// <summary>
        /// 根据ContentType类型，获取DocFormat
        /// </summary>
        /// <param name="contentType"></param>
        /// <returns></returns>
        public static DocFormat GetDocFormatByContentType(string contentType)
        {
            DocFormat result = DocFormat.Unknown;
            switch (contentType.ToLower())
            {
                case "application/msword":
                    result = DocFormat.Doc;
                    break;
                case "application/vnd.openxmlformats-officedocument.wordprocessingml.document":
                    result = DocFormat.Docx;
                    break;
                case "application/vnd.ms-excel":
                    result = DocFormat.Xls;
                    break;
                case "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet":
                    result = DocFormat.Xlsx;
                    break;
                case "application/vnd.ms-powerpoint":
                    result = DocFormat.Ppt;
                    break;
                case "application/vnd.openxmlformats-officedocument.presentationml.presentation":
                    result = DocFormat.Pptx;
                    break;
                case "application/pdf":
                    result = DocFormat.Pdf;
                    break;
                case "image/bmp":
                    result = DocFormat.Bmp;
                    break;
                case "image/gif":
                    result = DocFormat.Gif;
                    break;
                case "image/vnd.microsoft.icon":
                    result = DocFormat.Icon;
                    break;
                case "image/jpeg":
                    result = DocFormat.Jpeg;
                    break;
                case "image/png":
                    result = DocFormat.Png;
                    break;
                case "image/tiff":
                    result = DocFormat.Tiff;
                    break;
                case "image/wmf":
                    result = DocFormat.Wmf;
                    break;
                case "image/vnd.dwg":
                    result = DocFormat.Dwg;
                    break;
                case "text/plain":
                    result = DocFormat.Text;
                    break;
                case "text/xml":
                    result = DocFormat.Xml;
                    break;

                case "application/x-rar-compressed":
                    result = DocFormat.Rar;
                    break;
                case "application/zip":
                    result = DocFormat.Zip;
                    break;
                case "application/x-gzip":
                    result = DocFormat.Gzip;
                    break;
                default:
                    result = DocFormat.Unknown;
                    break;
            }
            return result;
        }
        /// <summary>
        /// 根据文件扩展名，获取DocFormat
        /// </summary>
        /// <param name="extensionStr"></param>
        /// <returns></returns>
        public static DocFormat GetDocFormatByExt(string extensionStr)
        {
            DocFormat result = DocFormat.Unknown;
            switch (extensionStr.ToLower())
            {
                case "doc":
                    result = DocFormat.Doc;
                    break;
                case "docx":
                    result = DocFormat.Docx;
                    break;
                case "xls":
                    result = DocFormat.Xls;
                    break;
                case "xlsx":
                    result = DocFormat.Xlsx;
                    break;
                case "ppt":
                    result = DocFormat.Ppt;
                    break;
                case "pptx":
                    result = DocFormat.Pptx;
                    break;
                case "pdf":
                    result = DocFormat.Pdf;
                    break;
                case "bmp":
                    result = DocFormat.Bmp;
                    break;
                case "gif":
                    result = DocFormat.Gif;
                    break;
                case "icon":
                    result = DocFormat.Icon;
                    break;
                case "jpg":
                case "jpeg":
                    result = DocFormat.Jpeg;
                    break;
                case "png":
                    result = DocFormat.Png;
                    break;
                case "tiff":
                    result = DocFormat.Tiff;
                    break;
                case "wmf":
                    result = DocFormat.Wmf;
                    break;
                case "dwg":
                    result = DocFormat.Dwg;
                    break;
                case "txt":
                    result = DocFormat.Text;
                    break;
                case "xml":
                    result = DocFormat.Xml;
                    break;
                case "rar":
                    result = DocFormat.Rar;
                    break;
                case "zip":
                    result = DocFormat.Zip;
                    break;
                case "gzip":
                    result = DocFormat.Gzip;
                    break;
                default:
                    result = DocFormat.Unknown;
                    break;
            }
            return result;
        }

        /// <summary>
        /// 根据ContentType类型，获取AudioFormat
        /// </summary>
        /// <param name="contentType"></param>
        /// <returns></returns>
        public static AudioFormat GetAudioFormatByContentType(string contentType)
        {
            AudioFormat result = AudioFormat.Unknown;
            switch (contentType.ToLower())
            {
                case "audio/basic":
                    result = AudioFormat.Basic;
                    break;
                case "audio/x-wav":
                    result = AudioFormat.Wav;
                    break;
                case "audio/mpeg":
                    result = AudioFormat.Mpeg;
                    break;
                case "audio/x-pn-realaudio":
                    result = AudioFormat.Ram;
                    break;
                case "audio/mid":
                    result = AudioFormat.Rmi;
                    break;
                case "audio/x-aiff":
                    result = AudioFormat.Aif;
                    break;
                default:
                    result = AudioFormat.Unknown;
                    break;
            }

            return result;
        }
        /// <summary>
        /// 根据文件扩展名，获取AudioFormat
        /// </summary>
        /// <param name="extensionStr"></param>
        /// <returns></returns>
        public static AudioFormat GetAudioFormatByExt(string extensionStr)
        {
            string[] Audio = new string[] { "au", "wav", "ram", "rmi", "mp3", "aif" };
            AudioFormat result = AudioFormat.Unknown;
            switch (extensionStr.ToLower())
            {
                case "au":
                case "snd":
                    result = AudioFormat.Basic;
                    break;
                case "wav":
                    result = AudioFormat.Wav;
                    break;
                case "ram":
                case "ra":
                    result = AudioFormat.Ram;
                    break;
                case "rmi":
                case "mid":
                    result = AudioFormat.Rmi;
                    break;
                case "mp3":
                    result = AudioFormat.Mpeg;
                    break;
                case "aif":
                case "aifc":
                case "aiff":
                    result = AudioFormat.Aif;
                    break;
                default:
                    result = AudioFormat.Unknown;
                    break;
            }

            return result;
        }
        /// <summary>
        /// 根据ContentType类型，获取VideoFormat
        /// </summary>
        /// <param name="contentType"></param>
        /// <returns></returns>
        public static VideoFormat GetVideoFormatByContentType(string contentType)
        {
            VideoFormat result = VideoFormat.Unknown;
            switch (contentType.ToLower())
            {
                case "video/x-ms-wmv":
                    result = VideoFormat.Wmv;
                    break;
                case "video/quicktime":
                    result = VideoFormat.Mov;
                    break;
                case "video/mp4":
                    result = VideoFormat.Mp4;
                    break;
                case "video/x-msvideo":
                    result = VideoFormat.Avi;
                    break;
                case "video/x-flv":
                    result = VideoFormat.Flv;
                    break;
                default:
                    result = VideoFormat.Unknown;
                    break;
            }

            return result;
        }
        /// <summary>
        /// 根据文件扩展名，获取VideoFormat
        /// </summary>
        /// <param name="extensionStr"></param>
        /// <returns></returns>
        public static VideoFormat GetVideoFormatByExt(string extensionStr)
        {
            // String[] Audio = new String[] { "au", "wav", "ram", "rmi", "mp3", "aif" };
            VideoFormat result = VideoFormat.Unknown;
            switch (extensionStr.ToLower())
            {
                case "flv":
                    result = VideoFormat.Flv;
                    break;
                case "mp4":
                    result = VideoFormat.Mp4;
                    break;
                case "mov":
                case "ra":
                    result = VideoFormat.Mov;
                    break;
                case "avi":
                case "mid":
                    result = VideoFormat.Avi;
                    break;
                case "wmv":
                    result = VideoFormat.Wmv;
                    break;
                default:
                    result = VideoFormat.Unknown;
                    break;
            }

            return result;
        }
        public static FileType GetFileTypeByContentType(string contentType)
        {
            FileType result = FileType.Unknown;
            switch (contentType.ToLower())
            {
                case "application/msword":
                case "application/vnd.openxmlformats-officedocument.wordprocessingml.document":
                    result = FileType.Word;
                    break;
                case "application/vnd.ms-excel":
                    result = FileType.Excel;
                    break;
                case "application/vnd.ms-powerpoint":
                case "application/vnd.openxmlformats-officedocument.presentationml.presentation":
                    result = FileType.PPT;
                    break;
                case "application/pdf":
                    result = FileType.PDF;
                    break;
                case "application/zip":
                case "application/x-gzip":
                case "application/x-rar-compressed":
                    result = FileType.Zip;
                    break;
                case "image/bmp":
                case "image/gif":
                case "image/vnd.microsoft.icon":
                case "image/jpeg":
                case "image/png":
                case "image/tiff":
                case "image/wmf":
                case "image/vnd.dwg":
                    result = FileType.Image;
                    break;
                case "text/plain":
                    result = FileType.Text;
                    break;
                case "text/xml":
                    result = FileType.Xml;
                    break;
                case "audio/basic":
                case "audio/x-wav":
                case "audio/mpeg":
                case "audio/x-pn-realaudio":
                case "audio/mid":
                case "audio/x-aiff":
                    result = FileType.Audio;
                    break;
                default:
                    result = FileType.Unknown;
                    break;
            }

            return result;
        }
        public static FileType GetFileType(string extensionStr)
        {
            string[] Image = new string[] { "jpg", "jpeg", "png", "gif", "bmp", "JPEG2000", "TIFF", "PSD", "PNG", "SVG", "PCX", "DXF", "WMF", "DWG" };
            string[] Word = new string[] { "doc", "docx", "rtf" };
            string[] Excel = new string[] { "xls", "xlsx", "xlsm", "xltm" };
            string[] PDF = new string[] { "pdf" };
            string[] PPT = new string[] { "ppt", "pptx", "ppsx", "ppsm" };
            string[] Text = new string[] { "txt" };
            string[] Xml = new string[] { "xml" };
            string[] Zip = new string[] { "zip", "rar", "gzip" };
            string[] Audio = new string[] { "au", "wav", "ram", "rmi", "mp3", "aif" };
            string[] Video = new string[] { "wmv", "mp4", "avi", "flv", "mov" };
            if (Image.Contains(extensionStr))
            {
                return FileType.Image;
            }
            if (Word.Contains(extensionStr))
            {
                return FileType.Word;
            }
            if (Excel.Contains(extensionStr))
            {
                return FileType.Excel;
            }
            if (PDF.Contains(extensionStr))
            {
                return FileType.PDF;
            }
            if (PPT.Contains(extensionStr))
            {
                return FileType.PPT;
            }
            if (Text.Contains(extensionStr))
            {
                return FileType.Text;
            }
            if (Xml.Contains(extensionStr))
            {
                return FileType.Xml;
            }
            if (Audio.Contains(extensionStr))
            {
                return FileType.Audio;
            }
            if (Video.Contains(extensionStr))
            {
                return FileType.Video;
            }
            if (Zip.Contains(extensionStr))
            {
                return FileType.Zip;
            }
            return FileType.Unknown;
        }

        public static string GetFileFormatByExt(string extensionStr)
        {
            var type = GetFileType(extensionStr);
            switch (type)
            {
                case FileType.Image:
                    return GetImageFormatByExt(extensionStr).ToString();
                case FileType.Audio:
                    return GetAudioFormatByExt(extensionStr).ToString();
                case FileType.Video:
                    return GetVideoFormatByExt(extensionStr).ToString();
                case FileType.Word:
                case FileType.Excel:
                case FileType.PDF:
                case FileType.PPT:
                case FileType.Text:
                case FileType.Xml:
                case FileType.Zip:
                    return GetDocFormatByExt(extensionStr).ToString();
                default:
                    return "Unknown";
            }
        }
        public static string GetFileFormatByContentType(string contentType)
        {
            var type = GetFileTypeByContentType(contentType);
            switch (type)
            {
                case FileType.Image:
                    return GetImageFormatByContentType(contentType).ToString();
                case FileType.Audio:
                    return GetAudioFormatByContentType(contentType).ToString();
                case FileType.Video:
                    return GetVideoFormatByContentType(contentType).ToString();
                case FileType.Word:
                case FileType.Excel:
                case FileType.PDF:
                case FileType.PPT:
                case FileType.Text:
                case FileType.Xml:
                case FileType.Zip:
                    return GetDocFormatByContentType(contentType).ToString();
                default:
                    return "Unknown";
            }
        }
    }
}
