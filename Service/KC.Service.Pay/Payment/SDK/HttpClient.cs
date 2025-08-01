﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace KC.Service.Pay.SDK
{
    public class HttpClient
    {

        // 请求的URL
        private readonly string _requestUrl = "";

        // 返回结果
        private string _result;

        public string Result
        {
            get { return _result; }
            set { _result = value; }
        }

        public HttpClient(string url)
        {
            _requestUrl = url;
        }

        /// <summary>
        /// 建立请求，以模拟远程HTTP的POST请求方式构造并获取银联的处理结果
        /// </summary>
        /// <param name="sParaTemp">请求参数数组</param>
        /// <param name="encoder"></param>
        /// <returns>银联处理结果</returns>
        public int Send(Dictionary<string, string> sParaTemp, Encoding encoder)
        {
            // System.Net.ServicePointManager.Expect100Continue = false;
            //待请求参数数组字符串
            string strRequestData = BuildRequestParaToString(sParaTemp, encoder);
            //把数组转换成流中所需字节数组类型
            byte[] bytesRequestData = encoder.GetBytes(strRequestData);
            HttpWebResponse httpWResp = null;
            try
            {
                System.Net.ServicePointManager.ServerCertificateValidationCallback =
                    new RemoteCertificateValidationCallback(CheckValidationResult);
                //设置HttpWebRequest基本信息
                var myReq = (HttpWebRequest)HttpWebRequest.Create(_requestUrl);
                myReq.Method = "post";
                myReq.ContentType = "application/x-www-form-urlencoded";
                //填充POST数据
                myReq.ContentLength = bytesRequestData.Length;

                Stream requestStream = myReq.GetRequestStream(); //获得请求流
                requestStream.Write(bytesRequestData, 0, bytesRequestData.Length);
                requestStream.Close();
                //发送POST数据请求服务器                
                httpWResp = (HttpWebResponse)myReq.GetResponse();
                Stream myStream = httpWResp.GetResponseStream();
                if (myStream != null)
                {
                    //获取服务器返回信息
                    var reader = new StreamReader(myStream, encoder);
                    var responseData = new StringBuilder();
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        responseData.Append(line);
                    }
                    //释放
                    myStream.Close();
                    _result = responseData.ToString();
                }
            }
            catch (Exception exp)
            {
                _result = "报错：" + exp.Message;
            }
            if (httpWResp != null)
                return (int)httpWResp.StatusCode;
            return 500;
        }

        /// <summary>
        /// 生成要请求给银联的参数数组
        /// </summary>
        /// <param name="sParaTemp">请求前的参数数组</param>
        /// <param name="code">字符编码</param>
        /// <returns>要请求的参数数组字符串</returns>
        private static string BuildRequestParaToString(Dictionary<string, string> sParaTemp, Encoding code)
        {

            //把参数组中所有元素，按照“参数=参数值”的模式用“&”字符拼接成字符串，并对参数值做urlencode
            string strRequestData = CreateLinkstringUrlencode(sParaTemp, code);
            return strRequestData;
        }

        /// <summary>
        /// 把数组所有元素，按照“参数=参数值”的模式用“&”字符拼接成字符串，并对参数值做urlencode
        /// </summary>
        /// <param name="dicArray">需要拼接的数组</param>
        /// <param name="code">字符编码</param>
        /// <returns>拼接完成以后的字符串</returns>
        public static string CreateLinkstringUrlencode(Dictionary<string, string> dicArray, Encoding code)
        {
            StringBuilder prestr = new StringBuilder();
            foreach (KeyValuePair<string, string> temp in dicArray)
            {
                prestr.Append(temp.Key + "=" + HttpUtility.UrlEncode(temp.Value, code) + "&");
            }

            //去掉最後一個&字符
            int nLen = prestr.Length;
            prestr.Remove(nLen - 1, 1);

            return prestr.ToString();
        }

        public bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain,
            SslPolicyErrors errors)
        {
            // 总是接受  
            return true;
        }
    }
}