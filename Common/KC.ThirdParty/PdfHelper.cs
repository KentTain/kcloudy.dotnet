using System;
using System.IO;
using System.Text;
using SelectPdf;
using KC.Framework.Extension;
using KC.Framework.Util;

namespace KC.ThirdParty
{
    public class PdfHelper
    {
        /// <summary>
        /// 將Html文字 输出到PDF文件
        /// </summary>
        /// <param name="htmlText">html文本</param>
        /// <returns></returns>
        public static byte[] ConvertHtmlTextToPdf(string htmlText)
        {
            PdfDocument doc = null;
            try
            {
                //避免当htmlText无任何html tag标签的纯文字时，转PDF时会挂掉，所以一律加上<p>标签
                htmlText = "<p>" + htmlText + "</p>";

                // instantiate a html to pdf converter object
                HtmlToPdf converter = new HtmlToPdf();
                // create a new pdf document converting an url
                doc = converter.ConvertHtmlString(htmlText);

                var bytes = doc.Save();
                //回传PDF
                return bytes;
            }
            catch (Exception ex)
            {
                LogUtil.LogException(ex);
                return null;
            }
            finally
            {
                if (doc != null)
                    doc.Close();
            }
        }

        public static void ConvertHtmlTextToPdfFile(string htmlText, string filePath)
        {
            if (string.IsNullOrEmpty(htmlText))
            {
                return;
            }

            PdfDocument doc = null;
            try
            {
                //避免当htmlText无任何html tag标签的纯文字时，转PDF时会挂掉，所以一律加上<p>标签
                htmlText = "<p>" + htmlText + "</p>";

                // instantiate a html to pdf converter object
                HtmlToPdf converter = new HtmlToPdf();
                // create a new pdf document converting an url
                doc = converter.ConvertHtmlString(htmlText);

                doc.Save(filePath);
            }
            catch (Exception ex)
            {
                LogUtil.LogException(ex);
            }
            finally
            {
                if (doc != null)
                    doc.Close();
            }

        }

        /// <summary>
        /// 根据Url地址，保存为pdf的byte[]
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static byte[] ConvertUrlToPdf(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return null;
            }

            if(!url.IsUrl())
            {
                return null;
            }

            PdfDocument doc = null;
            try
            {
                // instantiate a html to pdf converter object
                HtmlToPdf converter = new HtmlToPdf();

                // create a new pdf document converting an url
                doc = converter.ConvertHtmlString(url);

                var bytes = doc.Save();
                doc.Close();
                //回传PDF
                return bytes;
            }
            catch (Exception ex)
            {
                LogUtil.LogException(ex);
                return null;
            }
            finally
            {
                if (doc != null)
                    doc.Close();
            }

            
        }

        /// <summary>
        /// 根据Url地址，保存为pdf的byte[]
        /// </summary>
        /// <param name="url">可访问的URL路径</param>
        /// <param name="filePath">保存Pdf文件路径</param>
        /// <returns></returns>
        public static void ConvertUrlToPdfFile(string url, string filePath)
        {
            if (string.IsNullOrEmpty(url) || !url.IsUrl())
            {
                return;
            }

            PdfDocument doc = null;
            try
            {
                // instantiate a html to pdf converter object
                HtmlToPdf converter = new HtmlToPdf();

                // create a new pdf document converting an url
                doc = converter.ConvertHtmlString(url);

                doc.Save(filePath);
            }
            catch (Exception ex)
            {
                LogUtil.LogException(ex);
            }
            finally
            {
                if (doc != null)
                    doc.Close();
            }
        }
    }

}
