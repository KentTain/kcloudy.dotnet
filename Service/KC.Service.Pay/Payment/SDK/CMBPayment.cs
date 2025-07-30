using KC.Framework.SecurityHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CFW.PaymentPortal.SDK
{
    public class CMBPayment
    {
        static string CRC32_PASSWORD = "CMBChina2009";
        static string CRC32_PREFIX = "Z";

        public static string GetCheckSumWithCRC32(String key, String xmlData)
        {
            string str = CRC32_PASSWORD + key + xmlData.Replace("\n", "").Replace("\r", "");
            string result = Convert.ToString(CRC32.GetCRC32(str), 16).ToUpper();
            string pattern = "00000000";
            return CRC32_PREFIX + pattern.Substring(0, pattern.Length - result.Length) + result;
        }
    }
}