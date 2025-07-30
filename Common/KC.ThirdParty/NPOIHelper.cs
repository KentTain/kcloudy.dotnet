using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using KC.Framework.Util;
using NPOI.HSSF.UserModel;
using NPOI.SS.Formula.Eval;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;

namespace KC.ThirdParty
{
    public abstract class AExcelBase : IDisposable
    {
        private bool disposed;
        private Stream stream = null;
        protected IWorkbook Workbook;
        public const int iMaxCount = 65536;

        /// <summary>
        /// 年月日时分秒 默认格式
        /// </summary>
        public static SimpleDateFormat COMMON_DATE_FORMAT = new SimpleDateFormat("yyyy-MM-dd HH:mm:ss");
        /// <summary>
        /// 时间 默认格式
        /// </summary>
        public static SimpleDateFormat COMMON_TIME_FORMAT = new SimpleDateFormat("HH:mm:ss");
        /// <summary>
        /// 年月日 默认格式
        /// </summary>
        public static SimpleDateFormat COMMON_DATE_FORMAT_NYR = new SimpleDateFormat("yyyy-MM-dd");
        /// <summary>
        /// 年月 默认格式
        /// </summary>
        public static SimpleDateFormat COMMON_DATE_FORMAT_NY = new SimpleDateFormat("yyyy-MM");
        /// <summary>
        /// 月日 默认格式
        /// </summary>
        public static SimpleDateFormat COMMON_DATE_FORMAT_YR = new SimpleDateFormat("MM-dd");
        /// <summary>
        /// 月 默认格式
        /// </summary>
        public static SimpleDateFormat COMMON_DATE_FORMAT_Y = new SimpleDateFormat("MM");
        /// <summary>
        /// 星期 默认格式
        /// </summary>
        public static string COMMON_DATE_FORMAT_XQ = "星期";
        /// <summary>
        /// 周 默认格式
        /// </summary>
        public static string COMMON_DATE_FORMAT_Z = "周";
        /// <summary>
        /// 07版时间(非日期) 总time
        /// </summary>
        public static List<int> EXCEL_FORMAT_INDEX_07_TIME = new List<int>(
                new int[]{18, 19, 20, 21, 32, 33, 45, 46, 47, 55, 56, 176, 177, 178, 179, 180, 181,
                    182, 183, 184, 185, 186}
        );
        /// <summary>
        /// 07版日期(非时间) 总date
        /// </summary>
        public static List<int> EXCEL_FORMAT_INDEX_07_DATE = new List<int>(
                new int[]{14, 15, 16, 17, 22, 30, 31, 57, 58, 187, 188, 189, 190, 191, 192, 193,
                    194, 195, 196, 197, 198, 199, 200, 201, 202, 203, 204, 205, 206, 207, 208}
        );
        /// <summary>
        /// 03版时间(非日期) 总time
        /// </summary>
        public static List<int> EXCEL_FORMAT_INDEX_03_TIME = new List<int>(
                new int[]{18, 19, 20, 21, 32, 33, 45, 46, 47, 55, 56, 176, 177, 178, 179, 180, 181,
                    182, 183, 184, 185, 186}
        );
        /// <summary>
        /// 07版日期(非日期) 总date
        /// </summary>
        public static List<int> EXCEL_FORMAT_INDEX_03_DATE = new List<int>(
                new int[]{14, 15, 16, 17, 22, 30, 31, 57, 58, 187, 188, 189, 190, 191, 192, 193,
                    194, 195, 196, 197, 198, 199, 200, 201, 202, 203, 204, 205, 206, 207, 208}
        );
        /// <summary>
        /// date-年月日时分秒
        /// </summary>
        public static List<string> EXCEL_FORMAT_INDEX_DATE_NYRSFM_STRING = new List<string>() {"yyyy/m/d\\ h:mm;@", "m/d/yy h:mm", "yyyy/m/d\\ h:mm\\ AM/PM",
            "[$-409]yyyy/m/d\\ h:mm\\ AM/PM;@", "yyyy/mm/dd\\ hh:mm:dd", 
            "yyyy/mm/dd\\ hh:mm", "yyyy/m/d\\ h:m", "yyyy/m/d\\ h:m:s",
            "yyyy/m/d\\ h:mm", "m/d/yy h:mm;@", "yyyy/m/d\\ h:mm\\ AM/PM;@",
            "yyyy\\-mm\\-dd hh:mm:dd;@","yyyy\\/mm\\/dd hh:mm:dd;@"};
        /// <summary>
        /// date-年月日
        /// </summary>
        public static List<string> EXCEL_FORMAT_INDEX_DATE_NYR_STRING = new List<string>() {
            "m/d/yy", "[$-F800]dddd\\,\\ mmmm\\ dd\\,\\ yyyy",
            "[DBNum1][$-804]yyyy\"年\"m\"月\"d\"日\";@", "yyyy\"年\"m\"月\"d\"日\";@",
            "yyyy/m/d;@", "yy/m/d;@", "m/d/yy;@", "[$-409]d/mmm/yy", 
            "[$-409]dd/mmm/yy;@", "reserved-0x1F", "reserved-0x1E", "mm/dd/yy;@", 
            "yyyy/mm/dd", "d-mmm-yy", "[$-409]d\\-mmm\\-yy;@", "[$-409]d\\-mmm\\-yy", 
            "[$-409]dd\\-mmm\\-yy;@", "[$-409]dd\\-mmm\\-yy", "[DBNum1][$-804]yyyy\"年\"m\"月\"d\"日\"", 
            "yy/m/d", "mm/dd/yy", "dd\\-mmm\\-yy","yyyy\\-mm\\-dd;@","yyyy\\/mm\\/dd;@"
        };
        /// <summary>
        /// date-年月
        /// </summary>
        public static List<string> EXCEL_FORMAT_INDEX_DATE_NY_STRING = new List<string>() {
                "[DBNum1][$-804]yyyy\"年\"m\"月\";@", "[DBNum1][$-804]yyyy\"年\"m\"月\"",
                "yyyy\"年\"m\"月\";@", "yyyy\"年\"m\"月\"", "[$-409]mmm\\-yy;@", "[$-409]mmm\\-yy",
                "[$-409]mmm/yy;@", "[$-409]mmm/yy", "[$-409]mmmm/yy;@", "[$-409]mmmm/yy",
                "[$-409]mmmmm/yy;@", "[$-409]mmmmm/yy", "mmm-yy", "yyyy/mm", "mmm/yyyy",
                "[$-409]mmmm\\-yy;@", "[$-409]mmmmm\\-yy;@", "mmmm\\-yy", "mmmmm\\-yy"
        };
        /// <summary>
        /// date-月日
        /// </summary>
        public static List<string> EXCEL_FORMAT_INDEX_DATE_YR_STRING = new List<string>() {
                "[DBNum1][$-804]m\"月\"d\"日\";@", "[DBNum1][$-804]m\"月\"d\"日\"",
                "m\"月\"d\"日\";@", "m\"月\"d\"日\"", "[$-409]d/mmm;@", "[$-409]d/mmm",
                "m/d;@", "m/d", "d-mmm", "d-mmm;@", "mm/dd", "mm/dd;@", "[$-409]d\\-mmm;@", "[$-409]d\\-mmm"
        };
        /// <summary>
        /// date-星期X
        /// </summary>
        public static List<string> EXCEL_FORMAT_INDEX_DATE_XQ_STRING = new List<string>() { "[$-804]aaaa;@", "[$-804]aaaa" };
        /// <summary>
        /// date-周X
        /// </summary>
        public static List<string> EXCEL_FORMAT_INDEX_DATE_Z_STRING = new List<string>() { "[$-804]aaa;@", "[$-804]aaa" };
        /// <summary>
        /// date-月X
        /// </summary>
        public static List<string> EXCEL_FORMAT_INDEX_DATE_Y_STRING = new List<string>() { "[$-409]mmmmm;@", "mmmmm", "[$-409]mmmmm" };
        /// <summary>
        /// time - 时间
        /// </summary>
        public static List<string> EXCEL_FORMAT_INDEX_TIME_STRING = new List<string>() {
                "mm:ss.0", "h:mm", "h:mm\\ AM/PM", "h:mm:ss", "h:mm:ss\\ AM/PM",
                "reserved-0x20", "reserved-0x21", "[DBNum1]h\"时\"mm\"分\"", "[DBNum1]上午/下午h\"时\"mm\"分\"", "mm:ss",
                "[h]:mm:ss", "h:mm:ss;@", "[$-409]h:mm:ss\\ AM/PM;@", "h:mm;@", "[$-409]h:mm\\ AM/PM;@",
                "h\"时\"mm\"分\";@", "h\"时\"mm\"分\"\\ AM/PM;@", "h\"时\"mm\"分\"ss\"秒\";@", "h\"时\"mm\"分\"ss\"秒\"_ AM/PM;@", "上午/下午h\"时\"mm\"分\";@",
                "上午/下午h\"时\"mm\"分\"ss\"秒\";@", "[DBNum1][$-804]h\"时\"mm\"分\";@", "[DBNum1][$-804]上午/下午h\"时\"mm\"分\";@", "h:mm AM/PM", "h:mm:ss AM/PM",
                "[$-F400]h:mm:ss\\ AM/PM"
        };
        /// <summary>
        /// date-当formatString为空的时候-年月
        /// </summary>
        public static int EXCEL_FORMAT_INDEX_DATA_EXACT_NY = 57;
        /// <summary>
        /// date-当formatString为空的时候-月日
        /// </summary>
        public static int EXCEL_FORMAT_INDEX_DATA_EXACT_YR = 58;
        /// <summary>
        /// time-当formatString为空的时候-时间
        /// </summary>
        public static List<int> EXCEL_FORMAT_INDEX_TIME_EXACT = new List<int>(new int[] { 55, 56 });
        /// <summary>
        /// 格式化星期或者周显示
        /// </summary>
        public static string[] WEEK_DAYS = { "日", "一", "二", "三", "四", "五", "六" };
        /// <summary>
        /// 07版 excel dataformat
        /// </summary>
        public static DataFormatter EXCEL_07_DATA_FORMAT = new DataFormatter();
        /**
         * 07版excel后缀名
         */
        public static string EXCEL_SUFFIX_07 = "xlsx";
        /**
         * 03版excel后缀名
         */
        public static string EXCEL_SUFFIX_03 = "xls";

        public bool IsXlsx { get; private set; }
        protected AExcelBase(bool isXlsx)
        {
            IsXlsx = isXlsx;
            if (IsXlsx)
            {
                //iMaxCount = 1048576;
                Workbook = new XSSFWorkbook();
            }
            else
            {
                //iMaxCount = 65536;
                Workbook = new HSSFWorkbook();
            }
        }
        protected AExcelBase(byte[] bytes)
        {
            stream = new MemoryStream(bytes);
            try
            {
                Workbook = WorkbookFactory.Create(stream);
                IsXlsx = Workbook is XSSFWorkbook;
                //iMaxCount = Workbook is XSSFWorkbook ? 1048576 : 65536;
            }
            catch (Exception ex)
            {
                if (Workbook != null)
                {
                    Workbook.Close();
                    Workbook = null;
                }
                LogUtil.LogError("Cannot process excel. Please make sure the excel model or chart is build by Office 2003 and beyond", ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }
        protected AExcelBase(Stream stream)
        {
            this.stream = stream;

            try
            {
                Workbook = WorkbookFactory.Create(stream);
                IsXlsx = Workbook is XSSFWorkbook;
                //iMaxCount = Workbook is XSSFWorkbook ? 1048576 : 65536;
            }
            catch (Exception ex)
            {
                if (Workbook != null)
                {
                    Workbook.Close();
                    Workbook = null;
                }

                LogUtil.LogError("Cannot process excel. Please make sure the excel model or chart is build by Office 2003 and beyond", ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }

        protected string dateToWeek(DateTime date)
        {
            if (date == null)
            {
                return "";
            }
            int i = (int)date.DayOfWeek;
            if (i < 0)
                i = 0;
            return WEEK_DAYS[i];
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    if (stream != null)
                        stream.Close();
                }

                stream = null;
                disposed = true;
            }
        }
    }
    public class NPOIExcelReader : AExcelBase
    {
        public NPOIExcelReader(byte[] bytes)
            : base(bytes)
        {
        }
        public NPOIExcelReader(Stream stream)
            : base(stream)
        {
        }

        #region NPOI Reader

        public List<string> GetWorkSheetNames()
        {
            var worksheets = new List<string>();
            var count = Workbook.NumberOfNames;
            for (var i = 0; i < count; i++)
            {
                var name = Workbook.GetSheetAt(i);
                var sheetName = name.SheetName;
                worksheets.Add(sheetName);
            }

            return worksheets;
        }

        /// <summary>
        /// 获取所有的自定义单元格列表及其值
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetDefinedNames()
        {
            try
            {
                var definedNames = new Dictionary<string, string>();
                var count = Workbook.NumberOfNames;
                for (var i = 0; i < count; i++)
                {
                    var name = Workbook.GetNameAt(i);
                    var sheetName = name.SheetName;
                    var formula = name.RefersToFormula;

                    var sheet = Workbook.GetSheet(sheetName);
                    var cr = new CellReference(formula);
                    var row = sheet.GetRow(cr.Row);
                    var cell = row.GetCell(cr.Col);
                    string value = cell.StringCellValue;

                    definedNames.Add(name.NameName, value);
                }

                return definedNames;
            }
            catch (Exception ex)
            {
                if (Workbook != null)
                {
                    Workbook.Close();
                    Workbook = null;
                }

                LogUtil.LogError("Couldn't Get the single value by using defined name.", ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
            }
        }
        /// <summary>
        /// 获取自定义单元格的值
        /// </summary>
        /// <param name="definedName">自定义单元格名称（例如：定义A3单元格为Title）</param>
        public string GetDefinedNameSingleValue(string definedName)
        {
            try
            {
                var name = Workbook.GetName(definedName);
                if (string.IsNullOrEmpty(definedName)
                || name == null)
                    throw new Exception("Missing defined name " + definedName);

                var sheetName = name.SheetName;
                var formula = name.RefersToFormula;

                var sheet = Workbook.GetSheet(sheetName);
                var cr = new CellReference(formula);
                var row = sheet.GetRow(cr.Row);
                var cell = row.GetCell(cr.Col);
                return cell.StringCellValue;
            }
            catch (Exception ex)
            {
                if (Workbook != null)
                {
                    Workbook.Close();
                    Workbook = null;
                }
                LogUtil.LogError("Couldn't Get the single value by using defined name.", ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
            }
        }
        /// <summary>
        /// 获取某个单元格（例如：A1）的值
        /// </summary>
        /// <param name="workSheetName">Sheet名称</param>
        /// <param name="cellName">某个单元格（例如：A1）</param>
        /// <returns></returns>
        public string GetCellValueByName(string workSheetName, string cellName)
        {
            try
            {
                var sheet = Workbook.GetSheet(workSheetName)
                            ?? Workbook.GetSheetAt(0);
                if (sheet == null)
                    return null;

                var cr = new CellReference(cellName);
                var row = sheet.GetRow(cr.Row);
                if (row == null)
                    throw new Exception("Cannot process excel: The excel sheet's cell is empty or wrong.");
                var cell = row.GetCell(cr.Col);
                if (string.IsNullOrEmpty(cellName)
                    || cell == null)
                    throw new Exception("Cannot process excel: The excel sheet's cell is empty or wrong.");

                return GetCellValue(cell);
            }
            catch (Exception ex)
            {
                if (Workbook != null)
                {
                    Workbook.Close();
                    Workbook = null;
                }
                LogUtil.LogError("Couldn't Get the cell's value by using cell's name.", ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
            }
        }
        /// <summary>
        /// 获取某个表单（例如：Sheet1）的所有值：
        ///     A1-Value11、B1-Value12、C1-Value13
        ///     A2-Value21、B2-Value22、C2-Value23
        /// </summary>
        /// <param name="workSheetName"></param>
        /// <param name="maxRows">获取的最大行数限制</param>
        /// <param name="skipRowCount">获取跳过几行后的数据</param>
        /// <returns></returns>
        public Dictionary<string, string> GetWorksheetDictData(string workSheetName = "Sheet1", int maxRows = iMaxCount, int skipRowCount = 0)
        {
            try
            {
                var sheet = Workbook.GetSheet(workSheetName)
                            ?? Workbook.GetSheetAt(0);
                if (sheet == null)
                    return null;

                var result = new Dictionary<string, string>();
                var headerRow = sheet.GetRow(0);
                var cellCount = headerRow.LastCellNum;
                var cellMax = sheet.LastRowNum;
                var maxRow = cellMax <= maxRows ? cellMax : maxRows;
                for (var i = skipRowCount; i <= maxRow; i++)
                {
                    var row = sheet.GetRow(i);
                    if (row == null) continue;

                    cellCount = cellCount <= 0 ? cellCount : row.LastCellNum;
                    for (int j = 0; j < cellCount; j++)
                    {
                        var cell = row.GetCell(j);
                        if (cell == null) continue;

                        var cellValue = GetCellValue(cell);
                        var alphabet = IndexToColumn(j + 1);
                        if (!result.ContainsKey(alphabet))
                        {
                            result.Add(alphabet + (i + 1), cellValue);
                        }
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                if (Workbook != null)
                {
                    Workbook.Close();
                    Workbook = null;
                }
                LogUtil.LogError("Couldn't get the row's dictionary data by sheet name.", ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
            }
        }
        /// <summary>
        /// 根据Excel文件，获取Sheet中的数据：List<List<RowValue>>
        /// </summary>
        /// <param name="workSheetName"></param>
        /// <param name="maxRows">获取的最大行数限制</param>
        /// <param name="skipRowCount">获取跳过几行后的数据</param>
        /// <param name="useRowHeaderColumnSize">是否使用第一行列数为表格最大列数</param>
        /// <returns></returns>
        public List<List<RowValue>> GetWorksheetRowListData(string workSheetName = "Sheet1", int maxRows = iMaxCount, int skipRowCount = 0, bool useRowHeaderColumnSize = false)
        {
            try
            {
                var sheet = Workbook.GetSheet(workSheetName)
                            ?? Workbook.GetSheetAt(0);
                if (sheet == null)
                    return null;

                var result = new List<List<RowValue>>();
                var headerRow = sheet.GetRow(0);
                var cellCount = headerRow.LastCellNum;
                var cellMax = sheet.LastRowNum;
                var maxRow = cellMax <= maxRows ? cellMax : maxRows;
                for (var i = skipRowCount; i <= maxRow; i++)
                {
                    var row = sheet.GetRow(i);
                    if (row == null) continue;

                    cellCount = useRowHeaderColumnSize && cellCount != 0 ? cellCount : row.LastCellNum;
                    var columns = new List<RowValue>();
                    for (int j = 0; j < cellCount; j++)
                    {
                        var cell = row.GetCell(j);
                        if (cell == null) continue;

                        var alphabet = IndexToColumn(j + 1);
                        var headerCell = headerRow.GetCell(j);
                        var headerValue = GetCellValue(headerCell);
                        var columnName = !string.IsNullOrEmpty(headerValue)
                            ? headerValue
                            : i.ToString();
                        var cellValue = GetCellValue(cell);
                        var cellColor = GetCellColor(cell);

                        var rowValue = new RowValue
                        {
                            RowId = i,
                            ColumnId = j,
                            CellName = alphabet,
                            ColumnName = columnName,
                            CellValue = cellValue,
                            CellColor = cellColor,
                        };
                        columns.Add(rowValue);
                    }

                    result.Add(columns);
                }

                return result;
            }
            catch (Exception ex)
            {
                if (Workbook != null)
                {
                    Workbook.Close();
                    Workbook = null;
                }
                LogUtil.LogError("Couldn't get the row's List<RowValue> data by sheet name.", ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
            }
        }

        public DataSet GetDataset()
        {
            DataSet ds = new DataSet();
            DataTable dt = null;
            int sheetCount = Workbook.NumberOfSheets;
            for (int sheetIndex = 0; sheetIndex < sheetCount; sheetIndex++)
            {
                NPOI.SS.UserModel.ISheet sheet = Workbook.GetSheetAt(sheetIndex);
                if (sheet == null) continue;
                NPOI.SS.UserModel.IRow row = sheet.GetRow(0);
                if (row == null) continue;

                int firstCellNum = row.FirstCellNum;
                int lastCellNum = row.LastCellNum;
                if (firstCellNum == lastCellNum) continue;
                dt = new DataTable(sheet.SheetName);
                foreach (var item in row)
                {
                    dt.Columns.Add(item.StringCellValue, typeof(string));
                }
                for (int i = 1; i <= sheet.LastRowNum; i++)
                {
                    DataRow newRow = dt.Rows.Add();
                    for (int j = firstCellNum; j < lastCellNum; j++)
                    {
                        var cell = sheet.GetRow(i).GetCell(j);
                        if (cell == null) continue;
                        newRow[j] = GetCellValue(cell);
                    }
                }

                ds.Tables.Add(dt);
            }
            return ds;
        }

        public List<List<RowValue>> GetWorksheetRowListData(int sheetIndex)
        {
            try
            {
                var sheet = Workbook.GetSheetAt(sheetIndex);
                if (sheet == null)
                    return null;
                var result = new List<List<RowValue>>();
                var headerRow = sheet.GetRow(0);
                var cellCount = headerRow.LastCellNum;
                var maxRow = sheet.LastRowNum;
                for (var i = 0; i <= maxRow; i++)
                {
                    var row = sheet.GetRow(i);
                    var emptyRow = true;
                    if (row == null) continue;

                    cellCount = cellCount != 0 ? cellCount : row.LastCellNum;
                    var columns = new List<RowValue>();
                    for (int j = 0; j < cellCount; j++)
                    {
                        var cell = row.GetCell(j);

                        var alphabet = IndexToColumn(j + 1);
                        var headerCell = headerRow.GetCell(j);
                        var headerValue = GetCellValue(headerCell);
                        var columnName = !string.IsNullOrEmpty(headerValue)
                            ? headerValue
                            : i.ToString();
                        var cellValue = string.Empty;
                        var cellColor = string.Empty;
                        if (cell != null)
                        {
                            cellValue = GetCellValue(cell);
                            cellColor = GetCellColor(cell);
                        };
                        if (cellValue != null && !string.IsNullOrEmpty(cellValue.Trim()))
                        {
                            emptyRow = false;
                        }
                        var rowValue = new RowValue
                        {
                            RowId = i,
                            ColumnId = j,
                            CellName = alphabet,
                            ColumnName = columnName,
                            CellValue = cellValue,
                            CellColor = cellColor,
                        };
                        columns.Add(rowValue);
                    }
                    if (!emptyRow)
                    {
                        result.Add(columns);
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                if (Workbook != null)
                {
                    Workbook.Close();
                    Workbook = null;
                }
                LogUtil.LogError("Couldn't get the row's List<RowValue> data by sheet name.", ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
            }
        }

        const int ColumnBase = 26;
        const int DigitMax = 7; // ceil(log26(Int32.Max))
        const string Digits = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private string IndexToColumn(int index)
        {
            if (index <= 0)
                throw new IndexOutOfRangeException("index must be a positive number");

            if (index <= ColumnBase)
                return Digits[index - 1].ToString();

            var sb = new StringBuilder().Append(' ', DigitMax);
            var current = index;
            var offset = DigitMax;
            while (current > 0)
            {
                sb[--offset] = Digits[--current % ColumnBase];
                current /= ColumnBase;
            }
            return sb.ToString(offset, DigitMax - offset);
        }
        #endregion

        /// <summary>
        /// Excel文件导成Datatable
        /// </summary>
        /// <param name="workSheetName">表单Sheet的名称，如果为空，则获取索引为0的第一个表单</param>
        /// <param name="cellCount">获取数据的最大表单列数（Colums）限制</param>
        /// <returns></returns>
        public DataTable ImportToDataTable(int cellCount = 0, string workSheetName = "Sheet1")
        {
            try
            {
                var dt = new DataTable();
                dt.TableName = !string.IsNullOrEmpty(workSheetName)
                    ? workSheetName
                    : "Sheet1";

                var sheet = Workbook.GetSheet(workSheetName)
                            ?? Workbook.GetSheetAt(0);
                if (sheet == null)
                    return null;

                var index = 0;
                //列头
                foreach (ICell item in sheet.GetRow(sheet.FirstRowNum).Cells)
                {
                    index++;
                    if (index > cellCount)
                    {
                        break;
                    }
                    dt.Columns.Add(item.ToString(), typeof(string));
                }

                //写入内容
                System.Collections.IEnumerator rows = sheet.GetRowEnumerator();
                while (rows.MoveNext())
                {
                    IRow row;
                    if (base.IsXlsx)
                    {
                        row = (XSSFRow)rows.Current;
                    }
                    else
                    {
                        row = (HSSFRow)rows.Current;
                    }
                    if (row.RowNum == sheet.FirstRowNum)
                    {
                        continue;
                    }
                    var i = 0;
                    DataRow dr = dt.NewRow();
                    var valCollection = string.Empty;
                    foreach (ICell item in row.Cells)
                    {
                        i++;
                        if (i > cellCount)
                            break;

                        var cellValue = GetCellValue(item);
                        dr[item.ColumnIndex] = cellValue;
                        valCollection += cellValue;

                    }
                    if (!string.IsNullOrWhiteSpace(valCollection))
                        dt.Rows.Add(dr);
                }

                return dt;
            }
            catch (Exception ex)
            {
                if (Workbook != null)
                {
                    Workbook.Close();
                    Workbook = null;
                }
                LogUtil.LogError("Couldn't Import To DataTable by Excel Stream.", ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
            }
        }

        private string GetCellValue(ICell item)
        {
            if (item == null)
                return null;
            string result;
            switch (item.CellType)
            {
                case CellType.Boolean:
                    result = item.BooleanCellValue.ToString();
                    break;
                case CellType.Numeric:
                    result = GetDateStringFromNumbric(item);
                    break;
                case CellType.String:
                    string strValue = item.StringCellValue;
                    result = !string.IsNullOrEmpty(strValue) ? strValue : null;
                    break;
                case CellType.Error:
                    result = ErrorEval.GetText(item.ErrorCellValue);
                    break;
                case CellType.Formula:
                    switch (item.CachedFormulaResultType)
                    {
                        case CellType.Boolean:
                            result = item.BooleanCellValue.ToString();
                            break;
                        case CellType.Numeric:
                            result = GetDateStringFromNumbric(item);
                            break;
                        case CellType.String:
                            string str = item.StringCellValue;
                            result = !string.IsNullOrEmpty(str) ? str : null;
                            break;
                        case CellType.Error:
                            result = ErrorEval.GetText(item.ErrorCellValue);
                            break;
                        case CellType.Unknown:
                        case CellType.Blank:
                        default:
                            result = string.Empty;
                            break;
                    }
                    break;
                case CellType.Unknown:
                case CellType.Blank:
                default:
                    result = string.Empty;
                    break;
            }

            return result;
        }

        private string GetDateStringFromNumbric(ICell cell)
        {
            ICellStyle format = cell.CellStyle;
            string result = getDateValue(format.DataFormat, format.GetDataFormatString(), cell.NumericCellValue);
            if (result == null)
            {
                result = cell.NumericCellValue.ToString();
            }
            return result;
        }

        /// <summary>
        /// POI日期格式转换
        /// </summary>
        /// <param name="dataFormat">日期格式：https://blog.csdn.net/phoenixx123/article/details/12720431 </param>
        /// <param name="dataFormatString"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected string getDateValue(int dataFormat, string dataFormatString, double value)
        {
            if (!DateUtil.IsValidExcelDate(value))
            {
                return null;
            }

            DateTime date = DateUtil.GetJavaDate(value);
            /**
             * 年月日时分秒
             */
            if (EXCEL_FORMAT_INDEX_DATE_NYRSFM_STRING.Contains(dataFormatString))
            {
                return COMMON_DATE_FORMAT.Format(date);
            }
            /**
             * 年月日
             */
            if (EXCEL_FORMAT_INDEX_07_DATE.Contains(dataFormat) 
                || EXCEL_FORMAT_INDEX_03_DATE.Contains(dataFormat) 
                || EXCEL_FORMAT_INDEX_DATE_NYR_STRING.Contains(dataFormatString))
            {
                return COMMON_DATE_FORMAT_NYR.Format(date);
            }
            /**
             * 年月
             */
            if (EXCEL_FORMAT_INDEX_DATA_EXACT_NY.Equals(dataFormat) ||EXCEL_FORMAT_INDEX_DATE_NY_STRING.Contains(dataFormatString))
            {
                return COMMON_DATE_FORMAT_NY.Format(date);
            }
            /**
             * 月日
             */
            if (EXCEL_FORMAT_INDEX_DATE_YR_STRING.Contains(dataFormatString) || EXCEL_FORMAT_INDEX_DATA_EXACT_YR.Equals(dataFormat))
            {
                return COMMON_DATE_FORMAT_YR.Format(date);

            }
            /**
             * 月
             */
            if (EXCEL_FORMAT_INDEX_DATE_Y_STRING.Contains(dataFormatString))
            {
                return COMMON_DATE_FORMAT_Y.Format(date);
            }
            /**
             * 星期X
             */
            if (EXCEL_FORMAT_INDEX_DATE_XQ_STRING.Contains(dataFormatString))
            {
                return COMMON_DATE_FORMAT_XQ + dateToWeek(date);
            }
            /**
             * 周X
             */
            if (EXCEL_FORMAT_INDEX_DATE_Z_STRING.Contains(dataFormatString))
            {
                return COMMON_DATE_FORMAT_Z + dateToWeek(date);
            }
            /**
             * 时间格式
             */
            if (EXCEL_FORMAT_INDEX_07_TIME.Contains(dataFormat)
                || EXCEL_FORMAT_INDEX_03_TIME.Contains(dataFormat)
                || EXCEL_FORMAT_INDEX_TIME_STRING.Contains(dataFormatString) 
                || EXCEL_FORMAT_INDEX_TIME_EXACT.Contains(dataFormat))
            {
                return COMMON_TIME_FORMAT.Format(DateUtil.GetJavaDate(value));
            }
            /**
             * 单元格为其他未覆盖到的类型
             */
            if (DateUtil.IsADateFormat(dataFormat, dataFormatString))
            {
                return COMMON_TIME_FORMAT.Format(value);
            }

            return null;
        }

        private string GetCellColor(ICell cell)
        {
            if (cell == null)
                return null;
            ICellStyle cellStyle = cell.CellStyle;
            if(cellStyle is XSSFCellStyle)
            {
                //单元格样式
                var xs = (XSSFCellStyle)cellStyle;
                //单元格背景颜色
                var color = xs.FillForegroundXSSFColor;
                if (color != null)
                    return color.GetARGBHex();
            }
            else if(cellStyle is HSSFCellStyle)
            {
                //单元格样式
                var xs = (HSSFCellStyle)cellStyle;
                //单元格背景颜色
                NPOI.HSSF.Util.HSSFColor color = (NPOI.HSSF.Util.HSSFColor)xs.FillForegroundColorColor;
                if (color != null)
                    return color.GetHexString();
            }

            return null;
        }
    }
    public class NPOIExcelWriter : AExcelBase
    {
        public int MaxWidth = 40;
        public NPOIExcelWriter(bool isXlsx = true)
            : base(isXlsx)
        {
        }
        public NPOIExcelWriter(byte[] bytes)
            : base(bytes)
        {
        }
        public NPOIExcelWriter(Stream stream)
            : base(stream)
        {
        }

        #region NPOI Writer

        /// <summary>
        /// 设置自定义单元格的值
        /// </summary>
        /// <param name="definedName"></param>
        /// <param name="value"></param>
        public void SetDefinedNameSingleValue(string definedName, string value)
        {
            try
            {
                var name = Workbook.GetName(definedName);
                if (string.IsNullOrEmpty(definedName)
                    || name == null)
                    throw new Exception("Missing defined name " + definedName);

                var sheetName = name.SheetName;
                var formula = name.RefersToFormula;

                var sheet = Workbook.GetSheet(sheetName);
                var cr = new CellReference(formula);
                var row = sheet.GetRow(cr.Row);
                var cell = row.GetCell(cr.Col);
                cell.SetCellValue(value);
            }
            catch (Exception ex)
            {
                if (Workbook != null)
                {
                    Workbook.Close();
                    Workbook = null;
                }
                LogUtil.LogError("Couldn't set the single value by using defined name.", ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }
        /// <summary>
        /// 设置某个单元格（例如：A1）的值
        /// </summary>
        /// <param name="cellName">单元格的名称（例如：A1）</param>
        /// <param name="cellValue">单元格的值</param>
        /// <param name="style">单元格样式</param>
        /// <param name="workSheetName">Sheet的名称</param>
        public void SetCellValueByName(string cellName, string cellValue, object style = null, string workSheetName = "Sheet1")
        {
            try
            {
                if (string.IsNullOrWhiteSpace(workSheetName))
                    workSheetName = "Sheet1";

                var sheet = Workbook.GetSheet(workSheetName)
                    ?? Workbook.CreateSheet(workSheetName);

                var cr = new CellReference(cellName);
                if (string.IsNullOrEmpty(cellName) || cr.Row < 0 || cr.Col < 0)
                    throw new Exception("Cannot process excel: The excel sheet's cell is empty or wrong.");
                ICellStyle cellStyle = null;
                if (style == null)
                {
                    cellStyle = Workbook.CreateCellStyle();
                    cellStyle.WrapText = true;
                }
                else
                {
                    cellStyle = style as ICellStyle;
                }
                var row = sheet.GetRow(cr.Row)
                    ?? sheet.CreateRow(cr.Row);
                var cell = row.GetCell(cr.Col)
                    ?? row.CreateCell(cr.Col);
                cell.CellStyle = cellStyle;
                cell.SetCellValue(cellValue);
                var length = GetLength(cellValue);
                var width = length <= MaxWidth ? length * 256 : MaxWidth * 256;
                if (sheet.GetColumnWidth(cr.Col) < width)
                    sheet.SetColumnWidth(cr.Col, width);
            }
            catch (Exception ex)
            {
                if (Workbook != null)
                {
                    Workbook.Close();
                    Workbook = null;
                }
                LogUtil.LogError("Couldn't set the cell's value by using cell's name.", ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }
        /// <summary>
        /// 设置某个Sheet表单（例如：Sheet1）的值
        /// </summary>
        /// <param name="valueList">
        /// 设置Sheet表单的值列表：
        ///    var data = new List List<string/>>()
        ///    {
        ///        new List<string/>() {"Column1","Column2","Column3","Column4"},
        ///        new List<string/>() {"R11","R12","R13","R14"},
        ///        new List<string/>() {"R21","R22","R23","R24"},
        ///        new List<string/>() {"R31","R32","R33","R34"},
        ///    };
        /// </param>
        /// <param name="workSheetName">Sheet的名称</param>
        public void SetWorksheetDataRaw(List<List<string>> valueList, string workSheetName = "Sheet1")
        {
            try
            {
                if (string.IsNullOrWhiteSpace(workSheetName))
                    workSheetName = "Sheet1";

                var sheet = Workbook.GetSheet(workSheetName)
                    ?? Workbook.CreateSheet(workSheetName);

                int i = 0;
                foreach (var row in valueList)
                {
                    int j = 0;
                    IRow dataRow = sheet.CreateRow(i);
                    foreach (var column in row)
                    {
                        ICell newCell = dataRow.CreateCell(j);
                        newCell.SetCellValue(column);
                        j++;
                    }
                    i++;
                }
            }
            catch (Exception ex)
            {
                if (Workbook != null)
                {
                    Workbook.Close();
                    Workbook = null;
                }
                LogUtil.LogError("Couldn't set the row data by List<List<string>> object data.", ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }


        public void SetWorksheetDataRawAndWidth(List<List<string>> valueList, string workSheetName = "Sheet1", int startRow = 0, int startColumn = 0, object style = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(workSheetName))
                    workSheetName = "Sheet1";
                var sheet = Workbook.GetSheet(workSheetName)
                    ?? Workbook.CreateSheet(workSheetName);
                var columnWidths = new List<int>();
                for (var c = 0; c < valueList[0].Count; c++)
                    columnWidths.Add(0);
                ICellStyle cellStyle = null;
                if (style == null)
                {
                    cellStyle = Workbook.CreateCellStyle();
                    cellStyle.WrapText = true;
                }
                else
                {
                    cellStyle = style as ICellStyle;
                }
                foreach (var row in valueList)
                {
                    int j = startColumn;
                    IRow dataRow = sheet.CreateRow(startRow);
                    foreach (var column in row)
                    {
                        ICell newCell = dataRow.CreateCell(j);
                        newCell.SetCellValue(column);
                        newCell.CellStyle = cellStyle;
                        var length = GetLength(column);
                        if (columnWidths[j] < length)
                            columnWidths[j] = length;
                        j++;
                    }
                    startRow++;
                }

                for (var c = 0; c < columnWidths.Count; c++)
                {
                    var width = columnWidths[c] <= MaxWidth ? columnWidths[c] * 256 : MaxWidth * 256;
                    if (width > sheet.GetColumnWidth(c))
                        sheet.SetColumnWidth(c, width);
                }
            }
            catch (Exception ex)
            {
                if (Workbook != null)
                {
                    Workbook.Close();
                    Workbook = null;
                }
                LogUtil.LogError("Couldn't set the row data by List<List<string>> object data.", ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }
        /// <summary>
        /// 设置某个Sheet表单（例如：Sheet1）的值
        /// </summary>
        /// <param name="objList">
        /// 设置Sheet表单的值列表：
        /// </param>
        /// <param name="workSheetName">Sheet的名称</param>
        public void SetWorksheetObject<T>(List<T> objList, string workSheetName = "Sheet1") where T : class
        {
            try
            {
                if (string.IsNullOrWhiteSpace(workSheetName))
                    workSheetName = "Sheet1";

                var sheet = Workbook.GetSheet(workSheetName)
                    ?? Workbook.CreateSheet(workSheetName);

                ICellStyle dateStyle = Workbook.CreateCellStyle();
                dateStyle.BorderTop = BorderStyle.Thin;
                dateStyle.BorderRight = BorderStyle.Thin;
                dateStyle.BorderLeft = BorderStyle.Thin;
                dateStyle.BorderBottom = BorderStyle.Thin;
                IDataFormat format = Workbook.CreateDataFormat();
                dateStyle.DataFormat = format.GetFormat("yyyy-mm-dd");

                int i = 0;
                Type t = typeof(T);
                PropertyInfo[] properties = t.GetProperties();
                foreach (var obj in objList)
                {
                    int j = 0;
                    IRow dataRow = sheet.CreateRow(i);
                    foreach (var property in properties)
                    {
                        var drValue = property.GetValue(obj).ToString();
                        ICell newCell = dataRow.CreateCell(j);
                        var typeString = property.PropertyType;
                        switch (typeString.ToString())
                        {
                            case "System.String"://字符串类型
                                newCell.SetCellValue(drValue);
                                break;
                            case "System.DateTime"://日期类型
                                DateTime dateV;
                                DateTime.TryParse(drValue, out dateV);
                                newCell.SetCellValue(dateV);

                                newCell.CellStyle = dateStyle;//格式化显示
                                break;
                            case "System.Boolean"://布尔型
                                bool boolV = false;
                                bool.TryParse(drValue, out boolV);
                                newCell.SetCellValue(boolV);
                                break;
                            case "System.Int16"://整型
                            case "System.Int32":
                            case "System.Int64":
                            case "System.Byte":
                                int intV = 0;
                                int.TryParse(drValue, out intV);
                                newCell.SetCellValue(intV);
                                break;
                            case "System.Decimal"://浮点型
                            case "System.Double":
                                double doubV = 0;
                                double.TryParse(drValue, out doubV);
                                newCell.SetCellValue(doubV);
                                break;
                            case "System.DBNull"://空值处理
                                newCell.SetCellValue("");
                                break;
                            default:
                                newCell.SetCellValue("");
                                break;
                        }

                        j++;
                    }
                    i++;
                }
            }
            catch (Exception ex)
            {
                if (Workbook != null)
                {
                    Workbook.Close();
                    Workbook = null;
                }
                LogUtil.LogError("Couldn't set the row data by List<T> object list data.", ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }

        public void MergedRegion(int startRow, int endRow, int startColumn, int endColumn, string workSheetName = "Sheet1")
        {
            if (string.IsNullOrWhiteSpace(workSheetName))
                workSheetName = "Sheet1";

            var sheet = Workbook.GetSheet(workSheetName)
                ?? Workbook.CreateSheet(workSheetName);
            sheet.AddMergedRegion(new CellRangeAddress(startRow, endRow, startColumn, endColumn));
        }
        #endregion

        /// <summary>
        /// 将DataTable中的数据，导出到Excel
        /// </summary>
        /// <param name="dtSource"></param>
        /// <param name="strHeaderText"></param>
        /// <param name="workSheetName"></param>
        /// <param name="oldColumnNames"></param>
        /// <param name="newColumnNames"></param>
        public NpoiMemoryStream ExportToExcelStream(DataTable dtSource, string workSheetName = "Sheet1",
            string strHeaderText = "表头", string[] oldColumnNames = null, string[] newColumnNames = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(workSheetName))
                    workSheetName = "Sheet1";

                var sheet = Workbook.GetSheet(workSheetName)
                            ?? Workbook.CreateSheet(workSheetName);

                #region 获取Excel数据

                #region 右击文件 属性信息

                //{
                //    DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
                //    dsi.Company = "http://....../";
                //    Workbook.DocumentSummaryInformation = dsi;

                //    SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
                //    if (HttpContext.Current.Session["realname"] != null)
                //    {
                //        si.Author = HttpContext.Current.Session["realname"].ToString();
                //    }
                //    else
                //    {
                //        if (HttpContext.Current.Session["username"] != null)
                //        {
                //            si.Author = HttpContext.Current.Session["username"].ToString();
                //        }
                //    }                                       
                //    si.ApplicationName = "导出的excel";                
                //    si.LastAuthor = "";             
                //    si.Comments = "";           
                //    si.Title = strHeaderText;                   
                //    si.Subject = strHeaderText;                  
                //    si.CreateDateTime = DateTime.Now;
                //    Workbook.SummaryInformation = si;
                //}

                #endregion

                ICellStyle dateStyle = Workbook.CreateCellStyle();
                IDataFormat format = Workbook.CreateDataFormat();
                dateStyle.DataFormat = format.GetFormat("yyyy-mm-dd");

                if (oldColumnNames == null)
                {
                    oldColumnNames = new string[dtSource.Columns.Count];
                    foreach (DataColumn item in dtSource.Columns)
                    {
                        oldColumnNames[item.Ordinal] = item.ColumnName;
                    }
                }
                if (newColumnNames == null)
                {
                    newColumnNames = new string[oldColumnNames.Length];
                    oldColumnNames.CopyTo(newColumnNames, 0);
                }

                #region 取得列宽

                int[] arrColWidth = new int[oldColumnNames.Length];
                for (int i = 0; i < oldColumnNames.Length; i++)
                {
                    arrColWidth[i] = Encoding.GetEncoding(936).GetBytes(newColumnNames[i]).Length;
                }

                for (int i = 0; i < dtSource.Rows.Count; i++)
                {
                    for (int j = 0; j < oldColumnNames.Length; j++)
                    {
                        int intTemp =
                            Encoding.GetEncoding(936).GetBytes(dtSource.Rows[i][oldColumnNames[j]].ToString()).Length;
                        if (intTemp > arrColWidth[j])
                        {
                            arrColWidth[j] = intTemp;
                        }
                    }
                }

                #endregion

                int rowIndex = 0;
                foreach (DataRow row in dtSource.Rows)
                {
                    #region 新建表，填充表头，填充列头，样式

                    if (rowIndex == 65535 || rowIndex == 0)
                    {
                        if (rowIndex != 0)
                        {
                            sheet = Workbook.CreateSheet(workSheetName + ((int)rowIndex / 65535).ToString());
                        }


                        #region 列头及样式

                        IRow headerRow = sheet.CreateRow(0);

                        ICellStyle headStyle = Workbook.CreateCellStyle();
                        headStyle.Alignment = HorizontalAlignment.Center;
                        IFont font = Workbook.CreateFont();
                        font.FontHeightInPoints = 10;
                        font.Boldweight = 700;
                        headStyle.SetFont(font);

                        for (int i = 0; i < oldColumnNames.Length; i++)
                        {
                            headerRow.CreateCell(i).SetCellValue(newColumnNames[i]);
                            headerRow.GetCell(i).CellStyle = headStyle;
                            //设置列宽   
                            sheet.SetColumnWidth(i, (arrColWidth[i] + 1) * 256);
                        }

                        #endregion

                        rowIndex = 1;
                    }

                    #endregion

                    #region 填充内容

                    IRow dataRow = sheet.CreateRow(rowIndex);
                    for (int i = 0; i < oldColumnNames.Length; i++)
                    {
                        ICell newCell = dataRow.CreateCell(i);

                        string drValue = row[oldColumnNames[i]].ToString();

                        switch (dtSource.Columns[oldColumnNames[i]].DataType.ToString())
                        {
                            case "System.String": //字符串类型      
                                newCell.SetCellValue(drValue);
                                break;
                            case "System.DateTime": //日期类型      
                                DateTime dateV;
                                DateTime.TryParse(drValue, out dateV);
                                newCell.SetCellValue(dateV);

                                newCell.CellStyle = dateStyle; //格式化显示      
                                break;
                            case "System.Boolean": //布尔型      
                                bool boolV = false;
                                bool.TryParse(drValue, out boolV);
                                newCell.SetCellValue(boolV);
                                break;
                            case "System.Int16": //整型      
                            case "System.Int32":
                            case "System.Int64":
                            case "System.Byte":
                                int intV = 0;
                                int.TryParse(drValue, out intV);
                                newCell.SetCellValue(intV);
                                break;
                            case "System.Decimal": //浮点型      
                            case "System.Double":
                                double doubV = 0;
                                double.TryParse(drValue, out doubV);
                                newCell.SetCellValue(doubV);
                                break;
                            case "System.DBNull": //空值处理      
                                newCell.SetCellValue("");
                                break;
                            default:
                                newCell.SetCellValue("");
                                break;
                        }

                    }

                    #endregion

                    rowIndex++;
                }

                #endregion

                var ms = new NpoiMemoryStream();
                ms.AllowClose = false;
                Workbook.Write(ms);
                ms.Flush();
                ms.Position = 0;
                ms.AllowClose = true;

                return ms;
            }
            catch (Exception ex)
            {
                if (Workbook != null)
                {
                    Workbook.Close();
                    Workbook = null;
                }
                LogUtil.LogError("Couldn't Export To Excel Stream by DataTable.", ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
            }
        }

        /// <summary>
        /// 将表单保存为Excel文件（OpenXml的格式--Excel2010以后的版本）
        /// </summary>
        /// <param name="filePath">保存Excel文件路径</param>
        public void SaveAsExcel(string filePath)
        {
            try
            {
                using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    Workbook.Write(fs);
                }
            }
            catch (Exception ex)
            {
                if (Workbook != null)
                {
                    Workbook.Close();
                    Workbook = null;
                }
                LogUtil.LogException(ex);
            }
        }

        public int GetLength(string value)
        {
            var wrapNumber = Regex.Matches(value, @"\n").Count;
            int length = 0;
            if (wrapNumber == 0)
                return Encoding.UTF8.GetBytes(value).Length + Regex.Matches(value, @"\d").Count * 1;
            var strList = value.Split('\n').ToList();
            strList.ForEach(m =>
            {
                var item = Encoding.UTF8.GetBytes(m).Length + Regex.Matches(m, @"\d").Count * 1;
                if (item > length)
                    length = item;
            });
            return length;
        }

        public object SetCellStyle(int fontHeight, bool alignmenCenter = false, bool diagonalLine = false)
        {
            var style = Workbook.CreateCellStyle();
            var font = Workbook.CreateFont();
            font.FontHeight = fontHeight;
            font.FontName = "宋体";
            font.Boldweight = (short)FontBoldWeight.Bold;
            style.BorderTop = BorderStyle.Thin;
            style.BorderRight = BorderStyle.Thin;
            style.BorderLeft = BorderStyle.Thin;
            style.BorderBottom = BorderStyle.Thin;
            if (diagonalLine)
            {
                style.BorderDiagonalLineStyle = BorderStyle.Thin;
                style.BorderDiagonal = BorderDiagonal.Backward;
            }
            else
            {
                style.BorderDiagonalLineStyle = BorderStyle.None;
                style.BorderDiagonal = BorderDiagonal.None;
            }
            if (alignmenCenter)
                style.Alignment = HorizontalAlignment.Center;
            style.VerticalAlignment = VerticalAlignment.Center;
            style.WrapText = true;
            style.SetFont(font);
            return style;
        }



    }

    public class RowValue
    {
        public int RowId { get; set; }
        public int ColumnId { get; set; }
        public string CellName { get; set; }
        public string ColumnName { get; set; }
        public string CellValue { get; set; }
        public string CellColor { get; set; }
    }

    //新建类 重写Npoi流方法
    public class NpoiMemoryStream : MemoryStream
    {
        public NpoiMemoryStream()
        {
            AllowClose = true;
        }

        public bool AllowClose { get; set; }

        public override void Close()
        {
            if (AllowClose)
                base.Close();
        }
    }
}
