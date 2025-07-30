using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Common.FileHelper
{
    public class ImageHelper
    {
        /// <summary>
        /// 图片缩略图生成算法
        /// </summary>
        /// <param name="int_Width">宽度</param>
        /// <param name="int_Height">高度</param>
        /// <param name="input_ImgFile">文件路径</param>
        /// <param name="out_ImgFile">保存文件路径</param>
        /// <param name="filename">文件名</param>
        /// <returns></returns>
        public static bool MakeThumbnail(int int_Width, int int_Height, string input_ImgFile, string out_ImgFile, string filename)
        {
            System.Drawing.Image oldimage = System.Drawing.Image.FromFile(input_ImgFile + filename);
            float New_Width; // 新的宽度    
            float New_Height; // 新的高度    
            float Old_Width, Old_Height; //原始高宽    
            int flat = 0;//标记图片是不是等比    


            int xPoint = 0;//若果要补白边的话，原图像所在的x，y坐标。    
            int yPoint = 0;
            //判断图片    

            Old_Width = (float)oldimage.Width;
            Old_Height = (float)oldimage.Height;

            if ((Old_Width / Old_Height) > ((float)int_Width / (float)int_Height)) //当图片太宽的时候    
            {
                New_Height = Old_Height * ((float)int_Width / (float)Old_Width);
                New_Width = (float)int_Width;
                //此时x坐标不用修改    
                yPoint = (int)(((float)int_Height - New_Height) / 2);
                flat = 1;
            }
            else if ((oldimage.Width / oldimage.Height) == ((float)int_Width / (float)int_Height))
            {
                New_Width = int_Width;
                New_Height = int_Height;
            }
            else
            {
                New_Width = (int)oldimage.Width * ((float)int_Height / (float)oldimage.Height);  //太高的时候   
                New_Height = int_Height;
                //此时y坐标不用修改    
                xPoint = (int)(((float)int_Width - New_Width) / 2);
                flat = 1;
            }

            // System.Drawing.Image.GetThumbnailImageAbort callb = null;
            // ＝＝＝缩小图片＝＝＝    
            //调用缩放算法
            System.Drawing.Image thumbnailImage = MakeSmallImage(oldimage, (int)New_Width, (int)New_Height);
            Bitmap bm = new Bitmap(thumbnailImage);

            if (flat != 0)
            {
                Bitmap bmOutput = new Bitmap(int_Width, int_Height);
                Graphics gc = Graphics.FromImage(bmOutput);
                SolidBrush tbBg = new SolidBrush(Color.White);
                gc.FillRectangle(tbBg, 0, 0, int_Width, int_Height); //填充为白色    


                gc.DrawImage(bm, xPoint, yPoint, (int)New_Width, (int)New_Height);
                bmOutput.Save(out_ImgFile + filename);
            }
            else
            {
                bm.Save(out_ImgFile + filename);
            }
            oldimage.Dispose();
            return true;
        }

        /// <summary>
        /// 生成缩略图 (高清缩放)
        /// </summary>
        /// <param name="originalImage">原图片</param>
        /// <param name="width">缩放宽度</param>
        /// <param name="height">缩放高度</param>
        /// <returns></returns>
        public static Image MakeSmallImage(System.Drawing.Image originalImage, int width, int height)
        {

            int towidth = 0;
            int toheight = 0;
            if (originalImage.Width > width && originalImage.Height < height)
            {
                towidth = width;
                toheight = originalImage.Height;
            }

            if (originalImage.Width < width && originalImage.Height > height)
            {
                towidth = originalImage.Width;
                toheight = height;
            }
            if (originalImage.Width > width && originalImage.Height > height)
            {
                towidth = width;
                toheight = height;
            }
            if (originalImage.Width < width && originalImage.Height < height)
            {
                towidth = originalImage.Width;
                toheight = originalImage.Height;
            }
            int x = 0;//左上角的x坐标 
            int y = 0;//左上角的y坐标 


            //新建一个bmp图片 
            System.Drawing.Image bitmap = new System.Drawing.Bitmap(towidth, toheight);

            //新建一个画板 
            Graphics g = System.Drawing.Graphics.FromImage(bitmap);

            //设置高质量插值法 
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;

            //设置高质量,低速度呈现平滑程度 
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            //清空画布并以透明背景色填充 
            g.Clear(Color.Transparent);

            //在指定位置并且按指定大小绘制原图片的指定部分 
            g.DrawImage(originalImage, x, y, towidth, toheight);
            originalImage.Dispose();
            //bitmap.Dispose();
            g.Dispose();
            return bitmap;


        }

        /// <summary> 
        /// 生成缩略图 (没有补白)
        /// </summary> 
        /// <param name="originalImagePath">源图路径（物理路径）</param> 
        /// <param name="thumbnailPath">缩略图路径（物理路径）</param> 
        /// <param name="width">缩略图宽度</param> 
        /// <param name="height">缩略图高度</param>   
        public static void MakeThumbnail(string originalImagePath, string thumbnailPath, int width, int height)
        {
            System.Drawing.Image originalImage = System.Drawing.Image.FromFile(originalImagePath);

            int towidth = 0;
            int toheight = 0;
            if (originalImage.Width > width && originalImage.Height < height)
            {
                towidth = width;
                toheight = originalImage.Height;
            }

            if (originalImage.Width < width && originalImage.Height > height)
            {
                towidth = originalImage.Width;
                toheight = height;
            }
            if (originalImage.Width > width && originalImage.Height > height)
            {
                towidth = width;
                toheight = height;
            }
            if (originalImage.Width < width && originalImage.Height < height)
            {
                towidth = originalImage.Width;
                toheight = originalImage.Height;
            }
            int x = 0;//左上角的x坐标 
            int y = 0;//左上角的y坐标 


            //新建一个bmp图片 
            System.Drawing.Image bitmap = new System.Drawing.Bitmap(towidth, toheight);

            //新建一个画板 
            Graphics g = System.Drawing.Graphics.FromImage(bitmap);

            //设置高质量插值法 
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;

            //设置高质量,低速度呈现平滑程度 
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            //清空画布并以透明背景色填充 
            g.Clear(Color.Transparent);

            //在指定位置并且按指定大小绘制原图片的指定部分 
            g.DrawImage(originalImage, x, y, towidth, toheight);

            try
            {

                bitmap.Save(thumbnailPath);
            }
            catch (System.Exception e)
            {
                throw e;
            }
            finally
            {
                originalImage.Dispose();
                bitmap.Dispose();
                g.Dispose();
            }
        }

        #region 合并用户QR图片和用户头像
        /// <summary>
        /// 合并用户QR图片和用户头像
        /// </summary>
        /// <param name="qrImg">QR图片</param>
        /// <param name="headerImg">用户头像</param>
        /// <param name="n">缩放比例</param>
        /// <returns></returns>
        public static Bitmap MergeQrImg(Bitmap qrImg, Bitmap headerImg, double n = 0.23)
        {
            int margin = 10;
            float dpix = qrImg.HorizontalResolution;
            float dpiy = qrImg.VerticalResolution;
            var _newWidth = (10 * qrImg.Width - 46 * margin) * 1.0f / 46;
            var _headerImg = ZoomPic(headerImg, _newWidth / headerImg.Width);
            //处理头像
            int newImgWidth = _headerImg.Width + margin;
            Bitmap headerBgImg = new Bitmap(newImgWidth, newImgWidth);
            headerBgImg.MakeTransparent();
            Graphics g = Graphics.FromImage(headerBgImg);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g.Clear(Color.Transparent);
            Pen p = new Pen(new SolidBrush(Color.White));
            Rectangle rect = new Rectangle(0, 0, newImgWidth - 1, newImgWidth - 1);
            using (GraphicsPath path = CreateRoundedRectanglePath(rect, 7))
            {
                g.DrawPath(p, path);
                g.FillPath(new SolidBrush(Color.White), path);
            }
            //画头像
            Bitmap img1 = new Bitmap(_headerImg.Width, _headerImg.Width);
            Graphics g1 = Graphics.FromImage(img1);
            g1.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g1.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g1.Clear(Color.Transparent);
            Pen p1 = new Pen(new SolidBrush(Color.Gray));
            Rectangle rect1 = new Rectangle(0, 0, _headerImg.Width - 1, _headerImg.Width - 1);
            using (GraphicsPath path1 = CreateRoundedRectanglePath(rect1, 7))
            {
                g1.DrawPath(p1, path1);
                TextureBrush brush = new TextureBrush(_headerImg);
                g1.FillPath(brush, path1);
            }
            g1.Dispose();
            PointF center = new PointF((newImgWidth - _headerImg.Width) / 2, (newImgWidth - _headerImg.Height) / 2);
            g.DrawImage(img1, center.X, center.Y, _headerImg.Width, _headerImg.Height);
            g.Dispose();
            Bitmap backgroudImg = new Bitmap(qrImg.Width, qrImg.Height);
            backgroudImg.MakeTransparent();
            backgroudImg.SetResolution(dpix, dpiy);
            headerBgImg.SetResolution(dpix, dpiy);
            Graphics g2 = Graphics.FromImage(backgroudImg);
            g2.Clear(Color.Transparent);
            g2.DrawImage(qrImg, 0, 0);
            PointF center2 = new PointF((qrImg.Width - headerBgImg.Width) / 2, (qrImg.Height - headerBgImg.Height) / 2);
            g2.DrawImage(headerBgImg, center2);
            g2.Dispose();
            return backgroudImg;
        }
        #endregion

        #region 图形处理
        /// <summary>
        /// 创建圆角矩形
        /// </summary>
        /// <param name="rect">区域</param>
        /// <param name="cornerRadius">圆角角度</param>
        /// <returns></returns>
        private static GraphicsPath CreateRoundedRectanglePath(Rectangle rect, int cornerRadius)
        {
            //下午重新整理下，圆角矩形
            GraphicsPath roundedRect = new GraphicsPath();
            roundedRect.AddArc(rect.X, rect.Y, cornerRadius * 2, cornerRadius * 2, 180, 90);
            roundedRect.AddLine(rect.X + cornerRadius, rect.Y, rect.Right - cornerRadius * 2, rect.Y);
            roundedRect.AddArc(rect.X + rect.Width - cornerRadius * 2, rect.Y, cornerRadius * 2, cornerRadius * 2, 270, 90);
            roundedRect.AddLine(rect.Right, rect.Y + cornerRadius * 2, rect.Right, rect.Y + rect.Height - cornerRadius * 2);
            roundedRect.AddArc(rect.X + rect.Width - cornerRadius * 2, rect.Y + rect.Height - cornerRadius * 2, cornerRadius * 2, cornerRadius * 2, 0, 90);
            roundedRect.AddLine(rect.Right - cornerRadius * 2, rect.Bottom, rect.X + cornerRadius * 2, rect.Bottom);
            roundedRect.AddArc(rect.X, rect.Bottom - cornerRadius * 2, cornerRadius * 2, cornerRadius * 2, 90, 90);
            roundedRect.AddLine(rect.X, rect.Bottom - cornerRadius * 2, rect.X, rect.Y + cornerRadius * 2);
            roundedRect.CloseFigure();
            return roundedRect;
        }
        /// <summary>
        /// 图片按比例缩放
        /// </summary>
        private static Image ZoomPic(Image initImage, double n)
        {
            //缩略图宽、高计算
            double newWidth = initImage.Width;
            double newHeight = initImage.Height;
            newWidth = n * initImage.Width;
            newHeight = n * initImage.Height;
            //生成新图
            //新建一个bmp图片
            System.Drawing.Image newImage = new System.Drawing.Bitmap((int)newWidth, (int)newHeight);
            //新建一个画板
            System.Drawing.Graphics newG = System.Drawing.Graphics.FromImage(newImage);
            //设置质量
            newG.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            newG.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            //置背景色
            newG.Clear(Color.Transparent);
            //画图
            newG.DrawImage(initImage, new System.Drawing.Rectangle(0, 0, newImage.Width, newImage.Height), new System.Drawing.Rectangle(0, 0, initImage.Width, initImage.Height), System.Drawing.GraphicsUnit.Pixel);
            newG.Dispose();
            return newImage;
        }

        #endregion
    }
}
