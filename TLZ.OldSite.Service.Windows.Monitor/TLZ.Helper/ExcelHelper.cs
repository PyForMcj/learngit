using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ComponentModel;
using System.Text.RegularExpressions;
using NPOI.HPSF;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using System.Collections;

namespace TLZ.Helper
{
    /// <summary>
    /// Excel操作类
    /// </summary>
    /// <remarks>
    /// [2014-01-17] Develop by SunLiang.
    /// </remarks>
    public static class ExcelHelper
    {
        public const int EXCEL_2003_COLUMN_MAX_WIDTH = 0xff00;
        public const int EXCEL_2003_ROW_MAX_COUNT = 0xffff;

        //A B C D E F G H I G K  L  M  N  O  P  Q  R  S  T  U  V  W  S  Y  Z
        //0 1 2 3 4 5 6 7 8 9 10 11 12 13 14 15 16 17 18 19 20 21 22 23 24 25
        /// <summary>
        /// 用于excel表格中列号字母转成列索引，从1对应A开始
        /// </summary>
        /// <param name="colLabel">列号</param>
        /// <returns>列索引</returns>
        public static int GetColunmAddress(string colLabel)
        {
            if (!Regex.IsMatch(colLabel.ToUpper(), @"[a-zA-Z]{1,2}")) throw new ArgumentException("Invalid parameter");
            int index = 0;
            char[] chars = colLabel.ToUpper().ToCharArray();
            for (int i = 0; i < chars.Length; i++)
            {
                index += ((int)chars[i] - (int)'A' + 1) * (int)Math.Pow(26, chars.Length - i - 1);
            }
            return index;
        }
        /// <summary>
        /// 用于将excel表格中列索引转成列号字母，从A对应1开始
        /// </summary>
        /// <param name="colIndex">列索引</param>
        /// <returns>列号</returns>
        public static string GetColunmAddress(int colIndex)
        {
            if (colIndex < 1 || colIndex > 256) throw new ArgumentException("max coluum Index range is 1~ 256");
            colIndex--;
            string column = string.Empty;
            do
            {
                if (column.Length > 0) colIndex--;
                column = ((char)(colIndex % 26 + (int)'A')).ToString() + column;
                colIndex = (int)((colIndex - colIndex % 26) / 26);
            } while (colIndex > 0);
            return column;
        }
        /// <summary>
        /// 导入(请使用后释放Stream)，目前只支持excel2003含以下，不支持2007含以上。wangyunpeng
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sheet"></param>
        /// <param name="fieldMapping">类属性名称和excel中表头对应的列明键值对</param>
        /// <returns></returns>
        public static List<T> Import<T>(Stream stream, IEnumerable<ColumnOption> fieldMapping) where T : class,new()
        {
            if (fieldMapping == null)
            {
                throw new ArgumentNullException("fieldMapping");
            }
            List<T> list = new List<T>();
            if (fieldMapping.Count() == 0)
            {
                return list;
            }
            HSSFWorkbook workbook = null;
            try
            {
                workbook = new HSSFWorkbook(stream);
            }
            catch
            {
                throw new ArgumentException("无效的Excel文件:文件损坏或者系统不支持您的版本，请尝试保存为早期的*.xls格式文件。");
            }
            ISheet sheet = workbook.GetSheetAt(0);
            if (sheet == null) throw new Exception("没有可导入的工作表！");
            if (sheet.PhysicalNumberOfRows < 2) throw new Exception("没有可导入的数据！");
            var properties = typeof(T).GetProperties();
            IRow row = null;
            ICell cell = null;
            Type type = null;
            Cell cellParser = new Cell() { };
            int rowIndex = 0, len = 0;
            //判断合并行
            int regionsCuont = sheet.NumMergedRegions;
            CellRangeAddress region;
            bool hasTitle = false;
            for (int i = 0; i < regionsCuont; i++)
            {
                region = sheet.GetMergedRegion(i);
                if (region.FirstRow == sheet.FirstRowNum && region.FirstColumn == 0)
                {
                    rowIndex = region.LastRow + 1;
                    hasTitle = true;
                    break;
                }
            }
            if (!hasTitle && sheet.PhysicalNumberOfRows - rowIndex == 0)
            {
                throw new Exception("没有可导入的数据！");
            }
            else if (hasTitle && sheet.PhysicalNumberOfRows - rowIndex - 1 == 0)
            {
                throw new Exception("没有可导入的数据！");
            }
            row = sheet.GetRow(rowIndex);
            bool hasImportColumn = false;
            foreach (var filed in fieldMapping)
            {
                cell = row.Cells.Where(p => p.StringCellValue.Trim() == filed.PropertyName).FirstOrDefault();
                if (cell == null)
                {
                    continue;
                }
                filed.Index = cell.ColumnIndex;
                var pi = properties.Where(p => p.Name == filed.PropertyName).FirstOrDefault();
                if (pi == null)
                {
                    continue;
                }
                hasImportColumn = true;
                filed.Property = pi;
            }
            if (!hasImportColumn)
            {
                throw new Exception("导入数据与模版不匹配！");
            }
            object value = null;
            string stringValue = null;
            try
            {
                for (rowIndex = rowIndex + 1, len = sheet.LastRowNum + 1; rowIndex < len; rowIndex++)
                {
                    T t = new T();
                    row = sheet.GetRow(rowIndex);
                    foreach (var field in fieldMapping)
                    {
                        value = null;
                        stringValue = null;
                        if (field.Property == null)
                        {
                            continue;
                        }
                        cell = row.GetCell(field.Index, MissingCellPolicy.RETURN_NULL_AND_BLANK);
                        if (cell == null
                            || cell.CellType == CellType.Error
                            || cell.CellType == CellType.Unknown
                            //|| cell.CellType == CellType.Blank //数字和布尔值类型为null 是这个
                            )
                        {
                            if (field.ImportValueRequired)
                            {
                                throw new Exception("导入数据失败:在第[{0}]行，'{1}'列中数据非法！".Formater(rowIndex + 1, field.Title/* GetColunmAddress(cell.ColumnIndex + 1)*/));
                            }
                            else
                            {
                                continue;
                            }
                        }
                        switch (cell.CellType)
                        {
                            case CellType.String:
                                stringValue = cell.StringCellValue.Trim();
                                if (stringValue.Length > 0)
                                {
                                    value = stringValue;
                                }
                                break;
                            case CellType.Numeric:
                                value = cell.NumericCellValue;
                                break;
                            case CellType.Boolean:
                                value = cell.BooleanCellValue;
                                break;
                            case CellType.Formula:
                                switch (cell.CachedFormulaResultType)
                                {
                                    case CellType.String:
                                        stringValue = cell.StringCellValue.Trim();
                                        if (stringValue.Length > 0)
                                        {
                                            value = stringValue;
                                        }
                                        break;
                                    case CellType.Numeric:
                                        value = cell.NumericCellValue;
                                        break;
                                    case CellType.Boolean:
                                        value = cell.BooleanCellValue;
                                        break;
                                    case CellType.Unknown:
                                    case CellType.Error:
                                        throw new Exception("导入数据失败:在第[{0}]行，'{1}'列中数据非法！".Formater(rowIndex + 1, field.Title/* GetColunmAddress(cell.ColumnIndex + 1)*/));
                                }
                                break;
                        }
                        if (field.ExportValueInputed && field.ImportValueRequired)
                        {
                            if (value == null
                                || (value is string
                                    && value.ToString().Length == 0)
                                )
                            {
                                throw new Exception("导入数据失败:在第[{0}]行，'{1}'列中数据为必填项！".Formater(rowIndex + 1, field.Title/* GetColunmAddress(cell.ColumnIndex + 1)*/));
                            }
                        }
                        if (field.ImportValueParser != null)
                        {
                            cellParser.ColumnOption = field;
                            cellParser.ColumnIndex = field.Index;
                            cellParser.RowIndex = rowIndex;
                            cellParser.Value = value;
                            value = field.ImportValueParser(cellParser);
                        }
                        else
                        {
                            if (value == null)
                            {
                                continue;
                            }
                            stringValue = value.ToString().Trim();//去掉两边的空格wyp
                            type = field.Property.PropertyType;
                            if (type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                            {
                                type = type.GetGenericArguments()[0];
                            }
                            if (type.IsEnum)
                            {
                                value = Enum.Parse(type, stringValue, true);
                            }
                            else
                            {
                                switch (type.Name)
                                {
                                    case "String":
                                        value = stringValue;
                                        break;
                                    case "Int32":
                                        value = Int32.Parse(stringValue);
                                        break;
                                    case "Double":
                                        value = Double.Parse(stringValue);
                                        break;
                                    case "Decimal":
                                        value = System.Decimal.Parse(stringValue);
                                        break;
                                    case "DateTime":
                                        try
                                        {
                                            if (workbook.Workbook.IsUsing1904DateWindowing)
                                            {
                                                value = DateUtil.GetJavaDate(Double.Parse(stringValue), true);
                                            }
                                            else
                                            {
                                                value = DateUtil.GetJavaDate(Double.Parse(stringValue), false);
                                            }
                                        }
                                        catch
                                        {
                                            type = value.GetType();
                                            if (type.Name == "String")
                                            {
                                                value = DateTime.Parse(stringValue);
                                            }
                                        }
                                        //string[] d = stringValue.Split(new char[] { '/', '-','' }, StringSplitOptions.RemoveEmptyEntries);
                                        //value = new DateTime(2000 + int.Parse(d[2]), int.Parse(d[0]), int.Parse(d[1]));
                                        break;
                                    case "Boolean":
                                        stringValue = stringValue.ToUpper();
                                        if (stringValue == "是"
                                            /*|| stringValue == "TRUE"
                                            || stringValue == "YES"
                                            || stringValue == "Y"
                                            || stringValue == "真"
                                            || stringValue == "1"*/
                                            )
                                        {
                                            value = true;
                                        }
                                        else if (stringValue == "否"
                                            /*|| stringValue == "FALSE"
                                            || stringValue == "N"
                                            || stringValue == "NO"
                                            || stringValue == "假"
                                            || stringValue == "0"*/
                                            )
                                        {
                                            value = false;
                                        }
                                        else //if(!isNullable)
                                        {
                                            throw new Exception("导入数据失败:在第[{0}]行，'{1}'列中数据非法！".Formater(rowIndex + 1, field.Title/* GetColunmAddress(cell.ColumnIndex + 1)*/));
                                        }
                                        break;
                                    case "Byte":
                                        value = Byte.Parse(stringValue);
                                        break;
                                    case "Int16":
                                        value = Int16.Parse(stringValue);
                                        break;
                                    case "Int64":
                                        value = Int64.Parse(stringValue);
                                        break;
                                    case "Single":
                                        value = Single.Parse(stringValue);
                                        break;
                                }
                            }
                        }
                        if (value != null)
                        {
                            field.Property.SetValue(t, value, null);
                        }
                    }
                    list.Add(t);
                }
                return list;
            }
            catch (Exception ex)
            {
                if (ex.Message.StartsWith("导入"))
                {
                    throw ex;
                }
                else
                {
                    throw new Exception("导入数据失败:在第[{0}]行，第[{1}]列中数据非法！".Formater(rowIndex + 1, GetColunmAddress(cell.ColumnIndex + 1)));
                }
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }
        }
        /// <summary>
        /// 导出(请使用后释放Stream)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="fieldMapping">字段映射字典。对于key存在于list属性中，将使用list集合中属性的值；不存在的将使用value填充.</param>
        /// <returns>Stream</returns>
        public static Stream Export<T>(IEnumerable<T> list, IEnumerable<ColumnOption> fieldMapping, string title = null) where T : class
        {
            if (list == null) throw new ArgumentNullException("list");
            if (fieldMapping == null) throw new ArgumentNullException("fieldMapping");
            if (list.Count() >= EXCEL_2003_ROW_MAX_COUNT)
            {
                throw new Exception("导出的数据已经超过Excel 最大的行数限制，请分批导出！");
            }
            var properties = typeof(T).GetProperties();
            int rowIndex = -1, colIndex = 0;
            foreach (var field in fieldMapping)
            {
                if (field.PropertyName != null)
                {
                    field.Property = properties.Where(p => p.Name == field.PropertyName).FirstOrDefault();
                }
                field.Index = colIndex;
                colIndex++;
            }
            MemoryStream file = new MemoryStream();
            HSSFWorkbook workbook = new HSSFWorkbook();
            ISheet sheet = null;
            IRow row = null;
            ICell cell = null;
            ICellStyle style = null;
            Object value = null;
            Type type = null;
            Cell cellFormater = new Cell()
            {
                ColumnIndex = colIndex,
                RowIndex = rowIndex
            };
            CellRangeAddressList regions = null;
            DVConstraint constraint = null;
            HSSFDataValidation dataValidate = null;
            //工作表名
            sheet = workbook.CreateSheet("Sheet0");
            #region - 右击文件 属性信息 -
            {
                DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
                dsi.Company = "踏浪者国际";
                dsi.Category = "踏浪者国际";
                workbook.DocumentSummaryInformation = dsi;
                SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
                si.Author = "踏浪者国际"; //填加xls文件作者信息
                si.ApplicationName = "TideBuy ERP Robot"; //填加xls文件创建程序信息
                si.LastAuthor = "TideBuy ERP Robot"; //填加xls文件最后保存者信息
                si.Comments = "TideBuy ERP Robot"; //填加xls文件作者信息
                si.Title = title ?? "标题信息"; //填加xls文件标题信息
                si.Subject = title ?? "主题信息";//填加文件主题信息
                si.CreateDateTime = DateTime.Now;
                si.EditTime = 10;
                si.Keywords = "踏浪者国际";
                si.LastSaveDateTime = DateTime.Now;
                si.RevNumber = "1.0.0.0";
                workbook.SummaryInformation = si;
            }
            #endregion
            #region - 表头 -
            if (title != null)
            {
                //创建标题
                rowIndex++;
                row = sheet.CreateRow(rowIndex);
                row.Height = 3 * 256;
                style = style = CreateCellStyle(workbook, null, (_cellStyle, _font) =>
                {
                    _cellStyle.Alignment = HorizontalAlignment.Center;
                    _cellStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.LightTurquoise.Index;//Shallow green
                    _cellStyle.FillPattern = FillPattern.LessDots;
                    _cellStyle.FillBackgroundColor = NPOI.HSSF.Util.HSSFColor.LightTurquoise.Index;
                    _font.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;
                    _font.FontHeightInPoints = 14;
                });
                cell = row.CreateCell(0);
                var range = new CellRangeAddress(rowIndex, rowIndex, 0, fieldMapping.Count() - 1);
                sheet.AddMergedRegion(range);
                ((HSSFSheet)sheet).SetEnclosedBorderOfRegion(range, BorderStyle.Thin, NPOI.HSSF.Util.HSSFColor.Black.Index);
                cell.CellStyle = style;
                cell.SetCellValue(title);
            }
            //创建列头
            colIndex = 0;
            rowIndex++;
            row = sheet.CreateRow(rowIndex);
            row.Height = 2 * 256;
            style = CreateCellStyle(workbook, null, (_cellStyle, _font) =>
            {
                _cellStyle.Alignment = HorizontalAlignment.Center;
                _cellStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.LightTurquoise.Index;//Shallow green
                _cellStyle.FillPattern = FillPattern.LessDots;
                _cellStyle.FillBackgroundColor = NPOI.HSSF.Util.HSSFColor.LightTurquoise.Index;
                _font.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;
            });

            foreach (var field in fieldMapping)
            {
                cell = row.CreateCell(colIndex);
                cell.CellStyle = style;
                cell.SetCellValue(field.Title);
                colIndex++;
                //以下为格式
                if (field.Property != null)
                {
                    type = field.Property.PropertyType;
                    if (type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                    {
                        type = type.GetGenericArguments()[0];
                    }
                    switch (type.Name)
                    {
                        case "String":
                            if (field.ExportValueHyperlinked)
                            {
                                field.CellStyle = CreateCellStyle(workbook, null, (cellStyle, fontStyle) =>
                                {
                                    fontStyle.Color = NPOI.HSSF.Util.HSSFColor.Blue.Index;
                                    fontStyle.Underline = FontUnderlineType.Single;
                                    cellStyle.Alignment = HorizontalAlignment.Center;
                                    cellStyle.WrapText = true;
                                });
                            }
                            else if (!string.IsNullOrEmpty(field.ExportValueFormat))
                            {
                                field.CellStyle = CreateCellStyle(workbook, field.ExportValueFormat);
                            }
                            break;
                        case "Byte":
                        case "Int16":
                        case "Int32":
                        case "Int64":
                        case "Single":
                        case "Double":
                        case "Decimal":
                            if (!string.IsNullOrEmpty(field.ExportValueFormat))
                            {
                                field.CellStyle = CreateCellStyle(workbook, field.ExportValueFormat);
                            }
                            break;
                        case "DateTime":
                            if (string.IsNullOrEmpty(field.ExportValueFormat))
                            {
                                field.CellStyle = CreateCellStyle(workbook, field.ExportValueFormat ?? "yyyy-mm-dd HH:mm:ss");
                                field.CellStyle.Alignment = HorizontalAlignment.Right;
                            }
                            else
                            {
                                field.CellStyle = CreateCellStyle(workbook, field.ExportValueFormat);
                            }
                            break;
                        case "Boolean":
                            if (string.IsNullOrEmpty(field.ExportValueFormat))
                            {
                                field.CellStyle = CreateCellStyle(workbook, null);
                                field.CellStyle.Alignment = HorizontalAlignment.Center;
                            }
                            else
                            {
                                field.CellStyle = CreateCellStyle(workbook, field.ExportValueFormat);
                            }
                            break;
                    }
                }
                //以下为有效性验证
                if (
                    field.Property != null
                    && field.ImportValueValidate == null
                    && field.ExportValueInputed //TODO：这个有待考证是否只对输入值启用
                    )
                {
                    type = field.Property.PropertyType;
                    if (type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                    {
                        type = type.GetGenericArguments()[0];
                    }
                    regions = new CellRangeAddressList(rowIndex + 1, 65535, field.Index, field.Index);
                    if (type.IsEnum)
                    {
                        constraint = DVConstraint.CreateExplicitListConstraint(Enum.GetNames(type));
                        dataValidate = new HSSFDataValidation(regions, constraint);
                        dataValidate.CreateErrorBox("输入错误", "请输入/选择下拉列表中的值.");
                        dataValidate.CreatePromptBox("提示", "请输入/选择下拉列表中的值.");
                        dataValidate.SuppressDropDownArrow = false;
                        sheet.AddValidationData(dataValidate);
                    }
                    else
                    {
                        switch (type.Name)
                        {
                            case "String":
                                field.ImportValueValidate = (c) => { return ColumnValidate.String(); };
                                break;
                            case "Int16"://整型
                            case "Int32":
                            case "Byte":
                                field.ImportValueValidate = (c) => { return ColumnValidate.Integer(); };
                                break;
                            case "Int64":
                            case "Single":
                            case "Double":
                            case "Decimal":
                                field.ImportValueValidate = (c) => { return ColumnValidate.Number(); };
                                break;
                            case "DateTime":
                                field.ImportValueValidate = (c) => { return ColumnValidate.Date(); };
                                break;
                            case "Boolean":
                                field.ImportValueValidate = (c) => { return ColumnValidate.Boolean(); };
                                break;
                        }
                    }
                }
                //以下为有效性验证
                if (field.ImportValueValidate != null)
                {
                    cellFormater.ColumnOption = field;
                    cellFormater.Value = null;
                    cellFormater.ColumnIndex = colIndex;
                    cellFormater.RowIndex = rowIndex;
                    var valueValidate = field.ImportValueValidate(cellFormater);
                    if (valueValidate == null || valueValidate.Value == null) continue;
                    regions = new CellRangeAddressList(rowIndex + 1, 65535, field.Index, field.Index);
                    if (valueValidate.Value.Length == 1)
                    {
                        constraint = DVConstraint.CreateCustomFormulaConstraint(valueValidate.Value[0].TrimStart('='));
                        dataValidate = new HSSFDataValidation(regions, constraint);
                        dataValidate.CreateErrorBox("输入错误", valueValidate.Message ?? "输入有误.");
                        dataValidate.CreatePromptBox("提示", valueValidate.Message ?? "输入有误.");
                        sheet.AddValidationData(dataValidate);
                    }
                    else if (valueValidate.Value.Length > 1)
                    {
                        constraint = DVConstraint.CreateExplicitListConstraint(valueValidate.Value);
                        dataValidate = new HSSFDataValidation(regions, constraint);
                        dataValidate.CreateErrorBox("输入错误", valueValidate.Message ?? "请输入/选择下拉列表中的值.");
                        dataValidate.CreatePromptBox("提示", valueValidate.Message ?? "请输入/选择下拉列表中的值.");
                        dataValidate.SuppressDropDownArrow = false;
                        sheet.AddValidationData(dataValidate);
                    }
                }
            }
            //冻结
            //  第一个参数表示要冻结的列数；
            //  第二个参数表示要冻结的行数；
            //  第三个参数表示右边区域可见的首列序号，从1开始计算；
            //  第四个参数表示下边区域可见的首行序号，也是从1开始计算；
            sheet.CreateFreezePane(0, rowIndex + 1, 0, rowIndex + 1);
            #endregion
            #region - 填充数据 -
            //设置样式
            sheet.DefaultRowHeight = (short)(1.5 * 256);
            style = CreateCellStyle(workbook, null);
            style.WrapText = true;
            //int index = 0;
            foreach (var item in list)
            {
                //index++;//从1开始
                rowIndex++;
                colIndex = 0;
                row = sheet.CreateRow(rowIndex);
                row.Height = (short)(1.5 * 256);
                foreach (var field in fieldMapping)
                {
                    value = null;
                    cell = row.CreateCell(colIndex);
                    cell.CellStyle = field.CellStyle ?? style;
                    if (field.ExportValueInputed && field.ExportValueFormater == null)
                    {
                        colIndex++;
                        continue;
                    }
                    if (field.Property != null)
                    {
                        value = field.Property.GetValue(item, null);
                    }
                    /*
                     // TODO:暂时关闭这个，可以通过 ExportValueFormater实现序号列功能 如：field.ExportValueFormater=(cell)=>{return cell.RowIndex+1;}
                    else if (field.PropertyName != null && field.PropertyName.Length > 0 && field.PropertyName[0] == '$')
                    {
                        field.PropertyName = field.PropertyName.ToLower();
                        switch (field.PropertyName)
                        {
                            case "$index":
                                value = index;
                                break;
                        }

                    }
                    */
                    if (field.ExportValueFormater != null)
                    {
                        cellFormater.ColumnOption = field;
                        cellFormater.Value = value;
                        cellFormater.ColumnIndex = colIndex;
                        cellFormater.RowIndex = rowIndex;
                        cellFormater.Item = item;
                        value = field.ExportValueFormater(cellFormater);
                    }
                    colIndex++;
                    if (value == null)
                    {
                        cell.SetCellValue("");
                        continue;
                    }
                    type = value.GetType();
                    if (type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                    {
                        type = type.GetGenericArguments()[0];
                    }
                    if (type.IsEnum)
                    {
                        cell.SetCellValue(value.ToString());
                        cell.SetCellType(CellType.String);
                        /*
                         // TODO:暂时关闭这个，以后如果有非中文的枚举名称了，在启用这个。
                              cell.SetCellValue(((Enum)value).GetText());//cell.SetCellValue(((Enum)value).GetName());
                               cell.SetCellType(CellType.String);
                        */
                    }
                    else
                    {
                        switch (type.Name)
                        {
                            case "String":
                                if (value.ToString().Length == 0) break;
                                if (field.ExportValueFormula)
                                {
                                    cell.SetCellFormula(value.ToString().TrimStart('='));
                                    cell.SetCellType(CellType.Formula);
                                }
                                else if (field.ExportValueHyperlinked)
                                {
                                    cell.SetCellValue("点击进入");
                                    cell.SetCellType(CellType.String);
                                    cell.Hyperlink = new HSSFHyperlink(HyperlinkType.Url) { Address = value.ToString() };
                                }
                                else
                                {
                                    cell.SetCellValue(value.ToString());
                                    cell.SetCellType(CellType.String);
                                }
                                break;
                            case "Int32":
                                cell.SetCellValue((Int32)value);
                                cell.SetCellType(CellType.Numeric);
                                break;
                            case "Decimal":
                                cell.SetCellValue(Double.Parse(value.ToString()));
                                cell.SetCellType(CellType.Numeric);
                                break;
                            case "DateTime":
                                //Excel最小日期:1899-12-30
                                //TODO:以后出现问题在说吧
                                cell.SetCellValue((DateTime)value);
                                cell.SetCellType(CellType.Numeric);
                                break;
                            case "Boolean":
                                //cell.SetCellValue((Boolean)value);
                                //cell.SetCellType(CellType.Boolean);
                                cell.SetCellValue(((bool)value == true) ? "是" : "否");
                                cell.SetCellType(CellType.String);
                                break;
                            case "Double":
                                cell.SetCellValue((Double)value);
                                cell.SetCellType(CellType.Numeric);
                                break;
                            case "Single":
                                cell.SetCellValue((Single)value);
                                cell.SetCellType(CellType.Numeric);
                                break;
                            case "Byte":
                                cell.SetCellValue((Byte)value);
                                cell.SetCellType(CellType.Numeric);
                                break;
                            case "Int16":
                                cell.SetCellValue((Int16)value);
                                cell.SetCellType(CellType.Numeric);
                                break;
                            case "Int64":
                                cell.SetCellValue((Int64)value);
                                cell.SetCellType(CellType.Numeric);
                                break;
                            default:
                                cell.SetCellValue(value.ToString());
                                cell.SetCellType(CellType.String);
                                break;
                        }
                    }
                }
            }
            #endregion
            #region - 调整列宽自 -
            int width = 0;
            foreach (var field in fieldMapping)
            {
                if (field.Width > 0)
                {
                    width = (int)(field.Width * 256 * 1.3);
                }
                else
                {
                    double num = SheetUtil.GetColumnWidth(sheet, field.Index, false);
                    if (num != -1.0)
                    {
                        num *= 256.0;
                        num += 2 * 256;
                        width = (int)num;
                    }
                    else
                    {
                        width = (int)(Regex.Replace(field.Title, "[^x00-xff]+", "AA").Length * 256 * 1.3);
                    }
                }

                if (width > EXCEL_2003_COLUMN_MAX_WIDTH)
                    width = EXCEL_2003_COLUMN_MAX_WIDTH;
                sheet.SetColumnWidth(field.Index, width);
            }
            #endregion
            workbook.Write(file);
            file.Seek(0, SeekOrigin.Begin);
            return file;
        }

        /// <summary>
        /// 导出多个SheetExcel(请使用后释放Stream)
        /// Author:孟显民
        /// Date:2015-06-15
        /// </summary>
        /// <typeparam name="T">泛型　表示要转换成表格行数据的集合中某一实体对象类型</typeparam>
        /// <param name="Hashtable">Hashtable 实例　存储永远生成多个Sheet数据集合　value值必须是List形式 </param>
        /// <param name="fieldMapping">字段映射字典。对于key存在于list属性中，将使用list集合中属性的值；不存在的将使用value填充.</param>
        /// <returns></returns>
        public static Stream Export<T>(Hashtable hashList, IEnumerable<ColumnOption> fieldMapping, IEnumerable<Sheet> sheetList, int listCount, string title = null) where T : class  
        {
            if (hashList == null) throw new ArgumentNullException("list");
           
            if (fieldMapping == null) throw new ArgumentNullException("fieldMapping");


            if (listCount >= EXCEL_2003_ROW_MAX_COUNT)
            {
                throw new Exception("导出的数据已经超过Excel 最大的行数限制，请分批导出！");
            }
            var properties = typeof(T).GetProperties();
            int rowIndex = -1, colIndex = 0;
            foreach (var field in fieldMapping)
            {
                if (field.PropertyName != null)
                {
                    field.Property = properties.Where(p => p.Name == field.PropertyName).FirstOrDefault();
                }
                field.Index = colIndex;
                colIndex++;
            }
        

            MemoryStream file = new MemoryStream();
            HSSFWorkbook workbook = new HSSFWorkbook();
            ISheet sheet = null;
            IRow row = null;
            ICell cell = null;
            ICellStyle style = null;
            Object value = null;
            Type type = null;
            Cell cellFormater = new Cell()
            {
                ColumnIndex = colIndex,
                RowIndex = rowIndex
            };
            CellRangeAddressList regions = null;
            DVConstraint constraint = null;
            HSSFDataValidation dataValidate = null;
            IEnumerable<T> currentList=null;
            #region sheet
            foreach (var sheetItem in sheetList)
            {
                rowIndex = -1;

                //工作表名
                sheet = workbook.CreateSheet(sheetItem.SheetName);
                #region - 右击文件 属性信息 -
                {
                    DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
                    dsi.Company = "踏浪者国际";
                    dsi.Category = "踏浪者国际";
                    workbook.DocumentSummaryInformation = dsi;
                    SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
                    si.Author = "踏浪者国际"; //填加xls文件作者信息
                    si.ApplicationName = "TideBuy ERP Robot"; //填加xls文件创建程序信息
                    si.LastAuthor = "TideBuy ERP Robot"; //填加xls文件最后保存者信息
                    si.Comments = "TideBuy ERP Robot"; //填加xls文件作者信息
                    si.Title = title ?? "标题信息"; //填加xls文件标题信息
                    si.Subject = title ?? "主题信息";//填加文件主题信息
                    si.CreateDateTime = DateTime.Now;
                    si.EditTime = 10;
                    si.Keywords = "踏浪者国际";
                    si.LastSaveDateTime = DateTime.Now;
                    si.RevNumber = "1.0.0.0";
                    workbook.SummaryInformation = si;
                }
                #endregion
                #region - 表头 -
                if (title != null)
                {
                    //创建标题
                    rowIndex++;
                    row = sheet.CreateRow(rowIndex);
                    row.Height = 3 * 256;
                    style = style = CreateCellStyle(workbook, null, (_cellStyle, _font) =>
                    {
                        _cellStyle.Alignment = HorizontalAlignment.Center;
                        _cellStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.LightTurquoise.Index;//Shallow green
                        _cellStyle.FillPattern = FillPattern.LessDots;
                        _cellStyle.FillBackgroundColor = NPOI.HSSF.Util.HSSFColor.LightTurquoise.Index;
                        _font.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;
                        _font.FontHeightInPoints = 14;
                    });
                    cell = row.CreateCell(0);
                    var range = new CellRangeAddress(rowIndex, rowIndex, 0, fieldMapping.Count() - 1);
                    sheet.AddMergedRegion(range);
                    ((HSSFSheet)sheet).SetEnclosedBorderOfRegion(range, BorderStyle.Thin, NPOI.HSSF.Util.HSSFColor.Black.Index);
                    cell.CellStyle = style;
                    cell.SetCellValue(title);
                }
                //创建列头
                colIndex = 0;
                rowIndex++;
                row = sheet.CreateRow(rowIndex);
                row.Height = 2 * 256;
                style = CreateCellStyle(workbook, null, (_cellStyle, _font) =>
                {
                    _cellStyle.Alignment = HorizontalAlignment.Center;
                    _cellStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.LightTurquoise.Index;//Shallow green
                    _cellStyle.FillPattern = FillPattern.LessDots;
                    _cellStyle.FillBackgroundColor = NPOI.HSSF.Util.HSSFColor.LightTurquoise.Index;
                    _font.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;
                });

                foreach (var field in fieldMapping)
                {
                    cell = row.CreateCell(colIndex);
                    cell.CellStyle = style;
                    cell.SetCellValue(field.Title);
                    colIndex++;
                    //以下为格式
                    if (field.Property != null)
                    {
                        type = field.Property.PropertyType;
                        if (type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                        {
                            type = type.GetGenericArguments()[0];
                        }
                        switch (type.Name)
                        {
                            case "String":
                                if (field.ExportValueHyperlinked)
                                {
                                    field.CellStyle = CreateCellStyle(workbook, null, (cellStyle, fontStyle) =>
                                    {
                                        fontStyle.Color = NPOI.HSSF.Util.HSSFColor.Blue.Index;
                                        fontStyle.Underline = FontUnderlineType.Single;
                                        cellStyle.Alignment = HorizontalAlignment.Center;
                                        cellStyle.WrapText = true;
                                    });
                                }
                                else if (!string.IsNullOrEmpty(field.ExportValueFormat))
                                {
                                    field.CellStyle = CreateCellStyle(workbook, field.ExportValueFormat);
                                }
                                break;
                            case "Byte":
                            case "Int16":
                            case "Int32":
                            case "Int64":
                            case "Single":
                            case "Double":
                            case "Decimal":
                                if (!string.IsNullOrEmpty(field.ExportValueFormat))
                                {
                                    field.CellStyle = CreateCellStyle(workbook, field.ExportValueFormat);
                                }
                                break;
                            case "DateTime":
                                if (string.IsNullOrEmpty(field.ExportValueFormat))
                                {
                                    field.CellStyle = CreateCellStyle(workbook, field.ExportValueFormat ?? "yyyy-mm-dd HH:mm:ss");
                                    field.CellStyle.Alignment = HorizontalAlignment.Right;
                                }
                                else
                                {
                                    field.CellStyle = CreateCellStyle(workbook, field.ExportValueFormat);
                                }
                                break;
                            case "Boolean":
                                if (string.IsNullOrEmpty(field.ExportValueFormat))
                                {
                                    field.CellStyle = CreateCellStyle(workbook, null);
                                    field.CellStyle.Alignment = HorizontalAlignment.Center;
                                }
                                else
                                {
                                    field.CellStyle = CreateCellStyle(workbook, field.ExportValueFormat);
                                }
                                break;
                        }
                    }
                    //以下为有效性验证
                    if (
                        field.Property != null
                        && field.ImportValueValidate == null
                        && field.ExportValueInputed //TODO：这个有待考证是否只对输入值启用
                        )
                    {
                        type = field.Property.PropertyType;
                        if (type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                        {
                            type = type.GetGenericArguments()[0];
                        }
                        regions = new CellRangeAddressList(rowIndex + 1, 65535, field.Index, field.Index);
                        if (type.IsEnum)
                        {
                            constraint = DVConstraint.CreateExplicitListConstraint(Enum.GetNames(type));
                            dataValidate = new HSSFDataValidation(regions, constraint);
                            dataValidate.CreateErrorBox("输入错误", "请输入/选择下拉列表中的值.");
                            dataValidate.CreatePromptBox("提示", "请输入/选择下拉列表中的值.");
                            dataValidate.SuppressDropDownArrow = false;
                            sheet.AddValidationData(dataValidate);
                        }
                        else
                        {
                            switch (type.Name)
                            {
                                case "String":
                                    field.ImportValueValidate = (c) => { return ColumnValidate.String(); };
                                    break;
                                case "Int16"://整型
                                case "Int32":
                                case "Byte":
                                    field.ImportValueValidate = (c) => { return ColumnValidate.Integer(); };
                                    break;
                                case "Int64":
                                case "Single":
                                case "Double":
                                case "Decimal":
                                    field.ImportValueValidate = (c) => { return ColumnValidate.Number(); };
                                    break;
                                case "DateTime":
                                    field.ImportValueValidate = (c) => { return ColumnValidate.Date(); };
                                    break;
                                case "Boolean":
                                    field.ImportValueValidate = (c) => { return ColumnValidate.Boolean(); };
                                    break;
                            }
                        }
                    }
                    //以下为有效性验证
                    if (field.ImportValueValidate != null)
                    {
                        cellFormater.ColumnOption = field;
                        cellFormater.Value = null;
                        cellFormater.ColumnIndex = colIndex;
                        cellFormater.RowIndex = rowIndex;
                        var valueValidate = field.ImportValueValidate(cellFormater);
                        if (valueValidate == null || valueValidate.Value == null) continue;
                        regions = new CellRangeAddressList(rowIndex + 1, 65535, field.Index, field.Index);
                        if (valueValidate.Value.Length == 1)
                        {
                            constraint = DVConstraint.CreateCustomFormulaConstraint(valueValidate.Value[0].TrimStart('='));
                            dataValidate = new HSSFDataValidation(regions, constraint);
                            dataValidate.CreateErrorBox("输入错误", valueValidate.Message ?? "输入有误.");
                            dataValidate.CreatePromptBox("提示", valueValidate.Message ?? "输入有误.");
                            sheet.AddValidationData(dataValidate);
                        }
                        else if (valueValidate.Value.Length > 1)
                        {
                            constraint = DVConstraint.CreateExplicitListConstraint(valueValidate.Value);
                            dataValidate = new HSSFDataValidation(regions, constraint);
                            dataValidate.CreateErrorBox("输入错误", valueValidate.Message ?? "请输入/选择下拉列表中的值.");
                            dataValidate.CreatePromptBox("提示", valueValidate.Message ?? "请输入/选择下拉列表中的值.");
                            dataValidate.SuppressDropDownArrow = false;
                            sheet.AddValidationData(dataValidate);
                        }
                    }
                }
                //冻结
                //  第一个参数表示要冻结的列数；
                //  第二个参数表示要冻结的行数；
                //  第三个参数表示右边区域可见的首列序号，从1开始计算；
                //  第四个参数表示下边区域可见的首行序号，也是从1开始计算；
                sheet.CreateFreezePane(0, rowIndex + 1, 0, rowIndex + 1);
                #endregion
                #region - 填充数据 -
                //设置样式
                sheet.DefaultRowHeight = (short)(1.5 * 256);
                style = CreateCellStyle(workbook, null);
                style.WrapText = true;
                currentList = hashList[sheetItem.SheetKey] as IEnumerable<T>;          
                //int index = 0;
                foreach (var item in currentList)
                {
                             

                    //index++;//从1开始
                    rowIndex++;
                    colIndex = 0;
                    row = sheet.CreateRow(rowIndex);
                    row.Height = (short)(1.5 * 256);
                    foreach (var field in fieldMapping)
                    {
                        value = null;
                        cell = row.CreateCell(colIndex);
                        cell.CellStyle = field.CellStyle ?? style;
                        if (field.ExportValueInputed && field.ExportValueFormater == null)
                        {
                            colIndex++;
                            continue;
                        }
                        if (field.Property != null)
                        {
                            value = field.Property.GetValue(item, null);
                        }
                        /*
                         // TODO:暂时关闭这个，可以通过 ExportValueFormater实现序号列功能 如：field.ExportValueFormater=(cell)=>{return cell.RowIndex+1;}
                        else if (field.PropertyName != null && field.PropertyName.Length > 0 && field.PropertyName[0] == '$')
                        {
                            field.PropertyName = field.PropertyName.ToLower();
                            switch (field.PropertyName)
                            {
                                case "$index":
                                    value = index;
                                    break;
                            }

                        }
                        */
                        if (field.ExportValueFormater != null)
                        {
                            cellFormater.ColumnOption = field;
                            cellFormater.Value = value;
                            cellFormater.ColumnIndex = colIndex;
                            cellFormater.RowIndex = rowIndex;
                            cellFormater.Item = item;
                            value = field.ExportValueFormater(cellFormater);
                        }
                        colIndex++;
                        if (value == null)
                        {
                            cell.SetCellValue("");
                            continue;
                        }
                        type = value.GetType();
                        if (type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                        {
                            type = type.GetGenericArguments()[0];
                        }
                        if (type.IsEnum)
                        {
                            cell.SetCellValue(value.ToString());
                            cell.SetCellType(CellType.String);
                            /*
                             // TODO:暂时关闭这个，以后如果有非中文的枚举名称了，在启用这个。
                                  cell.SetCellValue(((Enum)value).GetText());//cell.SetCellValue(((Enum)value).GetName());
                                   cell.SetCellType(CellType.String);
                            */
                        }
                        else
                        {
                            switch (type.Name)
                            {
                                case "String":
                                    if (value.ToString().Length == 0) break;
                                    if (field.ExportValueFormula)
                                    {
                                        cell.SetCellFormula(value.ToString().TrimStart('='));
                                        cell.SetCellType(CellType.Formula);
                                    }
                                    else if (field.ExportValueHyperlinked)
                                    {
                                        cell.SetCellValue("点击进入");
                                        cell.SetCellType(CellType.String);
                                        cell.Hyperlink = new HSSFHyperlink(HyperlinkType.Url) { Address = value.ToString() };
                                    }
                                    else
                                    {
                                        cell.SetCellValue(value.ToString());
                                        cell.SetCellType(CellType.String);
                                    }
                                    break;
                                case "Int32":
                                    cell.SetCellValue((Int32)value);
                                    cell.SetCellType(CellType.Numeric);
                                    break;
                                case "Decimal":
                                    cell.SetCellValue(Double.Parse(value.ToString()));
                                    cell.SetCellType(CellType.Numeric);
                                    break;
                                case "DateTime":
                                    //Excel最小日期:1899-12-30
                                    //TODO:以后出现问题在说吧
                                    cell.SetCellValue((DateTime)value);
                                    cell.SetCellType(CellType.Numeric);
                                    break;
                                case "Boolean":
                                    //cell.SetCellValue((Boolean)value);
                                    //cell.SetCellType(CellType.Boolean);
                                    cell.SetCellValue(((bool)value == true) ? "是" : "否");
                                    cell.SetCellType(CellType.String);
                                    break;
                                case "Double":
                                    cell.SetCellValue((Double)value);
                                    cell.SetCellType(CellType.Numeric);
                                    break;
                                case "Single":
                                    cell.SetCellValue((Single)value);
                                    cell.SetCellType(CellType.Numeric);
                                    break;
                                case "Byte":
                                    cell.SetCellValue((Byte)value);
                                    cell.SetCellType(CellType.Numeric);
                                    break;
                                case "Int16":
                                    cell.SetCellValue((Int16)value);
                                    cell.SetCellType(CellType.Numeric);
                                    break;
                                case "Int64":
                                    cell.SetCellValue((Int64)value);
                                    cell.SetCellType(CellType.Numeric);
                                    break;
                                default:
                                    cell.SetCellValue(value.ToString());
                                    cell.SetCellType(CellType.String);
                                    break;
                            }
                        }
                    }
                }
                #endregion
                #region - 调整列宽自 -
                int width = 0;
                foreach (var field in fieldMapping)
                {
                    if (field.Width > 0)
                    {
                        width = (int)(field.Width * 256 * 1.3);
                    }
                    else
                    {
                        double num = SheetUtil.GetColumnWidth(sheet, field.Index, false);
                        if (num != -1.0)
                        {
                            num *= 256.0;
                            num += 2 * 256;
                            width = (int)num;
                        }
                        else
                        {
                            width = (int)(Regex.Replace(field.Title, "[^x00-xff]+", "AA").Length * 256 * 1.3);
                        }
                    }

                    if (width > EXCEL_2003_COLUMN_MAX_WIDTH)
                        width = EXCEL_2003_COLUMN_MAX_WIDTH;
                    sheet.SetColumnWidth(field.Index, width);
                }
                #endregion

            }
            #endregion
            workbook.Write(file);
            file.Seek(0, SeekOrigin.Begin);
            return file;
        }

        #region 根据datatable 导出数据
        
        /// <summary>
        /// 导出(请使用后释放Stream)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="fieldMapping">字段映射字典。对于key存在于list属性中，将使用list集合中属性的值；不存在的将使用value填充.</param>
        /// <returns></returns>
        public static Stream Export<T>(DataTable dt, IEnumerable<ColumnOption> fieldMapping, string title = null) where T : class
        {
            if (dt == null) throw new ArgumentNullException("list");
            if (fieldMapping == null) throw new ArgumentNullException("fieldMapping");
            if (dt.Rows.Count >= EXCEL_2003_ROW_MAX_COUNT) throw new Exception("导出的数据已经超过Excel 最大的行数限制，请分批导出！");

            var properties = typeof(T).GetProperties();
            int rowIndex = -1, colIndex = 0;
            foreach (var field in fieldMapping)
            {
                if (field.PropertyName != null)
                {
                    field.Property = properties.Where(p => p.Name == field.PropertyName).FirstOrDefault();
                }
                field.Index = colIndex;
                colIndex++;
            }

            MemoryStream file = new MemoryStream();
            HSSFWorkbook workbook = new HSSFWorkbook();
            ISheet sheet = null;
            IRow row = null;
            ICell cell = null;
            ICellStyle style = null;
            Object value = null;
            Type type = null;
            Cell cellFormater = new Cell()
            {
                ColumnIndex = colIndex,
                RowIndex = rowIndex
            };
            CellRangeAddressList regions = null;
            DVConstraint constraint = null;
            HSSFDataValidation dataValidate = null;
            //工作表名
            sheet = workbook.CreateSheet("Sheet0");
            #region - 右击文件 属性信息 -
            {
                DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
                dsi.Company = "踏浪者国际";
                dsi.Category = "踏浪者国际";
                workbook.DocumentSummaryInformation = dsi;
                SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
                si.Author = "踏浪者国际"; //填加xls文件作者信息
                si.ApplicationName = "TideBuy ERP Robot"; //填加xls文件创建程序信息
                si.LastAuthor = "TideBuy ERP Robot"; //填加xls文件最后保存者信息
                si.Comments = "TideBuy ERP Robot"; //填加xls文件作者信息
                si.Title = title ?? "标题信息"; //填加xls文件标题信息
                si.Subject = title ?? "主题信息";//填加文件主题信息
                si.CreateDateTime = DateTime.Now;
                si.EditTime = 10;
                si.Keywords = "踏浪者国际";
                si.LastSaveDateTime = DateTime.Now;
                si.RevNumber = "1.0.0.0";
                workbook.SummaryInformation = si;
            }
            #endregion
            #region - 表头 -
            if (title != null)
            {
                //创建标题
                rowIndex++;
                row = sheet.CreateRow(rowIndex);
                row.Height = 3 * 256;
                style = style = CreateCellStyle(workbook, null, (_cellStyle, _font) =>
                {
                    _cellStyle.Alignment = HorizontalAlignment.Center;
                    _cellStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.LightTurquoise.Index;//Shallow green
                    _cellStyle.FillPattern = FillPattern.LessDots;
                    _cellStyle.FillBackgroundColor = NPOI.HSSF.Util.HSSFColor.LightTurquoise.Index;
                    _font.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;
                    _font.FontHeightInPoints = 14;
                });
                cell = row.CreateCell(0);
                var range = new CellRangeAddress(rowIndex, rowIndex, 0, fieldMapping.Count() - 1);
                sheet.AddMergedRegion(range);
                ((HSSFSheet)sheet).SetEnclosedBorderOfRegion(range, BorderStyle.Thin, NPOI.HSSF.Util.HSSFColor.Black.Index);
                cell.CellStyle = style;
                cell.SetCellValue(title);
            }
            //创建列头
            colIndex = 0;
            rowIndex++;
            row = sheet.CreateRow(rowIndex);
            row.Height = 2 * 256;
            style = CreateCellStyle(workbook, null, (_cellStyle, _font) =>
            {
                _cellStyle.Alignment = HorizontalAlignment.Center;
                _cellStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.LightTurquoise.Index;//Shallow green
                _cellStyle.FillPattern = FillPattern.LessDots;
                _cellStyle.FillBackgroundColor = NPOI.HSSF.Util.HSSFColor.LightTurquoise.Index;
                _font.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;
            });

            foreach (var field in fieldMapping)
            {
                cell = row.CreateCell(colIndex);
                cell.CellStyle = style;
                cell.SetCellValue(field.Title);
                colIndex++;
                //以下为格式
                if (field.Property != null)
                {
                    type = field.Property.PropertyType;
                    if (type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                    {
                        type = type.GetGenericArguments()[0];
                    }
                    switch (type.Name)
                    {
                        case "String":
                            if (field.ExportValueHyperlinked)
                            {
                                field.CellStyle = CreateCellStyle(workbook, null, (cellStyle, fontStyle) =>
                                {
                                    fontStyle.Color = NPOI.HSSF.Util.HSSFColor.Blue.Index;
                                    fontStyle.Underline = FontUnderlineType.Single;
                                    cellStyle.Alignment = HorizontalAlignment.Center;
                                    cellStyle.WrapText = true;
                                });
                            }
                            else if (!string.IsNullOrEmpty(field.ExportValueFormat))
                            {
                                field.CellStyle = CreateCellStyle(workbook, field.ExportValueFormat);
                            }
                            break;
                        case "Byte":
                        case "Int16":
                        case "Int32":
                        case "Int64":
                        case "Single":
                        case "Double":
                        case "Decimal":
                            if (!string.IsNullOrEmpty(field.ExportValueFormat))
                            {
                                field.CellStyle = CreateCellStyle(workbook, field.ExportValueFormat);
                            }
                            break;
                        case "DateTime":
                            if (string.IsNullOrEmpty(field.ExportValueFormat))
                            {
                                field.CellStyle = CreateCellStyle(workbook, field.ExportValueFormat ?? "yyyy-mm-dd HH:mm:ss");
                                field.CellStyle.Alignment = HorizontalAlignment.Right;
                            }
                            else
                            {
                                field.CellStyle = CreateCellStyle(workbook, field.ExportValueFormat);
                            }
                            break;
                        case "Boolean":
                            if (string.IsNullOrEmpty(field.ExportValueFormat))
                            {
                                field.CellStyle = CreateCellStyle(workbook, null);
                                field.CellStyle.Alignment = HorizontalAlignment.Center;
                            }
                            else
                            {
                                field.CellStyle = CreateCellStyle(workbook, field.ExportValueFormat);
                            }
                            break;
                    }
                }
                //以下为有效性验证
                if (
                    field.Property != null
                    && field.ImportValueValidate == null
                    && field.ExportValueInputed //TODO：这个有待考证是否只对输入值启用
                    )
                {
                    type = field.Property.PropertyType;
                    if (type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                    {
                        type = type.GetGenericArguments()[0];
                    }
                    regions = new CellRangeAddressList(rowIndex + 1, 65535, field.Index, field.Index);
                    if (type.IsEnum)
                    {
                        constraint = DVConstraint.CreateExplicitListConstraint(Enum.GetNames(type));
                        dataValidate = new HSSFDataValidation(regions, constraint);
                        dataValidate.CreateErrorBox("输入错误", "请输入/选择下拉列表中的值.");
                        dataValidate.CreatePromptBox("提示", "请输入/选择下拉列表中的值.");
                        dataValidate.SuppressDropDownArrow = false;
                        sheet.AddValidationData(dataValidate);
                    }
                    else
                    {
                        switch (type.Name)
                        {
                            case "String":
                                field.ImportValueValidate = (c) => { return ColumnValidate.String(); };
                                break;
                            case "Int16"://整型
                            case "Int32":
                            case "Byte":
                                field.ImportValueValidate = (c) => { return ColumnValidate.Integer(); };
                                break;
                            case "Int64":
                            case "Single":
                            case "Double":
                            case "Decimal":
                                field.ImportValueValidate = (c) => { return ColumnValidate.Number(); };
                                break;
                            case "DateTime":
                                field.ImportValueValidate = (c) => { return ColumnValidate.Date(); };
                                break;
                            case "Boolean":
                                field.ImportValueValidate = (c) => { return ColumnValidate.Boolean(); };
                                break;
                        }
                    }
                }
                //以下为有效性验证
                if (field.ImportValueValidate != null)
                {
                    cellFormater.ColumnOption = field;
                    cellFormater.Value = null;
                    cellFormater.ColumnIndex = colIndex;
                    cellFormater.RowIndex = rowIndex;
                    var valueValidate = field.ImportValueValidate(cellFormater);
                    if (valueValidate == null || valueValidate.Value == null) continue;
                    regions = new CellRangeAddressList(rowIndex + 1, 65535, field.Index, field.Index);
                    if (valueValidate.Value.Length == 1)
                    {
                        constraint = DVConstraint.CreateCustomFormulaConstraint(valueValidate.Value[0].TrimStart('='));
                        dataValidate = new HSSFDataValidation(regions, constraint);
                        dataValidate.CreateErrorBox("输入错误", valueValidate.Message ?? "输入有误.");
                        dataValidate.CreatePromptBox("提示", valueValidate.Message ?? "输入有误.");
                        sheet.AddValidationData(dataValidate);
                    }
                    else if (valueValidate.Value.Length > 1)
                    {
                        constraint = DVConstraint.CreateExplicitListConstraint(valueValidate.Value);
                        dataValidate = new HSSFDataValidation(regions, constraint);
                        dataValidate.CreateErrorBox("输入错误", valueValidate.Message ?? "请输入/选择下拉列表中的值.");
                        dataValidate.CreatePromptBox("提示", valueValidate.Message ?? "请输入/选择下拉列表中的值.");
                        dataValidate.SuppressDropDownArrow = false;
                        sheet.AddValidationData(dataValidate);
                    }
                }
            }
            //冻结
            //  第一个参数表示要冻结的列数；
            //  第二个参数表示要冻结的行数；
            //  第三个参数表示右边区域可见的首列序号，从1开始计算；
            //  第四个参数表示下边区域可见的首行序号，也是从1开始计算；
            sheet.CreateFreezePane(0, rowIndex + 1, 0, rowIndex + 1);
            #endregion
            #region - 填充数据 -
            //设置样式
            sheet.DefaultRowHeight = (short)(1.5 * 256);
            style = CreateCellStyle(workbook, null);
            style.WrapText = true;
            //int index = 0;

            foreach (DataRow dr in dt.Rows)
            {
                //index++;//从1开始
                rowIndex++;
                colIndex = 0;
                row = sheet.CreateRow(rowIndex);
                row.Height = (short)(1.5 * 256);
                foreach (var field in fieldMapping)
                {
                    value = null;
                    cell = row.CreateCell(colIndex);
                    cell.CellStyle = field.CellStyle ?? style;
                    if (field.ExportValueInputed && field.ExportValueFormater == null)
                    {
                        colIndex++;
                        continue;
                    }
                    if (field.Property != null)
                    {
                        value = dr[field.PropertyName];
                    }
                    /*
                     // TODO:暂时关闭这个，可以通过 ExportValueFormater实现序号列功能 如：field.ExportValueFormater=(cell)=>{return cell.RowIndex+1;}
                    else if (field.PropertyName != null && field.PropertyName.Length > 0 && field.PropertyName[0] == '$')
                    {
                        field.PropertyName = field.PropertyName.ToLower();
                        switch (field.PropertyName)
                        {
                            case "$index":
                                value = index;
                                break;
                        }

                    }
                    */
                    if (field.ExportValueFormater != null)
                    {
                        cellFormater.ColumnOption = field;
                        cellFormater.Value = value;
                        cellFormater.ColumnIndex = colIndex;
                        cellFormater.RowIndex = rowIndex;
                        cellFormater.Item = dr;
                        value = field.ExportValueFormater(cellFormater);
                    }
                    colIndex++;
                    if (value == null)
                    {
                        cell.SetCellValue("");
                        continue;
                    }
                    type = value.GetType();
                    if (type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                    {
                        type = type.GetGenericArguments()[0];
                    }
                    if (type.IsEnum)
                    {
                        cell.SetCellValue(value.ToString());
                        cell.SetCellType(CellType.String);
                        /*
                         // TODO:暂时关闭这个，以后如果有非中文的枚举名称了，在启用这个。
                              cell.SetCellValue(((Enum)value).GetText());//cell.SetCellValue(((Enum)value).GetName());
                               cell.SetCellType(CellType.String);
                        */
                    }
                    else
                    {
                        switch (type.Name)
                        {
                            case "String":
                                if (value.ToString().Length == 0) break;
                                if (field.ExportValueFormula)
                                {
                                    cell.SetCellFormula(value.ToString().TrimStart('='));
                                    cell.SetCellType(CellType.Formula);
                                }
                                else if (field.ExportValueHyperlinked)
                                {
                                    cell.SetCellValue("点击进入");
                                    cell.SetCellType(CellType.String);
                                    cell.Hyperlink = new HSSFHyperlink(HyperlinkType.Url) { Address = value.ToString() };
                                }
                                else
                                {
                                    cell.SetCellValue(value.ToString());
                                    cell.SetCellType(CellType.String);
                                }
                                break;
                            case "Int32":
                                cell.SetCellValue((Int32)value);
                                cell.SetCellType(CellType.Numeric);
                                break;
                            case "Decimal":
                                cell.SetCellValue(Double.Parse(value.ToString()));
                                cell.SetCellType(CellType.Numeric);
                                break;
                            case "DateTime":
                                //Excel最小日期:1899-12-30
                                //TODO:以后出现问题在说吧
                                cell.SetCellValue((DateTime)value);
                                cell.SetCellType(CellType.Numeric);
                                break;
                            case "Boolean":
                                //cell.SetCellValue((Boolean)value);
                                //cell.SetCellType(CellType.Boolean);
                                cell.SetCellValue(((bool)value == true) ? "是" : "否");
                                cell.SetCellType(CellType.String);
                                break;
                            case "Double":
                                cell.SetCellValue((Double)value);
                                cell.SetCellType(CellType.Numeric);
                                break;
                            case "Single":
                                cell.SetCellValue((Single)value);
                                cell.SetCellType(CellType.Numeric);
                                break;
                            case "Byte":
                                cell.SetCellValue((Byte)value);
                                cell.SetCellType(CellType.Numeric);
                                break;
                            case "Int16":
                                cell.SetCellValue((Int16)value);
                                cell.SetCellType(CellType.Numeric);
                                break;
                            case "Int64":
                                cell.SetCellValue((Int64)value);
                                cell.SetCellType(CellType.Numeric);
                                break;
                            default:
                                cell.SetCellValue(value.ToString());
                                cell.SetCellType(CellType.String);
                                break;
                        }
                    }
                }
            }
            #endregion
            #region - 调整列宽自 -
            int width = 0;
            foreach (var field in fieldMapping)
            {
                if (field.Width > 0)
                {
                    width = (int)(field.Width * 256 * 1.3);
                }
                else
                {
                    double num = SheetUtil.GetColumnWidth(sheet, field.Index, false);
                    if (num != -1.0)
                    {
                        num *= 256.0;
                        num += 2 * 256;
                        width = (int)num;
                    }
                    else
                    {
                        width = (int)(Regex.Replace(field.Title, "[^x00-xff]+", "AA").Length * 256 * 1.3);
                    }
                }

                if (width > EXCEL_2003_COLUMN_MAX_WIDTH)
                    width = EXCEL_2003_COLUMN_MAX_WIDTH;
                sheet.SetColumnWidth(field.Index, width);
            }
            #endregion
            workbook.Write(file);
            file.Seek(0, SeekOrigin.Begin);
            return file;
        }

        #endregion



        /// <summary>
        /// 
        /// </summary>
        /// <param name="workbook"></param>
        /// <param name="formatText"></param>
        /// <returns></returns>
        private static ICellStyle CreateCellStyle(IWorkbook workbook, string formatText, Action<ICellStyle, IFont> other = null)
        {
            IFont font;
            ICellStyle style = null;
            //设置样式
            style = workbook.CreateCellStyle();
            font = workbook.CreateFont();
            font.FontName = "微软雅黑";
            font.FontHeightInPoints = 10;
            //style.Alignment = HorizontalAlignment.Center;
            style.VerticalAlignment = VerticalAlignment.Top;
            style.SetFont(font);
            style.BorderBottom
               = style.BorderLeft
               = style.BorderRight
               = style.BorderTop
               = NPOI.SS.UserModel.BorderStyle.Thin;
            style.LeftBorderColor
                = style.RightBorderColor
                = style.TopBorderColor
                = style.BottomBorderColor
                = NPOI.HSSF.Util.HSSFColor.Black.Index;
            if (!string.IsNullOrEmpty(formatText))
            {
                IDataFormat format = workbook.CreateDataFormat();
                style.DataFormat = format.GetFormat(formatText);
            }
            if (other != null)
            {
                other(style, font);
            }
            return style;
        }
        //Default LT LC LB CT CC CB RT RC RB
    }
    [Serializable]
    public sealed class ColumnOption
    {
        public ColumnOption()
        {
            ImportValueRequired = true;
        }
        public ColumnOption(string propertyName, string title)
            : this()
        {
            this.PropertyName = propertyName;
            this.Title = title;
        }
        internal int Index { get; set; }
        /// <summary>
        /// 设置列宽（单位：英文字符数，默认值：自动调整）
        /// </summary>
        public int Width { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 对应的属性名
        /// </summary>
        public string PropertyName { get; set; }
        /// <summary>
        /// 导出的时候是否是输入项，即为留空等待输入(默认值：false)
        /// </summary>
        public bool ExportValueInputed { get; set; }
        /// <summary>
        /// 导出值是超链接，只对文本类型有效
        /// </summary>
        public bool ExportValueHyperlinked { get; set; }
        /// <summary>
        /// 导出值格式化 (cell)=>{return "WP"+cell.Value.ToString();}
        /// <para>如果没有值请返回 null 而不是 字符串 "".</para>
        /// </summary>
        public Func<Cell, object> ExportValueFormater { get; set; }
        /// <summary>
        /// 数值单元格显示格式（如：#,##0.00 或者 yyyy"年"m"月"d"日";@）,具体格式参见Excel 单元格属性中的【数字】标签
        /// </summary>
        public string ExportValueFormat { get; set; }
        /// <summary>
        /// 导入值解析 (cell)=>{return cellParserValue;}
        /// <para>如果解析无值请返回 null.</para>
        /// </summary>
        public Func<Cell, object> ImportValueParser { get; set; }
        /// <summary>
        /// 输入值验证规则，请参看Excel 函数
        /// <para>规则：</para>
        /// <para>   下拉形式 格式： [选项1,选项2,选项3,...,选项n]</para>
        /// <para>   Excel自定义 格式： =AND(ISNUMBER($A1),VALUE($A1)>100)</para>
        /// </summary>
        public Func<Cell, ColumnValidate> ImportValueValidate { get; set; }
        /// <summary>
        /// 输入值是必填项(默认值：true)
        /// </summary>
        public bool ImportValueRequired { get; set; }
        internal PropertyInfo Property { get; set; }
        internal ICellStyle CellStyle { get; set; }
        /// <summary>
        /// 导出的该列为表达式，这个配合ExportValueFormater使用，返回的字符串形式的表达式，
        /// <para>如：ExportValueFormater=(cell)=>{return cell.Address+"*10";}</para>
        /// </summary>
        public bool ExportValueFormula { get; set; }
        public override string ToString()
        {
            return string.Format("[PropertyName:{0},Title:{1}]", this.PropertyName, this.Title);
        }
    }
    /*
    Excel导出配置说明:
    导出值为用户输入： 				ExportValueInputed=true
    导出值为用户输入并且有默认值 	ExportValueInputed=true,ExportValueFormater=(cell)=>{return return cell.value+"默认值";}
    导出值是超链接：				ExportValueHyperlinked=true
    导出值Excel显示格式：			ExportValueFormat="#,##0.00"  (具体格式参见Excel 单元格属性中的【数字】标签)
    导入值为必填项： 				ImportValueRequired=true
    导入值自定义解析：				ImportValueParser=(cell)=>{return if(cell.value=="1")return "值1"; else return "值2";}
     */
    public sealed class ColumnValidate : ICloneable
    {
        internal const string CELL_ADDRESS_FORMULA = "ADDRESS(ROW(),COLUMN())";
        internal const string CELL_ADDRESS_VALUE = "INDIRECT(ADDRESS(ROW(),COLUMN()))";
        internal static readonly DateTime CELL_MIN_DATE = new DateTime(1899, 12, 31);

        public static ColumnValidate Number()
        {
            //return new ColumnValidate() { Message = "输入值必须是数字.", Value = new string[] { "ISNUMBER(" + columnAddress + ")=TRUE" } };
            return new ColumnValidate()
            {
                Message = "输入值必须是数字.",
                Value = new string[] { 
                    string.Format("ISNUMBER({0})",CELL_ADDRESS_VALUE)
                }
            };
        }
        public static ColumnValidate Number(ValueOperator type, int value)
        {
            return new ColumnValidate()
            {
                Message = "输入值必须是数字,并且" + type.GetName() + value + ".",
                Value = new string[] { 
                    string.Format("AND(ISNUMBER({0}),{0}{1}{2})",
                    CELL_ADDRESS_VALUE,
                    type.GetDescription(),
                    value 
                    )
                }
            };
        }
        public static ColumnValidate Number(ValueOperator type, decimal value)
        {
            return new ColumnValidate()
            {
                Message = "输入值必须是数字,并且" + type.GetName() + value + ".",
                Value = new string[] { 
                    string.Format("AND(ISNUMBER({0}),{0}{1}{2})",
                    CELL_ADDRESS_VALUE,
                    type.GetDescription(),
                    value 
                    )
                }
            };
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="between">是否介于之间</param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static ColumnValidate Number(bool between, int min, int max)
        {
            if (between)
            {
                return new ColumnValidate()
                {
                    Message = "输入值必须是数字,并且大于" + min + ",小于" + max + ".",
                    Value = new string[] { 
                    string.Format("AND(ISNUMBER({0}),{0}>={1},{0}<={2})",
                        CELL_ADDRESS_VALUE,
                        min,
                        max
                    )
                }
                };
            }
            else
            {
                return new ColumnValidate()
                {
                    Message = "输入值必须是数字,并且小于" + min + ",大于" + max + ".",
                    Value = new string[] { 
                    string.Format("AND(ISNUMBER({0}),{0}<={1},{0}>={2})",
                        CELL_ADDRESS_VALUE,
                        min,
                        max
                    )
                }
                };
            }
        }
        public static ColumnValidate Number(bool between, decimal min, decimal max)
        {
            if (between)
            {
                return new ColumnValidate()
                {
                    Message = "输入值必须是数字,并且大于" + min + ",小于" + max + ".",
                    Value = new string[] { 
                    string.Format("AND(ISNUMBER({0}),{0}>={1},{0}<={2})",
                        CELL_ADDRESS_VALUE,
                        min,
                        max
                    )
                }
                };
            }
            else
            {
                return new ColumnValidate()
                {
                    Message = "输入值必须是数字,并且小于" + min + ",大于" + max + ".",
                    Value = new string[] { 
                    string.Format("AND(ISNUMBER({0}),{0}<={1},{0}>={2})",
                        CELL_ADDRESS_VALUE,
                        min,
                        max
                    )
                }
                };
            }
        }
        public static ColumnValidate Number(IEnumerable<int> list)
        {
            return List(list);
        }
        public static ColumnValidate Number(IEnumerable<decimal> list)
        {
            return List(list);
        }

        public static ColumnValidate Integer()
        {
            return new ColumnValidate()
            {
                Message = "输入值必须是整数.",
                Value = new string[] { 
                    string.Format("INT({0})={0}",CELL_ADDRESS_VALUE)
                }
            };
        }
        public static ColumnValidate Integer(ValueOperator type, int value)
        {
            return new ColumnValidate()
            {
                Message = "输入值必须是整数,并且" + type.GetName() + value + ".",
                Value = new string[] { 
                    string.Format("AND(ISNUMBER({0}),INT({0})={0},{0}{1}{2})",
                    CELL_ADDRESS_VALUE,
                    type.GetDescription(),
                    value 
                    )
                }
            };
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="between">是否介于之间</param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static ColumnValidate Integer(bool between, int min, int max)
        {
            if (between)
            {
                return new ColumnValidate()
                {
                    Message = "输入值必须是整数,并且大于" + min + ",小于" + max + ".",
                    Value = new string[] { 
                    string.Format("AND(ISNUMBER({0}),INT({0})={0},{0}>={1},{0}<={2})",
                        CELL_ADDRESS_VALUE,
                        min,
                        max
                    )
                }
                };
            }
            else
            {
                return new ColumnValidate()
                {
                    Message = "输入值必须是整数,并且小于" + min + ",大于" + max + ".",
                    Value = new string[] { 
                    string.Format("AND(ISNUMBER({0}),INT({0})={0},{0}<={1},{0}>={2})",
                        CELL_ADDRESS_VALUE,
                        min,
                        max
                    )
                }
                };
            }
        }
        public static ColumnValidate Integer(IEnumerable<int> list)
        {
            return List(list);
        }

        public static ColumnValidate Decimal()
        {
            return new ColumnValidate()
            {
                Message = "输入值必须是小数.",
                Value = new string[] { 
                    string.Format("AND(ISNUMBER({0}),ISNUMBER(FIND(\".\",{0})))",CELL_ADDRESS_VALUE)
                }
            };
        }
        public static ColumnValidate Decimal(ValueOperator type, decimal value)
        {
            return new ColumnValidate()
            {
                Message = "输入值必须是小数,并且" + type.GetName() + value + ".",
                Value = new string[] { 
                    string.Format("AND(ISNUMBER({0}),ISNUMBER(FIND(\".\",{0})),{0}{1}{2})",
                    CELL_ADDRESS_VALUE,
                    type.GetDescription(),
                    value 
                    )
                }
            };
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="between">是否介于之间</param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static ColumnValidate Decimal(bool between, decimal min, decimal max)
        {
            if (between)
            {
                return new ColumnValidate()
                {
                    Message = "输入值必须是小数,并且大于" + min + ",小于" + max + ".",
                    Value = new string[] { 
                    string.Format("AND(ISNUMBER({0}),ISNUMBER(FIND(\".\",{0})),{0}>={1},{0}<={2})",
                        CELL_ADDRESS_VALUE,
                        min,
                        max
                    )
                }
                };
            }
            else
            {
                return new ColumnValidate()
                {
                    Message = "输入值必须是小数,并且小于" + min + ",大于" + max + ".",
                    Value = new string[] { 
                    string.Format("AND(ISNUMBER({0}),ISNUMBER(FIND(\".\",{0})),{0}<={1},{0}>={2})",
                        CELL_ADDRESS_VALUE,
                        min,
                        max
                    )
                }
                };
            }
        }
        public static ColumnValidate Decimal(IEnumerable<decimal> list)
        {
            return List(list);
        }

        public static ColumnValidate Date()
        {
            //=AND(ISNUMBER(FIND("D",Cell("format",INDIRECT(ADDRESS(ROW(),COLUMN()))))),INDIRECT(ADDRESS(ROW(),COLUMN()))>=1,INDIRECT(ADDRESS(ROW(),COLUMN()))<=401768)
            //日期 1899年12月31日是数值 1
            //日期 2099年12月31日是数值 73050
            //日期 2999年12月31日是数值 401768
            return new ColumnValidate()
            {
                Message = "输入值必须是日期.",
                Value = new string[] { 
                        string.Format("AND(ISNUMBER(FIND(\"D\",CELL(\"format\",{0}))),{0}>=1,{0}<=401768)",
                                CELL_ADDRESS_VALUE
                        )                
                }
            };
        }
        public static ColumnValidate Date(ValueOperator type, DateTime value)
        {
            return new ColumnValidate()
            {
                Message = "输入值必须是日期,并且" + type.GetName() + value.ToString_yyyyMMdd() + ".",//AND(ISNUMBER(FIND(\"D\",CELL(\"format\",{0}))),{0}>={1},{0}<=401768)
                Value = new string[] { 
                    string.Format("AND(ISNUMBER({0}),ISNUMBER(FIND(\"D\",CELL(\"format\",{0}))),{0}{1}{2})",
                    CELL_ADDRESS_VALUE,
                    type.GetDescription(),
                    (value-CELL_MIN_DATE).Days 
                    )
                }
            };
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="between">是否介于之间</param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static ColumnValidate Date(bool between, DateTime min, DateTime max)
        {
            if (between)
            {
                return new ColumnValidate()
                {
                    Message = "输入值必须是日期,并且大于" + min.ToString_yyyyMMdd() + ",小于" + max.ToString_yyyyMMdd() + ".",
                    Value = new string[] { 
                    string.Format("AND(ISNUMBER({0}),ISNUMBER(FIND(\"D\",CELL(\"format\",{0}))),{0}>={1},{0}<={2})",
                        CELL_ADDRESS_VALUE,
                        (min-CELL_MIN_DATE).Days ,
                        (max-CELL_MIN_DATE).Days 
                    )
                }
                };
            }
            else
            {
                return new ColumnValidate()
                {
                    Message = "输入值必须是日期,并且小于" + min.ToString_yyyyMMdd() + ",大于" + max.ToString_yyyyMMdd() + ".",
                    Value = new string[] { 
                    string.Format("AND(ISNUMBER({0}),ISNUMBER(FIND(\"D\",CELL(\"format\",{0}))),{0}<={1},{0}>={2})",
                        CELL_ADDRESS_VALUE,
                       (min-CELL_MIN_DATE).Days ,
                        (max-CELL_MIN_DATE).Days 
                    )
                }
                };
            }
        }
        public static ColumnValidate Date(IEnumerable<DateTime> list)
        {
            return String(list.Select(p => p.ToString_yyyyMMdd()).ToArray());
        }

        public static ColumnValidate String()
        {
            return new ColumnValidate()
            {
                Message = "输入值必须是文本.",
                Value = new string[] { 
                    string.Format("AND(ISERR({0})=FALSE,ISNA({0})=FALSE)" ,
                                CELL_ADDRESS_VALUE
                        )  
            }
            };
        }
        public static ColumnValidate String(StringOperator type, string value)
        {
            string s = "";
            switch (type)
            {
                case StringOperator.等于:
                    s = "AND(ISERR({0})=FALSE,ISNA({0})=FALSE,{0}=\"{1}\")";
                    break;
                case StringOperator.不等于:
                    s = "AND(ISERR({0})=FALSE,ISNA({0})=FALSE,{0}<>\"{1}\")";
                    break;
                case StringOperator.包含:
                    s = "AND(ISERR({0})=FALSE,ISNA({0})=FALSE,ISNUMBER(FIND(\"{1}\",{0})))";
                    break;
                case StringOperator.不包含:
                    s = "AND(ISERR({0})=FALSE,ISNA({0})=FALSE,ISNUMBER(FIND(\"{1}\",{0}))=FALSE)";
                    break;
                case StringOperator.开始文本是:
                    s = "AND(ISERR({0})=FALSE,ISNA({0})=FALSE,LEFT({0}," + value.Length + ")=\"{1}\")";
                    break;
                case StringOperator.结束文本是:
                    s = "AND(ISERR({0})=FALSE,ISNA({0})=FALSE,RIGHT({0}," + value.Length + ")=\"{1}\")";
                    break;
                case StringOperator.开始文本不是:
                    s = "AND(ISERR({0})=FALSE,ISNA({0})=FALSE,LEFT({0}," + value.Length + ")<>\"{1}\")";
                    break;
                case StringOperator.结束文本不是:
                    s = "AND(ISERR({0})=FALSE,ISNA({0})=FALSE,RIGHT({0}," + value.Length + ")<>\"{1}\")";
                    break;
                case StringOperator.开始和结束文本是:
                    s = "AND(ISERR({0})=FALSE,ISNA({0})=FALSE,LEFT({0}," + value.Length + ")=\"{1}\",RIGHT({0}," + value.Length + ")=\"{1}\")";
                    break;
                case StringOperator.开始和结束文本不是:
                    s = "AND(ISERR({0})=FALSE,ISNA({0})=FALSE,LEFT({0}," + value.Length + ")<>\"{1}\",RIGHT({0}," + value.Length + ")<>\"{1}\")";
                    break;
                case StringOperator.开始和结束不是空格:
                    s = "AND(ISERR({0})=FALSE,ISNA({0})=FALSE,TRIM({0})={0})";
                    value = "";
                    break;
            }


            //switch (type)
            //{
            //    case ValueOperator.大于:
            //    case ValueOperator.小于:
            //    case ValueOperator.大于等于:
            //    case ValueOperator.小于等于:
            //        throw new ArithmeticException("字符串类型不支持这些操作！");
            //}
            return new ColumnValidate()
            {
                Message = "输入值必须是文本,并且" + type.GetName() + value + ".",
                Value = new string[] { 
                    string.Format(s ,
                                CELL_ADDRESS_VALUE,
                                value.Replace("\"","\"\"")
                        )  
            }
            };
        }
        public static ColumnValidate String(int length)
        {
            return new ColumnValidate()
            {
                Message = "输入值必须是文本,并且长度为" + length + ".",
                Value = new string[] { 
                    string.Format("AND(ISERR({0})=FALSE,ISNA({0})=FALSE,LEN({0})={1})",
                        CELL_ADDRESS_VALUE,
                        length
                        )
                }
            };
        }
        public static ColumnValidate String(int? minLenght, int? maxLenght)
        {
            if (minLenght == null && maxLenght == null) throw new ArgumentNullException("minValue AND maxValue", "两个参数不能同时为空");
            return new ColumnValidate()
            {
                Message = "输入值必须是文本，长度" + (minLenght.HasValue ? "大于" + minLenght : "") + (minLenght.HasValue ? " 小于" + maxLenght : "") + ".",
                Value = new string[] { 
                    string.Format("AND(ISERR({0})=FALSE,ISNA({0})=FALSE{1}{2})",
                        CELL_ADDRESS_VALUE,
                        minLenght.HasValue?",LEN("+CELL_ADDRESS_VALUE+")>="+minLenght.Value:"",
                        minLenght.HasValue?",LEN("+CELL_ADDRESS_VALUE+")<="+minLenght.Value:""
                        )
                }
            };
        }
        public static ColumnValidate String(IEnumerable<string> items)
        {
            return new ColumnValidate() { Message = "请输入/选择下拉列表中的值.", Value = items.ToArray() };
        }
        public static ColumnValidate String(params string[] items)
        {
            return new ColumnValidate() { Message = "请输入/选择下拉列表中的值.", Value = items };
        }

        public static ColumnValidate Boolean()
        {
            return new ColumnValidate() { Message = "请输入/选择下拉列表中的值.", Value = new string[] { "是", "否"/*, "TRUE", "FALSE", "Y", "N", "真", "假" ,"1", "0"*/} };
        }

        public static ColumnValidate List<T>(IEnumerable<T> items) where T : struct
        {
            return new ColumnValidate() { Message = "请输入/选择下拉列表中的值.", Value = items.Cast<string>().ToArray() };
        }
        public static ColumnValidate Enum<T>() where T : struct
        {
            return new ColumnValidate() { Message = "请输入/选择下拉列表中的值.", Value = System.Enum.GetNames(typeof(T)) };
        }
        public static ColumnValidate Enum<T>(params T[] items) where T : struct
        {
            return new ColumnValidate() { Message = "请输入/选择下拉列表中的值.", Value = items.Cast<Enum>().Select(p => p.GetName()).ToArray() };
        }
        public ColumnValidate()
        {
        }
        public string Message { get; set; }
        public string[] Value { get; set; }
        public enum ValueOperator
        {
            [Description("=")]
            等于,
            [Description("<>")]
            不等于,
            [Description(">")]
            大于,
            [Description(">")]
            小于,
            [Description(">=")]
            大于等于,
            [Description("<=")]
            小于等于
        }
        public enum StringOperator
        {
            等于,
            不等于,
            包含,
            不包含,
            开始文本是,
            结束文本是,
            开始文本不是,
            结束文本不是,
            开始和结束文本是,
            开始和结束文本不是,
            开始和结束不是空格
        }
        public ColumnValidate ICloneableClone()
        {
            return new ColumnValidate() { Value = this.Value, Message = Message };
        }
        object ICloneable.Clone()
        {
            return new ColumnValidate() { Value = this.Value, Message = Message };
        }
    }
    [Serializable]
    public sealed class Cell
    {
        /// <summary>
        /// 当前列配置信息
        /// </summary>
        public ColumnOption ColumnOption { get; internal set; }
        /// <summary>
        /// 当前列索引，从 0 开始
        /// </summary>
        public int ColumnIndex { get; internal set; }
        /// <summary>
        /// 当前列标号，从 A 开始
        /// </summary>
        public string ColumnAddress { get { return ExcelHelper.GetColunmAddress(this.ColumnIndex); } }
        /// <summary>
        /// 当前单元格地址
        /// </summary>
        public string Address { get { return "$" + ExcelHelper.GetColunmAddress(this.ColumnIndex) + (RowIndex + 1); } }
        /// <summary>
        /// 当前行标号，从 1 开始
        /// </summary>
        public string RowAddress { get { return (RowIndex + 1).ToString(); } }
        /// <summary>
        /// 当前行索引，从 0 开始
        /// </summary>
        public int RowIndex { get; internal set; }
        /// <summary>
        /// 当前单元格的值
        /// </summary>
        public object Value { get; internal set; }
        /// <summary>
        /// 当前行所对应的IEnumerable&lt;T&gt;中强类型T元素实例 （导入时候此属性为 null）
        /// </summary>
        public object Item { get; internal set; }
    }
    /// <summary>
    /// Sheet基础信息
    /// </summary>
     [Serializable]
    public sealed class Sheet
    {

          /// <summary>
          /// Sheet的Key
          /// </summary>
          public int SheetKey { get; set; }

        /// <summary>
        /// Sheet的名称
        /// </summary>
          public string SheetName { get; set; }
    
    }
}
