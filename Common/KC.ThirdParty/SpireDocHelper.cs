using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Spire.Doc;
using Spire.Doc.Documents;
using Spire.Doc.Fields;

namespace Com.ThirdParty.Common
{
    public class SpireDocHelper
    {
        public SpireDocHelper()
        {
           Document=new Document();
            Section = Document.AddSection();
        }
        public Document Document { get; set; }
        public Section Section { get; set; }

        public SpireMemoryStream GetDateStream()
        {
            var ms = new SpireMemoryStream();
            ms.AllowClose = false;
            Document.SaveToStream(ms, new FileFormat());
            ms.Flush();
            ms.Position = 0;
            ms.AllowClose = true;
            return ms;
        }

        public SpireMemoryStream ConvertToPdfStream(string fileFormat, byte[] fileBuffer)
        {
            using (var stream = new MemoryStream(fileBuffer))
            {
                var doc = new Document(stream);
                var docFormat = FileFormat.Doc;
                Enum.TryParse(fileFormat, out docFormat);
                doc.LoadFromStream(stream, docFormat);
                
                var pdfStream = new SpireMemoryStream();
                pdfStream.AllowClose = false;
                Document.SaveToStream(pdfStream, Spire.Doc.FileFormat.PDF);
                pdfStream.Flush();
                pdfStream.Position = 0;
                pdfStream.AllowClose = true;
                return pdfStream;
            }
        }

        public void CreateTitle(string title)
        {
            var textproperty = new Textproperty();
            textproperty.Text = title;
            textproperty.FontName = "仿宋";
            textproperty.FontSize = 14;
            textproperty.horizontalAlignment = HorizontalAlignment.Center;
            textproperty.AfterSpacing = 25;
            textproperty.Bold = true;
            CreateText(Section, textproperty);
        }
        public void CreateParagraph(string text)
        {
            var textproperty = new Textproperty();
            textproperty.Text = text;
            textproperty.FontName = "仿宋";
            textproperty.FontSize = 12;
            textproperty.BeforeSpacing = 5;
            textproperty.AfterSpacing = 5;
            textproperty.LineSpacing = 15;
            CreateText(Section, textproperty);
        }
        public void CreateTableData(TableData<TableBody> tableData)
        {
            if (tableData.TablepropertyModel == null)
            {
                Tableproperty tableproperty = new Tableproperty();
                tableproperty.HeaderBold = true;
                tableproperty.HeaderHeight = 25;
                tableproperty.LeftIndent = 20;
                tableproperty.HorizontalAlignment = HorizontalAlignment.Left;
                tableproperty.CellVerticalAlignment = VerticalAlignment.Middle;
                tableData.TablepropertyModel = tableproperty;
            }
            CreateTable(Section, tableData.HeaderKey, tableData.HeaderValue, tableData.TablepropertyModel,
                tableData.ListModel);
        }
        public void CreateText(Section section, Textproperty textproperty)
        {
            Paragraph para = section.Body.AddParagraph();
            TextRange textRange = para.AppendText("");

            if (string.IsNullOrWhiteSpace(textproperty.FontName))
            {
                textRange.CharacterFormat.FontName = textproperty.FontName;
            }
            if (textproperty.FontSize != 0)
            {
                textRange.CharacterFormat.FontSize = textproperty.FontSize;
            }
            para.Format.SuppressAutoHyphens = true;

            if (!string.IsNullOrEmpty(textproperty.Text))
            {
                textRange = para.AppendText(textproperty.Text);
            }
            if (!string.IsNullOrEmpty(textproperty.FontName))
            {
                textRange.CharacterFormat.FontName = textproperty.FontName;
            }
            if (textproperty.FontSize != 0)
            {
                textRange.CharacterFormat.FontSize = textproperty.FontSize;
            }
            if (textproperty.horizontalAlignment != HorizontalAlignment.Left)
            {
                para.Format.HorizontalAlignment = textproperty.horizontalAlignment;
            }
            if (textproperty.LineSpacing != 0)
            {
                para.Format.LineSpacing = textproperty.LineSpacing;
            }
            if (textproperty.BeforeSpacing != 0)
            {
                para.Format.BeforeSpacing = textproperty.BeforeSpacing;
            }
            if (textproperty.AfterSpacing != 0)
            {
                para.Format.AfterSpacing = textproperty.AfterSpacing;
            }
            if (textproperty.FirstLineIndent != 0)
            {
                para.Format.FirstLineIndent = textproperty.FirstLineIndent;
            }
            if (textproperty.Bold != false)
            {
                textRange.CharacterFormat.Bold = textproperty.Bold;
            }
            if (textproperty.LeftIndent != 0)
            {
                para.Format.LeftIndent = textproperty.LeftIndent;
            }
            if (textproperty.RightIndent != 0)
            {
                para.Format.RightIndent = textproperty.RightIndent;
            }
            if (textproperty.UnderlineStyle != UnderlineStyle.None)
            {
                textRange.CharacterFormat.UnderlineStyle = textproperty.UnderlineStyle;
            }
        }
        public void CreateTable<T>(Section section, string[] headerKey, string[] headerValue, Tableproperty tablepropertyModel, List<T> listModel) where T : class
        {
            Table table = section.Body.AddTable(true);
            table.TableFormat.Borders.BorderType = BorderStyle.Single;
            table.ResetCells(listModel.Count + 1, headerValue.Length);
            TableRow row = table.Rows[0];
            row.IsHeader = true;
            if (tablepropertyModel.HeaderHeight != 0)
            {
                row.Height = tablepropertyModel.HeaderHeight;
            }
            for (int i = 0; i < headerValue.Length; i++)
            {
                row.Cells[i].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                Paragraph p = row.Cells[i].AddParagraph();
                if (tablepropertyModel.HorizontalAlignment != HorizontalAlignment.Left)
                {
                    p.Format.HorizontalAlignment = tablepropertyModel.HorizontalAlignment;
                }
                if (tablepropertyModel.LeftIndent != 0)
                {
                    p.Format.LeftIndent = tablepropertyModel.LeftIndent;//左边距
                }
                if (tablepropertyModel.RightIndent != 0)
                {
                    p.Format.RightIndent = tablepropertyModel.RightIndent;//右边距
                }
                TextRange textRange = p.AppendText(headerValue[i]);
                if (tablepropertyModel.HeaderBold)
                {
                    textRange.CharacterFormat.Bold = tablepropertyModel.HeaderBold;
                }
            }
            for (int r = 0; r < listModel.Count; r++)
            {
                TableRow dataRow = table.Rows[r + 1];
                if (tablepropertyModel.HeaderHeight != 0)
                {
                    dataRow.Height = tablepropertyModel.HeaderHeight;
                }
                dataRow.HeightType = TableRowHeightType.Exactly;
                if (tablepropertyModel.RowHeight != 0)
                {
                    dataRow.Height = tablepropertyModel.RowHeight;
                }
                Type modelType = listModel[r].GetType();
                PropertyInfo[] props = modelType.GetProperties();
                for (int i = 0; i < headerKey.Length; i++)
                {
                    foreach (var propertyInfo in props)
                    {
                        if (headerKey[i].Trim() == propertyInfo.Name.Trim())
                        {
                            if (tablepropertyModel.CellVerticalAlignment != VerticalAlignment.Top)
                            {
                                dataRow.Cells[i].CellFormat.VerticalAlignment = tablepropertyModel.CellVerticalAlignment;
                            }
                            Paragraph p1 = dataRow.Cells[i].AddParagraph();
                            if (tablepropertyModel.LeftIndent != 0)
                            {
                                p1.Format.LeftIndent = tablepropertyModel.LeftIndent;//左边距
                            }
                            if (tablepropertyModel.RightIndent != 0)
                            {
                                p1.Format.RightIndent = tablepropertyModel.RightIndent;//左边距
                            }
                            p1.AppendText(propertyInfo.GetValue(listModel[r], null).ToString());
                            break;
                        }
                    }
                }
            }
        }
    }
    public class Textproperty
    {
        /// <summary>
        /// 文本
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// 字体名称
        /// </summary>
        public string FontName { get; set; }
        /// <summary>
        /// 字体大小
        /// </summary>
        public int FontSize { get; set; }
        /// <summary>
        /// 位置(左,中,右)
        /// </summary>
        public HorizontalAlignment horizontalAlignment { get; set; }
        /// <summary>
        /// 下划线（默认是none）
        /// </summary>
        public UnderlineStyle UnderlineStyle { get; set; }

        public int LineSpacing { get; set; }

        /// <summary>
        /// 段落上间距
        /// </summary>
        public int BeforeSpacing { get; set; }
        /// <summary>
        /// 段落下间距
        /// </summary>
        public int AfterSpacing { get; set; }
        /// <summary>
        /// 首行缩进（默认是0）
        /// </summary>
        public int FirstLineIndent { get; set; }
        /// <summary>
        /// 是否加粗
        /// </summary>
        public bool Bold { get; set; }
        /// <summary>
        /// 距离左边（默认是0）
        /// </summary>
        public int LeftIndent { get; set; }
        /// <summary>
        /// 距离右边（默认是0）
        /// </summary>
        public int RightIndent { get; set; }

    }
    public class Tableproperty
    {
        /// <summary>
        /// 表头行高度
        /// </summary>
        public int HeaderHeight { get; set; }
        /// <summary>
        /// 位置
        /// </summary>
        public HorizontalAlignment HorizontalAlignment { get; set; }
        /// <summary>
        /// 左边距
        /// </summary>
        public int LeftIndent { get; set; }
        /// <summary>
        /// 右边距
        /// </summary>
        public int RightIndent { get; set; }
        /// <summary>
        /// 表头行是否加粗
        /// </summary>
        public bool HeaderBold { get; set; }
        /// <summary>
        /// 内容行高度
        /// </summary>
        public int RowHeight { get; set; }
        /// <summary>
        /// 上下居中或居上或居下
        /// </summary>
        public VerticalAlignment CellVerticalAlignment { get; set; }
    }
    public class TableData<T> where T : class
    {
        public string[] HeaderKey { get; set; }
        public string[] HeaderValue { get; set; }
        public Tableproperty TablepropertyModel { get; set; }
        public List<T> ListModel { get; set; }
    }

    public class TableBody
    {
        /// <summary>
        /// 期次
        /// </summary>
        public string Num { get; set; }
        /// <summary>
        /// 时间
        /// </summary>
        public string Date { get; set; }
        /// <summary>
        /// 本金
        /// </summary>
        public double Principal { get; set; }
        /// <summary>
        /// 利息
        /// </summary>
        public double Interest { get; set; }
        /// <summary>
        /// 本息
        /// </summary>
        public decimal MonthlyRepaymentAmount { get; set; }
    }
    //新建类 重写Spire流方法
    public class SpireMemoryStream : MemoryStream
    {
        public SpireMemoryStream()
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
