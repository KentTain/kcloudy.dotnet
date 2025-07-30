using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using KC.Common.FileHelper;
using Microsoft.AspNetCore.Mvc;

namespace KC.Web.Result
{
    public class ImageResult : ActionResult
    {
        public ImageFormat ContentType { get; set; }
        public string SourceName { get; set; }
        public byte[] ImageBytes { get; set; }
        public ImageResult(byte[] imageBytes, string fileFormat)
        {
            this.ImageBytes = imageBytes;
            var eFormat = ImageFormat.Unknown;
            bool isSuccess = Enum.TryParse(fileFormat, out eFormat);
            this.ContentType = isSuccess 
                ? MimeTypeHelper.GetImageFormatByContentType(fileFormat) 
                : eFormat;
        }

        public ImageResult(byte[] imageBytes, ImageFormat contentType)
        {
            this.ImageBytes = imageBytes;
            this.ContentType = contentType;
        }

        public ImageResult(string sourceName, ImageFormat contentType)
        {
            this.SourceName = sourceName;
            this.ContentType = contentType;
        }

        public override void ExecuteResult(ActionContext context)
        {
            var response = context.HttpContext.Response;
            //response.Clear();
            response.ContentType = MimeTypeHelper.GetImageMineType(ContentType);

            if (ImageBytes != null)
            {
                //var stream = new MemoryStream(ImageBytes);
                //stream.WriteTo(response.Body);
                //stream.Dispose();

                response.Body.WriteAsync(ImageBytes);
            }
            else
            {
                //context.HttpContext.Response.TransmitFile(SourceName);
            }
        }
    }
}
