using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Common.ToolsHelper
{
    /// <summary>
    /// 利息计算(还款开始月都从0开始)
    /// </summary>
    public class InterestCalculationHelper
    {
        /// <summary>
        /// 等额本金还款方式的月还款额(本金+利息)
        /// </summary>
        /// <param name="annualRate">年利率</param>
        /// <param name="total">贷款总额</param>
        /// <param name="monthCount">贷款总月份</param>
        /// <param name="monthIndex">贷款当前月0～length-1</param>
        /// <returns></returns>
        public static double GetMonthlyRepayment(double annualRate, double total, int monthCount, int monthIndex)
        {
            var monthRates = annualRate / 12;//月利率
            var moneyPrincipal = total / monthCount;
            return (total - moneyPrincipal * monthIndex) * monthRates + moneyPrincipal;
        }

        /// <summary>
        /// 等额本金还款方式的总还款额(本金+利息)
        /// </summary>
        /// <param name="annualRate"></param>
        /// <param name="total"></param>
        /// <param name="monthCount"></param>
        /// <returns></returns>
        public static double GetPrincipalRepaymentAmount(double annualRate, double total, int monthCount)
        {
            var count = 0.0;
            for (var i = 0; i < monthCount; i++)
            {
                count += GetMonthlyRepayment(annualRate, total, monthCount, i);
            }
            return count;
        }

        /// <summary>
        /// 等额本息还款方式的月还款额(本金+利息)
        /// </summary>
        /// <param name="annualRate">年利率</param>
        /// <param name="total">贷款总额</param>
        /// <param name="monthCount">贷款总月份</param>
        /// <returns></returns>
        public static double GetMonthlyRepayment(double annualRate, double total, int monthCount)
        {
            var monthRates = annualRate / 12;//月利率
            var aa= Math.Pow(1 + monthRates, monthCount);
            var dd = Math.Pow(1 + monthRates, monthCount) - 1;
            return total * monthRates * Math.Pow(1 + monthRates, monthCount) / (Math.Pow(1 + monthRates, monthCount) - 1);
        }

        /// <summary>
        /// 等额本息还款方式的总还款额(本金+利息)
        /// </summary>
        /// <param name="annualRate"></param>
        /// <param name="total"></param>
        /// <param name="monthCount"></param>
        /// <returns></returns>
        public static double GetPrincipalAndInterestRepaymentAmount(double annualRate, double total, int monthCount)
        {
            return GetMonthlyRepayment(annualRate, total, monthCount) * monthCount;
        }

        /// <summary>
        /// 一次还本付息（本金+利息）
        /// </summary>
        /// <param name="annualRate">年利率</param>
        /// <param name="total">贷款总额</param>
        /// <param name="monthCount">贷款总月份</param>
        /// <returns></returns>
        public static double GetRepaymentAmount(double annualRate, double total, int monthCount)
        {
            var monthRates = annualRate / 12;//月利率
            return total * (1 + monthRates * monthCount);
        }
        /// <summary>
        /// 一次还本付息（利息）
        /// </summary>
        /// <param name="annualRate">年利率</param>
        /// <param name="total">贷款总额</param>
        /// <param name="monthCount">贷款总月份</param>
        /// <returns></returns>
        public static double GetRepaymentAmountInterest(double annualRate, double total, int monthCount)
        {
            var monthRates = annualRate / 12;//月利率
            return total*(monthRates*monthCount);
        }


        /// <summary>
        /// 逾期罚款(单利)
        /// </summary>
        /// <param name="count">逾期本息合计</param>
        /// <param name="penaltyRate">日罚息利率</param>
        /// <param name="dayCount">逾期天数</param>
        /// <returns></returns>
        public static double GetSimpleInterest(double count, double penaltyRate, int dayCount)
        {
            return count * penaltyRate * dayCount;
        }

        /// <summary>
        /// 逾期罚款(复利)
        /// </summary>
        /// <param name="count">逾期本息合计</param>
        /// <param name="penaltyRate">日罚息利率</param>
        /// <param name="dayCount">逾期天数</param>
        /// <returns></returns>
        public static double GetCompoundInterest(double count, double penaltyRate, int dayCount)
        {
            return count * Math.Pow(penaltyRate, dayCount);
        }

        /// <summary>
        /// 等额本金还款方式的月还款本金
        /// </summary>
        /// <param name="total">贷款总额</param>
        /// <param name="monthCount">总还款月</param>
        /// <returns></returns>
        public static double GetMonthlyRepaymentPrincipal(double total, int monthCount)
        {
            return total / monthCount;
        }

        /// <summary>
        /// 等额本金还款方式的月还款利息
        /// </summary>
        /// <param name="annualRate">年利率</param>
        /// <param name="total">贷款总额</param>
        /// <param name="monthCount">还款总月数</param>
        /// <param name="monthIndex">当前月(从0开始)</param>
        /// <returns></returns>
        public static double GetMonthlyRepaymentInterest(double annualRate, double total, int monthCount, int monthIndex)
        {
            return (total - total / monthCount * monthIndex) * (annualRate / 12);
        }

        /// <summary>
        /// 等额本息还款方式的月还款本金
        /// </summary>
        /// <param name="annualRate">年利率</param>
        /// <param name="total">贷款总额</param>
        /// <param name="monthCount">还款总月数</param>
        /// <param name="monthIndex">当前月</param>
        /// <returns></returns>
        public static double GetMonthlyRepaymentPrincipal(double annualRate, double total, int monthCount, int monthIndex)
        {
            var monthRates = annualRate / 12;//月利率
            return total * monthRates * Math.Pow(1 + monthRates, monthIndex) / (Math.Pow(1 + monthRates, monthCount) - 1);
        }

        /// <summary>
        /// 等额本息还款方式的月还款利息
        /// </summary>
        /// <param name="annualRate">年利率</param>
        /// <param name="total">贷款总额</param>
        /// <param name="monthCount">还款总月数</param>
        /// <param name="monthIndex">当前月</param>
        /// <returns></returns>
        public static double GetPrincipalAndInterestMonthlyRepaymentInterest(double annualRate, double total, int monthCount, int monthIndex)
        {
            var monthRates = annualRate / 12;//月利率

            return total * monthRates * (Math.Pow(1 + monthRates, monthCount) - Math.Pow(1 + monthRates, monthIndex)) /
             (Math.Pow(1 + monthRates, monthCount) - 1);
        }
        /// <summary>
        /// 等本等息还款方式的月还款额（本金+利息）
        /// </summary>
        /// <param name="annualRate">年利率</param>
        /// <param name="total">到款总额</param>
        /// <param name="monthCount">贷款总月份</param>
        /// <returns></returns>
        public static double GetWaitingfortherateForAll(double annualRate, double total, int monthCount)
        {
            return GetWaitingfortherate(annualRate, total, monthCount) * monthCount;
        }
        /// <summary>
        /// 等本等息还款方式的月还款额(本金+利息)
        /// </summary>
        /// <param name="annualRate">年利率</param>
        /// <param name="total">贷款总额</param>
        /// <param name="monthCount">贷款总月份</param>
        /// <returns></returns>
        public static double GetWaitingfortherate(double annualRate, double total, int monthCount)
        {
            var monthRates = annualRate / 12;//月利率
            return (total * monthRates * monthCount + total) / monthCount;
        }
        /// <summary>
        /// 等本等息还款方式的月还款额（本金） 
        /// </summary>
        /// <param name="total">贷款总额</param>
        /// <param name="monthCount">贷款总月份</param>
        /// <returns></returns>
        public static double GetWaitingfortherateForMonthliPrincipal(double total, int monthCount)
        {
            return total / monthCount;
        }
        /// <summary>
        /// 等本等息还款方式的月还款额（利息）
        /// </summary>
        /// <param name="annualRate">年利率</param>
        /// <param name="total">贷款总额</param>
        /// <returns></returns>
        public static double GetWaitingfortherateForInterest(double annualRate, double total)
        {
            var monthRates = annualRate / 12;//月利率
            return total * monthRates;
        }
        /// <summary>
        /// 先息后本还款方式的月还款额(利息)
        /// </summary>
        /// <param name="annualRate">年利率</param>
        /// <param name="total">贷款总额</param>
        /// <returns></returns>
        public static double GetMonthlyRepayment(double annualRate, double total)
        {
            var monthRates = annualRate / 12;//月利率
            return total * monthRates;
        }
    }
}
