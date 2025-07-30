using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using iTextSharp.text.pdf;

namespace KC.ThirdParty
{
    public static class WatermarkHelper
    {
        /// <summary>
        /// 通过文字添加到Image文档
        /// </summary>
        /// <param name="inputfilepath">原始图片的物理路径</param>
        /// <param name="watermarkfilepath">水印图片的物理路径</param>
        /// <param name="watermarkText">水印文字（不显示水印文字设为空串）</param>
        /// <param name="outputfilepath">输出合成后的图片的物理路径</param>
        public static void AddWatermarkToImage(string inputfilepath, string outputfilepath, string watermarkfilepath, string watermarkText)
        {
            //以下（代码）从一个指定文件创建了一个Image 对象，然后为它的 Width 和 Height定义变量。
            //这些长度待会被用来建立一个以24 bits 每像素的格式作为颜色数据的Bitmap对象。
            Image imgPhoto = Image.FromFile(inputfilepath);
            int phWidth = imgPhoto.Width;
            int phHeight = imgPhoto.Height;
            Bitmap bmPhoto = new Bitmap(phWidth, phHeight, PixelFormat.Format24bppRgb);
            bmPhoto.SetResolution(72, 72);
            Graphics grPhoto = Graphics.FromImage(bmPhoto);

            //这个代码以100%它的原始大小绘制imgPhoto 到Graphics 对象的（x=0,y=0）位置。
            //以后所有的绘图都将发生在原来照片的顶部。
            grPhoto.SmoothingMode = SmoothingMode.AntiAlias;
            grPhoto.DrawImage(
                 imgPhoto,
                 new Rectangle(0, 0, phWidth, phHeight),
                 0,
                 0,
                 phWidth,
                 phHeight,
                 GraphicsUnit.Pixel);

            // 添加文字水印
            if (!string.IsNullOrEmpty(watermarkText))
            {
                //为了最大化版权信息的大小，我们将测试7种不同的字体大小来决定我们能为我们的照片宽度使用的可能的最大大小。
                //为了有效地完成这个，我们将定义一个整型数组，接着遍历这些整型值测量不同大小的版权字符串。
                //一旦我们决定了可能的最大大小，我们就退出循环，绘制文本
                int[] sizes = new int[] { 16, 14, 12, 10, 8, 6, 4 };
                Font crFont = null;
                SizeF crSize = new SizeF();
                for (int i = 0; i < 7; i++)
                {
                    crFont = new Font("arial", sizes[i],
                          FontStyle.Bold);
                    crSize = grPhoto.MeasureString(watermarkText,
                          crFont);
                    if ((ushort)crSize.Width < (ushort)phWidth)
                        break;
                }
                //因为所有的照片都有各种各样的高度，所以就决定了从图象底部开始的5%的位置开始。
                //使用rMarkText字符串的高度来决定绘制字符串合适的Y坐标轴。
                //通过计算图像的中心来决定X轴，然后定义一个StringFormat 对象，设置StringAlignment 为Center。
                int yPixlesFromBottom = (int)(phHeight * .05);
                float yPosFromBottom = ((phHeight -
                     yPixlesFromBottom) - (crSize.Height / 2));
                float xCenterOfImg = (phWidth / 2);
                StringFormat StrFormat = new StringFormat();
                StrFormat.Alignment = StringAlignment.Center;
                //现在我们已经有了所有所需的位置坐标来使用60%黑色的一个Color(alpha值153)创建一个SolidBrush 。
                //在偏离右边1像素，底部1像素的合适位置绘制版权字符串。
                //这段偏离将用来创建阴影效果。使用Brush重复这样一个过程，在前一个绘制的文本顶部绘制同样的文本。
                SolidBrush semiTransBrush2 =
                     new SolidBrush(Color.FromArgb(153, 0, 0, 0));
                grPhoto.DrawString(watermarkText,
                     crFont,
                     semiTransBrush2,
                     new PointF(xCenterOfImg + 1, yPosFromBottom + 1),
                     StrFormat);
                SolidBrush semiTransBrush = new SolidBrush(
                     Color.FromArgb(153, 255, 255, 255));
                grPhoto.DrawString(watermarkText,
                     crFont,
                     semiTransBrush,
                     new PointF(xCenterOfImg, yPosFromBottom),
                     StrFormat);
            }

            //根据前面修改后的照片创建一个Bitmap。把这个Bitmap载入到一个新的Graphic对象。
            Bitmap bmWatermark = new Bitmap(bmPhoto);
            bmWatermark.SetResolution(
                 imgPhoto.HorizontalResolution,
                 imgPhoto.VerticalResolution);
            Graphics grWatermark = Graphics.FromImage(bmWatermark);

            Image imgWatermark = null;
            if (!string.IsNullOrEmpty(watermarkfilepath))
            {
                int wmWidth = 0;
                int wmHeight = 0;
                int markWidth = 0;
                int markHeight = 0;
                //这个代码载入水印图片，水印图片已经被保存为一个BMP文件，以绿色(A=0,R=0,G=255,B=0)作为背景颜色。
                //再一次，会为它的Width 和Height定义一个变量。
                imgWatermark = new Bitmap(watermarkfilepath);
                wmWidth = imgWatermark.Width;
                wmHeight = imgWatermark.Height;
                //这个代码以100%它的原始大小绘制imgPhoto 到Graphics 对象的（x=0,y=0）位置。
                //以后所有的绘图都将发生在原来照片的顶部。
                grPhoto.SmoothingMode = SmoothingMode.AntiAlias;
                grPhoto.DrawImage(
                     imgPhoto,
                     new Rectangle(0, 0, phWidth, phHeight),
                     0,
                     0,
                     phWidth,
                     phHeight,
                     GraphicsUnit.Pixel);

                //随着两个颜色处理加入到imageAttributes 对象，我们现在就能在照片右手边上绘制水印了。
                //我们会偏离10像素到底部，10像素到左边。

                //mark比原来的图宽
                if (phWidth <= wmWidth)
                {
                    markWidth = phWidth - 10;
                    markHeight = (markWidth * wmHeight) / wmWidth;
                }
                else if (phHeight <= wmHeight)
                {
                    markHeight = phHeight - 10;
                    markWidth = (markHeight * wmWidth) / wmHeight;
                }
                else
                {
                    markWidth = wmWidth;
                    markHeight = wmHeight;
                }

                #region 定义一个ImageAttributes
                //通过定义一个ImageAttributes 对象并设置它的两个属性，我们就是实现了两个颜色的处理，以达到半透明的水印效果。
                //处理水印图象的第一步是把背景图案变为透明的(Alpha=0, R=0, G=0, B=0)。我们使用一个Colormap 和定义一个RemapTable来做这个。
                //就像前面展示的，我的水印被定义为100%绿色背景，我们将搜到这个颜色，然后取代为透明。
                var imageAttributes = new ImageAttributes();
                var colorMap = new ColorMap();
                colorMap.OldColor = Color.FromArgb(255, 0, 255, 0);
                colorMap.NewColor = Color.FromArgb(0, 0, 0, 0);
                ColorMap[] remapTable = { colorMap };
                //第二个颜色处理用来改变水印的不透明性。
                //通过应用包含提供了坐标的RGBA空间的5x5矩阵来做这个。
                //通过设定第三行、第三列为0.3f我们就达到了一个不透明的水平。结果是水印会轻微地显示在图象底下一些。
                imageAttributes.SetRemapTable(remapTable, ColorAdjustType.Bitmap);
                float[][] colorMatrixElements = {
                                             new float[] {1.0f,  0.0f,  0.0f,  0.0f, 0.0f},
                                             new float[] {0.0f,  1.0f,  0.0f,  0.0f, 0.0f},
                                             new float[] {0.0f,  0.0f,  1.0f,  0.0f, 0.0f},
                                             new float[] {0.0f,  0.0f,  0.0f,  0.3f, 0.0f},
                                             new float[] {0.0f,  0.0f,  0.0f,  0.0f, 1.0f}
                                        };
                var wmColorMatrix = new ColorMatrix(colorMatrixElements);
                imageAttributes.SetColorMatrix(wmColorMatrix,
                     ColorMatrixFlag.Default,
                     ColorAdjustType.Bitmap);
                #endregion

                int xPosOfWm = ((phWidth - markWidth) - 10);
                int yPosOfWm = 10;
                grWatermark.DrawImage(imgWatermark,
                     new Rectangle(xPosOfWm, yPosOfWm, markWidth,
                     markHeight),
                     0,
                     0,
                     wmWidth,
                     wmHeight,
                     GraphicsUnit.Pixel,
                     imageAttributes);
            }

            //最后的步骤将是使用新的Bitmap取代原来的Image。 销毁两个Graphic对象，然后把Image 保存到文件系统。
            imgPhoto = bmWatermark;
            grPhoto.Dispose();
            grWatermark.Dispose();
            imgPhoto.Save(outputfilepath, System.Drawing.Imaging.ImageFormat.Jpeg);
            imgPhoto.Dispose();
            if (imgWatermark != null)
            {
                imgWatermark.Dispose();
            }
        }

        ///// <summary>
        ///// 通过文字添加到Word文档，使用：Spire.Doc.
        ///// </summary>
        ///// <param name="inputfilepath"></param>
        ///// <param name="outputfilepath"></param>
        ///// <param name="watermarkText"></param>
        //public static void AddWatermarkToWord(string inputfilepath, string outputfilepath, string watermarkText)
        //{
        //    var document = new Spire.Doc.Document();
        //    document.LoadFromFile(inputfilepath);

        //    //Insert TextWatermark
        //    var txtWatermark = new Spire.Doc.TextWatermark();
        //    txtWatermark.Text = watermarkText;
        //    txtWatermark.Color = Color.DeepSkyBlue;
        //    txtWatermark.FontSize = 45;
        //    txtWatermark.Layout = Spire.Doc.Documents.WatermarkLayout.Diagonal;
        //    document.Watermark = txtWatermark;

        //    //Save and Launch
        //    document.SaveToFile(outputfilepath, Spire.Doc.FileFormat.Docx);
        //    //System.Diagnostics.Process.Start(outputfilepath);
        //}

        ///// <summary>
        ///// 通过文字添加到Pdf文档，使用：Spire.Pdf
        ///// </summary>
        ///// <param name="inputfilepath"></param>
        ///// <param name="outputfilepath"></param>
        ///// <param name="watermarkText"></param>
        //public static void AddWatermarkToPdf1(string inputfilepath, string outputfilepath, string watermarkText)
        //{
        //    var document = new Spire.Pdf.PdfDocument();
        //    document.LoadFromFile(inputfilepath);

        //    foreach (Spire.Pdf.PdfPageBase page in document.Pages)
        //    {
        //        var brush = new Spire.Pdf.Graphics.PdfTilingBrush(new SizeF(page.Canvas.ClientSize.Width / 2, page.Canvas.ClientSize.Height / 3));
        //        brush.Graphics.SetTransparency(0.3f);
        //        brush.Graphics.Save();
        //        brush.Graphics.TranslateTransform(brush.Size.Width / 2, brush.Size.Height / 2);
        //        brush.Graphics.RotateTransform(-45);
        //        brush.Graphics.DrawString(watermarkText,
        //            new Spire.Pdf.Graphics.PdfFont(Spire.Pdf.Graphics.PdfFontFamily.Helvetica, 24), Spire.Pdf.Graphics.PdfBrushes.Violet, 0, 0,
        //            new Spire.Pdf.Graphics.PdfStringFormat(Spire.Pdf.Graphics.PdfTextAlignment.Center));
        //        brush.Graphics.Restore();
        //        brush.Graphics.SetTransparency(1);
        //        page.Canvas.DrawRectangle(brush, new RectangleF(new PointF(0, 0), page.Canvas.ClientSize));
        //    }

        //    document.SaveToFile(outputfilepath);
        //}

        /// <summary>
        /// 通过文字添加到Pdf文档，使用：iTextSharp.text.pdf
        /// </summary>
        /// <param name="inputStream"></param>
        /// <param name="outputStream"></param>
        /// <param name="watermarkText"></param>
        public static byte[] AddWatermarkToPdf(MemoryStream inputStream, string watermarkText)
        {
            PdfReader pdfReader = null;
            //PdfStamper pdfStamper = null;
            try
            {
                pdfReader = new PdfReader(inputStream);
                using (MemoryStream ms = new MemoryStream())
                {
                    //using (PdfStamper stamper = new PdfStamper(pdfReader, ms, '\0', true))
                    {
                        PdfStamper stamper = new PdfStamper(pdfReader, ms, '\0', true);
                        int total = pdfReader.NumberOfPages + 1;
                        PdfContentByte content;
                        PdfGState gs = new PdfGState();
                        for (int i = 1; i < total; i++)
                        {
                            content = stamper.GetOverContent(i);//在内容上方加水印
                            //content = stamper.GetUnderContent(i);//在内容下方加水印
                            //透明度
                            gs.FillOpacity = 0.5f;
                            content.SetGState(gs);
                            //开始写入文本
                            content.BeginText();
                            content.SetColorFill(iTextSharp.text.BaseColor.DarkGray);
                            if (File.Exists(FontPath))
                                content.SetFontAndSize(BaseFont.CreateFont(FontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED), 12);
                            else
                                content.SetFontAndSize(BaseFont.CreateFont(ArialFontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED), 12);
                            content.SetTextMatrix(0, 0);
                            content.ShowTextAligned(iTextSharp.text.Element.ALIGN_LEFT, watermarkText, 0, 2, 0);
                            content.EndText();
                        }

                    }
                    return ms.ToArray();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {

                if (pdfReader != null)
                    pdfReader.Close();
            }
        }

        public static string FontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "SIMSUN.TTC,0");
        public static string ArialFontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "SIMSUN.TTC,0");
        /// <summary>
        /// 通过文字添加到Pdf文档，使用：iTextSharp.text.pdf
        /// </summary>
        /// <param name="inputfilepath"></param>
        /// <param name="outputfilepath"></param>
        /// <param name="watermarkText"></param>
        public static void AddWatermarkToPdf(string inputfilepath, string outputfilepath, string watermarkText)
        {
            PdfReader pdfReader = null;
            PdfStamper pdfStamper = null;
            try
            {
                pdfReader = new PdfReader(inputfilepath);
                pdfStamper = new PdfStamper(pdfReader, new FileStream(outputfilepath, FileMode.Create));
                int total = pdfReader.NumberOfPages + 1;
                iTextSharp.text.Rectangle psize = pdfReader.GetPageSize(1);
                float width = psize.Width;
                float height = psize.Height;
                PdfContentByte content;

                PdfGState gs = new PdfGState();
                for (int i = 1; i < total; i++)
                {
                    content = pdfStamper.GetOverContent(i);//在内容上方加水印
                    //content = pdfStamper.GetUnderContent(i);//在内容下方加水印
                    //透明度
                    gs.FillOpacity = 0.3f;
                    content.SetGState(gs);
                    //content.SetGrayFill(0.3f);
                    //开始写入文本
                    content.BeginText();
                    content.SetColorFill(iTextSharp.text.BaseColor.LightGray);
                    if (File.Exists(FontPath))
                        content.SetFontAndSize(BaseFont.CreateFont(FontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED), 100);
                    else
                        content.SetFontAndSize(BaseFont.CreateFont(ArialFontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED), 100);
                    content.SetTextMatrix(0, 0);
                    content.ShowTextAligned(iTextSharp.text.Element.ALIGN_CENTER, watermarkText, width / 2 - 50, height / 2 - 50, 55);
                    //content.SetColorFill(BaseColor.BLACK);
                    //content.SetFontAndSize(font, 8);
                    //content.ShowTextAligned(Element.ALIGN_CENTER, waterMarkName, 0, 0, 0);
                    content.EndText();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (pdfStamper != null)
                    pdfStamper.Close();

                if (pdfReader != null)
                    pdfReader.Close();
            }
        }

        /// <summary>
        /// 通过水印图片添加到Pdf文档，使用：iTextSharp.text.pdf
        /// </summary>
        /// <param name="inputfilepath"></param>
        /// <param name="outputfilepath"></param>
        /// <param name="watermarkfilepath"></param>
        /// <returns></returns>
        public static bool AddWatermarkPictureToPdf(string inputfilepath, string outputfilepath, string watermarkfilepath)
        {
            float top = 10;
            float left = 20;

            //throw new NotImplementedException();
            PdfReader pdfReader = null;
            PdfStamper pdfStamper = null;
            try
            {
                pdfReader = new PdfReader(inputfilepath);
                int numberOfPages = pdfReader.NumberOfPages;
                iTextSharp.text.Rectangle psize = pdfReader.GetPageSize(1);
                float width = psize.Width;
                float height = psize.Height;
                pdfStamper = new PdfStamper(pdfReader, new FileStream(outputfilepath, FileMode.Create));

                PdfContentByte waterMarkContent;
                iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(watermarkfilepath);

                image.GrayFill = 10;//透明度，灰色填充
                //image.Rotation//旋转
                //image.RotationDegrees//旋转角度
                //水印的位置
                if (left < 0)
                {
                    left = width / 2 - image.Width + left;
                }
                //image.SetAbsolutePosition(left, (height - image.Height) - top);
                image.SetAbsolutePosition(left, (height / 2 - image.Height) - top);

                //每一页加水印,也可以设置某一页加水印
                for (int i = 1; i <= numberOfPages; i++)
                {
                    waterMarkContent = pdfStamper.GetUnderContent(i);//内容下层加水印
                    //waterMarkContent = pdfStamper.GetOverContent(i);//内容上层加水印

                    waterMarkContent.AddImage(image);
                }
                //strMsg = "success";
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (pdfStamper != null)
                    pdfStamper.Close();

                if (pdfReader != null)
                    pdfReader.Close();
            }
        }

        public static void AddFullWaterTextToImage(string oldPath, string newPath, string text, string watermarkFilePath = null)
        {
            try
            {
                var im = Image.FromFile(oldPath);
                var image = new Bitmap(im);
                if (!string.IsNullOrWhiteSpace(watermarkFilePath) && File.Exists(watermarkFilePath))
                {
                    var wImage = Image.FromFile(watermarkFilePath);
                    DrawImage(image, wImage, 1F, image.Width - wImage.Width - 50, image.Height - wImage.Height - 20);
                    wImage.Dispose();
                }
                if (!string.IsNullOrWhiteSpace(text))
                {
                    var font = new Font("宋体", 18, FontStyle.Bold);
                    var fontColor = Color.Black;
                    var textWatermarkResult = GetTextWatermark(text, image, font, fontColor);
                    Image textWatermark = textWatermarkResult.Item2;

                    var rowCount = image.Width / textWatermarkResult.Item1.Item1 + 1;
                    var rowLen = image.Height / textWatermarkResult.Item1.Item2 + 1;
                    if (textWatermarkResult.Item1.Item1 * 2 > image.Width &&
                        textWatermarkResult.Item1.Item2 * 2 > image.Height)
                    {
                        DrawImage(image, textWatermark, 0.2F, (image.Width - textWatermarkResult.Item1.Item1) / 2,
                                (image.Height - textWatermarkResult.Item1.Item2) / 2);
                    }
                    else if (textWatermarkResult.Item1.Item2 * 2 > image.Height)
                    {
                        for (var i = 0; i < rowCount; i++)
                        {
                            DrawImage(image, textWatermark, 0.2F, textWatermarkResult.Item1.Item1 * i,
                                (image.Height - textWatermarkResult.Item1.Item2) / 2);
                        }
                    }
                    else if (textWatermarkResult.Item1.Item1 * 2 > image.Width)
                    {
                        for (var j = 0; j < rowLen; j++)
                        {
                            DrawImage(image, textWatermark, 0.2F, (image.Width - textWatermarkResult.Item1.Item1) / 2,
                                textWatermarkResult.Item1.Item2 * j);
                        }
                    }
                    else
                    {
                        for (var i = 0; i < rowCount; i++)
                        {
                            for (var j = 0; j < rowLen; j++)
                            {
                                DrawImage(image, textWatermark, 0.2F, textWatermarkResult.Item1.Item1 * i, textWatermarkResult.Item1.Item2 * j);
                            }
                        }
                    }
                    textWatermark.Dispose();
                }
                image.Save(newPath, System.Drawing.Imaging.ImageFormat.Jpeg);
                image.Dispose();
                im.Dispose();
            }
            catch (Exception)
            {
                newPath = oldPath;
            }
        }

        private static Tuple<Tuple<int, int>, Image> GetTextWatermark(string text, Image image, Font font, Color fontColor)
        {
            Brush brush = new SolidBrush(fontColor);
            SizeF size;

            using (Graphics g = Graphics.FromImage(image))
            {
                size = g.MeasureString(text, font);
            }

            var width = (int)(size.Width);
            var height = (int)(size.Width);
            var marginTop = height / 2F;
            Bitmap bitmap = new Bitmap(width, height);
            bitmap.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (Graphics g = Graphics.FromImage(bitmap))
            {
                if (width < image.Height)
                {
                    g.TranslateTransform(-size.Width / 4, size.Width / 2);
                    g.RotateTransform(-45);
                }
                else
                {
                    marginTop = (image.Height - size.Height) / 2;
                }

                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixel;
                g.Clear(Color.White);
                g.DrawString(text, font, brush, 0, marginTop);
            }

            return new Tuple<Tuple<int, int>, Image>(new Tuple<int, int>(width, height), bitmap);
        }

        private static Image GetWatermarkImage(Image watermark)
        {
            int newWidth = watermark.Width;
            int newHeight = watermark.Height;

            Rectangle sourceRect = new Rectangle(0, 0, newWidth, newHeight);
            Rectangle destRect = new Rectangle(0, 0, watermark.Width, watermark.Height);

            Bitmap bitmap = new Bitmap(newWidth, newHeight);
            bitmap.SetResolution(watermark.HorizontalResolution, watermark.VerticalResolution);

            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.DrawImage(watermark, sourceRect, destRect, GraphicsUnit.Pixel);
            }

            return bitmap;
        }

        public static void DrawImage(Bitmap image, Image watermark, float opacity = 1, int x = 0, int y = 0)
        {
            if (watermark == null)
                throw new ArgumentOutOfRangeException("Watermark");

            var mWatermark = GetWatermarkImage(watermark);

            Point waterPos = new Point(x, y);

            Rectangle destRect = new Rectangle(waterPos.X, waterPos.Y, mWatermark.Width, mWatermark.Height);

            ColorMatrix colorMatrix = new ColorMatrix(
                new float[][] {
                    new float[] { 1, 0f, 0f, 0f, 0f},
                    new float[] { 0f, 1, 0f, 0f, 0f},
                    new float[] { 0f, 0f, 1, 0f, 0f},
                    new float[] { 0f, 0f, 0f, opacity, 0f},
                    new float[] { 0f, 0f, 0f, 0f, 1}
                });

            ImageAttributes attributes = new ImageAttributes();

            attributes.SetColorMatrix(colorMatrix);
            attributes.SetColorKey(Color.Transparent, Color.Transparent);

            using (Graphics gr = Graphics.FromImage(image))
            {
                gr.DrawImage(mWatermark, destRect, 0, 0, mWatermark.Width, mWatermark.Height, GraphicsUnit.Pixel, attributes);
            }
        }
    }
}