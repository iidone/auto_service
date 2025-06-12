using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Auto_Service.Models;
using Auto_Service.Services;
using Avalonia.Controls;
using ReactiveUI;

namespace Auto_Service.ViewModels;

public class CloseRequestViewModel : ReactiveObject
{
    private ObservableCollection<SparePartsModel> _availableParts = new();
    private ObservableCollection<SparePartsModel> _selectedParts = new();
    private SparePartsService _partsService;
    private readonly Window _currentWindow;
    private MaintenancesService _maintenance_service;
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
    
    public CloseRequestViewModel(SparePartsService partsService, int WorkId, Window currentWindow, MaintenancesService maintenancesService)
    {
        _currentWindow = currentWindow;
        _partsService = partsService;
        _maintenance_service =  maintenancesService;
        _workId =  WorkId;
        
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
        Console.WriteLine(_workId);
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
                    Console.WriteLine("PartsRemoved");
                }
            }

            LoadAvailableParts();
            _currentWindow.Close();
            Console.WriteLine("Operation completed");
            
            _currentWindow.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Operation Failed: {ex.Message}");
        }
    }
}