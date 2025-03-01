using ClosedXML.Excel;
using System;
using System.Data;

namespace GetOracleData
{
    public class Excel
    {
        public static bool Export(DataTable dataTable, string filePath)
        {
            try
            {
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Sheet1");
                    worksheet.Cell(1, 1).InsertTable(dataTable); // DataTable を挿入
                    workbook.SaveAs(filePath);
                }
                return true;
            }
            catch (Exception ex)
            {
                logger.Error(ex, true);
                return false;
            }
        }
    }
}
