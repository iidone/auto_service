using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Avalonia.Controls;
using ClosedXML.Excel;

namespace Auto_Service.Services;

public interface IExportService
{
    void ExportToExcel(IEnumerable<object> data, string filePath, List<string> excludedFields = null);
    Task<string> GetExportFilePathAsync(string defaultFileName);
}

public class ExportService : IExportService
{
    private readonly Window _parentWindow;

    public ExportService(Window parentWindow)
    {
        _parentWindow = parentWindow;
    }
    
    public void ExportToExcel(IEnumerable<object> data, string filePath, List<string> excludedFields = null)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Export");
        excludedFields ??= new List<string> { "SecretField", "Password" };
        
        var properties = data.FirstOrDefault()?
                .GetType()
                .GetProperties()
                .Where(p => !excludedFields.Contains(p.Name))
                .ToArray();
            
        
        for (int i = 0; i < properties.Length; i++)
        {
            worksheet.Cell(1, i + 1).Value = properties[i].Name;
        }
        
        int row = 2;
        foreach (var item in data)
        {
            for (int col = 0; col < properties.Length; col++)
            {
                var value = properties[col].GetValue(item);
                worksheet.Cell(row, col + 1).Value = value?.ToString() ?? string.Empty;
            }
            row++;
        }

        worksheet.Columns().AdjustToContents();
        workbook.SaveAs(filePath);
    }

    public async Task<string> GetExportFilePathAsync(string defaultFileName)
    {
        var saveFileDialog = new SaveFileDialog
        {
            Title = "Экспорт в Excel",
            Filters = new List<FileDialogFilter>
            {
                new() { Name = "Excel Files", Extensions = { "xlsx" } }
            },
            InitialFileName = defaultFileName
        };
            
        return await saveFileDialog.ShowAsync(_parentWindow);
    }
}