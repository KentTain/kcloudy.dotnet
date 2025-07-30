using System;
using System.Collections.Generic;

namespace KC.Framework.Extension
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// 日期时间格式：yyyyMMddHHmmssSSS
        /// </summary>
        public static string milliSecondFmt1 = "yyyyMMddHHmmssSSS";
        /// <summary>
        /// 日期时间格式：yyyy-MM-dd HH:mm:ss:SSS
        /// </summary>
        public static string milliSecondFmt2 = "yyyy-MM-dd HH:mm:ss:SSS";
        /// <summary>
        /// 日期格式：yyyy-MM-dd
        /// </summary>
        public static string FMT_yMd1 = "yyyy-MM-dd";
        /// <summary>
        /// 日期时间格式：yyyy-MM-dd HH:mm:ss
        /// </summary>
        public static string FMT_yMdHms1 = "yyyy-MM-dd HH:mm:ss";
        /// <summary>
        /// 日期格式：yyyy-MM-dd HH:mm:ss
        /// </summary>
        public static string FMT_yMd2 = "yyyy/MM/dd";
        /// <summary>
        /// 日期时间格式：yyyy/MM/dd HH:mm:ss
        /// </summary>
        public static string FMT_yMdHms2 = "yyyy/MM/dd HH:mm:ss";
        /// <summary>
        /// 日期格式：yyyyMMdd
        /// </summary>
        public static string FMT_yMd3 = "yyyyMMdd";
        /// <summary>
        /// 日期时间格式：yyyyMMddHHmmss
        /// </summary>
        public static string FMT_yMdHms3 = "yyyyMMddHHmmss";
        /// <summary>
        /// 日期时间格式：yyyy-MM-dd HH:mm
        /// </summary>
        public static string FMT_yMdHm1 = "yyyy-MM-dd HH:mm";
        /// <summary>
        /// 日期时间格式：yyyy/MM/dd HH:mm
        /// </summary>
        public static string FMT_yMdHm2 = "yyyy/MM/dd HH:mm";
        /// <summary>
        /// 日期时间格式：yyyyMMddHHmm
        /// </summary>
        public static string FMT_yMdHm3 = "yyyyMMddHHmm";
        /// <summary>
        /// 日期格式：yyyy-MM-dd HH:mm:ss
        /// </summary>
        public static string FMT_yM = "yyyy/MM";
        /// <summary>
        /// 日期格式：yyyy
        /// </summary>
        public static string FMT_y = "yyyy";
        /// <summary>
        /// 日期格式：yyyy/M/dd
        /// </summary>
        public static string FMT_yMd4 = "yyyy/M/dd";
        /// <summary>
        /// 日期格式：yyyy/M/d
        /// </summary>
        public static string FMT_yMd5 = "yyyy/M/d";
        public static string[] FMTS = new string[] { FMT_yMd1, FMT_yMdHms1, FMT_yMd2, FMT_yMdHms2,
            FMT_yMd3, FMT_yMdHms3, FMT_yMdHm1, FMT_yMdHm2, FMT_yMdHm3, FMT_yM, FMT_y, FMT_yMd4, FMT_yMd5 };

        /// <summary>
        /// 获取本月第几周的周六
        /// </summary>
        /// <param name="firstDayOfMonth"></param>
        /// <param name="nthWeekendInMonth">第几周</param>
        /// <returns></returns>
        public static DateTime GetNthWeekendOfMonth(this DateTime firstDayOfMonth, int nthWeekendInMonth)
        {
            int iCount = 0;
            while (iCount < nthWeekendInMonth)
            {
                if (!firstDayOfMonth.IsBusinessDay())
                {
                    iCount++;
                }
                firstDayOfMonth = firstDayOfMonth.AddDays(1);
            }

            return firstDayOfMonth.AddDays(-1);
        }
        /// <summary>
        /// 获取本月最后一周的周六
        /// </summary>
        /// <param name="firstDayOfMonth"></param>
        /// <returns></returns>
        public static DateTime GetTheLastWeekendOfMonth(this DateTime firstDayOfMonth)
        {
            DateTime result = new DateTime();

            DateTime lastDayOfMonth = GetTheLastDayOfMonth(firstDayOfMonth);
            for (int i = 1; i <= 7; i++)
            {
                if (!lastDayOfMonth.IsBusinessDay())
                {
                    result = lastDayOfMonth;
                    break;
                }

                lastDayOfMonth = lastDayOfMonth.AddDays(-1);
            }

            return result;
        }
        /// <summary>
        /// 获取本月第几周的周一
        /// </summary>
        /// <param name="firstDayOfMonth"></param>
        /// <param name="nthWeekdayInMonth">第几周</param>
        /// <returns></returns>
        public static DateTime GetNthWeekdayOfMonth(this DateTime firstDayOfMonth, int nthWeekdayInMonth)
        {
            int iCount = 0;
            while (iCount < nthWeekdayInMonth)
            {
                if (firstDayOfMonth.IsBusinessDay())
                {
                    iCount++;
                }
                firstDayOfMonth = firstDayOfMonth.AddDays(1);
            }

            return firstDayOfMonth.AddDays(-1);
        }
        /// <summary>
        /// 获取本月最后一周的周一
        /// </summary>
        /// <param name="firstDayOfMonth"></param>
        /// <returns></returns>
        public static DateTime GetTheLastWeekdayOfMonth(this DateTime firstDayOfMonth)
        {
            DateTime result = new DateTime();

            DateTime lastDayOfMonth = GetTheLastDayOfMonth(firstDayOfMonth);
            for (int i = 1; i <= 7; i++)
            {
                if (lastDayOfMonth.IsBusinessDay())
                {
                    result = lastDayOfMonth;
                    break;
                }

                lastDayOfMonth = lastDayOfMonth.AddDays(-1);
            }

            return result;
        }
        /// <summary>
        /// 获取本月第几周的第几个工作日
        /// </summary>
        /// <param name="firstDayOfMonth"></param>
        /// <param name="nthWeekInMonth">第几周</param>
        /// <param name="weekday">第几个工作日</param>
        /// <returns></returns>
        public static DateTime GetNthWeekOfMonth(this DateTime firstDayOfMonth, int nthWeekInMonth, int weekday)
        {
            int iCount = 0;
            while (iCount < nthWeekInMonth)
            {
                if ((int)firstDayOfMonth.DayOfWeek == weekday)
                {
                    iCount++;
                }
                firstDayOfMonth = firstDayOfMonth.AddDays(1);
            }

            return firstDayOfMonth.AddDays(-1);
        }
        /// <summary>
        /// 获取本月最后一周的第几个工作日
        /// </summary>
        /// <param name="firstDayOfMonth"></param>
        /// <param name="weekday">第几个工作日</param>
        /// <returns></returns>
        public static DateTime GetTheLastWeekOfMonth(this DateTime firstDayOfMonth, int weekday)
        {
            DateTime result = new DateTime();
            DateTime lastDayOfMonth = GetTheLastDayOfMonth(firstDayOfMonth);

            for (int i = 1; i <= 7; i++)
            {
                if ((int)lastDayOfMonth.DayOfWeek == weekday)
                {
                    result = lastDayOfMonth;
                    break;
                }

                lastDayOfMonth = lastDayOfMonth.AddDays(-1);
            }

            return result;
        }
        /// <summary>
        /// 获取本月的第几天
        /// </summary>
        /// <param name="firstDayOfMonth"></param>
        /// <param name="nthDayInMonth">第几天</param>
        /// <returns></returns>
        public static DateTime GetNthDayOfMonth(this DateTime firstDayOfMonth, int nthDayInMonth)
        {
            DateTime result = new DateTime();
            int lastDay = GetTheLastDayOfMonth(firstDayOfMonth).Day;

            if (nthDayInMonth <= lastDay)
            {
                result = new DateTime(firstDayOfMonth.Year, firstDayOfMonth.Month, nthDayInMonth);
            }

            return result;
        }
        /// <summary>
        /// 获取本周的第1天
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static DateTime GetTheFirstDayOfWeek(this DateTime dateTime)
        {
            return dateTime.AddDays((int)dateTime.DayOfWeek * -1).Date;
        }
        /// <summary>
        /// 获取本月的第一天
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static DateTime GetTheFirstDayOfMonth(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, 1, 0, 0, 0, dateTime.Kind);
        }
        /// <summary>
        /// 获取本月的最后一天
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static DateTime GetTheLastDayOfMonth(this DateTime dateTime)
        {
            return dateTime.GetTheFirstDayOfMonth().AddMonths(1).AddDays(-1);
        }
        /// <summary>
        /// 获取本年的第一天
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static DateTime GetTheFirstDayOfYear(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, 1, 1, 0, 0, 0, dateTime.Kind);
        }
        /// <summary>
        /// 获取本天的零点时刻
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static DateTime GetZeroHourOfDay(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0, dateTime.Kind);
        }
        /// <summary>
        /// 获取下一天的零点时刻
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static DateTime GetZeroHourOfTheNextDay(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day + 1, 0, 0, 0, dateTime.Kind);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="weeklyCycleWeekdays"></param>
        /// <returns></returns>
        public static bool IsWeeklyCycleWeekdays(this DateTime startDate, List<int> weeklyCycleWeekdays)
        {
            return weeklyCycleWeekdays.Contains((int)startDate.DayOfWeek);
        }
        /// <summary>
        /// 是否为工作日
        /// </summary>
        /// <param name="startDateTime"></param>
        /// <returns></returns>
        public static bool IsBusinessDay(this DateTime startDateTime)
        {
            return (int)startDateTime.DayOfWeek >= 1 && (int)startDateTime.DayOfWeek <= 5;
        }
        /// <summary>
        /// 获取已经过去的时间的描述（与本地时间为基准）</br>
        ///     超过1年，显示：几年几月几日多少小时前；</br>
        ///     1年内，大于等于7天，显示：今年几月几日前；</br>
        ///     7天内，大于1天，显示：多少天前；</br>
        ///     等于1天，显示：昨天；</br>
        ///     小于1天，大于等于2小时，显示：多少小时前；</br>
        ///     小于2小时，大于等于60分钟，显示：一小时前；</br>
        ///     小于60分钟，大于等于5分钟，显示：多少分钟前；</br>
        ///     小于5分钟，大于等于1分钟，显示：一分钟前；</br>
        ///     小于1分钟，显示：一分钟内；</br>
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string ToRelativeDateString(this DateTime date)
        {
            return GetRelativeDateValue(date, DateTime.Now);
        }
        /// <summary>
        /// 获取已经过去的时间的描述（与UTC时间为基准）</br>
        ///     超过1年，显示：几年几月几日多少小时前；</br>
        ///     1年内，大于等于7天，显示：今年几月几日前；</br>
        ///     7天内，大于1天，显示：多少天前；</br>
        ///     等于1天，显示：昨天；</br>
        ///     小于1天，大于等于2小时，显示：多少小时前；</br>
        ///     小于2小时，大于等于60分钟，显示：一小时前；</br>
        ///     小于60分钟，大于等于5分钟，显示：多少分钟前；</br>
        ///     小于5分钟，大于等于1分钟，显示：一分钟前；</br>
        ///     小于1分钟，显示：一分钟内；</br>
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string ToRelativeDateStringUtc(this DateTime date)
        {
            return GetRelativeDateValue(date, DateTime.UtcNow);
        }
        private static string GetRelativeDateValue(DateTime date, DateTime comparedTo)
        {
            TimeSpan diff = comparedTo.Subtract(date);
            if (diff.TotalDays >= 365)
                return string.Concat("on ", date.ToString(FMT_yMd1));
            if (diff.TotalDays >= 7)
                return string.Concat("今年", date.ToString("MM-dd") + "前");
            else if (diff.TotalDays > 1)
                return string.Format("{0:N0} 天前", diff.TotalDays);
            else if (diff.TotalDays == 1)
                return "昨天";
            else if (diff.TotalHours >= 2)
                return string.Format("{0:N0} 小时前", diff.TotalHours);
            else if (diff.TotalMinutes >= 60)
                return "一小时前";
            else if (diff.TotalMinutes >= 5)
                return string.Format("{0:N0} 分钟前", diff.TotalMinutes);
            if (diff.TotalMinutes >= 1)
                return "一分钟前";
            else
                return "一分钟内";
        }

        /// <summary>
        /// 把UTC时间转换成北京时间(UTC+08:00)
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime ToLocalDateTime(this DateTime date)
        {
            TimeZoneInfo.ClearCachedData();
            try
            {
                var timeZoneDestination =
                TimeZoneInfo.FindSystemTimeZoneById("China Standard Time");
                return TimeZoneInfo.ConvertTimeFromUtc(date, timeZoneDestination);
            }
            catch (Exception)
            {
                
            }
            return date;
        }

        /// <summary>
        /// 先转换为北京时间(UTC+08:00)，再ToString
        /// </summary>
        /// <param name="date"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string ToLocalDateTimeStr(this DateTime date, string format)
        {
            TimeZoneInfo.ClearCachedData();
            try
            {
                var timeZoneDestination =
                TimeZoneInfo.FindSystemTimeZoneById("China Standard Time");
                var localDate = TimeZoneInfo.ConvertTimeFromUtc(date, timeZoneDestination);
                return localDate.ToString(format);
            }
            catch (Exception)
            {
                
            }
            return date.ToString(format);
        }

        #region 获得两个日期的间隔
        /// <summary>
        /// 获得两个日期间隔的毫秒数
        /// </summary>
        /// <param name="DateTime1">日期一。</param>
        /// <param name="DateTime2">日期二。</param>
        /// <returns>日期间隔TimeSpan。</returns>
        public static TimeSpan DateDiff(this DateTime DateTime1, DateTime DateTime2)
        {
            TimeSpan ts1 = new TimeSpan(DateTime1.Ticks);
            TimeSpan ts2 = new TimeSpan(DateTime2.Ticks);
            TimeSpan ts = ts1.Subtract(ts2).Duration();
            return ts;
        }

        /// <summary>
        /// 获得两个日期间隔的描述，例如：</br>
        ///     超过一小时，显示：几月几号几日多少小时前；
        ///     少于一小时，显示：几月几号几日多少分钟前；
        /// </summary>
        /// <param name="DateTime1"></param>
        /// <param name="DateTime2"></param>
        /// <returns></returns>
        public static string DateDiff2(this DateTime DateTime1, DateTime DateTime2)
        {
            string dateDiff = null;
            try
            {
                TimeSpan ts = DateTime2 - DateTime1;
                if (ts.Days >= 1)
                {
                    dateDiff = DateTime1.Month.ToString() + "月" + DateTime1.Day.ToString() + "日";
                }
                else
                {
                    if (ts.Hours > 1)
                    {
                        dateDiff = ts.Hours.ToString() + "小时前";
                    }
                    else
                    {
                        dateDiff = ts.Minutes.ToString() + "分钟前";
                    }
                }
            }
            catch
            { }
            return dateDiff;
        }

        /// <summary>
        /// 获取两个时间之间的所有日期（日期格式列表）
        /// </summary>
        /// <param name="startDate">开始时间</param>
        /// <param name="endDate">结束时间</param>
        /// <returns></returns>
        public static List<DateTime> FindAllDaysInDateDiff(this DateTime startDate, DateTime endDate)
        {
            List<DateTime> dates = new List<DateTime>();
            for (DateTime i = startDate.Date; i <= endDate.Date; i = i.AddDays(1))
            {
                dates.Add(i);
            }
            return dates;
        }

        /// <summary>
        /// 获取两个时间之间的所有日期（yyyy-MM-dd格式的字符串列表）
        /// </summary>
        /// <param name="startDate">开始时间</param>
        /// <param name="endDate">结束时间</param>
        /// <returns></returns>
        public static List<string> FindAllDayStringsInDateDiff(this DateTime startDate, DateTime endDate)
        {
            List<string> dates = new List<string>();
            for (DateTime i = startDate.Date; i <= endDate.Date; i = i.AddDays(1))
            {
                dates.Add(i.ToString(FMT_yMd1));
            }
            return dates;
        }
        #endregion
    }
}
