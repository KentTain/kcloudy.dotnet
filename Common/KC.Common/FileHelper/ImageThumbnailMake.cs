using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Xml.Linq;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace KC.Common.FileHelper
{
    public enum ThumbnailMode
    {
        /// <summary>
        /// This option will change the target size to match the source image's aspect ratio.
        /// </summary>
        KeepAll,
        /// <summary>
        /// The target size will keep. The source image will draw in specific position in target.
        /// </summary>
        FitTarget,
        /// <summary>
        /// The target size will keep. Fill target image with specific part of source image.
        /// </summary>
        CropSource,
    }
    public enum ThumbnailPosition
    {
        LeftTop,
        LeftCenter,
        LeftBottom,
        CenterTop,
        CenterCenter,
        CenterBottom,
        RightTop,
        RightCenter,
        RightBottom,
    }

    /// <summary>
    ///pic_zip 图片缩略图生成类
    /// </summary>
    public class ImageThumbnailMake
    {
        private static ImageCodecInfo JPEGCodec;
        private static ImageCodecInfo PNGCodec;

        static ImageThumbnailMake()
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
                if (codec.MimeType == "image/jpeg")
                    JPEGCodec = codec;
                else if (codec.MimeType == "image/png")
                    PNGCodec = codec;
        }

        private readonly Image Source;
        private readonly Size SourceSize;

        public ImageThumbnailMake(Image image)
        {
            this.Source = image;
            this.SourceSize = this.Source.Size;
        }
        public ImageThumbnailMake(byte[] imageBytes)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                stream.Write(imageBytes, 0, imageBytes.Length);
                this.Source = Image.FromStream(stream);
                this.SourceSize = this.Source.Size;
            }
        }

        private Rectangle GetTarget(ref int width, ref int height, ThumbnailMode mode, ThumbnailPosition sourcePostion, ThumbnailPosition targetPosition, out Rectangle rectangleSource)
        {
            switch (mode)
            {
                case ThumbnailMode.KeepAll:
                    {
                        rectangleSource = new Rectangle(new Point(), this.SourceSize);
                        if (this.SourceSize.Width * height > width * this.SourceSize.Height)
                        {
                            height = width * this.SourceSize.Height / this.SourceSize.Width;
                        }
                        else
                        {
                            width = this.SourceSize.Width * height / this.SourceSize.Height;
                        }
                        return new Rectangle(0, 0, width, height);
                    }
                case ThumbnailMode.FitTarget:
                    {
                        rectangleSource = new Rectangle(0, 0, this.SourceSize.Width, this.SourceSize.Height);

                        int targetWidth = width;
                        int targetHeight = height;
                        int left = 0;
                        int top = 0;
                        if (this.SourceSize.Width * height > width * this.SourceSize.Height)
                        {
                            targetHeight = this.SourceSize.Height * width / this.SourceSize.Width;
                        }
                        if (this.SourceSize.Width * height < width * this.SourceSize.Height)
                        {
                            targetWidth = this.SourceSize.Width * height / this.SourceSize.Height;
                        }
                        switch (targetPosition)
                        {
                            case ThumbnailPosition.LeftTop: { } break;
                            case ThumbnailPosition.LeftCenter: { top = (height - targetHeight) / 2; } break;
                            case ThumbnailPosition.LeftBottom: { top = height - targetHeight; } break;
                            case ThumbnailPosition.CenterTop: { left = (width - targetWidth) / 2; } break;
                            case ThumbnailPosition.CenterCenter: { left = (width - targetWidth) / 2; top = (height - targetHeight) / 2; } break;
                            case ThumbnailPosition.CenterBottom: { left = (width - targetWidth) / 2; top = height - targetHeight; } break;
                            case ThumbnailPosition.RightTop: { left = width - targetWidth; } break;
                            case ThumbnailPosition.RightCenter: { left = width - targetWidth; top = (height - targetHeight) / 2; } break;
                            case ThumbnailPosition.RightBottom: { left = width - targetWidth; top = height - targetHeight; } break;
                            default: { throw new NotImplementedException(); }
                        }
                        return new Rectangle(left, top, targetWidth, targetHeight);
                    }
                case ThumbnailMode.CropSource:
                    {
                        int sourceWidth = this.SourceSize.Width, cropWidth = sourceWidth;
                        int sourceHeight = this.SourceSize.Height, cropHeight = sourceHeight;
                        int left = 0;
                        int top = 0;
                        if (this.SourceSize.Width * height > width * this.SourceSize.Height)
                        {
                            cropWidth = this.SourceSize.Height * width / height;
                        }
                        if (this.SourceSize.Width * height < width * this.SourceSize.Height)
                        {
                            cropHeight = this.SourceSize.Width * height / width;
                        }
                        switch (sourcePostion)
                        {
                            case ThumbnailPosition.LeftTop: { } break;
                            case ThumbnailPosition.LeftCenter: { top = (sourceHeight - cropHeight) / 2; } break;
                            case ThumbnailPosition.LeftBottom: { top = (sourceHeight - cropHeight); } break;
                            case ThumbnailPosition.CenterTop: { left = (sourceWidth - cropWidth) / 2; } break;
                            case ThumbnailPosition.CenterCenter: { left = (sourceWidth - cropWidth) / 2; top = (sourceHeight - cropHeight) / 2; } break;
                            case ThumbnailPosition.CenterBottom: { left = (sourceWidth - cropWidth) / 2; top = sourceHeight - cropHeight; } break;
                            case ThumbnailPosition.RightTop: { left = (sourceWidth - cropWidth); } break;
                            case ThumbnailPosition.RightCenter: { left = (sourceWidth - cropWidth); top = (sourceHeight - cropHeight) / 2; } break;
                            case ThumbnailPosition.RightBottom: { left = (sourceWidth - cropWidth); top = (sourceHeight - cropHeight); } break;
                            default: { throw new NotImplementedException(); }
                        }
                        rectangleSource = new Rectangle(left, top, cropWidth, cropHeight);
                        return new Rectangle(0, 0, width, height);
                    }
                default: { throw new NotImplementedException(); }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="width">Target size width.</param>
        /// <param name="height">Target size height.</param>
        /// <param name="mode">Thumbnail mode. (KeepAll; FitTarget; CropSource)</param>
        /// <param name="sourcePostion">While mode is CropSource, which part will be keep.</param>
        /// <param name="targetPosition">While mode is FitTarget, what position to put source image.</param>
        /// <returns></returns>
        public Bitmap GetOutput(int width, int height,
            ThumbnailMode mode = ThumbnailMode.KeepAll,
            ThumbnailPosition sourcePostion = ThumbnailPosition.CenterCenter,
            ThumbnailPosition targetPosition = ThumbnailPosition.CenterCenter)
        {
            Rectangle rectangleSource;
            Rectangle targetSize = this.GetTarget(ref width, ref height, mode, sourcePostion, targetPosition, out rectangleSource);

            Bitmap bitmap = new Bitmap(width, height);
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                graphics.Clear(Color.Transparent);
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.DrawImage(this.Source, targetSize, rectangleSource, GraphicsUnit.Pixel);
            }

            return bitmap;
        }

        public byte[] GetOutputBytes(int width, int height, ImageFormat format,
            ThumbnailMode mode = ThumbnailMode.KeepAll,
            ThumbnailPosition sourcePostion = ThumbnailPosition.CenterCenter,
            ThumbnailPosition targetPosition = ThumbnailPosition.CenterCenter)
        {
            byte[] imageBytes = new byte[0];
            Bitmap bitmap = this.GetOutput(width, height, mode, sourcePostion, targetPosition);
            using (MemoryStream stream = new MemoryStream())
            {
                var sourceFormat = this.Source.RawFormat;
                if (ImageFormat.Png.ToString() == sourceFormat.ToString())
                {
                    bitmap.Save(stream, sourceFormat);
                }
                else
                {
                    EncoderParameters encoderParameters = new EncoderParameters(1);
                    encoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Compression, 95L);
                    bitmap.Save(stream, JPEGCodec, encoderParameters);
                }
                imageBytes = stream.ToArray();
            }
            return imageBytes;
        }

    }

}
