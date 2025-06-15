using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Auto_Service.Models;
using Auto_Service.Services;
using Avalonia.Controls;
using ReactiveUI;
using Telegram.Bot;
using Telegram.Bot.Types.InputFiles;

namespace Auto_Service.ViewModels;

public class CloseRequestViewModel : ReactiveObject
{
    private ObservableCollection<SparePartsModel> _availableParts = new();
    private ObservableCollection<SparePartsModel> _selectedParts = new();
    private SparePartsService _partsService;
    private readonly Window _currentWindow;
    private MaintenancesService _maintenance_service;
    private readonly TelegramBotClient _telegramBot;
    private readonly string _chatId;
    private int _workId;
    private decimal _totalPrice;
    
    public ReactiveCommand<Unit, Unit> CloseRequestCommand { get; }
    public ReactiveCommand<Unit, Unit> CancelCommand { get; }
    
    public ObservableCollection<SparePartsModel> AvailableParts
    {
        get => _availableParts;
        set => this.RaiseAndSetIfChanged(ref _availableParts, value);
    }
    
    public ObservableCollection<SparePartsModel> SelectedParts
    {
        get => _selectedParts;
        set => this.RaiseAndSetIfChanged(ref _selectedParts, value);
    }
    
    public decimal TotalPrice
    {
        get => _totalPrice;
        set => this.RaiseAndSetIfChanged(ref _totalPrice, value);
    }
    
    public CloseRequestViewModel(SparePartsService partsService, 
        int workId, 
        Window currentWindow, 
        MaintenancesService maintenancesService,
        TelegramBotClient telegramBot,
        string chatId)
    {
        _currentWindow = currentWindow;
        _partsService = partsService;
        _maintenance_service = maintenancesService;
        _workId = workId;
        _telegramBot = telegramBot;
        _chatId = chatId;
        
        LoadAvailableParts();
        
        CloseRequestCommand = ReactiveCommand.CreateFromTask(CloseRequest);
        CancelCommand = ReactiveCommand.Create(() => _currentWindow.Close());
        
        this.WhenAnyValue(x => x.AvailableParts)
            .Where(parts => parts != null)
            .Subscribe(parts =>
            {
                foreach (var part in parts)
                {
                    part.WhenAnyValue(p => p.IsSelected)
                        .Subscribe(_ => UpdateSelectedParts());
                }
            });
    }
    
    private async Task LoadAvailableParts()
    {
        var parts = await _partsService.GetAllSpareParts();
        AvailableParts = new ObservableCollection<SparePartsModel>(parts);
    }
    
    private void UpdateSelectedParts()
    {
        SelectedParts = new ObservableCollection<SparePartsModel>(
            AvailableParts.Where(part => part.IsSelected)
        );
        UpdateTotalPrice();
    }
    
    private void UpdateTotalPrice()
    {
        TotalPrice = SelectedParts.Sum(part => part.PriceDecimal);
    }
    
    private async Task CloseRequest()
    {
        try 
        {
            var selectedPartIds = SelectedParts
                .Where(p => p.IsSelected)
                .Select(p => p.Id)
                .ToArray();

            var isSuccess = await _maintenance_service.UpdateMaintenance(_workId, "Completed", TotalPrice);
            if (!isSuccess)
            {
                Console.WriteLine("Не удалось обновить статус заявки");
                return;
            }
            
            if (selectedPartIds.Length > 0)
            {
                var partsRemoved = await _partsService.DeletePart(selectedPartIds);
                if (!partsRemoved)
                {
                    Console.WriteLine("Не удалось удалить запчасти");
                }
            }
            
            var response = await _maintenance_service.GetMaintenanceById(_workId);
            if (response == null || response.Client == null)
            {
                Console.WriteLine("Не удалось получить данные для чека");
                return;
            }
            
            var receipt = new MaintenanceRequest(
                Id: _workId,
                ClientName: $"{response.Client.first_name} {response.Client.last_name}",
                Car: $"{response.Client.brand} {response.Client.series}",
                Description:$"{response.Maintenance.description}",
                TelegramChatId: _chatId,
                UsedParts: SelectedParts.Select(p => 
                    new SparePart(p.Id, p.title, p.PriceDecimal)).ToList(),
                TotalPrice: TotalPrice,
                Date: DateTime.Now
            );

            var pdfBytes = new PdfReceiptGenerator().GenerateReceipt(receipt);
            
            if (!string.IsNullOrEmpty(_chatId))
            {
                Console.WriteLine("Отправка чека в Telegram...");
                using var stream = new MemoryStream(pdfBytes);
                await _telegramBot.SendDocumentAsync(
                    chatId: _chatId,
                    document: new InputOnlineFile(stream, $"Чек_{_workId}.{DateTime.Now}.pdf"),
                    caption: $"Чек по заявке #{_workId}\nКлиент: {receipt.ClientName}\nСумма: {receipt.TotalPrice} ₽"
                );
                Console.WriteLine("Чек отправлен");
            }

            _currentWindow.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при закрытии заявки: {ex.Message}");
        }
    }
}