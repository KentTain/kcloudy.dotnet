using System;
using System.Collections.Generic;
using System.IO;
using KC.Framework.Extension;
using KC.ThirdParty;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

namespace KC.UnitTest.ThirdParty.UnitTest
{
    
    public class NPOIHelper_Test : KC.UnitTest.Core.ThirdPartyTestBase
    {
        private ILogger _logger;

        public NPOIHelper_Test(CommonFixture data, ITestOutputHelper tempOutput)
            : base(data, tempOutput)
        {
            _logger = LoggerFactory.CreateLogger(nameof(NPOIHelper_Test));
        }

        #region NPOI

        [Xunit.Fact]
        public void NPOI_ExcelReader_Test()
        {
            var excelPath = @"Excel\ExcelReaderTest.xlsx";
            var sheet1 = "Sheet1";

            var bytes = File.ReadAllBytes(excelPath);
            using (var er = new NPOIExcelReader(bytes))
            {
                var definedNames = er.GetDefinedNames();
                var definedValue = er.GetDefinedNameSingleValue("Title");
                Assert.Equal("Test Title", definedValue);

                var cellValue = er.GetCellValueByName(sheet1, "B3");
                var cellValue1 = er.GetCellValueByName(sheet1, "E4");
                Assert.Equal("B3", cellValue);
                Assert.Equal("E4", cellValue1);

                //根据Excel日期格式转换对应的String字符串
                var dateValue = er.GetCellValueByName(sheet1, "B5");//2013/07/19（实际：2013/07/19）
                var dateValue1 = er.GetCellValueByName(sheet1, "C5");//2013年07月19日（实际：2013/07/19）
                var dateValue2 = er.GetCellValueByName(sheet1, "D5");//2013-07-19（实际：2013/07/19）
                var dateValue3 = er.GetCellValueByName(sheet1, "E5");//11:00:00（实际：2013/07/19  11:00:00）
                var dateValue4 = er.GetCellValueByName(sheet1, "F5");//2013年7月（实际：2013/07/19）
                Assert.Equal("2013-07-19", dateValue);
                Assert.Equal("2013-07-19", dateValue1);
                Assert.Equal("2013-07-19", dateValue2);
                Assert.Equal("11:00:00", dateValue3);
                Assert.Equal("2013-07-19", dateValue4);

                var cellResult = er.GetWorksheetDictData(sheet1, 10);
            }

            excelPath = @"Excel\ExcelReaderTest.xls";
            bytes = File.ReadAllBytes(excelPath);
            using (var er = new NPOIExcelReader(bytes))
            {
                var definedNames = er.GetDefinedNames();
                var definedValue = er.GetDefinedNameSingleValue("Title");
                Assert.Equal("Test Title", definedValue);

                var cellValue = er.GetCellValueByName(sheet1, "B3");
                var cellValue1 = er.GetCellValueByName(sheet1, "E4");
                Assert.Equal("B3", cellValue);
                Assert.Equal("E4", cellValue1);

                //根据Excel日期格式转换对应的String字符串
                var dateValue = er.GetCellValueByName(sheet1, "B5");//2013/07/19（实际：2013/07/19）
                var dateValue1 = er.GetCellValueByName(sheet1, "C5");//2013年07月19日（实际：2013/07/19）
                var dateValue2 = er.GetCellValueByName(sheet1, "D5");//2013-07-19（实际：2013/07/19）
                var dateValue3 = er.GetCellValueByName(sheet1, "E5");//11:00:00（实际：2013/07/19 11:00:00）
                var dateValue4 = er.GetCellValueByName(sheet1, "F5");//2013年7月（实际：2013/07/19）
                Assert.Equal("2013-07-19", dateValue);
                Assert.Equal("2013-07-19", dateValue1);
                Assert.Equal("2013-07-19", dateValue2);
                Assert.Equal("11:00:00", dateValue3);
                Assert.Equal("2013-07-19", dateValue4);

                var cellResult = er.GetWorksheetDictData(sheet1, 10);
            }
        }

        [Xunit.Fact]
        public void NPOI_ExcelWriter_Test()
        {
            #region Data
            var sheet1 = "Sheet1";
            var sheet2 = "Sheet2";
            var sheet3 = "Sheet3";
            var excelPath = @"Excel\ExcelWriterTest.xlsx";
            var saveFilePath = excelPath.Replace("ExcelWriterTest", "ExcelWriterTest-0-" + DateTime.UtcNow.ToString("yyyyMMdd-hhmm"));
            var saveFilePath1 = excelPath.Replace("ExcelWriterTest", "ExcelWriterTest-1-" + DateTime.UtcNow.ToString("yyyyMMdd-hhmm"));

            var dataList = new List<List<string>>()
            {
                new List<string>() {"Column1","Column2","Column3","Column4"},
                new List<string>() {"R11","R12","R13","R14"},
                new List<string>() {"R21","R22","R23","R24"},
                new List<string>() {"R31","R32","R33","R34"},
            };

            var objList = new List<TestObj>()
            {
                new TestObj() {Id = 1, Name = "Name1", Birthday = DateTime.UtcNow, Price = 12.1M, Sex = false},
                new TestObj() {Id = 2, Name = "Name2", Birthday = DateTime.UtcNow.AddYears(1), Price = 22.1M, Sex = true},
                new TestObj() {Id = 3, Name = "Name3", Birthday = DateTime.UtcNow.AddYears(2), Price = 32.1M, Sex = false}
            };
            #endregion

            #region 先读Excel文件，再向该Excel写入数据
            var bytes = File.ReadAllBytes(excelPath);
            using (var er = new NPOIExcelWriter(bytes))
            {
                er.SetDefinedNameSingleValue("Title", "Test Title");

                er.SetCellValueByName("B3", "B3");
                er.SetCellValueByName("E4", "E4");

                er.SetWorksheetDataRaw(dataList, sheet2);
                er.SetWorksheetObject(objList, sheet3);

                er.SaveAsExcel(saveFilePath);
            }

            bytes = File.ReadAllBytes(saveFilePath);
            using (var er = new NPOIExcelReader(bytes))
            {
                var definedNames = er.GetDefinedNames();
                var definedValue = er.GetDefinedNameSingleValue("Title");
                Assert.Equal("Test Title", definedValue);

                var cellValue = er.GetCellValueByName(sheet1, "B3");
                var cellValue1 = er.GetCellValueByName(sheet1, "E4");
                Assert.Equal("B3", cellValue);
                Assert.Equal("E4", cellValue1);

                var cellResult = er.GetWorksheetDictData(sheet1, 10, 3);
                var cellResult2 = er.GetWorksheetDictData(sheet2, 10, 1);
            }
            #endregion

            #region 生成一个空的Excel文件，再写入数据
            using (var er = new NPOIExcelWriter())
            {
                //所生成的空Excel文件，不包含自定义单元格，所有不能用该方法
                //er.SetDefinedNameSingleValue("Title", "Test Title");

                er.SetCellValueByName("B3", "B3");
                er.SetCellValueByName("E4", "E4");

                er.SetWorksheetDataRaw(dataList, sheet2);
                er.SetWorksheetObject(objList, sheet3);

                er.SaveAsExcel(saveFilePath1);
            }
            bytes = File.ReadAllBytes(saveFilePath);
            using (var er = new NPOIExcelReader(bytes))
            {
                var definedNames = er.GetDefinedNames();
                //所生成的空Excel文件，不包含自定义单元格，所有不能用该方法
                //var definedValue = er.GetDefinedNameSingleValue("Title");
                //Assert.Equal("Test Title", definedValue);

                var cellValue = er.GetCellValueByName(sheet1, "B3");
                var cellValue1 = er.GetCellValueByName(sheet1, "E4");
                Assert.Equal("B3", cellValue);
                Assert.Equal("E4", cellValue1);

                var cellResult = er.GetWorksheetDictData(sheet1, 10, 3);
                var cellResult2 = er.GetWorksheetDictData(sheet2, 10, 1);
            }
            #endregion

        }

        [Xunit.Fact]
        public void NPOI_ExcelImportFromFile_Test()
        {
            var excelPath = @"Excel\poolImportTemplate.xlsx";
            var singleSheetName = "单选题";

            var bytes = File.ReadAllBytes(excelPath);
            using (var er = new NPOIExcelReader(bytes))
            {
                List<List<RowValue>> rows = er.GetWorksheetRowListData(singleSheetName, 10, 1);

                var colum = rows[0][2];
                Assert.True(!colum.CellColor.IsNullOrEmpty());
                foreach (List<RowValue> columns in rows)
                {
                    if (columns.Count <= 0)
                        continue;
                    foreach (RowValue column in columns)
                    {
                        if (column == null || column.CellValue.IsNullOrEmpty())
                            continue;

                        var msg = string.Format("-----Row name: {0}, value: {1}, color: {2}", column.CellName + column.RowId, column.CellValue, column.CellColor);
                        _logger.LogInformation(msg);
                        Output.WriteLine(msg);
                    }
                }
            }
        }

        #endregion
    }

    public class TestObj
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Sex { get; set; }
        public DateTime Birthday { get; set; }
        public decimal Price { get; set; }
    }
}
