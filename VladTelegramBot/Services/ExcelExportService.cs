using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using VladTelegramBot.Data;
using LicenseContext = OfficeOpenXml.LicenseContext;

namespace VladTelegramBot.Services;


public class ExcelExportService(AppDbContext dbContext)
{
    public async Task<byte[]> ExportSurveyResultsToExcelAsync()
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        var results = await dbContext.SurveyResults.ToListAsync();

        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Survey Results");

        // Заголовки
        worksheet.Cells[1, 1].Value = "TelegramName";
        worksheet.Cells[1, 2].Value = "Answer1";
        worksheet.Cells[1, 3].Value = "Answer2";
        worksheet.Cells[1, 4].Value = "Answer3";
        worksheet.Cells[1, 5].Value = "Answer4";
        worksheet.Cells[1, 6].Value = "Answer5";
        worksheet.Cells[1, 7].Value = "SubmittedAt";

        // Данные
        for (var i = 0; i < results.Count; i++)
        {
            var row = i + 2;
            var r = results[i];

            worksheet.Cells[row, 1].Value = r.TelegramName;
            worksheet.Cells[row, 2].Value = r.Answer1;
            worksheet.Cells[row, 3].Value = r.Answer2;
            worksheet.Cells[row, 4].Value = r.Answer3;
            worksheet.Cells[row, 5].Value = r.Answer4;
            worksheet.Cells[row, 6].Value = r.Answer5;
            worksheet.Cells[row, 7].Value = r.SubmittedAt.ToString("yyyy-MM-dd HH:mm:ss");
        }

        worksheet.Cells.AutoFitColumns();

        return await package.GetAsByteArrayAsync(); // можно сохранить в файл или отправить по email
    }
}
