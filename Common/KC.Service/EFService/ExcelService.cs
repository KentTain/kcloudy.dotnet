using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Com.Enums.CreditLevel;
using Com.Framework.Tenant;
using Com.Framework.Util;
using Com.ThirdParty;

namespace Com.Service.Core.EFService
{
    public class ExcelService
    {
        private Tenant _tenant;
        private const int maxRows = 500;
        public const string BalanceSheet = "资产负债表";
        public const string ProfitSheet = "利润表";
        public const string CashFlowSheet = "现金流量表";

        public ExcelService(Tenant tenant)
        {
            _tenant = tenant;
        }

        /// <summary>
        /// Excel导入财务报表数据
        /// </summary>
        /// <param name="excelData"></param>
        /// <param name="sheetNameList"></param>
        /// <returns></returns>
        public Dictionary<string, Dictionary<string, string>> GetExcelReportData(byte[] excelData)
        {
            var result = new Dictionary<string, Dictionary<string, string>>();
            try
            {
                using (var er = new NPOIExcelReader(excelData))
                {
                    var parserSheetNames = new List<string>() { BalanceSheet, ProfitSheet, CashFlowSheet };

                    var sheetNames = er.GetWorkSheetNames();
                    foreach (var sheetName in sheetNames)
                    {
                        if (!parserSheetNames.Contains(sheetName)) continue;

                        LogUtil.LogDebug("---Begin to get the excel data: " + sheetName + "--------------");
                        var data = er.GetWorksheetDictData(sheetName);
                        LogUtil.LogDebug("---End to get the excel data: " + sheetName + "--------------");
                        result.Add(sheetName, data);
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtil.LogError("读取财务报表Excel文件出错: ", ex.Message + Environment.NewLine + ex.Source);
            }


            return result;
        }

        public Dictionary<string, Dictionary<string, string>> GetExcelReportData(byte[] excelData, ref List<string> sheetNameList)
        {
            var result = new Dictionary<string, Dictionary<string, string>>();
            try
            {
                using (var er = new NPOIExcelReader(excelData))
                {
                    var parserSheetNames = new List<string>() { BalanceSheet, ProfitSheet, CashFlowSheet };

                    var sheetNames = er.GetWorkSheetNames();
                    if (sheetNameList != null) { sheetNameList = sheetNames; }
                    foreach (var sheetName in sheetNames)
                    {
                        if (!parserSheetNames.Contains(sheetName)) continue;

                        LogUtil.LogDebug("---Begin to get the excel data: " + sheetName + "--------------");
                        var data = er.GetWorksheetDictData(sheetName);
                        LogUtil.LogDebug("---End to get the excel data: " + sheetName + "--------------");
                        result.Add(sheetName, data);
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtil.LogError("读取财务报表Excel文件出错: ", ex.Message + Environment.NewLine + ex.Source);
            }


            return result;
        }

        #region 生成资产负债表数据 + List<BalanceSheetDTO> GenerateBalanceSheetData(List<string> parserSheetNames, string sheetName, Dictionary<string, Dictionary<string, string>> sheetDatas, Dictionary<string, string> parmsDic, List<Model.CreditLevel.ReportTemplateDTO> reportTemplateList)
        /// <summary>
        /// 生成资产负债表数据 + List<BalanceSheetDTO> GenerateBalanceSheetData(List<string> parserSheetNames, string sheetName, Dictionary<string, Dictionary<string, string>> sheetDatas, Dictionary<string, string> parmsDic, List<Model.CreditLevel.ReportTemplateDTO> reportTemplateList)
        /// </summary>
        /// <param name="parserSheetNames"></param>
        /// <param name="sheetName"></param>
        /// <param name="sheetDatas"></param>
        /// /// <param name="parmsDic"></param>
        /// <param name="reportTemplateList"></param>
        /// <returns></returns>
        public List<BalanceSheetDTO> GenerateBalanceSheetData(List<string> parserSheetNames, string sheetName, Dictionary<string, Dictionary<string, string>> sheetDatas, Dictionary<string, string> parmsDic, List<ReportTemplateDTO> reportTemplateList)
        {
            if (sheetDatas.Count != parserSheetNames.Count && !sheetDatas.ContainsKey(sheetName))
            {
                //isSuccess = false;
                return null;
            }
            if (reportTemplateList.Any())
            {
                LogUtil.LogDebug("---Begin to get the sheet data: " + sheetName + "--------------");
                var data = sheetDatas[sheetName];

                int i = 0;
                var balanceSheets = new List<BalanceSheetDTO>();
                foreach (var reportTemplate in reportTemplateList)
                {
                    string value = data.ContainsKey(reportTemplate.SourceFieldValue)
                        ? data[reportTemplate.SourceFieldValue]
                        : string.Empty;
                    //AppendFormatErrorMessage(report, reportTemplate, sheetName, value);
                    var parms = ParasPrams(parmsDic);

                    balanceSheets.Add(new BalanceSheetDTO
                    {
                        UserId = parms.Item1,
                        SourceType = (SourceType)parms.Item2,
                        DurationType = (DurationType)parms.Item3,
                        Duration = parms.Item4,
                        ValueType = reportTemplate.TargetType,
                        Name = reportTemplate.SourceField,
                        Value = value.ObjToDecimal(),
                        Priority = i,
                    });
                    i++;
                }
                LogUtil.LogDebug("---End to get the sheet data: " + sheetName + "--------------");
                if (balanceSheets.Any())
                {
                    return balanceSheets;
                }
            }
            return null;
        }
        #endregion

        #region 生成利润表数据 + List<ProfitSheetDTO> GenerateProfitSheetData(List<string> parserSheetNames, string sheetName, Dictionary<string, Dictionary<string, string>> sheetDatas, Dictionary<string, string> parmsDic, List<Model.CreditLevel.ReportTemplateDTO> reportTemplateList)
        /// <summary>
        /// 生成利润表数据 + List<ProfitSheetDTO> GenerateProfitSheetData(List<string> parserSheetNames, string sheetName, Dictionary<string, Dictionary<string, string>> sheetDatas, Dictionary<string, string> parmsDic, List<Model.CreditLevel.ReportTemplateDTO> reportTemplateList)
        /// </summary>
        /// <param name="parserSheetNames"></param>
        /// <param name="sheetName"></param>
        /// <param name="sheetDatas"></param>
        /// /// <param name="parmsDic"></param>
        /// <param name="reportTemplateList"></param>
        /// <returns></returns>
        public List<ProfitSheetDTO> GenerateProfitSheetData(List<string> parserSheetNames, string sheetName, Dictionary<string, Dictionary<string, string>> sheetDatas, Dictionary<string, string> parmsDic, List<ReportTemplateDTO> reportTemplateList)
        {
            if (sheetDatas.Count != parserSheetNames.Count && !sheetDatas.ContainsKey(sheetName))
            {
                //isSuccess = false;
                return null;
            }
            if (reportTemplateList.Any())
            {
                LogUtil.LogDebug("---Begin to get the sheet data: " + sheetName + "--------------");
                var data = sheetDatas[sheetName];

                int i = 0;
                var profitSheets = new List<ProfitSheetDTO>();
                foreach (var reportTemplate in reportTemplateList)
                {
                    string value = data.ContainsKey(reportTemplate.SourceFieldValue)
                        ? data[reportTemplate.SourceFieldValue]
                        : string.Empty;
                    //AppendFormatErrorMessage(report, reportTemplate, sheetName, value);
                    var parms = ParasPrams(parmsDic);

                    profitSheets.Add(new ProfitSheetDTO
                    {
                        UserId = parms.Item1,
                        SourceType = (SourceType)parms.Item2,
                        DurationType = (DurationType)parms.Item3,
                        Duration = parms.Item4,
                        ValueType = reportTemplate.TargetType,
                        Name = reportTemplate.SourceField,
                        Value = value.ObjToDecimal(),
                        Priority = i,
                    });
                    i++;
                }
                LogUtil.LogDebug("---End to get the sheet data: " + sheetName + "--------------");
                if (profitSheets.Any())
                {
                    return profitSheets;
                }
            }
            return null;
        }
        #endregion

        #region 生成现金流表数据 + List<CashFlowSheetDTO> GenerateCashFlowSheetData(List<string> parserSheetNames, string sheetName, Dictionary<string, Dictionary<string, string>> sheetDatas, Dictionary<string, string> parmsDic, List<Model.CreditLevel.ReportTemplateDTO> reportTemplateList)
        /// <summary>
        /// 生成现金流表数据 + List<CashFlowSheetDTO> GenerateCashFlowSheetData(List<string> parserSheetNames, string sheetName, Dictionary<string, Dictionary<string, string>> sheetDatas, Dictionary<string, string> parmsDic, List<Model.CreditLevel.ReportTemplateDTO> reportTemplateList)
        /// </summary>
        /// <param name="parserSheetNames"></param>
        /// <param name="sheetName"></param>
        /// <param name="sheetDatas"></param>
        ///  <param name="reportTemplateList"></param>
        /// <returns></returns>
        public List<CashFlowSheetDTO> GenerateCashFlowSheetData(List<string> parserSheetNames, string sheetName, Dictionary<string, Dictionary<string, string>> sheetDatas, Dictionary<string, string> parmsDic, List<ReportTemplateDTO> reportTemplateList)
        {
            if (sheetDatas.Count != parserSheetNames.Count && !sheetDatas.ContainsKey(sheetName))
            {
                //isSuccess = false;
                return null;
            }
            if (reportTemplateList.Any())
            {
                LogUtil.LogDebug("---Begin to get the sheet data: " + sheetName + "--------------");
                var data = sheetDatas[sheetName];

                int i = 0;
                var cashFlowSheets = new List<CashFlowSheetDTO>();
                foreach (var reportTemplate in reportTemplateList)
                {
                    string value = data.ContainsKey(reportTemplate.SourceFieldValue)
                        ? data[reportTemplate.SourceFieldValue]
                        : string.Empty;
                    //AppendFormatErrorMessage(report, reportTemplate, sheetName, value);
                    var parms = ParasPrams(parmsDic);

                    cashFlowSheets.Add(new CashFlowSheetDTO
                    {
                        UserId = parms.Item1,
                        SourceType = (SourceType)parms.Item2,
                        DurationType = (DurationType)parms.Item3,
                        Duration = parms.Item4,
                        ValueType = reportTemplate.TargetType,
                        Name = reportTemplate.SourceField,
                        Value = value.ObjToDecimal(),
                        Priority = i,
                    });
                    i++;
                }
                LogUtil.LogDebug("---End to get the sheet data: " + sheetName + "--------------");
                if (cashFlowSheets.Any())
                {
                    return cashFlowSheets;
                }
            }
            return null;
        }
        #endregion

        #region 解析参数集 - Tuple<string, int, int, int> ParasPrams(Dictionary<string, string> parmsDic)
        /// <summary>
        /// 解析参数集 - Tuple<string, int, int, int> ParasPrams(Dictionary<string, string> parmsDic)
        /// </summary>
        /// <param name="parmsDic"></param>
        /// <returns></returns>
        Tuple<string, int, int, int> ParasPrams(Dictionary<string, string> parmsDic)
        {
            var userId = parmsDic.ContainsKey("userId")
                ? parmsDic["userId"]
                : string.Empty;
            var sourceType = parmsDic.ContainsKey("sourceType")
                ? int.Parse(parmsDic["sourceType"])
                : (int)SourceType.ClientSource;
            var durationType = (int)DurationType.Year;
            var duration = parmsDic.ContainsKey("yearType")
                ? int.Parse(parmsDic["yearType"])
                : DateTime.UtcNow.Year;
            Tuple<string, int, int, int> parmsTuple = new Tuple<string, int, int, int>(userId, sourceType, durationType, duration);
            return parmsTuple;
        }
        #endregion
    }
}
