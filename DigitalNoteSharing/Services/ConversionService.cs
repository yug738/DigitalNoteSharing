using OfficeOpenXml;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace DigitalNoteSharing.Services
{
    public static class ConversionService
    {
        // CSV -> XLSX
        public static void CsvToXlsx(string csvPath, string xlsxPath)
        {
            OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

            using var package = new ExcelPackage();
            var ws = package.Workbook.Worksheets.Add("Sheet1");

            var lines = File.ReadAllLines(csvPath);
            for (int r = 0; r < lines.Length; r++)
            {
                var cells = lines[r].Split(',');
                for (int c = 0; c < cells.Length; c++)
                {
                    ws.Cells[r + 1, c + 1].Value = cells[c];
                }
            }

            package.SaveAs(new FileInfo(xlsxPath));
        }

        // XLSX -> CSV
        public static void XlsxToCsv(string xlsxPath, string csvPath)
        {
            OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

            using var package = new ExcelPackage(new FileInfo(xlsxPath));
            var ws = package.Workbook.Worksheets[0];

            using var sw = new StreamWriter(csvPath);
            int rows = ws.Dimension.End.Row;
            int cols = ws.Dimension.End.Column;

            for (int r = 1; r <= rows; r++)
            {
                var cells = new string[cols];
                for (int c = 1; c <= cols; c++)
                {
                    var value = ws.Cells[r, c].Text?.Replace(",", " ") ?? "";
                    cells[c - 1] = value;
                }
                sw.WriteLine(string.Join(",", cells));
            }
        }
    }
}
