using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Web;

namespace KC.Web.Util
{
    public static class FormUtil
    {
        /// <summary>
        /// 获取特定编码页面的Form的相关参数
        /// </summary>
        /// <param name="requestFormBody">请求的Body：this.Request.Body</param>
        /// <param name="name">表单Key</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        public static string GetFormValueByNameWithEncoding(Stream requestFormBody, string name, Encoding encoding)
        {
            var stream = requestFormBody;
            var bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            // 设置当前流的位置为流的开始
            stream.Seek(0, SeekOrigin.Begin);

            var formNameValue = FillFromEncodedBytes(bytes, encoding);
            return formNameValue[name];
        }

        private static NameValueCollection FillFromEncodedBytes(byte[] bytes, Encoding encoding)
        {
            var formCollection = new NameValueCollection();
            int num = (bytes != null) ? bytes.Length : 0;
            for (int i = 0; i < num; i++)
            {
                string str;
                string str2;
                int offset = i;
                int num4 = -1;
                while (i < num)
                {
                    byte num5 = bytes[i];
                    if (num5 == 0x3d)
                    {
                        if (num4 < 0)
                        {
                            num4 = i;
                        }
                    }
                    else if (num5 == 0x26)
                    {
                        break;
                    }
                    i++;
                }
                if (num4 >= 0)
                {
                    str = HttpUtility.UrlDecode(bytes, offset, num4 - offset, encoding);
                    str2 = HttpUtility.UrlDecode(bytes, num4 + 1, (i - num4) - 1, encoding);
                }
                else
                {
                    str = null;
                    str2 = HttpUtility.UrlDecode(bytes, offset, i - offset, encoding);
                }
                formCollection.Add(str, str2);
                if ((i == (num - 1)) && (bytes[i] == 0x26))
                {
                    formCollection.Add(null, string.Empty);
                }
            }

            return formCollection;
        }
    }
}
