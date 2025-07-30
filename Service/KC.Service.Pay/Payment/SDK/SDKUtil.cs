using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using KC.Common;
using KC.Framework.Base;
using KC.Framework.Tenant;

namespace KC.Service.Pay.SDK
{
    public class SDKUtil
    {
        /// <summary>
        /// 签名
        /// </summary>
        /// <param name="data"></param>
        /// <param name="encoder"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public static bool Sign(Tenant tenant, string userId, Dictionary<string, string> data, Encoding encoder, ConfigStatus state)
        {
            //设置签名证书序列号 ？

            data["certId"] = CertUtil.GetSignCertId(tenant, userId, state);

            //将Dictionary信息转换成key1=value1&key2=value2的形式
            string stringData = CoverDictionaryToString(data);

            string stringSign = null;

            byte[] signDigest = SecurityUtil.Sha1X16(stringData, encoder);

            string stringSignDigest = BitConverter.ToString(signDigest).Replace("-", "").ToLower();

            byte[] byteSign = SecurityUtil.SignBySoft(CertUtil.GetSignProviderFromPfx(tenant, userId, state), encoder.GetBytes(stringSignDigest));

            stringSign = Convert.ToBase64String(byteSign);

            //设置签名域值

            data["signature"] = stringSign;

            return true;//
        }

        /// <summary>
        /// 验证签名
        /// </summary>
        /// <param name="data"></param>
        /// <param name="encoder"></param>
        /// <returns></returns>
        public static ValidateCertResult Validate(Tenant tenant, string userId,string encryptCertName, Dictionary<string, string> data, Encoding encoder, ConfigStatus state)
        {
            //获取签名
            string signValue = data["signature"];
            byte[] signByte = Convert.FromBase64String(signValue);
            data.Remove("signature");
            string stringData = CoverDictionaryToString(data);
            byte[] signDigest = SecurityUtil.Sha1X16(stringData, encoder);
            string stringSignDigest = BitConverter.ToString(signDigest).Replace("-", "").ToLower();
            RSACryptoServiceProvider provider = CertUtil.GetValidateProviderFromPath(tenant, userId, encryptCertName, data["certId"], state);
            if (null == provider)
            {
                return ValidateCertResult.NotFoundCert;
            }
            if (SecurityUtil.ValidateBySoft(provider, signByte, encoder.GetBytes(stringSignDigest)))
                return ValidateCertResult.Success;
            return ValidateCertResult.Falid;
        }


        /// <summary>
        /// 将Dictionary内容排序后输出为键值对字符串
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string CoverDictionaryToString(Dictionary<string, string> data)
        {
            //如果不加stringComparer.Ordinal，排序方式和java treemap有差异
            var treeMap = new SortedDictionary<string, string>(StringComparer.Ordinal);

            foreach (KeyValuePair<string, string> kvp in data)
            {
                treeMap.Add(kvp.Key, kvp.Value);
            }

            var builder = new StringBuilder();

            foreach (KeyValuePair<string, string> element in treeMap)
            {
                builder.Append(element.Key + "=" + element.Value + "&");
            }

            return builder.ToString().Substring(0, builder.Length - 1);
        }

        /// <summary>
        /// 将字符串key1=value1&key2=value2转换为Dictionary数据结构
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static Dictionary<string, string> CoverstringToDictionary(string data)
        {
            if (string.IsNullOrWhiteSpace(data))
            {
                return null;
            }
            string[] arrray = data.Split(new[] { '&' });
            var res = new Dictionary<string, string>();
            foreach (string s in arrray)
            {
                int n = s.IndexOf("=", StringComparison.CurrentCulture);
                string key = s.Substring(0, n);
                string value = s.Substring(n + 1);
                res.Add(key, value);
            }
            return res;
        }


        public static string CreateAutoSubmitForm(string url, Dictionary<string, string> data, Encoding encoder)
        {
            var html = new StringBuilder();
            html.AppendLine("<html>");
            html.AppendLine("<head>");
            html.AppendFormat("<meta http-equiv=\"Content-Type\" content=\"text/html; charset={0}\" />", encoder.BodyName);
            html.AppendLine("</head>");
            html.AppendLine("<body onload=\"OnLoadSubmit();\">");
            html.AppendFormat("<form id=\"pay_form\" action=\"{0}\" method=\"post\">", url);
            foreach (KeyValuePair<string, string> kvp in data)
            {
                html.AppendFormat("<input type=\"hidden\" name=\"{0}\" id=\"{0}\" value=\"{1}\" />", kvp.Key, kvp.Value);
            }
            html.AppendLine("</form>");
            html.AppendLine("<script type=\"text/javascript\">");
            html.AppendLine("<!--");
            html.AppendLine("function OnLoadSubmit()");
            html.AppendLine("{");
            html.AppendLine("document.getElementById(\"pay_form\").submit();");
            html.AppendLine("}");
            html.AppendLine("//-->");
            html.AppendLine("</script>");
            html.AppendLine("</body>");
            html.AppendLine("</html>");
            return html.ToString();
        }

        /// <summary>
        /// 将Dictionary内容排序后输出为键值对字符串,供打印报文使用
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string PrintDictionaryToString(Dictionary<string, string> data)
        {
            ////如果不加stringComparer.Ordinal，排序方式和java treemap有差异
            //var treeMap = new SortedDictionary<string, string>(StringComparer.Ordinal);

            //foreach (KeyValuePair<string, string> kvp in data)
            //{
            //    treeMap.Add(kvp.Key, kvp.Value);
            //}

            //var builder = new StringBuilder();
            //foreach (KeyValuePair<string, string> element in treeMap)
            //{
            //    builder.Append(element.Key + "=" + element.Value + "&");
            //}

            var builder = new StringBuilder();
            foreach (KeyValuePair<string, string> element in data)
            {
                builder.Append(element.Key + "=" + element.Value + "&");
            }

            return builder.ToString().Substring(0, builder.Length - 1);
        }


        /// <summary>
        /// pinblock 16进制计算
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>

        public static string PrintHexString(byte[] b)
        {
            var sb = new StringBuilder();

            for (int i = 0; i < b.Length; i++)
            {
                string hex = Convert.ToString(b[i] & 0xFF, 16);
                if (hex.Length == 1)
                {
                    hex = '0' + hex;
                }
                sb.Append("0x");
                sb.Append(hex + " ");
            }
            sb.Append("");
            return sb.ToString();
        }


        //public sealed class X509Certificate2
        //{
        //    private static X509Certificate2 pc;

        //    /// <summary>
        //    /// 构造方法私有，外键不能通过New类实例化此类
        //    /// </summary>
        //    private X509Certificate2()
        //    {
        //    }
        //    /// <summary>
        //    /// 此方法是本类实例的唯一全局访问点           
        //    /// <returns></returns>
        //    public static X509Certificate2 GetInstance()
        //    {
        //        //如实例不存在，则New一个新实例，否则返回已有实例
        //        if (pc == null)
        //        {
        //            pc = new X509Certificate2();
        //        }
        //        return pc;
        //    }
        //}

        /// <summary>
        /// 密码pinblock加密
        /// </summary>
        /// <param name="card"></param>
        /// <param name="pwd"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string EncryptPin(string card, string pwd, string encoding)
        {
            /** 生成PIN Block **/
            byte[] pinBlock = SecurityUtil.Pin2PinBlockWithCardNO(pwd, card);
            PrintHexString(pinBlock);

            var pc = new X509Certificate2(SDKConfig.EncryptCert);

            var p = new RSACryptoServiceProvider();

            p = (RSACryptoServiceProvider)pc.PublicKey.Key;

            byte[] enBytes = p.Encrypt(pinBlock, false);

            return Convert.ToBase64String(enBytes);

            // return SecurityUtil.EncryptPin(pwd, card, encoding);
        }

        /// <summary>
        /// 数据加密
        /// </summary>
        /// <param name="data"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string EncryptData(string data, string encoding)
        {
            var pc = new X509Certificate2(SDKConfig.EncryptCert);

            var p = new RSACryptoServiceProvider();

            p = (RSACryptoServiceProvider)pc.PublicKey.Key;

            byte[] enBytes = p.Encrypt(Encoding.UTF8.GetBytes(data), false);

            return Convert.ToBase64String(enBytes);
        }



        /// <summary>
        /// 将Dictionary中的数据转换成key1=value1&key2=value2的形式 不包含签名域signature
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string CustomerInfo(Dictionary<string, string> data)
        {
            var str = new StringBuilder();

            foreach (string key in data.Keys)
            {
                if (SDKConstants.param_signature.Equals(key.Trim()))
                {
                    continue;
                }
                str.Append(key + SDKConstants.EQUAL + data[key] + SDKConstants.AMPERSAND);
            }
            return str.ToString().Substring(0, str.Length - 1);

        }

        /// <summary>
        ///  根据获取到的biz文件要求填写 参看数据元相关子域说明，json报文结构
        /// </summary>
        /// 
        /// <returns></returns>
        public static string GetbillDetailInfo()
        {
            var di = new Dictionary<string, object>();
            di.Add("usr_num", "88888");
            di.Add("query_month", "000000");
            string result = SerializeHelper.ToJson(di);
            return result;
        }

        /// <summary>
        /// 组装银行卡验证信息及身份信息(customerInfo):证件类型、证件号码、姓名、手机号、短信验证码、持卡人密码、CVN2、有效期
        /// </summary>
        /// <param name="pParam"></param>
        /// <param name="pan"></param>
        /// <param name="encoding"></param>
        /// <param name="isEncrypt"></param>
        /// <returns></returns>
        public static string GenerateCustomerInfo(Dictionary<string, string> pParam, string pan, string encoding, bool isEncrypt)
        {
            // 设置编码方式
            if (string.IsNullOrWhiteSpace(encoding))
            {
                encoding = SDKConstants.UTF_8_ENCODING;
            }
            // 持卡人身份信息 --证件类型&证件号码&姓名&手机号&短信验证码&持卡人密码&CVN2&有效期
            var sf = new StringBuilder("{");
            if (!string.IsNullOrEmpty(pParam["customerInfo01"]))//
            {
                sf.Append(pParam["customerInfo01"] + SDKConstants.AMPERSAND);
            }
            if (!string.IsNullOrEmpty(pParam["customerInfo02"]))//
            {
                sf.Append(pParam["customerInfo02"] + SDKConstants.AMPERSAND);
            }
            if (!string.IsNullOrEmpty(pParam["customerInfo03"]))//
            {
                sf.Append(pParam["customerInfo03"] + SDKConstants.AMPERSAND);
            }
            if (!string.IsNullOrEmpty(pParam["customerInfo04"]))//
            {
                sf.Append(pParam["customerInfo04"] + SDKConstants.AMPERSAND);
            }
            if (!string.IsNullOrEmpty(pParam["customerInfo05"]))//
            {
                sf.Append(pParam["customerInfo05"] + SDKConstants.AMPERSAND);
            }
            // 密码处理
            if (!string.IsNullOrEmpty(pParam["customerInfo06"]))
            {
                if (!string.IsNullOrEmpty(pan))
                {
                    sf.Append(SDKUtil.EncryptPin(pan.Trim(), pParam["customerInfo06"],
                            encoding)
                            + SDKConstants.AMPERSAND);
                }
                else
                {
                    sf.Append(pParam["customerInfo06"] + SDKConstants.AMPERSAND);
                }

            }
            // CVN2处理
            if (!string.IsNullOrEmpty(pParam["customerInfo07"]))
            {
                if (isEncrypt)
                {
                    sf.Append(SDKUtil.EncryptData(pParam["customerInfo07"], encoding)
                            + SDKConstants.AMPERSAND);
                }
                else
                {
                    sf.Append(pParam["customerInfo07"] + SDKConstants.AMPERSAND);
                }
            }

            // 有效期处理
            if (!string.IsNullOrEmpty(pParam["customerInfo087"]))
            {
                if (isEncrypt)
                {
                    sf.Append(SDKUtil.EncryptData(pParam["customerInfo087"], encoding));
                }
                else
                {
                    sf.Append(pParam["customerInfo087"]);
                }
            }
            sf.Append("}");
            try
            {
                return SecurityUtil.EncodeBase64(Encoding.Default, sf.ToString());
            }

            catch (Exception e)
            {
                Console.WriteLine(e.Message); ;
                return "";
            }
        }


    }

    /// <summary>
    /// 校验返回结果
    /// </summary>
    public enum ValidateCertResult
    {
        /// <summary>
        /// 未找到验证签名的证书
        /// </summary>
        NotFoundCert = 0,
        /// <summary>
        /// 成功
        /// </summary>
        Success = 1,
        /// <summary>
        /// 失败
        /// </summary>
        Falid = 2

    }

}
