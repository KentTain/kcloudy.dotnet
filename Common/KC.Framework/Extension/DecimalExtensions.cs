using System;

namespace KC.Framework.Extension
{
    public static class DecimalExtensions
    {
        /// <summary>
        /// 将金额值除以10000
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public static decimal GetValueDividedByTenThousand(this decimal amount)
        {
            return (amount / 10000M);
        }

        /// <summary>
        /// 将金额值除以10000并转换成￥00.00
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public static string GetStrDividedByTenThousand(this decimal amount)
        {
            return (amount / 10000M).ToString("c2");
        }
        /// <summary>
        /// 将金额值除以10000并转换成string，保留四位小数
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public static string GetStrDividedByTenThousand2(this decimal amount)
        {
            return (amount / 10000M).ToString("c4");
        }
        /// <summary>
        /// 将金额值除以10000并转换成xxxx万元
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public static string GetStrWithUnitTenThousand(this decimal amount)
        {
            return string.Format("{0}万元", GetStrDividedByTenThousand(amount));
        }

        /// <summary>
        /// 将小数转换成百分比00.00%,保留两位小数
        /// </summary>
        /// <param name="percent"></param>
        /// <returns></returns>
        public static string GetPercentStr(this decimal percent)
        {
            return (percent * 100M).ToString("F2") + "%";
        }


        /// <summary>
        /// 将金额值乘以10000
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public static decimal GetValueMultiplyTenThousand(this decimal amount)
        {
            return amount * 10000M;
        }

        /// <summary>
        /// 将金额值乘以10000并转换成￥00.00
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public static string GetStrMultiplyTenThousand(this decimal amount)
        {
            return (amount * 10000M).ToString("c2");
        }

        /// <summary> 
        /// 将小数值按指定的小数位数截断 
        /// </summary> 
        /// <param name="d">要截断的小数</param> 
        /// <param name="s">小数位数，s大于等于0，小于等于28</param> 
        /// <returns></returns> 
        public static decimal ToFixed(this decimal d, int s)
        {
            decimal sp = Convert.ToDecimal(Math.Pow(10, s));

            if (d < 0)
                return Math.Truncate(d) + Math.Ceiling((d - Math.Truncate(d)) * sp) / sp;
            else
                return Math.Truncate(d) + Math.Floor((d - Math.Truncate(d)) * sp) / sp;
        } 
    }
}
