using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using Auto_Service.Models;

namespace Auto_Service.Services;

public class PdfReceiptGenerator
{
    public byte[] GenerateReceipt(MaintenanceRequest request)
    {
        QuestPDF.Settings.License = LicenseType.Community;
        
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Content().Padding(20).Column(col =>
                {
                    col.Item().Row(row =>
                    {
                        row.RelativeItem().Text($"Чек #{request.Id}").Bold().FontSize(20);
                        row.AutoItem().Text(request.Date.ToString("dd.MM.yyyy"));
                    });
                    
                    col.Item().PaddingTop(10).Text($"Клиент: {request.ClientName}");
                    col.Item().PaddingBottom(10).PaddingTop(10).Text($"Автомобиль: {request.Car}");
                    col.Item().PaddingTop(15).PaddingBottom(15).Text($"Описание работ:  {request.Description}");
                    
                    col.Item().PaddingTop(10).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                            columns.ConstantColumn(80);
                            columns.ConstantColumn(80);
                        });
                        
                        table.Header(header =>
                        {
                            header.Cell().Text("Запчасть");
                            header.Cell().AlignRight().Text("Цена");
                            header.Cell().AlignRight().Text("Сумма");
                        });
                        
                        foreach (var part in request.UsedParts)
                        {
                            table.Cell().Text(part.Name);
                            table.Cell().AlignRight().Text($"{part.Price} ₽");
                            table.Cell().AlignRight().Text($"{part.Price * part.Quantity} ₽");
                        }
                    });
                    
                    col.Item().PaddingTop(10).AlignRight()
                        .Text($"Итого: {request.TotalPrice} ₽").Bold();
                });
            });
        }).GeneratePdf();
    }
}